using System.Text;
using HotelSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace HotelSystem.Areas.Identity.Pages.Account
{
	public class RegisterConfirmationModel : PageModel
	{
		private readonly UserManager<User> _userManager;
		private readonly IEmailSender _emailSender;

		public RegisterConfirmationModel(UserManager<User> userManager, IEmailSender emailSender)
		{
			_userManager = userManager;
			_emailSender = emailSender;
		}

		[BindProperty(SupportsGet = true)]
		public string Email { get; set; }

		public bool DisplayConfirmAccountLink { get; set; } = true;

		public string EmailConfirmationUrl { get; set; }

		public async Task<IActionResult> OnGetAsync(string returnUrl = null)
		{
			if (Email == null)
			{
				return RedirectToPage("/Index");
			}

			var user = await _userManager.FindByEmailAsync(Email);
			if (user == null)
			{
				return NotFound($"Nie znaleziono u¿ytkownika o adresie '{Email}'.");
			}

			if (DisplayConfirmAccountLink)
			{
				var userId = await _userManager.GetUserIdAsync(user);
				var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				EmailConfirmationUrl = Url.Page(
					"/Account/ConfirmEmail",
					pageHandler: null,
					values: new { area = "Identity", userId, code, returnUrl },
					protocol: Request.Scheme);
			}

			return Page();
		}
	}
}