using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Waku
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FileController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger<FileController> logger;

        public FileController(IHostingEnvironment hostingEnvironment, ILogger<FileController> logger)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
        }

        [HttpDelete]
        public IActionResult Delete(string filename)
        {
            try
            {
                string folderName = "Upload";
                string webRootPath = hostingEnvironment.WebRootPath;
                string fullPath = Path.Combine(webRootPath, folderName, filename);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    return Ok($"Deleted {filename}");
                }
                else
                {
                    return Ok($"File {filename} does not exist");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete {filename}");
                return BadRequest($"Failed to delete {filename}: {ex}");
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                string folderName = "Upload";
                string webRootPath = hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                    // Change file name to MD5 hash.
                    Stream readStream = file.OpenReadStream();
                    MemoryStream memory = new MemoryStream();
                    readStream.CopyTo(memory);

                    using (MD5 md5 = MD5.Create())
                    {
                        var hash = string.Join("", md5.ComputeHash(memory.ToArray()).Select(x => x.ToString("X2")));
                        fileName =  hash + Path.GetExtension(file.FileName);
                    }

                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var results = new
                    {
                        filename = fileName
                    };

                    logger.LogInformation($"Uploaded file {fileName} successfully.");
                    return Created("Upload successful.", results);
                }

                throw new Exception();
            }
            catch (Exception ex)
            {
                logger.LogError($"Upload failed.");
                return BadRequest($"Upload failed: {ex}");
            }
        }

        [HttpGet("{filename}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string filename)
        {
            if (filename == null)
            {
                return Content("File does not exist.");
            }

            string folderName = "Upload";
            string webRootPath = hostingEnvironment.WebRootPath;
            var path = Path.Combine(webRootPath, folderName, filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };

            var ext = Path.GetExtension(path).ToLowerInvariant();

            return types[ext];
        }
    }
}
