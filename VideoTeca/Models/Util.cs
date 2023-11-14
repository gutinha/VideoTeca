using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoTeca.Models
{
    public class Util
    {
        public static string BaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl.EndsWith("/"))
            {
                appUrl = appUrl.Substring(0, appUrl.Length - 1);
            }

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }

        public static string BaseUrl(string v)
        {
            if (v.StartsWith("/"))
            {
                return BaseUrl() + v;
            }
            else
            {
                return BaseUrl() + "/" + v;
            }
        }

    }
}