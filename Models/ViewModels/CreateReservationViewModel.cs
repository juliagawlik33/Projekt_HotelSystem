using System.ComponentModel.DataAnnotations;


namespace HotelSystem.Models.ViewModels
{
    public class CreateReservationViewModel
    {
        [Required]
        public int RoomId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

    }
}
