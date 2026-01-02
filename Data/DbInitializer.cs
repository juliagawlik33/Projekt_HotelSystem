using HotelSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelSystem.Data
{
	public class DbInitializer
	{
		public static async Task InitializeAsync(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
		{
			await context.Database.MigrateAsync();

			if (context.Rooms.Any())
			{
				return;
			}

			// ===== ROOM TYPES =====
			var single = new RoomType
			{
				Name = "Single",
				Beds = 1,
				PricePerNight = 100
			};

			var doubleRoom = new RoomType
			{
				Name = "Double",
				Beds = 2,
				PricePerNight = 150
			};

			var suite = new RoomType
			{
				Name = "Suite",
				Beds = 3,
				PricePerNight = 300
			};

			context.RoomTypes.AddRange(single, doubleRoom, suite);
			await context.SaveChangesAsync();

			// ===== ROOMS =====
			context.Rooms.AddRange(
				new Room { Number = "101", RoomTypeId = single.Id },
				new Room { Number = "102", RoomTypeId = doubleRoom.Id },
				new Room { Number = "201", RoomTypeId = suite.Id }
			);

			await context.SaveChangesAsync();


			// ===== USERS =====
			if (!await context.Users.AnyAsync())
			{
				var testUser = new User
				{
					UserName = "user@test.com",
					Email = "user@test.com",
					Name = "USER",
					EmailConfirmed = true
				};
				await userManager.CreateAsync(testUser, "TestUser123!");

				var adminUser = new User
				{
					UserName = "admin@admin.com",
					Email = "admin@admin.com",
					Name = "ADMIN",
					EmailConfirmed = true
				};
				await userManager.CreateAsync(adminUser, "TestAdmin123!");
			}

			// ===== ROLES =====
			if (!await roleManager.RoleExistsAsync("Admin"))
			{
				await roleManager.CreateAsync(new IdentityRole("Admin"));
			}
			var admin = await userManager.FindByEmailAsync("admin@admin.com");
			if (admin != null && !await userManager.IsInRoleAsync(admin, "Admin"))
			{
				await userManager.AddToRoleAsync(admin, "Admin");
			}

		}
	}
}