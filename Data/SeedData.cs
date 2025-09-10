using LaboGrocery.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LaboGrocery.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Roles
            string[] roles = new[] { "Admin", "Staff" };
            foreach (var role in roles)
                if (!await roleMgr.RoleExistsAsync(role))
                    await roleMgr.CreateAsync(new IdentityRole(role));

            // Admin
            var adminEmail = "admin@local.test";
            var admin = await userMgr.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await userMgr.CreateAsync(admin, "Admin#123"); // Cambia en prod
                await userMgr.AddToRoleAsync(admin, "Admin");
            }

            // Languages
            if (!await context.Languages.AnyAsync())
            {
                context.Languages.AddRange(
                    new Language { Culture = "fr-FR", DisplayName = "Français" },
                    new Language { Culture = "en-US", DisplayName = "English" }
                );
            }

            // Categories + Products
            if (!await context.Categories.AnyAsync())
            {
                var catLait = new Category { Name = "Produits laitiers" };
                var catViande = new Category { Name = "Viandes" };
                var catBoul = new Category { Name = "Boulangerie" };
                var catFruits = new Category { Name = "Fruits & Légumes" };

                context.Categories.AddRange(catLait, catViande, catBoul, catFruits);
                await context.SaveChangesAsync();

                context.Products.AddRange(
                    new Product { CategoryId = catLait.Id, Name = "Lait bio 1L", Description = "Lait de ferme local", Price = 3.49m, ImagePath = "/images/products/lait.png" },
                    new Product { CategoryId = catViande.Id, Name = "Poulet fermier", Description = "Poulet élevé en plein air (~1.2kg)", Price = 12.99m, ImagePath = "/images/products/viande.png" },
                    new Product { CategoryId = catBoul.Id, Name = "Pain complet", Description = "Pain artisanal", Price = 2.49m, ImagePath = "/images/products/boulangerie.png" },
                    new Product { CategoryId = catFruits.Id, Name = "Panier fruits & légumes", Description = "Sélection saisonnière", Price = 15.00m, ImagePath = "/images/products/fruits.png" }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}
