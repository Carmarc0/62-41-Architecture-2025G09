using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Services;

namespace MVC.Controllers
{
    /// <summary>
    /// Point of Sale controller for student credit management
    /// </summary>
    public class PoSController : Controller
    {
        private readonly IApiService _apiService;

        public PoSController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // Display student login page
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        // Handle student authentication
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            bool isAuthenticated = await _apiService.AuthenticateStudentAsync(model.Username, model.Password);

            if (isAuthenticated)
            {
                HttpContext.Session.SetString("Username", model.Username);
                return RedirectToAction("Dashboard");
            }
            else
            {
                model.ErrorMessage = "Invalid student credentials";
                return View("Index", model);
            }
        }

        // Display student dashboard with balance and credit addition
        public async Task<IActionResult> Dashboard()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index");
            }

            try
            {
                double currentBalance = await _apiService.GetBalanceAsync(username);

                var model = new AddCreditViewModel
                {
                    Username = username,
                    CurrentBalance = currentBalance
                };

                return View(model);
            }
            catch (Exception ex)
            {
                var errorModel = new AddCreditViewModel
                {
                    Username = username,
                    ErrorMessage = $"Error retrieving balance: {ex.Message}"
                };
                return View(errorModel);
            }
        }

        // Process credit addition
        [HttpPost]
        public async Task<IActionResult> AddCredit(AddCreditViewModel model)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index");
            }

            model.Username = username;

            if (!ModelState.IsValid)
            {
                model.CurrentBalance = await _apiService.GetBalanceAsync(username);
                return View("Dashboard", model);
            }

            try
            {
                Console.WriteLine($"[PoSController] Attempting to add {model.Amount} CHF to {username}");

                bool success = await _apiService.AddAmountAsync(username, (double)model.Amount);

                if (success)
                {
                    double newBalance = await _apiService.GetBalanceAsync(username);
                    model.CurrentBalance = newBalance;
                    model.SuccessMessage = $"{model.Amount} CHF added successfully! New balance: {newBalance:F2} CHF";
                    model.Amount = 0;
                    Console.WriteLine($"[PoSController] Successfully added credit to {username}");
                }
                else
                {
                    model.CurrentBalance = await _apiService.GetBalanceAsync(username);
                    model.ErrorMessage = "Error adding credit. Please verify that your account exists and try again.";
                    Console.WriteLine($"[PoSController] Failed to add credit to {username}");
                }
            }
            catch (Exception ex)
            {
                model.CurrentBalance = await _apiService.GetBalanceAsync(username);
                model.ErrorMessage = $"Technical error: {ex.Message}";
                Console.WriteLine($"[PoSController] Exception adding credit: {ex.Message}");
            }

            return View("Dashboard", model);
        }

        // Refresh balance (redirect to dashboard)
        public async Task<IActionResult> RefreshBalance()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Dashboard");
        }

        // Logout and clear session
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Index", "Home");
        }
    }
}