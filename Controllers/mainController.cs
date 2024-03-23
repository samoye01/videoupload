using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;


namespace brosky.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class mainController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        public mainController()
        {
            var acc = new Account(
                "your-cloud-name",
                "your-api-key",
                "your-api-secret"
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file received");
            }

            var uploadParams = new VideoUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                return BadRequest(uploadResult.Error.Message);
            }

            return Ok(new { url = uploadResult.SecureUrl.ToString() });
        }

        [HttpGet("getvideobyid{publicId}")]
        public IActionResult Get(string publicId)
        {
            var videoUrl = _cloudinary.Api.UrlVideoUp.BuildUrl(string.Format("{0}.mp4", publicId));

            return Ok(new { url = videoUrl });
        }
        [HttpGet("getvideos{tag}")]
        public async Task<ListResourcesResult> GetVideosByTag(string tag)
        {
            var parameters = new ListResourcesByTagParams()
            {
                Tag = tag,
                ResourceType = ResourceType.Video
            };

            return await _cloudinary.ListResourcesByTagAsync(Convert.ToString(parameters));
        }
    }
}
