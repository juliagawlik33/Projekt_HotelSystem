using HotelSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace HotelSystem.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            ApplicationDbContext context,
            UserManager<User> userManager)
        {
            context.Database.EnsureCreated();

            if (!context.RoomTypes.Any())
            {
                var single = new RoomType
                {
                    Name = "Single",
                    Beds = 1,
                    PricePerNight = 150
                };

                var doubleRoom = new RoomType
                {
                    Name = "Double",
                    Beds = 2,
                    PricePerNight = 250
                };

                context.RoomTypes.AddRange(single, doubleRoom);
                await context.SaveChangesAsync();
            }

            if (!context.Rooms.Any())
            {
                var singleType = context.RoomTypes.First(r => r.Name == "Single");
                var doubleType = context.RoomTypes.First(r => r.Name == "Double");

                context.Rooms.AddRange(
                    new Room { Number = "101", RoomTypeId = singleType.Id },
                    new Room { Number = "102", RoomTypeId = singleType.Id },
                    new Room { Number = "201", RoomTypeId = doubleType.Id }
                );

                await context.SaveChangesAsync();
            }

            User testUser = await userManager.FindByEmailAsync("test@hotel.com");

            if (testUser == null)
            {
                testUser = new User
                {
                    UserName = "test@hotel.com",
                    Email = "test@hotel.com",
                    Name = "Test User",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(testUser, "Test123!");
            }

            if (!context.Reservations.Any())
            {
                var room = context.Rooms.First();

                var reservation = new Reservation
                {
                    RoomId = room.Id,
                    UserId = testUser.Id,
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(4)
                };

                context.Reservations.Add(reservation);
                await context.SaveChangesAsync();
            }
        }
    }
}