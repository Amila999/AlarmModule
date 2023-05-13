using AlarmModule.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlarmModule.Pages
{
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
    }
}