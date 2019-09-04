using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PdfGeneration.Data;
using PdfGeneration.Data.Entities;
using PdfGeneration.Data.Extensions;
using PdfGeneration.Identity;

namespace PdfGeneration.Web.Controllers
{
    [Route("api/[controller]")]
    public class IdentityController : Controller
    {
        private AppDbContext db;
        private IUserProvider provider;

        public IdentityController(AppDbContext db, IUserProvider provider)
        {
            this.db = db;
            this.provider = provider;
        }

        [HttpGet("[action]")]
        public async Task<List<AdUser>> GetDomainUsers() => await provider.GetDomainUsers();

        [HttpGet("[action]/{search}")]
        public async Task<List<AdUser>> FindDomainUser([FromRoute]string search) => await provider.FindDomainUser(search);

        [HttpGet("[action]")]
        public async Task<List<User>> GetUsers() => await db.GetUsers();

        [HttpGet("[action]")]
        public async Task<List<User>> GetDeletedUsers() => await db.GetUsers(true);

        [HttpGet("[action]/{search}")]
        public async Task<List<User>> SearchUsers([FromRoute]string search) => await db.SearchUsers(search);

        [HttpGet("[action]/{search}")]
        public async Task<List<User>> SearchDeletedUsers([FromRoute]string search) => await db.SearchUsers(search, true);

        [HttpGet("[action]/{id}")]
        public async Task<User> GetUser([FromRoute]int id) => await db.GetUser(id);

        [HttpGet("[action]/{guid}")]
        public async Task<int> GetCurrentUserId([FromRoute]Guid guid) => await db.GetCurrentUserId(guid);

        [HttpGet("[action]")]
        public async Task<User> SyncUser() => await provider.CurrentUser.SyncUser(db);

        [HttpPost("[action]")]
        public async Task AddUser([FromBody]AdUser adUser) => await db.AddUser(adUser);

        [HttpPost("[action]")]
        public async Task UpdateUser([FromBody]User user) => await db.UpdateUser(user);

        [HttpPost("[action]")]
        public async Task ToggleUserDeleted([FromBody]User user) => await db.ToggleUserDeleted(user);

        [HttpPost("[action]")]
        public async Task RemoveUser([FromBody]User user) => await db.RemoveUser(user);
    }
}