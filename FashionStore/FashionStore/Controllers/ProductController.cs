using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FashionStore.Models;
using PagedList;
using PagedList.Mvc;
using System.Text.RegularExpressions;

namespace FashionStore.Controllers
{
    [RoutePrefix("san-pham")]
    public class ProductController : Controller
    {
        dbDoryShopDataContext db = new dbDoryShopDataContext();
        //
        // GET: /Product/
        [Route("{metatitle}/{itemperpage?}/{price?}/{sort?}/{page?}")]
        public ActionResult Index(string metatitle, int? itemperpage,string price, string sort, int? page)
        {
            var catalog = db.catalogs.SingleOrDefault(x => x.route == metatitle);//Danh mục gốc
            ViewBag.Title = catalog.name;
            //Danh sách danh mục của sản phẩm
            var catalogs = db.catalogs.ToList();
            var catalog_1 = catalogs.FindAll(x => x.parent_id == catalog.catalog_id);
            var catalog_2 = catalogs.FindAll(x => catalog_1.Any(y => x.parent_id == y.catalog_id));
            //breadcrumbs
            ViewBag.catalog2 = catalog;
            var catalog1 = catalogs.Find(x => x.catalog_id == catalog.parent_id);
            ViewBag.catalog1 = catalog1;
            catalog catalog0 = null;
            if (catalog1 != null)
            {
                catalog0 = catalogs.Find(x => x.catalog_id == catalog1.parent_id);
            }
            ViewBag.catalog0 = catalog0;
            //Lấy danh sách sản phẩm
            var products = new List<GetListProductByCatalogIdResult>();
            if (catalog_2.Count == 0)
            {
                if (catalog_1.Count == 0)
                {
                    products = db.GetListProductByCatalogId(catalog.catalog_id).ToList();
                }
                else
                {
                    foreach (var catalog_ in catalog_1)
                    {
                        var pro = db.GetListProductByCatalogId(catalog_.catalog_id).ToList();
                        products.AddRange(pro);
                    }
                }
            }
            else
            {
                foreach (var catalog_ in catalog_2)
                {
                    var pro = db.GetListProductByCatalogId(catalog_.catalog_id).ToList();
                    products.AddRange(pro);
                }
            }

            ViewBag.provider = db.providers.ToList();

            var pageNum = page ?? 1;
            var pageSize = itemperpage ?? 15;

            if (price != null)
            {
                string[] prices = Regex.Split(price, "-");
                long firstPrice = long.Parse(prices[0]);
                long lastPrice = long.Parse(prices[1]);
                if (firstPrice <= lastPrice)
                {
                    products = products.FindAll(x => x.discount_price >= firstPrice && x.discount_price <=lastPrice)
                        .OrderBy(y => y.discount_price).ToList();
                }
            }

            if (sort != null)
            {
                if (sort == "priceasc")
                {
                    products = products.OrderBy(x => x.discount_price).ToList();
                }
                else if (sort == "pricedesc")
                {
                    products = products.OrderByDescending(x => x.discount_price).ToList();
                }
                else if (sort == "discountspecial")
                {
                    products = products.OrderByDescending(x => x.last_price).ToList();
                }
                else
                {
                    products = products.OrderBy(x => x.name).ToList();
                }
            }

            return View(products.ToPagedList(pageNum, pageSize));
        }
    }
}