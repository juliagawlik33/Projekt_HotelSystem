namespace HotelSystem.Models.ViewModels
{
	public class EditReservationViewModel
	{
		public int Id { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int RoomId { get; set; }
	}
}