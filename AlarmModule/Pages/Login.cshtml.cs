using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using AlarmModule.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AlarmModule.Pages
{
    public class LoginModel : PageModel
    {
        private readonly string _connectionString;

        public LoginModel()
        {
            _connectionString = "Data Source=DESKTOP-D4R0VU6\\WINCC;Initial Catalog=SCADA_SYSTEM;Integrated Security=True;TrustServerCertificate=True";
        }

        [BindProperty]
        public UserLoginModel Input { get; set; }

        public IActionResult OnGet()
        {
            // Redirect to the index page if the user is already authenticated
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Model validation failed
                return Page();
            }

            if (string.IsNullOrEmpty(Input.Username) || string.IsNullOrEmpty(Input.Password))
            {
                // Empty username or password
                ModelState.AddModelError(string.Empty, "Username and password are required.");
                return Page();
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = "EXEC LoginUser @Username, @Password";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@Username", Input.Username);
                command.Parameters.AddWithValue("@Password", Input.Password);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        // Successful login
                        int userId = (int)reader["UserId"];
                        string name = (string)reader["Name"];
                        string roleName = (string)reader["RoleName"];

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, name),
                            new Claim(ClaimTypes.Role, roleName),
                            new Claim("UserId", userId.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            // You can customize the cookie properties here if needed
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                        return RedirectToPage("/Index");
                    }
                }
            }

            // Invalid credentials or login failed
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return Page();
        }
    }

    public class UserLoginModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
