using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FashionStore.App_Code;
using FashionStore.Areas.Admin.Models;

namespace FashionStore.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        dbAdminDataContext db = new dbAdminDataContext();

        public static List<string> permissions;
        //
        // GET: /Admin/Account/
        public ActionResult Index()
        {
            if (Session["user"] == null)
                return RedirectToAction("login");
            var groupID = (Session["user"] as user).user_group_id;
            permissions = Permission.GetPermission(groupID);
            ViewBag.permission = permissions;
            return View();
        }

        private user GetUser(string username, string password)
        {
            return db.users.SingleOrDefault(x => x.username == username && x.password.ToString() == MD5.md5(password, true));
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["user"] != null)
                return RedirectToAction("index");
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection col)
        {
            var username = col["username"];
            var password = col["password"];
            var user = GetUser(username, password);
            if (user == null)
            {
                ViewBag.Msg = "Tên đăng nhập hoặc mật khẩu chưa đúng!";
                return View();
            }
            Session["user"] = user;
            return RedirectToAction("index");
        }


        public ActionResult Logout()
        {
            Session["user"] = null;
            return RedirectToAction("login");
        }


        public ActionResult Notificationauth()
        {
            ViewBag.permission = permissions;
            return View();
        }
    }
}
