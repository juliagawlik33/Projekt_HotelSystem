using HotelSystem.Data;
using HotelSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelSystem.Services
{
	public class AdminService
	{
		private readonly ApplicationDbContext _context;
		public AdminService(ApplicationDbContext context)
		{
			_context = context;
		}
		public List<Room> GetAllRooms()
		{
			return _context.Rooms
				.Include(r => r.RoomType)
				.ToList();
		}

		public Room? GetRoomById(int id)
		{
			return _context.Rooms.Find(id);
		}

		public void AddRoom(Room room)
		{
			_context.Rooms.Add(room);
			_context.SaveChanges();
		}

		public bool UpdateRoom(Room room)
		{
			var existing = _context.Rooms.Find(room.Id);
			if (existing == null) return false;

			existing.Number = room.Number;
			existing.RoomTypeId = room.RoomTypeId;

			_context.SaveChanges();
			return true;
		}

		public bool DeleteRoom(int id)
		{
			var room = _context.Rooms.Find(id);
			if (room == null) return false;

			_context.Rooms.Remove(room);
			_context.SaveChanges();
			return true;
		}

		public bool IsRoomNumberUnique(string number, int excludeId = 0)
		{
			if (string.IsNullOrWhiteSpace(number))
				return true;

			var normalizedNumber = number.Trim().ToLower();

			return !_context.Rooms
				.Where(r => r.Id != excludeId)
				.Any(r => r.Number.Trim().ToLower() == normalizedNumber);
		}

		public List<RoomType> GetAllRoomTypes()
		{
			return _context.RoomTypes.ToList();
		}

		public async Task<bool> UpdateRoomTypePrices(List<RoomType> updatedTypes)
		{
			bool anyChange = false;

			foreach (var updated in updatedTypes)
			{
				var existing = await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.Id == updated.Id);
				if (existing != null && updated.PricePerNight != existing.PricePerNight)
				{
					existing.PricePerNight = updated.PricePerNight;
					anyChange = true;
				}
			}

			if (anyChange)
			{
				await _context.SaveChangesAsync();
			}

			return anyChange;
		}

	}
}