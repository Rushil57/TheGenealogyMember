using FamilyMember.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FamilyMember.Controllers
{
    public class StaffMemberController : Controller
    {
        // GET: StaffMember
        public ActionResult Index()
        {
            return PartialView();
        }
    }
}