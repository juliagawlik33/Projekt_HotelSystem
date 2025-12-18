using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace HotelSystem.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
	public int Id { get; set; }
	public string Username { get; set; }
	public string Password { get; set; } // na początek plain text
	public string Name { get; set; }
	public string Email { get; set; }
	public ICollection<Reservation> Reservations { get; set; }

	public bool IsAdmin { get; set; }
}

