using System.Web;
using System.Web.Optimization;

namespace BugTracker {
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/plugins/scripts").Include(
                        "~/plugins/jquery/jquery-2.1.4.min.js",
                        "~/plugins/slider.revolution/js/jquery.themepunch.tools.min.js",
                        "~/plugins/slider.revolution/js/jquery.themepunch.revolution.min.js",
                        "~/plugins/slider.revolution.v5/js/extensions/revolution.extension.migration.min.js",
                        "~/plugins/slider.revolution.v5/js/extensions/source/revolution.extension.migration.js",
                        "~/plugins/datatables/js/jquery.dataTables.min.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/layout.css",
                      "~/Content/header-1.css",
                      "~/Content/essentials.css"));

            bundles.Add(new StyleBundle("~/plugins/css").Include(
                "~/plugins/slider.revolution/css/settings.css",
                "~/plugins/slider.revolution/css/settings-ie8.css",
                "~/plugins/slider.revolution/css/extralayers.css"));
        }
    }
}