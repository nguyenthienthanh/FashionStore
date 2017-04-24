using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FashionStore.Models;
using FashionStore.App_Code;

namespace FashionStore.Controllers
{
    public class HomeController : Controller
    {
        public dbDoryShopDataContext data = new dbDoryShopDataContext();
        //
        // GET: /Product/

        public List<GetListProductBySortResult> GetListProductBySort(int iNumber)
        {
            //-- Sort = 0: Không sắp xếp.
            //-- Sort = 1: Sắp xếp theo ngày thêm.
            //-- Sort = 2: Sắp xếp theo lượt đánh giá.
            //-- Sort = 3: Sắp xếp theo giá. 
            return data.GetListProductBySort(iNumber).ToList();
        }

        [Route("")]
        public ActionResult Index()
        {
            var product = GetListProductBySort(20).OrderBy(x => x.discount_price).ToList();
            ViewBag.BestSellerProduct = GetListProductBySort(20).OrderByDescending(x => x.view).ToList();
            return View(product);
        }

        public List<CatalogByParentResult> GetCatalogByParent(int iParent)
        {
            return data.CatalogByParent(iParent).ToList();
        }

        public PartialViewResult Catalog_par()
        {
            var catalog = GetCatalogByParent(0);
            return PartialView(catalog);
        }

        //Nav menu
        public PartialViewResult Catalog_child(int iParent)
        {
            var catalog = GetCatalogByParent(iParent);
            return PartialView(catalog);
        }

        public PartialViewResult Catalog_sub(int iParent)
        {
            var catalog = GetCatalogByParent(iParent);
            return PartialView(catalog);
        }

        /// <summary>
        /// Mobile menu
        /// </summary>
        /// <returns></returns>
        public PartialViewResult MCatalog_child(int iParent)
        {
            var catalog = GetCatalogByParent(iParent);
            return PartialView(catalog);
        }

        public PartialViewResult MCatalog_sub(int iParent)
        {
            var catalog = GetCatalogByParent(iParent);
            return PartialView(catalog);
        }

        public List<GetProductDetailResult> GetProductDetail(string strProduct_code)
        {
            return data.GetProductDetail(strProduct_code).ToList();
        }

        [Route("~/chi-tiet/{code}")]
        public ActionResult ProductDetail(string code)
        {
            var productDetail = GetProductDetail(code);
            if (productDetail.Count() == 0)
            {
                return Redirect("/");
            }
            var catalogs = new List<GetCataLogByIdResult>();
            var catalog = data.GetCataLogById(productDetail.First().catalog_id).ToList();
            catalogs.Add(catalog.First());
            while (catalog.First().parent_id > 0)
            {
                catalog = data.GetCataLogById(catalog.First().parent_id).ToList();
                catalogs.Add(catalog.First());
            }

            var bestsaleProduct = GetListProductBySort(20).OrderByDescending(x => x.last_price).ToList();

            ViewBag.bestsaleProduct = bestsaleProduct;
            ViewBag.Catalogs = catalogs;
            return View(productDetail);
        }

    }
}