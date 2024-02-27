using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FamilyMember.App_Start
{
    public class CustomSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["User"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "Controller", "Account" },
                    { "Action", "Login" }
                });

            }
            if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" || filterContext.HttpContext.Request.IsAjaxRequest())
            {
                if (HttpContext.Current.Session["UserId"] == null)
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            IsSessionExpired = true,
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}