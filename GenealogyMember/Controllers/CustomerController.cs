using System.Web.Mvc;

namespace FamilyMember.Controllers
{
    public class CustomerController : Controller
    {
        public ActionResult Index()
        {
            return PartialView();
        }
    }
}