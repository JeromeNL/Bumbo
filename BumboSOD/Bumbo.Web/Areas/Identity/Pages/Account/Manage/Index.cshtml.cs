// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Bumbo.Data;
using Bumbo.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Web.Areas.Identity.Pages.Account.Manage;

public class IndexModel : PageModel
{
    private readonly BumboDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        BumboDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [DisplayName("Gebruikersnaam")]
    public string Username { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [TempData]
    public string StatusMessage { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    private async Task LoadAsync(ApplicationUser user)
    {
        var userName = await _userManager.GetUserNameAsync(user);
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        var firstName = user.FirstName;
        var middleName = user.MiddleName;
        var lastName = user.LastName;
        var zipcode = await _context.Users.Include(u => u.Address).Where(u => u.Id == user.Id)
            .Select(u => u.Address.Zipcode).FirstOrDefaultAsync();
        var houseNumber = await _context.Users.Include(u => u.Address).Where(u => u.Id == user.Id)
            .Select(u => u.Address.HouseNumber).FirstOrDefaultAsync();
        var city = await _context.Users.Include(u => u.Address).Where(u => u.Id == user.Id).Select(u => u.Address.City)
            .FirstOrDefaultAsync();
        Username = userName;

        Input = new InputModel
        {
            PhoneNumber = phoneNumber,
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
            Zipcode = zipcode,
            HouseNumber = houseNumber,
            City = city
        };
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (Input.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set phone number.";
                return RedirectToPage();
            }
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class InputModel
    {
        [Required]
        [MaxLength(60)]
        [DisplayName("Voornaam")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        [DisplayName("Tussenvoegsel")]
        public string MiddleName { get; set; }

        [Required]
        [MaxLength(60)]
        [DisplayName("Achternaam")]
        public string LastName { get; set; }

        // Adress details
        [Required]
        [MaxLength(10)]
        [DisplayName("Postcode")]
        public string Zipcode { get; set; }

        [Required]
        [DisplayName("Huisnummer")]
        public int HouseNumber { get; set; }

        [Required]
        [MaxLength(150)]
        [DisplayName("Woonplaats")]
        public string City { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Phone]
        [Display(Name = "Telefoonnummer")]
        public string PhoneNumber { get; set; }
    }
}