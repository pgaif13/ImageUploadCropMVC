using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PassPhotoMVC4
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("uploaded_images/{*pathInfo}");
            routes.IgnoreRoute("css/{*pathInfo}");
            routes.IgnoreRoute("js/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "PassPhoto", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}