#region Copyright(c) Alexey Sadomov. All Rights Reserved.
// -----------------------------------------------------------------------------
// Copyright(c) 2010 Alexey Sadomov. All Rights Reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
//   1. No Trademark License - Microsoft Public License (Ms-PL) does not grant you rights to use
//      authors names, logos, or trademarks.
//   2. If you distribute any portion of the software, you must retain all copyright,
//      patent, trademark, and attribution notices that are present in the software.
//   3. If you distribute any portion of the software in source code form, you may do
//      so only under this license by including a complete copy of Microsoft Public License (Ms-PL)
//      with your distribution. If you distribute any portion of the software in compiled
//      or object code form, you may only do so under a license that complies with
//      Microsoft Public License (Ms-PL).
//   4. The names of the authors may not be used to endorse or promote products
//      derived from this software without specific prior written permission.
//
// The software is licensed "as-is." You bear the risk of using it. The authors
// give no express warranties, guarantees or conditions. You may have additional consumer
// rights under your local laws which this license cannot change. To the extent permitted
// under your local laws, the authors exclude the implied warranties of merchantability,
// fitness for a particular purpose and non-infringement.
// -----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.SharePoint;

namespace SPGraphvizWebPart
{
    public static class Utils
    {
        public static string GetHandlerRelativeUrl(string dotUrl, ImageType type, LayoutType layoutType)
        {
            dotUrl = HttpUtility.UrlEncode(dotUrl);
            string url = string.Format("/_layouts/SPGraphviz/GraphHandler.ashx?{0}={1}&{2}={3}&{4}={5}",
                Constants.URL.DOT_URL_KEY, dotUrl,
                Constants.URL.IMAGE_TYPE_KEY, type.ToString().ToLower(),
                Constants.URL.LAYOUT_TYPE_KEY, layoutType.ToString().ToLower());
            return url;
        }

        public static string GetDotFileContent(string dotUrl)
        {
            try
            {
                using (var site = new SPSite(dotUrl))
                {
                    using (var web = site.OpenWeb())
                    {
                        string content = web.GetFileAsString(dotUrl);
                        return content;                        
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static ImageType GetTypeFromQueryString(string type)
        {
            var defaultType = ImageType.PNG;
            try
            {
                if (string.IsNullOrEmpty(type))
                {
                    return defaultType;
                }
                var obj = (ImageType)Enum.Parse(typeof(ImageType), type, true);
                return obj;
            }
            catch
            {
                return defaultType;
            }
        }

        public static LayoutType GetLayoutFromQueryString(string layout)
        {
            var defaultLayout = LayoutType.DOT;
            try
            {
                if (string.IsNullOrEmpty(layout))
                {
                    return defaultLayout;
                }
                var obj = (LayoutType)Enum.Parse(typeof(LayoutType), layout, true);
                return obj;
            }
            catch
            {
                return defaultLayout;
            }
        }

        public static bool IsHostAllowed(string targetUrl)
        {
            if (string.IsNullOrEmpty(targetUrl))
            {
                return false;
            }

            if (isCurrentWeb(targetUrl))
            {
                return true;
            }

            Uri targetUri = null;
            try
            {
                targetUri = new Uri(targetUrl);
            }
            catch
            {
                return false;
            }

            string hosts = ConfigurationManager.AppSettings[Constants.Config.ALLOWED_HOSTS];
            if (string.IsNullOrEmpty(hosts))
            {
                return false;
            }
            string[] allowedHosts = hosts.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
            if (allowedHosts == null || allowedHosts.Length == 0)
            {
                return false;
            }
            return allowedHosts.Any(h => string.Compare(h, targetUri.Host, true) == 0);
        }

        private static bool isCurrentWeb(string targetUrl)
        {
            using (var site = new SPSite(targetUrl))
            {
                using (var web = site.OpenWeb())
                {
                    return web.ID == SPContext.Current.Web.ID;
                }
            }
        }
    }
}
