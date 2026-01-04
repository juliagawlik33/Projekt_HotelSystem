using Microsoft.AspNetCore.Identity;

namespace HotelSystem.Services
{
	public class ErrorDescriber : IdentityErrorDescriber
	{
		public override IdentityError DuplicateEmail(string email)
		{
			return new IdentityError
			{
				Code = nameof(DuplicateEmail),
				Description = $"Ten adres e-mail '{email}' jest już zarejestrowany."
			};
		}

		public override IdentityError DuplicateUserName(string userName)
		{
			return new IdentityError
			{
				Code = nameof(DuplicateUserName),
				Description = $"Ten adres e-mail '{userName}' jest już zarejestrowany."
			};
		}

		public override IdentityError PasswordRequiresNonAlphanumeric()
		{
			return new IdentityError
			{
				Code = nameof(PasswordRequiresNonAlphanumeric),
				Description = "Hasło musi zawierać co najmniej jeden znak niealfanumeryczny (np. ! @ # $ % ^ & * ( ) - _ + =)."
			};
		}

		public override IdentityError PasswordRequiresDigit()
		{
			return new IdentityError
			{
				Code = nameof(PasswordRequiresDigit),
				Description = "Hasło musi zawierać co najmniej jedną cyfrę (0-9)."
			};
		}

		public override IdentityError PasswordRequiresLower()
		{
			return new IdentityError
			{
				Code = nameof(PasswordRequiresLower),
				Description = "Hasło musi zawierać co najmniej jedną małą literę (a-z)."
			};
		}

		public override IdentityError PasswordRequiresUpper()
		{
			return new IdentityError
			{
				Code = nameof(PasswordRequiresUpper),
				Description = "Hasło musi zawierać co najmniej jedną wielką literę (A-Z)."
			};
		}
	}
}