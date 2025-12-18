using HotelSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelSystem.Areas.Identity.Data
{
    public class DbInitializer
    {
		public static void Initialize(DbContext context)
		{
			context.Database.Migrate();

			// jeśli są pokoje to seed już był
			if (context.Rooms.Any()) return;

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
			context.SaveChanges();

			// ===== ROOMS =====
			context.Rooms.AddRange(
				new Room { Number = "101", RoomTypeId = single.Id },
				new Room { Number = "102", RoomTypeId = doubleRoom.Id },
				new Room { Number = "201", RoomTypeId = suite.Id }
			);

			// ===== USERS (Guest + Admin) =====
			context.Users.AddRange(
				new User
				{
					Username = "admin",
					Password = "admin",
					Name = "Administrator",
					Email = "admin@hotel.local",
					IsAdmin = true
				},
				new User
				{
					Username = "user",
					Password = "user",
					Name = "Jan Kowalski",
					Email = "user@hotel.local",
					IsAdmin = false
				}
			);

			context.SaveChanges();
		}
	}
}
