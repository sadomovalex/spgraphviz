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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.SharePoint;

namespace SPGraphvizWebPart
{
    public class GraphHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (string.IsNullOrEmpty(context.Request.QueryString[Constants.URL.DOT_URL_KEY]))
            {
                throw new Exception("Dot URL is empty");
            }
            string dotUrl = HttpUtility.UrlDecode(context.Request.QueryString[Constants.URL.DOT_URL_KEY]);
            if (string.IsNullOrEmpty(dotUrl))
            {
                throw new Exception("Dot URL is empty");
            }

            if (!Utils.IsHostAllowed(dotUrl))
            {
                throw new Exception(string.Format("Specified url '{0}' is not allowed. Use '{1}' app settings to allow this URL",
                    dotUrl, Constants.Config.ALLOWED_HOSTS));
            }

            string dotContent = Utils.GetDotFileContent(dotUrl);
            if (string.IsNullOrEmpty(dotContent))
            {
                throw new Exception("Dot content is empty");
            }
            
            var type = Utils.GetTypeFromQueryString(context.Request.QueryString[Constants.URL.IMAGE_TYPE_KEY]);
            var layout = Utils.GetLayoutFromQueryString(context.Request.QueryString[Constants.URL.LAYOUT_TYPE_KEY]);
            byte[] img = Graphviz.RenderImage(dotContent, layout.ToString().ToLower(), type.ToString().ToLower());
            if (img == null || img.Length == 0)
            {
                throw new Exception("Image is null");
            }
            this.renderImage(img, type, context);
        }

        private void renderImage(byte[] img, ImageType type, HttpContext context)
        {
            // verify properties
            if (img == null)
            {
                return;
            }

            // output
            context.Response.Clear();

            var imageFormat = this.getContentType(type);
            context.Response.ContentType = imageFormat;

            using (var ms = new MemoryStream(img))
            {
                ms.WriteTo(context.Response.OutputStream);
            }
            context.Response.End();
        }

        protected string getContentType(ImageType type)
        {
            switch (type)
            {
                case ImageType.BMP:
                    return "image/bmp";
                case ImageType.GIF:
                    return "image/gif";
                case ImageType.JPEG:
                    return "image/jpeg";
                case ImageType.PNG:
                    return "image/png";
                default:
                    return "image/png";
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
