using AlarmModule.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlarmModule.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<Alarms> alarmList = new List<Alarms>();
        public void OnGet()
        {
            Alarms alarms = new Alarms();
            alarmList = alarms.GetAlarmData();
        }

        public IActionResult OnGetAlarms()
        {
            Alarms alarms = new Alarms();
            var alarmData = alarms.GetAlarmData();
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