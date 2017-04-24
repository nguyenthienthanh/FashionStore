using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FashionStore.Models;
using FashionStore.App_Code;

namespace FashionStore.Controllers
{
    public class CartController : Controller
    {
        dbDoryShopDataContext db = new dbDoryShopDataContext();
        private List<Cart> GetCarts()
        {
            var carts = Session["carts"] as List<Cart>;
            if (carts == null)
            {
                carts = new List<Cart>();
                Session["carts"] = carts;
            }
            return carts;
        }
        //
        // GET: /Cart/
        public ActionResult Index()
        {
            var carts = GetCarts();
            ViewBag.Amount = carts.Sum(x => x.amount);
            ViewBag.NumberCart = carts.Sum(x => x.quantity);
            return View(carts);
        }

        [Route("them-gio")]
        [HttpPost]
        public ActionResult AddCart(FormCollection col)
        {
            var product_code = col["code"];
            var carts = GetCarts();
            //Kiểm tra sản phầm trong giỏ hàng.
            var cart = carts.Find(x => x.product_code == product_code);
            //Nếu chưa có trong giỏ hàng.
            if (cart == null)
            {
                cart = new Cart(product_code);
                carts.Add(cart);
            }
            else
            {
                cart.quantity++;
            }
            return Json(carts, JsonRequestBehavior.AllowGet);
        }

        [Route("xoa-gio")]
        [HttpPost]
        public ActionResult DelCart(FormCollection col)
        {
            var product_code = col["code"];
            var carts = GetCarts();
            var cart = carts.Find(x => x.product_code == product_code);
            carts.Remove(cart);
            Session["carts"] = carts;
            return RedirectToAction("index", "cart");
        }

        [Route("dang-nhap")]
        [HttpGet]
        public ActionResult LoginCart()
        {
            return View();
        }

        [Route("dang-nhap")]
        [HttpPost]
        public ActionResult LoginCart(FormCollection col)
        {
            ViewBag.msg = "";
            var email = col["email"];
            var pass = col["password"];
            if (pass.Length < 8)
            {
                ViewBag.msg = "Mật khẩu phải từ 8-16 ký tự!";
                return View();
            }

            var customer = db.customers.SingleOrDefault(x => x.email == email && x.password.ToString() == MD5.md5(pass, true));
            if (customer != null)
            {
                Session["customer"] = customer;
                return RedirectToAction("order");
            }
            else
            {
                ViewBag.msg = "Email hoặc mật khẩu chưa đúng!";
            }
            return View();
        }

        [Route("thanh-toan")]
        [HttpGet]
        public ActionResult Order()
        {
           
            if (Session["carts"] == null)
            {
                return RedirectToAction("index", "home");
            }
            var customer = Session["customer"] as customer;
            if (customer == null || customer.ToString() == "")
            {
                return RedirectToAction("LoginCart");
            }
            var carts = Session["carts"] as List<Cart>;
            ViewBag.Quantity = carts.Sum(x => x.quantity);
            ViewBag.Amount = carts.Sum(x => x.amount);

            return View(carts);
        }

        [Route("thanh-toan")]
        [HttpPost]
        public ActionResult Order(FormCollection col)
        {
           
            if (Session["carts"] == null)
            {
                return RedirectToAction("index", "home");
            }
            var customer = Session["customer"] as customer;
            if (customer == null || customer.ToString() == "")
            {
                return RedirectToAction("LoginCart");
            }
            var carts = Session["carts"] as List<Cart>;
            ViewBag.Quantity = carts.Sum(x => x.quantity);
            ViewBag.Amount = carts.Sum(x => x.amount);

            var date_end = DateTime.Parse(col["date_end"]);

            var transaction = new transaction();
            transaction.customer_id = customer.customer_id;
            transaction.customer_name = customer.name;
            transaction.customer_phone = customer.phone;
            transaction.status = false; // Tình trạng thanh toán.
            transaction.amount = carts.Sum(x => x.amount);
            transaction.date_added = DateTime.Now;
            transaction.date_end = date_end;
            db.transactions.InsertOnSubmit(transaction);
            db.SubmitChanges();

            foreach (var item in carts)
            {
                var order = new order();
                order.transaction_id = transaction.transaction_id;
                order.product_id = item.product_id;
                order.product_code = item.product_code;
                order.name = item.name;
                order.image = item.image;
                order.price = item.discount_price;
                order.quantity = item.quantity;
                order.amount = item.amount;
                db.orders.InsertOnSubmit(order);
            }
            db.SubmitChanges();
            Session["carts"] = null;

            return RedirectToAction("index", "home");
        }
    }
}