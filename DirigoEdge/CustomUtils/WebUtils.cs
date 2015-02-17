using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirigoEdge.CustomUtils
{
    public class WebUtils
    {
        public static string ClientIPAddress()
        {
            string ipAddr = string.Empty;
            string[] variables = { "HTTP_CLIENT_IP", "HTTP_X_FORWARDED_FOR", "HTTP_X_FORWARDED", "HTTP_X_CLUSTER_CLIENT_IP", "HTTP_FORWARDED_FOR", "HTTP_FORWARDED", "REMOTE_ADDR" };
            foreach (var item in variables)
            {
                if (HttpContext.Current.Request.ServerVariables[item] != null)
                {
                    ipAddr = HttpContext.Current.Request.ServerVariables[item].ToString();
                    if (!string.IsNullOrEmpty(ipAddr) && ipAddr.ToLower() !=  "unknown")
                    {
                        break;
                    }
                }
            }
            return ipAddr.Split(',')[0];
        }
    }
}