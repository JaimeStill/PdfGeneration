using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PdfGeneration.Data.Entities;

namespace PdfGeneration.Data.Extensions
{
    public static class PersonExtensions
    {
        public static async Task<List<Person>> GetPeople(this AppDbContext db, bool isDeleted = false)
        {
            var people = await db.People
                .Where(x => x.IsDeleted == isDeleted)
                .OrderBy(x => x.LastName)
                .ToListAsync();

            return people;
        }

        public static async Task<List<Person>> GetTruePeople(this AppDbContext db, bool isDeleted = false)
        {
            var people = await db.Associates
                .Select(x => x.Person)
                .Where(x => x.IsDeleted == isDeleted)
                .Distinct()
                .ToListAsync();

            return people;
        }

        public static async Task<List<Person>> GetAssociates(this AppDbContext db, bool isDeleted = false)
        {
            var associates = await db.Associates
                .Select(x => x.Associate)
                .Where(x => x.IsDeleted == isDeleted)
                .Distinct()
                .ToListAsync();

            return associates;
        }

        public static async Task<List<Person>> SearchPeople(this AppDbContext db, string search, bool isDeleted = false)
        {
            search = search.ToLower();

            var people = await db.People
                .Where(x => x.IsDeleted == isDeleted)
                .Where(x =>
                    x.Allergies.ToLower().Contains(search) ||
                    x.AttachedSection.ToLower().Contains(search) ||
                    x.BloodType.ToLower().Contains(search) ||
                    x.Branch.ToLower().Contains(search) ||
                    x.CityOfBirth.ToLower().Contains(search) ||
                    x.DutyPhone.ToLower().Contains(search) ||
                    x.Edipi.ToLower().Contains(search) ||
                    x.EyeColor.ToLower().Contains(search) ||
                    x.FingerPrints.ToLower().Contains(search) ||
                    x.FirstName.ToLower().Contains(search) ||
                    x.Gender.ToLower().Contains(search) ||
                    x.HairColor.ToLower().Contains(search) ||
                    x.HomePhone.ToLower().Contains(search) ||
                    x.LastName.ToLower().Contains(search) ||
                    x.MiddleName.ToLower().Contains(search) ||
                    x.MosRate.ToLower().Contains(search) ||
                    x.MothersMaidenName.ToLower().Contains(search) ||
                    x.Mpc.ToLower().Contains(search) ||
                    x.Nickname.ToLower().Contains(search) ||
                    x.OtherPhone.ToLower().Contains(search) ||
                    x.Race.ToLower().Contains(search) ||
                    x.Rank.ToLower().Contains(search) ||
                    x.Religion.ToLower().Contains(search) ||
                    x.Remarks.ToLower().Contains(search) ||
                    x.SectionAssigned.ToLower().Contains(search) ||
                    x.StateOfBirth.ToLower().Contains(search) ||
                    x.Suffix.ToLower().Contains(search) ||
                    x.Unit.ToLower().Contains(search)
                )
                .OrderBy(x => x.LastName)
                .ToListAsync();

            return people;
        }

        public static async Task<Person> GetPerson(this AppDbContext db, int personId)
        {
            var person = await db.People.FindAsync(personId);
            return person;
        }

        public static async Task<Person> AddPerson(this AppDbContext db, Person person)
        {
            if (await person.Validate(db))
            {
                await db.People.AddAsync(person);
                await db.SaveChangesAsync();
            }

            return person;
        }

        public static async Task<Person> UpdatePerson(this AppDbContext db, Person person)
        {
            if (await person.Validate(db))
            {
                db.People.Update(person);
                await db.SaveChangesAsync();
            }

            return person;
        }

        public static async Task TogglePersonDeleted(this AppDbContext db, Person person)
        {
            db.People.Attach(person);
            person.IsDeleted = !person.IsDeleted;
            await db.SaveChangesAsync();
        }

        public static async Task RemovePerson(this AppDbContext db, Person person)
        {
            db.People.Remove(person);
            await db.SaveChangesAsync();
        }

        public static async Task<bool> Validate(this Person person, AppDbContext db)
        {
            if (string.IsNullOrEmpty(person.FirstName))
            {
                throw new Exception("A person must have a first name");
            }

            if (string.IsNullOrEmpty(person.LastName))
            {
                throw new Exception("A person must have a last name");
            }

            var check = await db.People.FirstOrDefaultAsync(x =>
                x.Id != person.Id &&
                x.FirstName.ToLower() == person.FirstName.ToLower() &&
                x.LastName.ToLower() == person.LastName.ToLower() &&
                x.Ssn == person.Ssn
            );

            if (check != null)
            {
                throw new Exception("The provided person already exists");
            }

            return true;
        }

        public static async Task<List<Person>> GetPersonAssociates(this AppDbContext db, int personId, bool isDeleted = false)
        {
            var associates = await db.Associates
                .Where(x => x.PersonId == personId)
                .Select(x => x.Associate)
                .Where(x => x.IsDeleted == isDeleted)
                .OrderBy(x => x.LastName)
                .ToListAsync();

            return associates;
        }

        public static async Task<int> GetTruePersonId(this AppDbContext db, int personId)
        {
            var check = await db.Associates.FirstOrDefaultAsync(x => x.AssociateId == personId);

            return check != null ?
                check.PersonId :
                personId;
        }

        public static async Task<Person> GetTruePerson(this AppDbContext db, int personId)
        {
            var check = await db.Associates.FirstOrDefaultAsync(x => x.AssociateId == personId);

            return check != null ?
                await db.People.FindAsync(check.PersonId) :
                await db.People.FindAsync(personId);
        }

        public static async Task<Person> GetAssociatePerson(this AppDbContext db, int associateId)
        {
            var match = await db.Associates
                .Include(x => x.Person)
                .FirstOrDefaultAsync(x => x.AssociateId == associateId);

            return match.Person;
        }

        public static async Task AddAssociateToPerson(this AppDbContext db, Person associate, int personId)
        {
            if (await associate.ValidateAssociation(db, personId))
            {
                var personAssociate = new PersonAssociate
                {
                    PersonId = personId,
                    AssociateId = associate.Id
                };

                await db.Associates.AddAsync(personAssociate);
                await db.SaveChangesAsync();
            }
        }

        public static async Task AddPersonAssociate(this AppDbContext db, Person associate, int personId)
        {
            await db.AddPerson(associate);
            await db.AddAssociateToPerson(associate, personId);
        }

        public static async Task RemovePersonAssociate(this AppDbContext db, PersonAssociate personAssociate)
        {
            db.Associates.Remove(personAssociate);
            await db.SaveChangesAsync();
        }

        public static async Task<bool> ValidateAssociation(this Person person, AppDbContext db, int personId)
        {
            var isAssociate = await db.Associates.FirstOrDefaultAsync(x => x.AssociateId == person.Id);

            if (isAssociate != null)
            {
                throw new Exception("Cannot add association for a person who is already an associate");
            }

            var isPerson = await db.Associates.FirstOrDefaultAsync(x => x.PersonId == person.Id);

            if (isPerson != null)
            {
                throw new Exception("Cannot make a person with associates an associate");
            }

            var isPersonAssociate = await db.Associates.FirstOrDefaultAsync(x => x.AssociateId == personId);

            if (isPersonAssociate != null)
            {
                throw new Exception("Cannot add an associate to another associate");
            }

            return true;
        }
        
    }
}