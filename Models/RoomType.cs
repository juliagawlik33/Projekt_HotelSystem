namespace HotelSystem.Models
{
    public class RoomType
    {
		public int Id { get; set; }
		public string Name { get; set; }
		public int Beds { get; set; }
		public double PricePerNight { get; set; }

		public ICollection<Room> Rooms { get; set; }
	}
}
