namespace HotelSystem.DTOs.Reservations
{
    public class CreateReservationDto
    {
        public int RoomId { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
