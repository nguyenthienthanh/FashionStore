using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FashionStore.Areas.Admin.Models;

namespace FashionStore.Areas.Admin.Models
{
    public class AuthController : ActionFilterAttribute
    {
        private List<string> GetPermission(int groupID)
        {
            dbAdminDataContext db = new dbAdminDataContext();
            var dbpermissions = from u in db.user_groups
                                from r in db.roles
                                from p in db.permissions
                                where u.user_group_id == p.user_group_id
                                && u.user_group_id == groupID && r.role_id == p.role_id
                                select r;
            var permissions = new List<string>();
            foreach (var permission in dbpermissions)
            {
                permissions.Add(permission.role_id);
            }
            return permissions;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["user"] == null)
                filterContext.Result = new RedirectResult("~/admin/admin/login");
            else
            {
                var groupID = (HttpContext.Current.Session["user"] as user).user_group_id;
                List<string> permissions = GetPermission(groupID);
                string actionName = (filterContext.ActionDescriptor.ActionName + "_"
                    + filterContext.ActionDescriptor.ControllerDescriptor.ControllerName).ToUpper();
                if (!permissions.Contains(actionName))
                {
                    filterContext.Result = new RedirectResult("~/admin/admin/notificationauth");
                }
            }
        }
    }
}