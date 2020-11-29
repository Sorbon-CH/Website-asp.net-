using System.Web;
using System.Web.Mvc;

namespace Gaestebuch_Sebastian
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
