using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FaceApi.Face
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceRecognitionController : ControllerBase
    {
        private readonly IFaceRecognitionService _faceRecognitionService;

        public FaceRecognitionController(IFaceRecognitionService faceRecognitionService)
        {
            _faceRecognitionService = faceRecognitionService;
        }

        [HttpPost("compare")]
        public async Task<IActionResult> CompareFaces(CompareFacesRequestDTO compareFacesRequestDTO)
        {
            if (compareFacesRequestDTO.Image1 == null || compareFacesRequestDTO.Image2 == null)
                return BadRequest("Both images are required.");

            var result = await _faceRecognitionService.CompareFacesAsync(compareFacesRequestDTO);
            return Ok(result);
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                Status = "Face Recognition API is running",
                Timestamp = DateTime.UtcNow
            });
        }

    }
}
