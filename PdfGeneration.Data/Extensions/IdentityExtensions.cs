using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PdfGeneration.Data.Entities;
using PdfGeneration.Identity;

namespace PdfGeneration.Data.Extensions
{
    public static class IdentityExtensions
    {
        public static async Task<List<User>> GetUsers(this AppDbContext db, bool isDeleted = false)
        {
            var users = await db
                .Users
                .Where(x => x.IsDeleted == isDeleted)
                .OrderBy(x => x.LastName)
                .ToListAsync();

            return users;
        }

        public static async Task<List<User>> SearchUsers(this AppDbContext db, string search, bool isDeleted = false)
        {
            search = search.ToLower();

            var users = await db
                .Users
                .Where(x => x.IsDeleted == isDeleted)
                .Where(x =>
                    x.Email.ToLower().Contains(search) ||
                    x.FirstName.ToLower().Contains(search) ||
                    x.LastName.ToLower().Contains(search) ||
                    x.Username.ToLower().Contains(search)
                )
                .OrderBy(x => x.LastName)
                .ToListAsync();

            return users;
        }

        public static async Task<User> GetUser(this AppDbContext db, int id)
        {
            var user = await db.Users.FindAsync(id);
            return user;
        }

        public static async Task<int> GetCurrentUserId(this AppDbContext db, Guid guid)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(x => x.Guid == guid);

            if (user == null)
            {
                throw new Exception("The user provided does not have an account");
            }

            return user.Id;
        }

        public static async Task<User> SyncUser(this AdUser adUser, AppDbContext db)
        {
            var user = await db
                .Users
                .FirstOrDefaultAsync(x => x.Guid == adUser.Guid);

            user = user == null ?
                await db.AddUser(adUser) :
                await db.UpdateUser(adUser);

            return user;
        }

        public static async Task<User> AddUser(this AppDbContext db, AdUser adUser)
        {
            User user = null;

            if (await adUser.Validate(db))
            {
                user = new User
                {
                    Email = adUser.UserPrincipalName,
                    FirstName = adUser.GivenName,
                    Guid = adUser.Guid.Value,
                    IsDeleted = false,
                    LastName = adUser.Surname,
                    SocketName = $@"{adUser.GetDomainPrefix()}\{adUser.SamAccountName}",
                    Theme = "light-blue",
                    Username = adUser.SamAccountName
                };

                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();
            }

            return user;
        }

        public static async Task UpdateUser(this AppDbContext db, User user)
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
        }

        static async Task<User> UpdateUser(this AppDbContext db, AdUser adUser)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Guid == adUser.Guid);

            user.Email = adUser.UserPrincipalName;
            user.FirstName = adUser.GivenName;
            user.LastName = adUser.Surname;
            user.SocketName = $@"{adUser.GetDomainPrefix()}\{adUser.SamAccountName}";
            user.Username = adUser.SamAccountName;

            await db.SaveChangesAsync();

            return user;
        }

        public static async Task ToggleUserDeleted(this AppDbContext db, User user)
        {
            db.Users.Attach(user);
            user.IsDeleted = !user.IsDeleted;
            await db.SaveChangesAsync();
        }

        public static async Task RemoveUser(this AppDbContext db, User user)
        {
            db.Users.Remove(user);
            await db.SaveChangesAsync();
        }

        public static async Task<bool> Validate(this AdUser user, AppDbContext db)
        {
            var check = await db
                .Users
                .FirstOrDefaultAsync(x => x.Guid == user.Guid.Value);

            if (check != null)
            {
                throw new Exception("The provided user already has an account");
            }

            return true;
        }
    }
}