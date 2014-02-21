using System.Web;
using System.Web.Optimization;

namespace PassPhotoMVC4
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            
            bundles.Add(new ScriptBundle("~/bundles/jcrop").Include(
                      "~/js/jquery.min.js",
                      "~/js/jquery.Jcrop.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/css/jquery.Jcrop.css",
                      "~/css/site.css"));
        }
    }
}
