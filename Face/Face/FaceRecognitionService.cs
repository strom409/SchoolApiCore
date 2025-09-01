using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Drawing;
using System.Linq;

namespace FaceApi.Face
{
    public class FaceRecognitionService : IFaceRecognitionService
    {
        private readonly InferenceSession _detectionSession;
        private readonly InferenceSession _recognitionSession;
        private const float SimilarityThreshold = 0.6f;
        private const int TargetWidth = 640;
        private const int TargetHeight = 480;

        public FaceRecognitionService()
        {
            // Load models (adjust paths as needed)
            var modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models");
            _detectionSession = new InferenceSession(Path.Combine(modelPath, "ultraface.onnx"));
            _recognitionSession = new InferenceSession(Path.Combine(modelPath, "facenet.onnx"));
        }

        public async Task<FaceRecognitionModel> CompareFacesAsync(CompareFacesRequestDTO compareFacesRequestDTO)
        {
            try
            {
                var isFirstHuman = await IsHumanFaceAsync(compareFacesRequestDTO.Image1);
                if (!isFirstHuman)
                {
                    return new FaceRecognitionModel
                    {
                        IsFirstImageHuman = false,
                        IsSecondImageHuman = false,
                        IsSamePerson = false,
                        Message = "First image does not contain a human face."
                    };
                }

                var isSecondHuman = await IsHumanFaceAsync(compareFacesRequestDTO.Image2);
                if (!isSecondHuman)
                {
                    return new FaceRecognitionModel
                    {
                        IsFirstImageHuman = true,
                        IsSecondImageHuman = false,
                        IsSamePerson = false,
                        Message = "Second image does not contain a human face."
                    };
                }

                var isSamePerson = await AreSamePersonAsync(compareFacesRequestDTO.Image1, compareFacesRequestDTO.Image2);
                return new FaceRecognitionModel
                {
                    IsFirstImageHuman = true,
                    IsSecondImageHuman = true,
                    IsSamePerson = isSamePerson,
                    Message = isSamePerson ? "Both images are of the same person." : "Images are of different people."
                };
            }
            catch (Exception ex)
            {
                // Log error here
                return new FaceRecognitionModel
                {
                    IsFirstImageHuman = false,
                    IsSecondImageHuman = false,
                    IsSamePerson = false,
                    Message = $"Error processing images: {ex.Message}"
                };
            }
        }

        public async Task<bool> IsHumanFaceAsync(IFormFile image)
        {
            using var imageStream = new MemoryStream();
            await image.CopyToAsync(imageStream);
            imageStream.Position = 0;

            using var img = await SixLabors.ImageSharp.Image.LoadAsync<SixLabors.ImageSharp.PixelFormats.Rgb24>(imageStream);
            var faces = await DetectFacesAsync(img);
            return faces != null && faces.Count > 0;
        }



        public async Task<bool> AreSamePersonAsync(IFormFile image1, IFormFile image2)
        {
            var encoding1 = await GetFaceEncodingAsync(image1);
            var encoding2 = await GetFaceEncodingAsync(image2);

            if (encoding1 == null || encoding2 == null)
                return false;

            return CalculateSimilarity(encoding1, encoding2) > SimilarityThreshold;
        }

        private async Task<FaceEncoding> GetFaceEncodingAsync(IFormFile image)
        {
            using var imageStream = new MemoryStream();
            await image.CopyToAsync(imageStream);
            imageStream.Position = 0;

            using var img = await SixLabors.ImageSharp.Image.LoadAsync<SixLabors.ImageSharp.PixelFormats.Rgb24>(imageStream);  // Fixed
            var faces = await DetectFacesAsync(img);

            if (faces.Count == 0)
                return null;

            using var croppedFace = CropAndAlignFace(img, faces[0]);
            return await GetFaceEmbeddingAsync(croppedFace);
        }

        private async Task<List<System.Drawing.Rectangle>> DetectFacesAsync(Image<Rgb24> image)
        {
            var faces = new List<System.Drawing.Rectangle>();

            // Preprocess image
            using var resizedImage = image.Clone(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(TargetWidth, TargetHeight),
                Mode = ResizeMode.Stretch
            }));

            // Convert image to tensor
            var input = new DenseTensor<float>(new[] { 1, 3, TargetHeight, TargetWidth });
            for (int y = 0; y < TargetHeight; y++)
            {
                for (int x = 0; x < TargetWidth; x++)
                {
                    var pixel = resizedImage[x, y];
                    input[0, 0, y, x] = (pixel.R / 255f - 0.5f) / 0.5f;
                    input[0, 1, y, x] = (pixel.G / 255f - 0.5f) / 0.5f;
                    input[0, 2, y, x] = (pixel.B / 255f - 0.5f) / 0.5f;
                }
            }

            // Run ONNX inference
            var inputs = new List<NamedOnnxValue>
    {
        NamedOnnxValue.CreateFromTensor("input", input)
    };

            using var results = _detectionSession.Run(inputs);

            // ✅ Swap outputs: First is scores, second is boxes (based on your dimensions)
            var scores = results.ElementAt(0).AsTensor<float>();
            var boxes = results.ElementAt(1).AsTensor<float>();

            // Debug dimensions
            var boxDims = boxes.Dimensions.ToArray();
            var scoreDims = scores.Dimensions.ToArray();
            Console.WriteLine($"Boxes Dimensions: {string.Join(", ", boxDims)}");
            Console.WriteLine($"Scores Dimensions: {string.Join(", ", scoreDims)}");

            // Safety check
            if (boxDims.Length < 3 || scoreDims.Length < 3)
            {
                Console.WriteLine("Invalid tensor dimensions. Skipping detection.");
                return faces;
            }

            // Safe loop limit
            int numBoxes = Math.Min(boxes.Dimensions[1], scores.Dimensions[1]);

            // Detect faces
            for (int i = 0; i < numBoxes; i++)
            {
                float score = scores[0, i, 1];  // Class 1 score (face)
                if (score > 0.7f)  // Threshold
                {
                    float x1 = boxes[0, i, 0] * image.Width;
                    float y1 = boxes[0, i, 1] * image.Height;
                    float x2 = boxes[0, i, 2] * image.Width;
                    float y2 = boxes[0, i, 3] * image.Height;

                    faces.Add(new System.Drawing.Rectangle(
                        (int)x1,
                        (int)y1,
                        (int)(x2 - x1),
                        (int)(y2 - y1)
                    ));
                }
            }

            return faces;
        }

        //private async Task<List<System.Drawing.Rectangle>> DetectFacesAsync(Image<Rgb24> image)
        //{
        //    var resizedImage = image.Clone(ctx => ctx.Resize(new ResizeOptions
        //    {
        //        Size = new SixLabors.ImageSharp.Size(TargetWidth, TargetHeight),
        //        Mode = ResizeMode.Stretch
        //    }));

        //    try
        //    {
        //        Convert image to tensor
        //            var input = new DenseTensor<float>(new[] { 1, 3, TargetHeight, TargetWidth });

        //        for (int y = 0; y < TargetHeight; y++)
        //        {
        //            for (int x = 0; x < TargetWidth; x++)
        //            {
        //                var pixel = resizedImage[x, y];
        //                input[0, 0, y, x] = (pixel.R / 255f - 0.5f) / 0.5f;
        //                input[0, 1, y, x] = (pixel.G / 255f - 0.5f) / 0.5f;
        //                input[0, 2, y, x] = (pixel.B / 255f - 0.5f) / 0.5f;
        //            }
        //        }

        //        var inputs = new List<NamedOnnxValue>
        //    {
        //        NamedOnnxValue.CreateFromTensor("input", input)
        //    };

        //        using var results = _detectionSession.Run(inputs);
        //        var output = results.First().AsTensor<float>();

        //        Console.WriteLine($"Output Tensor Dimensions: {string.Join(", ", output.Dimensions.ToArray())}");


        //        Safety Check Before Access
        //            if (output.Rank < 4 || output.Dimensions[2] <= 0)
        //        {
        //            Console.WriteLine("Invalid or empty output tensor shape. No detections.");
        //            return new List<System.Drawing.Rectangle>();
        //        }

        //        var faces = new List<System.Drawing.Rectangle>();
        //        int numDetections = output.Dimensions[2];

        //        for (int i = 0; i < numDetections; i++)
        //        {
        //            float confidence = output[0, 1, i, 0];  // Safe after rank check

        //            if (confidence > 0.7f)  // Confidence threshold
        //            {
        //                int x1 = (int)(output[0, 1, i, 1] * image.Width);
        //                int y1 = (int)(output[0, 1, i, 2] * image.Height);
        //                int x2 = (int)(output[0, 1, i, 3] * image.Width);
        //                int y2 = (int)(output[0, 1, i, 4] * image.Height);
        //                faces.Add(new System.Drawing.Rectangle(x1, y1, x2 - x1, y2 - y1));
        //            }
        //        }

        //        return faces;
        //    }
        //    finally
        //    {
        //        resizedImage.Dispose();  // Dispose resized image manually
        //    }
        //}



        private Image<Rgb24> CropAndAlignFace(Image<Rgb24> image, System.Drawing.Rectangle faceRect)
        {
            // Basic cropping - replace with proper alignment if needed
            return image.Clone(ctx => ctx
                .Crop(new SixLabors.ImageSharp.Rectangle(
                    faceRect.X,
                    faceRect.Y,
                    faceRect.Width,
                    faceRect.Height))
                .Resize(new SixLabors.ImageSharp.Size(112, 112))); // Standard size for face recognition
        }
        private async Task<FaceEncoding> GetFaceEmbeddingAsync(Image<Rgb24> faceImage)
        {
            // ✅ Resize face image to model input size
            using var resizedFace = faceImage.Clone(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(368, 368),
                Mode = ResizeMode.Stretch
            }));

            var input = new DenseTensor<float>(new[] { 1, 3, 368, 368 });

            for (int y = 0; y < 368; y++)
            {
                for (int x = 0; x < 368; x++)
                {
                    var pixel = resizedFace[x, y];
                    input[0, 0, y, x] = (pixel.R / 255f - 0.5f) / 0.5f;
                    input[0, 1, y, x] = (pixel.G / 255f - 0.5f) / 0.5f;
                    input[0, 2, y, x] = (pixel.B / 255f - 0.5f) / 0.5f;
                }
            }

            var inputs = new List<NamedOnnxValue>
    {
        NamedOnnxValue.CreateFromTensor("input", input)
    };

            using var results = _recognitionSession.Run(inputs);
            var output = results.First().AsTensor<float>();

            var embedding = output.ToArray();

            return new FaceEncoding(embedding);
        }

        //private async Task<FaceEncoding> GetFaceEmbeddingAsync(Image<Rgb24> faceImage)
        //{
        //    Convert to tensor
        //   var input = new DenseTensor<float>(new[] { 1, 3, faceImage.Height, faceImage.Width });

        //    Normalize pixels(model specific)
        //    for (int y = 0; y < faceImage.Height; y++)
        //    {
        //        for (int x = 0; x < faceImage.Width; x++)
        //        {
        //            var pixel = faceImage[x, y];
        //            input[0, 0, y, x] = (pixel.R / 255f - 0.5f) / 0.5f;
        //            input[0, 1, y, x] = (pixel.G / 255f - 0.5f) / 0.5f;
        //            input[0, 2, y, x] = (pixel.B / 255f - 0.5f) / 0.5f;
        //        }
        //    }

        //    Run inference
        //    var inputs = new List<NamedOnnxValue>
        //    {
        //        NamedOnnxValue.CreateFromTensor("input", input)
        //    };

        //    using var results = _recognitionSession.Run(inputs);
        //    var output = results.First().AsTensor<float>();

        //    Get embedding
        //    var embedding = output.ToArray();

        //    return new FaceEncoding(embedding);
        //}

        private float CalculateSimilarity(FaceEncoding encoding1, FaceEncoding encoding2)
        {
            // Cosine similarity
            float dot = 0, mag1 = 0, mag2 = 0;
            for (int i = 0; i < encoding1.Values.Length; i++)
            {
                dot += encoding1.Values[i] * encoding2.Values[i];
                mag1 += encoding1.Values[i] * encoding1.Values[i];
                mag2 += encoding2.Values[i] * encoding2.Values[i];
            }
            return dot / (float)(Math.Sqrt(mag1) * Math.Sqrt(mag2));
        }
    }

    public class FaceEncoding
    {
        public float[] Values { get; }

        public FaceEncoding(float[] values)
        {
            Values = values;
        }
    }
}