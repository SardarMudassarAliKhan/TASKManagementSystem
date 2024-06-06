using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TASKManagementSystem.Enums;

namespace TASKManagementSystem.Controllers
{
    public class DashBoardController : Controller
    {
        [Authorize(Roles = "Developer")]
        [ActionName("Developer")]
        public IActionResult DeveloperDashBoard()
        {
            return View();
        }

        [Authorize(Roles= "Manager")]
        [ActionName("Manager")]
        public IActionResult ManagerDashBoard()
        {
            return View();
        }

        [Authorize(Roles = "TeamLead")]
        [ActionName("TeamLead")]
        public IActionResult TeamLeadDashBoard()
        {
            return View();
        }

        [Authorize(Roles="Admin")]
        [ActionName("Admin")]
        public IActionResult AdminDashBoard()
        {
            return View();
        }
    }
}
