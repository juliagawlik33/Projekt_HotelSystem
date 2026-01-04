using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace HotelSystem.Models
{
    public class Room
    {
		public int Id { get; set; }

		[Required(ErrorMessage = "Numer pokoju jest wymagany.")]
		[StringLength(5, ErrorMessage = "Numer pokoju może mieć maksymalnie 5 znaków.")]
		[Remote(action: "VerifyRoomNumber", controller: "Admin", AdditionalFields = "Id", ErrorMessage = "Pokój o podanym numerze już istnieje.")]
		public string Number { get; set; }

		public int RoomTypeId { get; set; }
		public RoomType RoomType { get; set; }

		public ICollection<Reservation> Reservations { get; set; }
	}
}
