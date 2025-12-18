namespace HotelSystem.Models
{
    public class Room
    {
		public int Id { get; set; }
		public string Number { get; set; }

		public int RoomTypeId { get; set; }
		public RoomType RoomType { get; set; }

		public ICollection<Reservation> Reservations { get; set; }
	}
}
