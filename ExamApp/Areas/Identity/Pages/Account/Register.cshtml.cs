// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using ExamApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ExamApplicationUser> _signInManager;
        private readonly UserManager<ExamApplicationUser> _userManager;
        private readonly IUserStore<ExamApplicationUser> _userStore;

        public RegisterModel(
            UserManager<ExamApplicationUser> userManager,
            IUserStore<ExamApplicationUser> userStore,
            SignInManager<ExamApplicationUser> signInManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(255, ErrorMessage = "The first name field should have a maximum of 255 characters")]
            [Display(Name = "FirstName")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(255, ErrorMessage = "The last name field should have a maximum of 255 characters")]
            [Display(Name = "LastName")]
            public string LastName { get; set; }

            [Required]
            [StringLength(255)]
            [Display(Name = "UserName")]
            public string username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
        }


        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                // Custom properties
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;

                await _userStore.SetUserNameAsync(user, Input.username, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);

                    //if (await _userManager.Users.CountAsync() == 1)
                    //{
                    //    await _userManager.AddToRoleAsync(user, "Admin");
                    //}
                    //else
                    //{
                    //    await _userManager.AddToRoleAsync(user, "User");
                    //}

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect("/");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ExamApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ExamApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ExamApplicationUser)}'. " +
                    $"Ensure that '{nameof(ExamApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }
}
