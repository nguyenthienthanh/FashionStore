using System.Web.Mvc;

namespace FashionStore.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
                "Product_ShortURL",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", controller = "Product", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_ShortURL",
                "Admin/{action}/{id}",
                new { action = "Index", controller = "admin", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}