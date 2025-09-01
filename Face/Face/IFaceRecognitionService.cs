namespace FaceApi.Face
{
    public interface IFaceRecognitionService
    {
        Task<FaceRecognitionModel> CompareFacesAsync(CompareFacesRequestDTO compareFacesRequestDTO);
    }
}
