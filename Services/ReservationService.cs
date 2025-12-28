using HotelSystem.Data;
using HotelSystem.Models;
using HotelSystem.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace HotelSystem.Services
{
    public class ReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sprawdzanie dostępności pokoju

        public bool IsRoomAvailable(int roomId, DateTime startDate, DateTime endDate)
        {
            return !_context.Reservations.Any(r =>
                r.RoomId == roomId &&
                r.StartDate < endDate &&
                r.EndDate > startDate
            );
        }

        // Dostępne pokoje w danym terminie

        public List<Room> GetAvailableRooms(DateTime startDate, DateTime endDate)
        {
            return _context.Rooms
               .Include(r => r.RoomType)
               .Where(r => !_context.Reservations.Any(res =>
                    res.RoomId == r.Id &&
                    startDate < res.EndDate &&
                    endDate > res.StartDate
                ))
                .ToList();
        }
        public List<RoomSelectViewModel> GetSelectRooms()
        {
            return _context.Rooms
                .Include(r => r.RoomType)
                .Select(r => new RoomSelectViewModel
                {
                    Id = r.Id,
                    DisplayName = $"Pokój {r.Number} - {r.RoomType.Name} ({r.RoomType.PricePerNight} zł/noc)"
                })     
                .ToList();
        }

        // Tworzenie rezerwacji
        public bool CreateReservation(int roomId, string userId, DateTime startDate, DateTime endDate)
        {
            if (!IsRoomAvailable(roomId, startDate, endDate))
            {
                throw new InvalidOperationException("Pokój nie jest dostępny w wybranym terminie.");
            }

            var reservation = new Reservation
            {
                RoomId = roomId,
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            };
            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return true;

        }

        //Pobieranie rezerwacji użytkownika
        public List<Reservation> GetUserReservations(string userId)
        {
            return _context.Reservations
                .Include(r => r.Room)
                .ThenInclude(r => r.RoomType)
                .Where(r => r.UserId == userId)
                .ToList();
        }


        //Usuwanie rezerwacji
        public void DeleteReservation (int reservationId)
        {
            var reservation = _context.Reservations.Find(reservationId);
            if (reservation == null) return;

            _context.Reservations.Remove(reservation);
            _context.SaveChanges();
        }
        
        //Edycja rezerwacji
        public bool UpdateReservation(int reservationId, int roomId, DateTime newStartDate, DateTime newEndDate)
        {
            var reservation = _context.Reservations.Find(reservationId);
            if (reservation == null) return false;


            bool isAvailable = !_context.Reservations.Any(r =>
                r.RoomId == roomId &&
                r.Id != reservationId &&
                newStartDate < r.EndDate &&
                newEndDate > r.StartDate
            );

            if (!isAvailable) return false;

            reservation.RoomId = roomId;
            reservation.StartDate = newStartDate;
            reservation.EndDate = newEndDate;

            _context.SaveChanges();
            return true;
        }

        
    }
}
