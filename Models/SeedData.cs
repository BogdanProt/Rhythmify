﻿using Rhythmify.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rhythmify.Models;

// PASUL 4 - useri si roluri

namespace Rhythmify.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService
                <DbContextOptions<ApplicationDbContext>>()))
            {
                // Verificam daca in baza de date exista cel putin un rol
                // insemnand ca a fost rulat codul 
                // De aceea facem return pentru a nu insera rolurile inca o data
                // Acesta metoda trebuie sa se execute o singura data 
                if (context.Roles.Any())
                {
                    return;   // baza de date contine deja roluri
                }

                // CREAREA ROLURILOR IN BD
                // daca nu contine roluri, acestea se vor crea
                context.Roles.AddRange(
                    new IdentityRole { Id = "92a00e19-b450-4e34-82ec-01f3b21c129e", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                    new IdentityRole { Id = "02b7c2dc-cc8d-4558-ab46-f6788a970b60", Name = "User", NormalizedName = "User".ToUpper() }
                );

                // o noua instanta pe care o vom utiliza pentru crearea parolelor utilizatorilor
                // parolele sunt de tip hash
                var hasher = new PasswordHasher<User>();

                // CREAREA USERILOR IN BD
                // Se creeaza cate un user pentru fiecare rol
                context.Users.AddRange(
                    new User
                    {
                        Id = "ae70cc1f-832f-4f3b-99ab-e4b23ebac7fb", // primary key
                        UserName = "admin@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "ADMIN@TEST.COM",
                        Email = "admin@test.com",
                        NormalizedUserName = "ADMIN@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Admin1!")
                    },
                    new User
                    {
                        Id = "11278279-873d-4a11-8d3f-18fda1ecf99d", // primary key
                        UserName = "user@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "USER@TEST.COM",
                        Email = "user@test.com",
                        NormalizedUserName = "USER@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "User1!")
                    }
                );

                // ASOCIEREA USER-ROLE
                context.UserRoles.AddRange(
                    new IdentityUserRole<string>
                    {
                        RoleId = "92a00e19-b450-4e34-82ec-01f3b21c129e",
                        UserId = "ae70cc1f-832f-4f3b-99ab-e4b23ebac7fb"
                    },
                    new IdentityUserRole<string>
                    {
                        RoleId = "02b7c2dc-cc8d-4558-ab46-f6788a970b60",
                        UserId = "11278279-873d-4a11-8d3f-18fda1ecf99d"
                    }
                );

                context.SaveChanges();

            }
        }
    }
}