using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using FashionStore.Areas.Admin.Models;
using System.IO;

namespace FashionStore.Areas.Admin.Controllers
{
    [AuthController]
    public class ProductController : Controller
    {
        dbAdminDataContext db = new dbAdminDataContext();
        //
        // GET: /Admin/Product/
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("index", "admin");
            }

            ViewBag.permission = AdminController.permissions;
            var product = db.GetListProductBySort(0).ToList();

            return View(product);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.permission = AdminController.permissions;
            var providers = (from p in db.providers
                             select p).ToList();
            ViewBag.Providers = providers;
            var catalogs = (from c in db.catalogs
                            select c).ToList();
            ViewBag.Catalogs = catalogs;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormCollection col)
        {
            HttpFileCollectionBase fileUpload = Request.Files;
            if (fileUpload.Count == 0)
            {
                ViewBag.Err = "Vui lòng nhập ảnh bìa!";
                return View();
            }
            ViewBag.permission = AdminController.permissions;
            var providers = (from p in db.providers
                             select p).ToList();
            ViewBag.Providers = providers;
            var catalogs = (from c in db.catalogs
                            select c).ToList();
            ViewBag.Catalogs = catalogs;

            var product_code = col["product-code"];
            var product_name = col["product-name"];
            var provider = col["provider"];
            var catalog = col["catalog"];
            var quantity = col["quantity"];
            var price = col["price"];
            var description = col["description"];

            var filePath = Server.MapPath("~/products-images/" + product_code);
            Directory.CreateDirectory(filePath);
            var product = new product();
            product.product_code = product_code;
            product.catalog_id = int.Parse(catalog);
            product.provider_id = int.Parse(provider);
            product.quantity = int.Parse(quantity);
            product.price = decimal.Parse(price);
            product.image = Path.GetFileName(fileUpload[0].FileName);
            product.date_added = DateTime.Now;
            product.view = 0;
            product.rating = 90;
            db.products.InsertOnSubmit(product);
            db.SubmitChanges();
            var p_description = new procduct_description();
            p_description.name = product_name;
            p_description.description = description;
            p_description.product_id = product.product_id;
            db.procduct_descriptions.InsertOnSubmit(p_description);
            db.SubmitChanges();

            for (int i = 0; i < fileUpload.Count; i++)
            {
                HttpPostedFileBase file = fileUpload[i];
                try
                {
                    if (file.ContentLength > 0)
                    {
                        if (ModelState.IsValid)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(filePath, fileName);
                            if (System.IO.File.Exists(path))
                            {
                                ViewBag.Err = "Hình ảnh đã tồn tại!";
                                return View();
                            }
                            else
                            {
                                file.SaveAs(path);
                                var images = new product_image();
                                images.product_id = product.product_id;
                                images.image = fileName;
                                db.product_images.InsertOnSubmit(images);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Err = e.Message;
                    return View();
                }

            }
            db.SubmitChanges();

            return RedirectToAction("index");
        }

        [HttpGet]
        public ActionResult Edit(string code)
        {
            ViewBag.permission = AdminController.permissions;
            var providers = (from p in db.providers
                             select p).ToList();
            ViewBag.Providers = providers;
            var catalogs = (from c in db.catalogs
                            select c).ToList();
            ViewBag.Catalogs = catalogs;
            var product = db.GetProductDetail(code).ToList();
            var id = product.FirstOrDefault().product_id;
            return View(product);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection col)
        {
            HttpFileCollectionBase fileUpload = Request.Files;
            if (fileUpload.Count == 0)
            {
                ViewBag.Err = "Vui lòng nhập ảnh bìa!";
                return View();
            }
            ViewBag.permission = AdminController.permissions;
            var providers = (from p in db.providers
                             select p).ToList();
            ViewBag.Providers = providers;
            var catalogs = (from c in db.catalogs
                            select c).ToList();
            ViewBag.Catalogs = catalogs;

            var product_code = col["product-code"];
            var product_name = col["product-name"];
            var provider = col["provider"];
            var catalog = col["catalog"];
            var quantity = col["quantity"];
            var price = col["price"];
            var description = col["description"];

            var filePath = Server.MapPath("~/products-images/" + product_code);
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            var product = db.products.SingleOrDefault(x => x.product_code == product_code);
            product.catalog_id = int.Parse(catalog);
            product.provider_id = int.Parse(provider);
            product.quantity = int.Parse(quantity);
            product.price = decimal.Parse(price);
            product.image = Path.GetFileName(fileUpload[0].FileName);
            product.date_added = DateTime.Now;
            product.view = 0;
            product.rating = 90;
            UpdateModel(product);
            db.SubmitChanges();
            var p_description = db.procduct_descriptions.SingleOrDefault(x => x.product_id == product.product_id);
            p_description.name = product_name;
            p_description.description = description;
            UpdateModel(p_description);
            db.SubmitChanges();

            for (int i = 0; i < fileUpload.Count; i++)
            {
                HttpPostedFileBase file = fileUpload[i];
                try
                {
                    if (file.ContentLength > 0)
                    {
                        if (ModelState.IsValid)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(filePath, fileName);
                            if (System.IO.File.Exists(path))
                            {
                                ViewBag.Err = "Hình ảnh đã tồn tại!";
                                return View();
                            }
                            else
                            {
                                file.SaveAs(path);
                                var images = new product_image();
                                images.product_id = product.product_id;
                                images.image = fileName;
                                db.product_images.InsertOnSubmit(images);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Err = e.Message;
                    return View();
                }

            }
            db.SubmitChanges();

            return RedirectToAction("index");
        }

        private void DeleteDirectory(string path)
        {
            // Delete all files from the Directory
            foreach (string filename in Directory.GetFiles(path))
            {
                System.IO.File.Delete(filename);
            }
            // Check all child Directories and delete files
            foreach (string subfolder in Directory.GetDirectories(path))
            {
                DeleteDirectory(subfolder);
            }
            Directory.Delete(path);
        }

        [HttpPost]
        public ActionResult Del(FormCollection col)
        {
            ViewBag.permission = AdminController.permissions;
            string product_code = col["code"];
            var order = db.orders.FirstOrDefault(x => x.product_code == product_code);
            if (order != null)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var product = db.products.SingleOrDefault(x => x.product_code == product_code);
                if (product == null)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }

                var discounts = (from d in db.discount_details
                                 where d.product_id == product.product_id
                                 select d).ToList();
                foreach (var item in discounts)
                {
                    db.discount_details.DeleteOnSubmit(item);
                }
                db.SubmitChanges();
                var images = (from i in db.product_images
                              where i.product_id == product.product_id
                              select i).ToList();
                foreach (var item in images)
                {
                    db.product_images.DeleteOnSubmit(item);
                }
                db.SubmitChanges();
                var descriptions = (from i in db.procduct_descriptions
                                    where i.product_id == product.product_id
                                    select i).ToList();
                foreach (var item in descriptions)
                {
                    db.procduct_descriptions.DeleteOnSubmit(item);
                }
                db.SubmitChanges();
                string path = Server.MapPath("~/products-images/" + product_code);

                DeleteDirectory(path);

                db.products.DeleteOnSubmit(product);
                db.SubmitChanges();
                return Json(1, JsonRequestBehavior.AllowGet);
            }
        }
    }
}