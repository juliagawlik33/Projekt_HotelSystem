using HotelSystem.Data;
using HotelSystem.Models;
using HotelSystem.Models.ViewModels;
using HotelSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelSystem.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly ReservationService _reservationService;

		public AdminController(ApplicationDbContext context, ReservationService reservationService)
		{
			_context = context;
			_reservationService = reservationService;
		}

		public IActionResult Reservations()
		{
			var reservations = _context.Reservations
				.Include(r => r.User)
				.Include(r => r.Room)
					.ThenInclude(room => room.RoomType)
				.OrderBy(r => r.StartDate)
				.ToList();

			var rooms = _context.Rooms
				.Include(r => r.RoomType)
				.ToList();

			ViewBag.Rooms = rooms;

			return View(reservations);
		}

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

			bool success = _reservationService.UpdateReservation(
				model.Id,
				model.RoomId,
				model.StartDate,
				model.EndDate
			);

			if (!success)
			{
				ModelState.AddModelError("RoomId", "Wybrany pokój jest już zajęty w podanym terminie.");
				ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Number", model.RoomId);
				return View(model);
			}

			_context.SaveChanges();

			TempData["Success"] = "Rezerwacja zaktualizowana.";
			return RedirectToAction("Reservations");
		}
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

		public IActionResult Rooms()
		{
			var rooms = _context.Rooms
				.Include(r => r.RoomType)
				.ToList();

			return View(rooms);
		}


		[HttpGet]
		public IActionResult AddRoom()
		{
			ViewBag.RoomTypes = new SelectList(_context.RoomTypes, "Id", "Name");
			return View(new EditRoomViewModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult AddRoom(EditRoomViewModel model)
		{
			var existingRoom = _context.Rooms
				.FirstOrDefault(r => r.Number == model.Number);

			if (existingRoom != null)
			{
				ModelState.AddModelError("Number", "Pokój o podanym numerze już istnieje.");
			}

			if (ModelState.IsValid)
			{
				var room = new Room
				{
					Number = model.Number,
					RoomTypeId = model.RoomTypeId.Value
				};

				_context.Rooms.Add(room);
				_context.SaveChanges();

				TempData["Success"] = "Pokój dodany pomyślnie.";
				return Redirect(Url.Action("Reservations", "Admin") + "#rooms");
			}

			ViewBag.RoomTypes = new SelectList(_context.RoomTypes, "Id", "Name", model.RoomTypeId);
			return View(model);
		}

		public IActionResult RoomTypes()
		{
			var types = _context.RoomTypes.ToList();
			return View(types);
		}


		[HttpGet]
		public IActionResult EditRoom(int id)
		{
			var room = _context.Rooms.Find(id);
			if (room == null) return NotFound();

			var model = new EditRoomViewModel
			{
				Id = room.Id,
				Number = room.Number,
				RoomTypeId = room.RoomTypeId
			};

			ViewBag.RoomTypes = new SelectList(_context.RoomTypes, "Id", "Name", model.RoomTypeId);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult EditRoom(EditRoomViewModel model)
		{
			var existingRoom = _context.Rooms
				.FirstOrDefault(r => r.Number == model.Number && r.Id != model.Id);

			if (existingRoom != null)
			{
				ModelState.AddModelError("Number", "Pokój o podanym numerze już istnieje.");
			}

			if (ModelState.IsValid)
			{
				var room = _context.Rooms.Find(model.Id);
				if (room == null) return NotFound();

				room.Number = model.Number;
				room.RoomTypeId = model.RoomTypeId.Value;

				_context.SaveChanges();

				TempData["Success"] = "Pokój zaktualizowany.";
				return Redirect(Url.Action("Reservations", "Admin") + "#rooms");
			}

			ViewBag.RoomTypes = new SelectList(_context.RoomTypes, "Id", "Name", model.RoomTypeId);
			return View(model);
		}


		public IActionResult DeleteRoom(int id)
		{
			var room = _context.Rooms.Find(id);
			if (room != null)
			{
				_context.Rooms.Remove(room);
				_context.SaveChanges();
				TempData["Success"] = "Pokój usunięty.";
			}
			return Redirect(Url.Action("Reservations", "Admin") + "#rooms");
		}

		public JsonResult VerifyRoomNumber(string number, int id = 0)
		{
			if (string.IsNullOrWhiteSpace(number))
				return Json(true);

			var exists = _context.Rooms.Any(r => r.Number.Trim().ToLower() == number.Trim().ToLower() && r.Id != id);

			return Json(!exists);
		}
	}
}