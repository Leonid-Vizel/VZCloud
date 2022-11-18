using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using VZCloud.Models;

namespace VZCloud.Controllers
{
    public class CloudController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        public CloudController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        #region CreateFile
        [HttpPost]
        public async Task<IActionResult> CreateFile([FromBody] CreateFileModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            if (model.Bytes == null)
            {
                return BadRequest("Укажите наполнение файла!");
            }
            if (model.Path == null)
            {
                return BadRequest("Укажите путь к файлу!");
            }
            model.Path = $"{_environment.WebRootPath}/storage/{model.Path}";
            if (System.IO.File.Exists(model.Path))
            {
                if (!model.Rewrite)
                {
                    return Forbid("Такой файл уже существует. Если хотите его перезаписать задайте параметр \"Rewrite\" : true");
                }
                System.IO.File.Delete(model.Path);
            }
            await System.IO.File.WriteAllBytesAsync(model.Path, model.Bytes);
            return Ok();
        }
        #endregion

        #region CreateDirectory
        [HttpPost]
        public IActionResult CreateDirectory([FromBody] PathInputModel model)
        {
            if (model == null || model.Path == null)
            {
                return BadRequest("Укажите путь!");
            }
            string dirPath = $"{_environment.WebRootPath}/storage/{model.Path}";
            if (System.IO.Directory.Exists(dirPath))
            {
                return StatusCode(406, "Такая папка уже существует!");
            }
            System.IO.Directory.CreateDirectory(dirPath);
            return Ok();
        }
        #endregion

        #region Directory
        [HttpPost]
        public IActionResult Directory([FromBody] PathInputModel model)
        {
            if (model == null || model.Path == null)
            {
                return BadRequest("Укажите путь!");
            }
            string dirPath = $"{_environment.WebRootPath}/storage/{model.Path}";
            if (!System.IO.Directory.Exists(dirPath))
            {
                return NotFound("Папка не найдена!");
            }
            return Ok(new Dictionary<string, object>
            {
                { "Folders", System.IO.Directory.GetDirectories(dirPath).Select(x=>Path.GetFileName(x)).ToArray() },
                { "Files", System.IO.Directory.GetFiles(dirPath).Select(x=>Path.GetFileName(x)).ToArray() }
            });
        }
        #endregion

        #region File
        [HttpPost]
        public IActionResult File([FromBody] PathInputModel model)
        {
            if (model == null || model.Path == null)
            {
                return BadRequest("Укажите путь!");
            }
            string filePath = $"{_environment.WebRootPath}/storage/{model.Path}";
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Файл не найден!");
            }
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(model.Path, out contentType))
            {
                contentType = "text/plain";
            }
            return File(System.IO.File.ReadAllBytes(filePath), contentType);
        }
        #endregion
    }
}
