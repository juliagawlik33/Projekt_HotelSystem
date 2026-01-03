using System.ComponentModel.DataAnnotations;
namespace HotelSystem.Models.ViewModels
{
    public class EditRoomViewModel
    {
		public int Id { get; set; }

		[Required(ErrorMessage = "Numer pokoju jest wymagany.")]
		[Display(Name = "Numer pokoju")]
		public string Number { get; set; }

		[Required(ErrorMessage = "Typ pokoju jest wymagany.")]
		[Display(Name = "Typ pokoju")]
		public int RoomTypeId { get; set; }
	}
}
