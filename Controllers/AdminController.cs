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
		private readonly AdminService _adminService;
		private readonly ReservationService _reservationService;

		public AdminController(AdminService adminService, ReservationService reservationService)
		{
			_adminService = adminService;
			_reservationService = reservationService;
		}

		public IActionResult Reservations()
		{
			var reservations = _reservationService.GetAdminReservations();
			var rooms = _adminService.GetAllRooms();
			ViewBag.Rooms = rooms;

			return View(reservations);
		}

		[HttpGet]
		public IActionResult EditReservation(int id)
		{
			var reservation = _reservationService.GetReservationById(id);
			if (reservation == null) return NotFound();

			ViewBag.Rooms = new SelectList(_reservationService.GetAllRoomsForSelect(), "Id", "Number", reservation.RoomId);

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
				ViewBag.Rooms = new SelectList(_reservationService.GetAllRoomsForSelect(), "Id", "Number", model.RoomId);
				return View(model);
			}

			bool success = _reservationService.UpdateReservation(
				model.Id,
				model.RoomId,
				model.StartDate,
				model.EndDate
			);

			if (!success)
			{
				ModelState.AddModelError("RoomId", "Wybrany pokój jest już zajęty w podanym terminie.");
				ViewBag.Rooms = new SelectList(_reservationService.GetAllRoomsForSelect(), "Id", "Number", model.RoomId);
				return View(model);
			}

			TempData["Success"] = "Rezerwacja zaktualizowana.";
			return RedirectToAction("Reservations");
		}
		public IActionResult DeleteReservation(int id)
		{
			if (_reservationService.DeleteReservationAdmin(id))
			{
				TempData["Success"] = "Rezerwacja usunięta.";
			}

			return Redirect("/Admin/Reservations#rooms");
		}

		[HttpGet]
		public IActionResult AddRoom()
		{
			ViewBag.RoomTypes = new SelectList(_adminService.GetAllRoomTypes(), "Id", "Name");
			return View(new EditRoomViewModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult AddRoom(EditRoomViewModel model)
		{
			if (!_adminService.IsRoomNumberUnique(model.Number))
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

				_adminService.AddRoom(room);
				TempData["Success"] = "Pokój dodany pomyślnie.";
				return Redirect("/Admin/Reservations#rooms");
			}

			ViewBag.RoomTypes = new SelectList(_adminService.GetAllRoomTypes(), "Id", "Name", model.RoomTypeId);
			return View(model);
		}

		public IActionResult RoomTypes()
		{
			var types = _adminService.GetAllRoomTypes();
			return View(types);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RoomTypes(List<RoomType> roomTypes)
		{
			if (roomTypes == null || !roomTypes.Any())
			{
				return RedirectToAction("RoomTypes");
			}

			bool changed = await _adminService.UpdateRoomTypePrices(roomTypes);

			if (changed)
			{
				TempData["Success"] = "Ceny pokoi zostały zaktualizowane.";
			}
			else
			{
				TempData["Info"] = "Brak zmian do zapisania.";
			}

			return RedirectToAction("RoomTypes");
		}

		[HttpGet]
		public IActionResult EditRoom(int id)
		{
			var room = _adminService.GetRoomById(id);
			if (room == null) return NotFound();

			var model = new EditRoomViewModel
			{
				Id = room.Id,
				Number = room.Number,
				RoomTypeId = room.RoomTypeId
			};

			ViewBag.RoomTypes = new SelectList(_adminService.GetAllRoomTypes(), "Id", "Name", model.RoomTypeId);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult EditRoom(EditRoomViewModel model)
		{
			if (!_adminService.IsRoomNumberUnique(model.Number, model.Id))
			{
				ModelState.AddModelError("Number", "Pokój o podanym numerze już istnieje.");
			}

			if (ModelState.IsValid)
			{
				var room = _adminService.GetRoomById(model.Id);
				if (room == null) return NotFound();

				room.Number = model.Number;
				room.RoomTypeId = model.RoomTypeId.Value;

				if (_adminService.UpdateRoom(room))
				{
					TempData["Success"] = "Pokój zaktualizowany.";
				}

				return Redirect("/Admin/Reservations#rooms");
			}

			ViewBag.RoomTypes = new SelectList(_adminService.GetAllRoomTypes(), "Id", "Name", model.RoomTypeId);
			return View(model);
		}

		public IActionResult DeleteRoom(int id)
		{
			if (_adminService.DeleteRoom(id))
			{
				TempData["Success"] = "Pokój usunięty.";
			}
			return Redirect("/Admin/Reservations#rooms");
		}

		public JsonResult VerifyRoomNumber(string number, int id = 0)
		{
			return Json(_adminService.IsRoomNumberUnique(number, id));
		}
	}
}