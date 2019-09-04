using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PdfGeneration.Data.Entities;

namespace PdfGeneration.Data.Extensions
{
    public static class DbInitializer
    {
        public static async Task Initialize(this AppDbContext db)
        {
            Console.WriteLine("Initializing database");
            await db.Database.MigrateAsync();
            Console.WriteLine("Database initialized");

            List<Person> people;

            if (!(await db.People.AnyAsync()))
            {
                Console.WriteLine("Seeding people...");
                people = new List<Person>
                {
                    new Person { FirstName = "Elsie", LastName = "Hunt", Ssn = "123456757" },
                    new Person { FirstName = "Mike", LastName = "Simonetti", Ssn = "357456853" },
                    new Person { FirstName = "Kylie", LastName = "Piers", Ssn = "457234568" },
                    new Person { FirstName = "Gisselle", LastName = "Wheeler", Ssn = "142546789" },
                    new Person { FirstName = "Rebeca", LastName = "Hartman", Ssn = "109394857" },
                    new Person { FirstName = "Marvin", LastName = "Andersen", Ssn = "234643644" },
                    new Person { FirstName = "Jose", LastName = "Marinus", Ssn = "345745642" },
                    new Person { FirstName = "Kiera", LastName = "Smith", Ssn = "132537945" },
                    new Person { FirstName = "Jayden", LastName = "Sanders", Ssn = "845733256" },
                    new Person { FirstName = "Zoe", LastName = "Gonzales", Ssn = "247353858" },
                    new Person { FirstName = "Ryder", LastName = "Cooley", Ssn = "829849602" },
                    new Person { FirstName = "Kolby", LastName = "Jackson", Ssn = "253636784" },
                    new Person { FirstName = "Erwin", LastName = "Porter", Ssn = "357345343" },
                    new Person { FirstName = "Emma", LastName = "Xi", Ssn = "645756755" },
                    new Person { FirstName = "Zariah", LastName = "Hampton", Ssn = "345674567" },
                    new Person { FirstName = "Karissa", LastName = "Yan", Ssn = "345374675" },
                    new Person { FirstName = "Ellen", LastName = "West", Ssn = "856744324" },
                    new Person { FirstName = "Vanesa", LastName = "Olson", Ssn = "257346458" },
                    new Person { FirstName = "Luna", LastName = "Oneal", Ssn = "846234637" },
                    new Person { FirstName = "Caroline", LastName = "Williamson", Ssn = "210948567" }
                };

                await db.People.AddRangeAsync(people);
                await db.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Retrieving seed people...");
                people = await db.People
                    .Take(20)
                    .ToListAsync();
            }

            if (!(await db.Associates.AnyAsync()))
            {
                Console.WriteLine("Seeding person associates...");

                var associates = new List<PersonAssociate>
                {
                    new PersonAssociate
                    {
                        PersonId = people[0].Id,
                        AssociateId = people[3].Id
                    },
                    new PersonAssociate
                    {
                        PersonId = people[0].Id,
                        AssociateId = people[6].Id
                    },
                    new PersonAssociate
                    {
                        PersonId = people[1].Id,
                        AssociateId = people[4].Id
                    },
                    new PersonAssociate
                    {
                        PersonId = people[2].Id,
                        AssociateId = people[5].Id
                    }
                };

                await db.Associates.AddRangeAsync(associates);
                await db.SaveChangesAsync();
            }
        }
    }
}