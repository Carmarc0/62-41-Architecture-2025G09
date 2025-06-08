using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Services;

namespace MVC.Controllers
{
    /// <summary>
    /// Admin interface controller for faculty quota management
    /// </summary>
    public class FacultiesController : Controller
    {
        private readonly IApiService _apiService;

        public FacultiesController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // Display admin login page
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        // Handle admin authentication
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            bool isAuthenticated = await _apiService.AuthenticateAdminAsync(model.Username, model.Password);

            if (isAuthenticated)
            {
                HttpContext.Session.SetString("AdminUsername", model.Username);
                return RedirectToAction("AddQuota");
            }
            else
            {
                model.ErrorMessage = "Invalid admin credentials";
                return View("Index", model);
            }
        }

        // Display quota management interface
        public async Task<IActionResult> AddQuota()
        {
            var adminUsername = HttpContext.Session.GetString("AdminUsername");
            if (string.IsNullOrEmpty(adminUsername))
            {
                return RedirectToAction("Index");
            }

            ViewBag.AdminUsername = adminUsername;

            // Load user list automatically
            try
            {
                var users = await _apiService.GetAllUsersAsync();
                ViewBag.Users = users;
            }
            catch (Exception ex)
            {
                ViewBag.UsersError = ex.Message;
            }

            return View(new AddQuotaViewModel());
        }

        // Process quota addition
        [HttpPost]
        public async Task<IActionResult> AddQuota(AddQuotaViewModel model)
        {
            var adminUsername = HttpContext.Session.GetString("AdminUsername");
            if (string.IsNullOrEmpty(adminUsername))
            {
                return RedirectToAction("Index");
            }

            ViewBag.AdminUsername = adminUsername;

            if (!ModelState.IsValid)
            {
                // Reload user list on error
                try
                {
                    var users = await _apiService.GetAllUsersAsync();
                    ViewBag.Users = users;
                }
                catch (Exception ex)
                {
                    ViewBag.UsersError = ex.Message;
                }
                return View(model);
            }

            try
            {
                // Verify user exists before adding quota
                bool userExists = await _apiService.CheckUserExistsAsync(model.Username);
                if (!userExists)
                {
                    model.ErrorMessage = $"User '{model.Username}' does not exist in the system. Only existing users can receive quotas.";

                    // Reload user list
                    try
                    {
                        var users = await _apiService.GetAllUsersAsync();
                        ViewBag.Users = users;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.UsersError = ex.Message;
                    }
                    return View(model);
                }

                bool success = await _apiService.AddSemesterQuotaAsync(model.Username, (double)model.Amount);

                if (success)
                {
                    model.SuccessMessage = $"Semester quota of {model.Amount} CHF successfully added to {model.Username}";
                    model.Username = string.Empty;
                    model.Amount = 0;
                }
                else
                {
                    model.ErrorMessage = "Error adding quota. Please try again.";
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Technical error: {ex.Message}";
            }

            // Reload user list after operation
            try
            {
                var users = await _apiService.GetAllUsersAsync();
                ViewBag.Users = users;
            }
            catch (Exception ex)
            {
                ViewBag.UsersError = ex.Message;
            }

            return View(model);
        }

        // AJAX endpoint to refresh user list
        [HttpGet]
        public async Task<IActionResult> RefreshUsers()
        {
            var adminUsername = HttpContext.Session.GetString("AdminUsername");
            if (string.IsNullOrEmpty(adminUsername))
            {
                return RedirectToAction("Index");
            }

            try
            {
                var users = await _apiService.GetAllUsersAsync();
                return Json(new { success = true, users = users });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error retrieving users: {ex.Message}" });
            }
        }

        // Logout and clear session
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminUsername");
            return RedirectToAction("Index", "Home");
        }
    }
}