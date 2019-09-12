using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PdfGeneration.Core.Upload;
using PdfGeneration.Data;
using PdfGeneration.Data.Entities;
using PdfGeneration.Data.Extensions;

namespace PdfGeneration.Web.Controllers
{
    [Route("api/[controller]")]
    public class PersonController : Controller
    {
        AppDbContext db;
        UploadConfig config;
        IHostingEnvironment env;

        public PersonController(AppDbContext db, UploadConfig config, IHostingEnvironment env)
        {
            this.db = db;
            this.config = config;
            this.env = env;
        }

        [HttpGet("[action]")]
        public async Task<List<Person>> GetPeople() => await db.GetPeople();

        [HttpGet("[action]")]
        public async Task<List<Person>> GetDeletedPeople() => await db.GetPeople(true);

        [HttpGet("[action]")]
        public async Task<List<Person>> GetTruePeople() => await db.GetTruePeople();

        [HttpGet("[action]")]
        public async Task<List<Person>> GetDeletedTruePeople() => await db.GetTruePeople(true);

        [HttpGet("[action]")]
        public async Task<List<Person>> GetAssociates() => await db.GetAssociates();

        [HttpGet("[action]")]
        public async Task<List<Person>> GetDeletedAssociates() => await db.GetAssociates(true);

        [HttpGet("[action]/{search}")]
        public async Task<List<Person>> SearchPeople([FromRoute]string search) => await db.SearchPeople(search);

        [HttpGet("[action]/{search}")]
        public async Task<List<Person>> SearchDeletedPeople([FromRoute]string search) => await db.SearchPeople(search, true);

        [HttpGet("[action]/{id}")]
        public async Task<Person> GetPerson([FromRoute]int id) => await db.GetPerson(id);

        [HttpPost("[action]")]
        public async Task AddPerson([FromBody]Person person) => await db.AddPerson(person);

        [HttpPost("[action]")]
        public async Task UpdatePerson([FromBody]Person person) => await db.UpdatePerson(person);

        [HttpPost("[action]")]
        public async Task TogglePersonDeleted([FromBody]Person person) => await db.TogglePersonDeleted(person);

        [HttpPost("[action]")]
        public async Task RemovePerson([FromBody]Person person) => await db.RemovePerson(person);

        [HttpGet("[action]/{personId}")]
        public async Task<List<Person>> GetPersonAssociates([FromRoute]int personId) => await db.GetPersonAssociates(personId);

        [HttpGet("[action]/{personId}")]
        public async Task<List<Person>> GetDeletedPersonAssociates([FromRoute]int personId) => await db.GetPersonAssociates(personId, true);

        [HttpGet("[action]/{personId}")]
        public async Task<int> GetTruePersonId([FromRoute]int personId) => await db.GetTruePersonId(personId);

        [HttpGet("[action]/{personId}")]
        public async Task<Person> GetTruePerson([FromRoute]int personId) => await db.GetTruePerson(personId);

        [HttpGet("[action]/{associateId}")]
        public async Task<Person> GetAssociatePerson([FromRoute]int associateId) => await db.GetAssociatePerson(associateId);

        [HttpPost("[action]/{personId}")]
        public async Task AddAssociateToPerson([FromRoute]int personId, [FromBody]Person associate) => await db.AddAssociateToPerson(associate, personId);

        [HttpPost("[action]/{personId}")]
        public async Task AddPersonAssociate([FromRoute]int personId, [FromBody]Person associate) => await db.AddPersonAssociate(associate, personId);

        [HttpPost("[action]")]
        public async Task RemovePersonAssociate([FromBody]PersonAssociate personAssociate) => await db.RemovePersonAssociate(personAssociate);

        [HttpPost("[action]")]
        public async Task GeneratePdf([FromBody]Person person, [FromRoute]string pdfTemplate)
        {
             var file = person.GeneratePdfFile(config.DirectoryBasePath, pdfTemplate);
            //await person.GeneratePdf(file, $"{env.WebRootPath}/templates/SS5-App.pdf");    //SSN Application
            await person.GeneratePdf(file, $"{env.WebRootPath}/templates/447-NC.pdf");     //NC Drivers License Application
            //await person.GeneratePdf(file, $"{env.WebRootPath}/templates/CC-App.pdf");     //Credit Card Application
        }        
    }
}