using HotelSystem.Models;
using HotelSystem.Models.ViewModels;
using HotelSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelSystem.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly ReservationService _reservationService;
        private readonly UserManager<User> _userManager;

        public ReservationController(
            ReservationService reservationService,
            UserManager<User> userManager)
        {
            _reservationService = reservationService;
            _userManager = userManager;
        }

        public IActionResult MyReservations()
        {
			if (User.IsInRole("Admin"))
			{
				return RedirectToAction("Reservations", "Admin");
			}

			string userId = _userManager.GetUserId(User);
            var reservations = _reservationService.GetUserReservations(userId);
            return View(reservations);
        }

        public IActionResult Create(int roomId)
        {
            var rooms = _reservationService.GetSelectRooms();
              
        
            ViewBag.Rooms = new SelectList(rooms, "Id", "DisplayName");

            return View(new CreateReservationViewModel
            {
                RoomId = roomId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(3)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateReservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Rooms = new SelectList(_reservationService.GetSelectRooms(), "Id", "DisplayName", model.RoomId);

                return View(model);
            }

            string userId = _userManager.GetUserId(User);

            try
            {
                _reservationService.CreateReservation(
                    model.RoomId,
                    userId,
                    model.StartDate,
                    model.EndDate
                );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                ViewBag.Rooms = new SelectList(_reservationService.GetSelectRooms(), "Id", "DisplayName", model.RoomId);

                return View(model);
            }

            return RedirectToAction(nameof(MyReservations));
        }

        public IActionResult Delete(int id)
        {
            _reservationService.DeleteReservation(id);
            return RedirectToAction(nameof(MyReservations));
        }
    }
}
