using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelSystem.Data;
using HotelSystem.Models;
using HotelSystem.Models.ViewModels;

namespace HotelSystem.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		private readonly ApplicationDbContext _context;

		public AdminController(ApplicationDbContext context)
		{
			_context = context;
		}

		//wszystkie rezerwacje
		public IActionResult Reservations()
		{
			var reservations = _context.Reservations
				.Include(r => r.User)
				.Include(r => r.Room)
					.ThenInclude(room => room.RoomType)
				.OrderBy(r => r.StartDate)
				.ToList();

			return View(reservations);
		}

		//edycja rezerwacji - GET
		[HttpGet]
		public IActionResult EditReservation(int id)
		{
			var reservation = _context.Reservations
				.Include(r => r.Room)
				.FirstOrDefault(r => r.Id == id);

			if (reservation == null) return NotFound();

			ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Number", reservation.RoomId);

			var model = new EditReservationViewModel
			{
				Id = reservation.Id,
				StartDate = reservation.StartDate,
				EndDate = reservation.EndDate,
				RoomId = reservation.RoomId
			};

			return View(model);
		}

		//edycja rezerwacji - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult EditReservation(EditReservationViewModel model)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Number", model.RoomId);
				return View(model);
			}

			var reservation = _context.Reservations.Find(model.Id);
			if (reservation == null) return NotFound();

			reservation.StartDate = model.StartDate;
			reservation.EndDate = model.EndDate;
			reservation.RoomId = model.RoomId;

			_context.SaveChanges();

			TempData["Success"] = "Rezerwacja zaktualizowana.";
			return RedirectToAction("Reservations");
		}

		//usuwanie rezerwacji
		public IActionResult DeleteReservation(int id)
		{
			var reservation = _context.Reservations.Find(id);
			if (reservation != null)
			{
				_context.Reservations.Remove(reservation);
				_context.SaveChanges();
				TempData["Success"] = "Rezerwacja usunięta.";
			}

			return RedirectToAction("Reservations");
		}

		//lista pokoi i link do dodawania
		public IActionResult Rooms()
		{
			var rooms = _context.Rooms
				.Include(r => r.RoomType)
				.ToList();

			return View(rooms);
		}

		//dodaj nowy pokój - GET
		[HttpGet]
		public IActionResult AddRoom()
		{
			ViewBag.RoomTypes = new SelectList(_context.RoomTypes, "Id", "Name");
			return View();
		}

		//dodaj nowy pokój - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult AddRoom(Room room)
		{
			if (ModelState.IsValid)
			{
				_context.Rooms.Add(room);
				_context.SaveChanges();
				TempData["Success"] = "Pokój dodany.";
				return RedirectToAction("Rooms");
			}

			ViewBag.RoomTypes = new SelectList(_context.RoomTypes, "Id", "Name", room.RoomTypeId);
			return View(room);
		}

		//lista typów pokoi
		public IActionResult RoomTypes()
		{
			var types = _context.RoomTypes.ToList();
			return View(types);
		}

		//edytuj cenę pokoju - GET
		[HttpGet]
		public IActionResult EditRoomType(int id)
		{
			var type = _context.RoomTypes.Find(id);
			if (type == null) return NotFound();
			return View(type);
		}

		//edytuj cenę pokoju - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult EditRoomType(RoomType type)
		{
			if (ModelState.IsValid)
			{
				_context.RoomTypes.Update(type);
				_context.SaveChanges();
				TempData["Success"] = "Cena zaktualizowana.";
				return RedirectToAction("RoomTypes");
			}
			return View(type);
		}
	}
}