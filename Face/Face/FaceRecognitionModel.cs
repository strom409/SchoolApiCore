namespace FaceApi.Face
{
    public class FaceRecognitionModel
    {
        public bool IsFirstImageHuman { get; set; }
        public bool IsSecondImageHuman { get; set; }
        public bool IsSamePerson { get; set; }
        public string Message { get; set; }
    }
}
