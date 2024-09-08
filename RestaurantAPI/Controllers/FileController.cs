namespace RestaurantAPI.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

[Route("file")]
[Authorize]
public class FileController : ControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 1200, VaryByQueryKeys = ["filename"])]
    public ActionResult GetFile([FromQuery] string filename)
    {
        var rootPath = Directory.GetCurrentDirectory();
        var filePath = $"{rootPath}/PrivateFiles/{filename}";
        var fileExists = System.IO.File.Exists(filePath);

        if (!fileExists)
            return NotFound();

        var contentProvide = new FileExtensionContentTypeProvider();
        contentProvide.TryGetContentType(filename, out string contentType);

        var file = System.IO.File.ReadAllBytes(filePath);
        return File(file, contentType, filename);
    }

    [HttpPost]
    public ActionResult Upload([FromForm] IFormFile file)
    {
        if (file is { Length: > 0 })
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = $"{rootPath}/PrivateFiles/{file.FileName}";

            var stream = new FileStream(filePath, FileMode.Create);
            try
            {
                file.CopyTo(stream);
                return Ok();
            }
            finally
            {
                stream.Dispose();
            }
        }

        return BadRequest();
    }
}