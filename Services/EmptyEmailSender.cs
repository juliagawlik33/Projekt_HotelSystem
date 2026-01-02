using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace HotelSystem.Services
{
	public class EmptyEmailSender : IEmailSender
	{
		public Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			System.Diagnostics.Debug.WriteLine($"WYSYŁKA MAIL: Do: {email}, Temat: {subject}, Treść: {htmlMessage}");
			return Task.CompletedTask;
		}
	}
}