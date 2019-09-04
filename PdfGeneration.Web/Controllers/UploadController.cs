using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PdfGeneration.Core.Upload;
using PdfGeneration.Data;
using PdfGeneration.Data.Entities;
using PdfGeneration.Data.Extensions;
using PdfGeneration.Identity;

namespace PdfGeneration.Web.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private AppDbContext db;
        private UploadConfig config;
        private IUserProvider provider;

        public UploadController(
            AppDbContext db,
            UploadConfig config,
            IUserProvider provider
        )
        {
            this.db = db;
            this.config = config;
            this.provider = provider;
        }

        [HttpGet("[action]")]
        public async Task<List<Upload>> GetUploads() => await db.GetUploads();

        [HttpGet("[action]")]
        public async Task<List<Upload>> GetDeletedUploads() => await db.GetUploads(true);

        [HttpGet("[action]/{search}")]
        public async Task<List<Upload>> SearchUploads([FromRoute]string search) => await db.SearchUploads(search);

        [HttpGet("[action]/{search}")]
        public async Task<List<Upload>> SearchDeletedUploads([FromRoute]string search) => await db.SearchUploads(search, true);

        [HttpGet("[action]/{id}")]
        public async Task<Upload> GetUpload([FromRoute]int id) => await db.GetUpload(id);

        [HttpGet("[action]/{file}")]
        public async Task<Upload> GetUploadByName([FromRoute]string file) => await db.GetUploadByName(file);

        [HttpPost("[action]/{userId}")]
        [DisableRequestSizeLimit]
        public async Task<List<Upload>> UploadFiles([FromRoute]int userId) =>
            await db.UploadFiles(
                Request.Form.Files,
                config.DirectoryBasePath,
                config.UrlBasePath,
                userId
            );

        [HttpPost("[action]")]
        public async Task ToggleUploadDeleted([FromBody]Upload upload) => await db.ToggleUploadDeleted(upload);

        [HttpPost("[action]")]
        public async Task RemoveUpload([FromBody]Upload upload) => await db.RemoveUpload(upload);
    }
}