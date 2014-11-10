using System.Web;
using System.Web.Optimization;

namespace MyAdmin.Mvc.App_Start
{
    public class BundleConfig 
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/codemirror-themes").Include(
                        "~/Content/codemirror/theme/3024-day.css",
                        "~/Content/codemirror/theme/3024-night.css",
                        "~/Content/codemirror/theme/ambiance-mobile.css",
                        "~/Content/codemirror/theme/ambiance.css",
                        "~/Content/codemirror/theme/base16-dark.css",
                        "~/Content/codemirror/theme/base16-light.css",
                        "~/Content/codemirror/theme/blackboard.css",
                        "~/Content/codemirror/theme/cobalt.css",
                        "~/Content/codemirror/theme/eclipse.css",
                        "~/Content/codemirror/theme/elegant.css",
                        "~/Content/codemirror/theme/erlang-dark.css",
                        "~/Content/codemirror/theme/lesser-dark.css",
                        "~/Content/codemirror/theme/managstudio.css",
                        "~/Content/codemirror/theme/mbo.css",
                        "~/Content/codemirror/theme/mdn-like.css",
                        "~/Content/codemirror/theme/midnight.css",
                        "~/Content/codemirror/theme/monokai.css",
                        "~/Content/codemirror/theme/neat.css",
                        "~/Content/codemirror/theme/neo.css",
                        "~/Content/codemirror/theme/night.css",
                        "~/Content/codemirror/theme/paraiso-dark.css",
                        "~/Content/codemirror/theme/paraiso-light.css",
                        "~/Content/codemirror/theme/pastel-on-dark.css",
                        "~/Content/codemirror/theme/rubyblue.css",
                        "~/Content/codemirror/theme/solarized.css",
                        "~/Content/codemirror/theme/the-matrix.css",
                        "~/Content/codemirror/theme/tomorrow-night-eighties.css",
                        "~/Content/codemirror/theme/twilight.css",
                        "~/Content/codemirror/theme/vibrant-ink.css",
                        "~/Content/codemirror/theme/xq-dark.css",
                        "~/Content/codemirror/theme/xq-light.css"
                        ));
        }
    }
}