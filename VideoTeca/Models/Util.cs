using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        public static string hash(String input)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] data = Encoding.ASCII.GetBytes(input);
            byte[] hash = sha.ComputeHash(data);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

    }
}