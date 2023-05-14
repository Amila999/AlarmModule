using AlarmModule.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace AlarmModule.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        readonly IConfiguration _configuration;
        public string connectionString;
        public List<Alarms> alarmList = new List<Alarms>();

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            Alarms alarms = new Alarms();
            alarmList = alarms.GetAlarmData(_configuration.GetConnectionString("connectionString"));
        }

        public IActionResult OnGetAlarms()
        {
            connectionString = _configuration.GetConnectionString("connectionString");
            Alarms alarms = new Alarms();
            var alarmData = alarms.GetAlarmData(connectionString);
            return new JsonResult(alarmData);
        }
        public IActionResult OnPostLogout()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Perform logout actions, such as signing out the user
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return RedirectToPage("/Login");
        }

    }
}