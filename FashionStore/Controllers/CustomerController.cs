using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FashionStore.Models;
using FashionStore.App_Code;
using System.Text.RegularExpressions;
using Facebook;
using Google;
using System.Configuration;

namespace FashionStore.Controllers
{
    [RoutePrefix("tai-khoan")]
    public class CustomerController : Controller
    {
        dbDoryShopDataContext data = new dbDoryShopDataContext();
        //
        // GET: /Customer/
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [Route("login")]
        [HttpPost]
        public JsonResult Login(FormCollection collection)
        {
            var msg = "";
            var email = collection["email"];
            var pass = collection["password"];
            if (pass.Length < 8)
            {
                msg = "Mật khẩu phải từ 8-16 ký tự!";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }

            var customer = data.customers.SingleOrDefault(x => x.email == email && x.password.ToString() == MD5.md5(pass, true));
            if (customer != null)
            {
                Session["customer"] = customer;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            else
            {
                msg = "Email hoặc mật khẩu chưa đúng!";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        #region Facebook login

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallBack");
                return uriBuilder.Uri;
            }
        }

        [Route("fb-login")]
        public ActionResult LoginFB()
        {
            var fb = new FacebookClient();
            var loginURL = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["fbAppID"],
                client_secret = ConfigurationManager.AppSettings["fbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });
            return Redirect(loginURL.AbsoluteUri);
        }

        public ActionResult FacebookCallBack(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["fbAppID"],
                client_secret = ConfigurationManager.AppSettings["fbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code,
            });

            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                dynamic me = fb.Get("me?fields=name,email");
                string email = me.email;
                string name = me.name;
                string pass = String.Format("{0:dd}", DateTime.Now) + String.Format("{0:MM}", DateTime.Now) + DateTime.Now.Year.ToString();

                var customer = data.customers.SingleOrDefault(x => x.email == email);
                if (customer != null)
                {
                    Session["customer"] = customer;
                }
                else
                {
                    var cus = new customer();
                    cus.email = email;
                    cus.name = name;
                    cus.password = MD5.md5(pass, true);
                    cus.status = true;
                    cus.date_added = DateTime.Now;
                    data.customers.InsertOnSubmit(cus);
                    data.SubmitChanges();
                    Session["customer"] = data.customers.SingleOrDefault(x => x.email == email);
                }
            }
            return RedirectToAction("index", "home");
        }
        #endregion  

        [Route("dang-xuat")]
        public ActionResult Logout()
        {
            Session["customer"] = null;
            return RedirectToAction("index", "home");
        }

        private List<GetListCityResult> GetListCity()
        {
            return data.GetListCity().ToList();
        }

        [Route("dang-ky")]
        [HttpGet]
        public ActionResult SignUp()
        {
            if (Session["customer"] != null)
            {
                return RedirectToAction("index", "home");
            }
            var citys = GetListCity();
            ViewBag.Citys = citys;
            return View();
        }

        [Route("dang-ky")]
        [HttpPost]
        public JsonResult SignUp(FormCollection collection)
        {
            var citys = GetListCity();
            ViewBag.Citys = citys;
            Regex reg = new Regex("");

            var name = collection["name"];
            var email = collection["email"];
            var pass1st = collection["pass1st"];
            var pass2nd = collection["pass2nd"];
            var phone = collection["phone"];
            var street = collection["street"];
            var district = collection["district"];
            var province = collection["Province"];
            var city_id = int.Parse(collection["city"]);

            string name_msg = "", pass2_msg = "", phone_msg = "", email_msg = "";

            reg = new Regex(@"[0-9_\\\`\~\!\@\#\$\%\^\&\*\(\)\-\=\+\{\}\[\]\,\<\.\>\/\?\|\;\:]");
            if (reg.IsMatch(name))
            {
                name_msg = "Họ tên không được có ký tự đặc biệt!";
            }
            if (pass1st != pass2nd)
            {
                pass2_msg = "Mật khẩu không khớp!";
            }
            if (phone[0] != 0)
                phone = '0' + phone;
            reg = new Regex(@"[a-zA-Z_\\\`\~\!\@\#\$\%\^\&\*\(\)\-\=\+\{\}\[\]\,\<\.\>\/\?\|\;\:]");
            if (reg.IsMatch(phone))
            {
                phone_msg = "Số điện thoại chưa đúng định dạng";
            }

            var customer = data.GetCustomer(email);

            if (customer.Count() > 0)
            {
                email_msg = "Email đã tồn tại!";
            }
            else
            {
                var cus = new customer();
                cus.city_id = city_id;
                cus.date_added = DateTime.Now;
                cus.email = email;
                cus.name = name;
                cus.password = MD5.md5(pass1st, true);
                cus.phone = phone;
                cus.street = street;
                cus.ward = province;
                cus.status = true;
                data.customers.InsertOnSubmit(cus);
                data.SubmitChanges();
            }
            return Json(new { email_msg, name_msg, pass2_msg, phone_msg }, JsonRequestBehavior.AllowGet);
        }
    }
}