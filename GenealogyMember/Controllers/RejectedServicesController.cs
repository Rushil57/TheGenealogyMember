using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FamilyMember.Controllers
{
    public class RejectedServicesController : Controller
    {
        // GET: RejectedServices
        public ActionResult Index()
        {
            return PartialView();
        }
    }
}