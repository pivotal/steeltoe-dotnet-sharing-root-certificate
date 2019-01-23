using System.Web;
using System.Web.Mvc;

namespace NET_framework4_6_1_win_valid_certificate
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
