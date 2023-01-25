// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Data.ValidationAttributes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Bumbo.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IEmailSender _emailSender;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IUserStore<ApplicationUser> _userStore;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, IUserService userService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _userService = userService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                var currentUser = await _userService.GetUserAdvanced();
                var userName = Input.FirstName + Input.MiddleName + Input.LastName;
                userName = userName.Replace(" ", "");
                await _userStore.SetUserNameAsync(user, userName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.FirstName = Input.FirstName;
                user.MiddleName = Input.MiddleName;
                user.LastName = Input.LastName;
                user.Address = new Address { Zipcode = Input.Zipcode, HouseNumber = Input.HouseNumber, Street = Input.Street, City = Input.City };
                user.PhoneNumber = Input.PhoneNumber;
                user.BranchId = currentUser.BranchId;
                user.PayoutScale = Input.PayoutScale;
                user.BirthDate = Input.BirthDate;
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    if (HttpContext.User.IsInRole(nameof(Role.Admin)))
                    {
                        await _userManager.AddToRoleAsync(user, nameof(Role.BranchManager));
                    }
                    else if (HttpContext.User.IsInRole(nameof(Role.BranchManager)))
                    {
                        await _userManager.AddToRoleAsync(user, nameof(Role.Employee));
                    }

                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    // Email sender is not required for now
                    // await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                                                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                                                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }

            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required(ErrorMessage = "Verplicht veld")]
            [MaxLength(60, ErrorMessage = "Ongeldige invoer")]
            [DisplayName("Voornaam")]
            public string FirstName { get; set; }

            [MaxLength(20, ErrorMessage = "Ongeldige invoer")]
            [DisplayName("Tussenvoegsel")]
            public string MiddleName { get; set; }

            [Required(ErrorMessage = "Verplicht veld")]
            [MaxLength(60, ErrorMessage = "Ongeldige invoer")]
            [DisplayName("Achternaam")]
            public string LastName { get; set; }

            // Adress details
            [Required(ErrorMessage = "Verplicht veld")]
            [MaxLength(10, ErrorMessage = "Ongeldige invoer")]
            [DisplayName("Postcode")]
            public string Zipcode { get; set; }

            [Required(ErrorMessage = "Verplicht veld")]
            [MaxLength(100, ErrorMessage = "Ongeldige invoer")]
            [DisplayName("Straat")]
            [RegularExpression(@"^[^\W\d_]+\.?(?:[-\s][^\W\d_]+\.?)*$", ErrorMessage = "Geef een geldige straatnaam op")]
            public string Street { get; set; }

            [Required(ErrorMessage = "Verplicht veld")]
            [DisplayName("Huisnummer")]
            public int HouseNumber { get; set; }

            [Required(ErrorMessage = "Verplicht veld")]
            [MaxLength(150, ErrorMessage = "Ongeldige invoer")]
            [DisplayName("Woonplaats")]
            public string City { get; set; }

            [Required(ErrorMessage = "Verplicht veld")]
            [DisplayName("Telefoonnummer")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Verplicht veld")]
            [DisplayName("Loonschaal")]
            public int PayoutScale { get; set; }

            [Required(ErrorMessage = "Verplicht veld")]
            [DataType(DataType.Date, ErrorMessage = "Ongeldige invoer")]
            [BirthDateMayNotBeInTheFuture]
            [DisplayName("Geboortedatum")]
            public DateTime BirthDate { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Verplicht veld")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Verplicht veld")]
            [StringLength(100, ErrorMessage = "Het {0} moet tenminste {2} en maximaal {1} characters lang zijn.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Wachtwoord komt niet overeen")]
            public string ConfirmPassword { get; set; }
        }
    }
}