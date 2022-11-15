using FluentValidation;
using Kerbero.Identity.Common;
using Kerbero.Identity.Modules.Authentication.Services;
using Kerbero.Identity.Modules.Claims.Services;
using Kerbero.Identity.Modules.Notifier.Services;
using Kerbero.Identity.Modules.Roles.Entities;
using Kerbero.Identity.Modules.Roles.Services;
using Kerbero.Identity.Modules.Users.Entities;
using Kerbero.Identity.Modules.Users.Services;
using Kerbero.Identity.Modules.Users.Validators;
using Kerbero.Identity.Library.Modules.Users.Dtos;
using Kerbero.Identity.Library.Modules.Users.Models;
using Kerbero.Identity.Modules.Email.Options;
using Kerbero.Identity.Modules.Email.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid.Extensions.DependencyInjection;

namespace Kerbero.Identity.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static void AddKerberoIdentity<TDbContext>(this IServiceCollection services,
		ConfigurationManager configurationManager,
		KerberoIdentityConfiguration kerberoIdentityConfiguration,
		KerberoIdentityServicesOptions kuIdentityOptions)
		where TDbContext : KerberoIdentityDbContext
	{
		if (kerberoIdentityConfiguration is null)
		{
			throw new ArgumentNullException(nameof(kerberoIdentityConfiguration));
		}

		services.AddSingleton(kerberoIdentityConfiguration);

		services.AddIdentity<User, Role>(options =>
			{
				// Password settings.
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = true;
				options.Password.RequiredLength = 6;
				options.Password.RequiredUniqueChars = 1;

				// User settings.
				options.User.RequireUniqueEmail = true;

				// SignIn settings.
				options.SignIn.RequireConfirmedEmail = true;
			})
			.AddEntityFrameworkStores<TDbContext>()
			.AddDefaultTokenProviders();

		services.ConfigureApplicationCookie(options =>
		{
			options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			options.Cookie.HttpOnly = true;
			options.Cookie.SameSite = SameSiteMode.Strict;
			options.ExpireTimeSpan = TimeSpan.FromDays(1);
			options.Events.OnRedirectToLogin = context =>
			{
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				return Task.CompletedTask;
			};
			options.Events.OnRedirectToAccessDenied = context =>
			{
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				return Task.CompletedTask;
			};
		});

		services.AddAuthorization(options =>
		{
			options.AddKuIdentityPolicies();

			kuIdentityOptions.AuthorizationOptionsConfigure?.Invoke(options);
		});

		// our services
		services.AddScoped<ITokenHelper, TokenHelper>();
		services.AddScoped<IAuthenticationService, AuthenticationService>();
		services.AddScoped<IAuthenticationManager, AuthenticationManager>();
		services.AddScoped<IAuthenticationHelper, AuthenticationHelper>();

		services.AddSingleton<IValidator<IHavePassword>, PasswordValidator>();
		services.AddSingleton<IValidator<UserCreateDto>, UserCreateDtoValidator>();
		services.AddSingleton<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();
		
		services.AddSingleton<IValidator<IHavePassword>, PasswordValidator>();
		services.AddSingleton<IValidator<UserCreateDto>, UserCreateDtoValidator>();
		services.AddSingleton<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();

		services.AddScoped<IEmailSenderService, EmailSenderService>();
		services.AddSendGrid(options => { options.ApiKey = kerberoIdentityConfiguration.SendGridKey; });
		services.Configure<EmailSenderServiceOptions>(
			configurationManager.GetSection(key: nameof(EmailSenderServiceOptions)));

		services.AddScoped<IUserService, UserService>();
		services.AddScoped<IUserManager, UserManager>();
		
		services.AddScoped<IRoleService, RoleService>();
		services.AddScoped<IRoleManager, RoleManager>();

		services.AddScoped<IClaimService, ClaimService>();
		services.AddSingleton<IClaimManager>(ClaimManager.Create(kuIdentityOptions.Claims));

		services.AddSingleton<IKerberoIdentityNotifier, KerberoIdentityNotifier>();
	}
}
