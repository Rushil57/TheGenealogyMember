using FamilyMember.App_Start;
using System.Web;
using System.Web.Mvc;

namespace FamilyMember.Controllers
{
    [CustomSessionAttribute]
    public class DashboardController : Controller
    {
        public ActionResult Default()
        {
            return View();
        }

        public ActionResult Index()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult GetServiceManual()
        {
            string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"500px\">";
            embed += "If you are unable to view file, you can download from <a href = \"{0}\">here</a>";
            embed += " or download <a target = \"_blank\" href = \"http://get.adobe.com/reader/\">Adobe PDF Reader</a> to view the file.";
            embed += "</object>";
            TempData["Embed"] = string.Format(embed, VirtualPathUtility.ToAbsolute("~/Content/Service_Manual.pdf"));

            return Json(new { embeddedString = TempData["Embed"] }, JsonRequestBehavior.AllowGet);
        }
    }
}