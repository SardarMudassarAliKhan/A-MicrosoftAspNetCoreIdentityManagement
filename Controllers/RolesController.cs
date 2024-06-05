using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace A_MicrosoftAspNetCoreIdentityManagement.Controllers
{
    public class RolesController : Controller
    {
        [Authorize(Roles= "Vendor")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Artist")]
        public IActionResult BuyerDashBoard()
        {
            return View();
        }
    }
}
