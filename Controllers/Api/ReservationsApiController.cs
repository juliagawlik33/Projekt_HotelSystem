using HotelSystem.Data;
using HotelSystem.DTOs.Reservations;
using HotelSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace HotelSystem.Controllers.Api
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task <IActionResult> GetReservations()
        {
            var reservations = await _context.Reservations
                .AsNoTracking()
                .ToListAsync();
            return Ok(reservations);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var reservation = await _context.Reservations
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            return Ok(reservation);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateReservationDto dto)
        {
            var reservation = new Reservation
            {
                RoomId = dto.RoomId,
                UserId = dto.UserId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = reservation.Id },
                reservation
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateReservationDto dto)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            reservation.RoomId = dto.RoomId;
            reservation.StartDate = dto.StartDate;
            reservation.EndDate = dto.EndDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
