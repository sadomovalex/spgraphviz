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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebPartPages;
using WebPart = Microsoft.SharePoint.WebPartPages.WebPart;

namespace SPGraphvizWebPart
{
    public class SPGraphvizWebPart : WebPart
    {
        private bool error;
        private ResourceManager resourceManager;

        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true)]
        [ResourcesAttribute("DotUrl_DisplayName", "Category_DisplayName", "DotUrl_Description")]
        public string DotUrl
        {
            get;
            set;
        }

        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true)]
        [ResourcesAttribute("ImageType_DisplayName", "Category_DisplayName", "ImageType_Description")]
        [DefaultValue(ImageType.PNG)]
        public ImageType ImageType
        {
            get;
            set;
        }

        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true)]
        [ResourcesAttribute("LayoutType_DisplayName", "Category_DisplayName", "LayoutType_Description")]
        [DefaultValue(LayoutType.DOT)]
        public LayoutType LayoutType
        {
            get;
            set;
        }

        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true)]
        [ResourcesAttribute("CssClass_DisplayName", "Category_DisplayName", "CssClass_Description")]
        public string ImageCssClass
        {
            get;
            set;
        }

        public SPGraphvizWebPart()
        {
            this.resourceManager = new ResourceManager("SPGraphvizWebPart.SPGraphvizWebPart",
                Assembly.GetExecutingAssembly());
        }

        protected override void CreateChildControls()
        {
            if (!this.error)
            {
                try
                {
                    base.CreateChildControls();

                    if (!string.IsNullOrEmpty(this.DotUrl) &&
                        !string.IsNullOrEmpty(this.DotUrl.Trim()))
                    {
                        var img = new Image();
                        img.ID = "imgGraph";

                        string url = SPUrlUtility.CombineUrl(SPContext.Current.Web.Url,
                            Utils.GetHandlerRelativeUrl(this.DotUrl.Trim(), this.ImageType, this.LayoutType));
                        img.ImageUrl = url;

                        if (!string.IsNullOrEmpty(this.ImageCssClass) && !string.IsNullOrEmpty(this.ImageCssClass.Trim()))
                        {
                            img.CssClass = this.ImageCssClass;
                        }

                        this.Controls.Add(img);
                    }
                }
                catch (Exception x)
                {
                    this.handleException(x);
                }
            }
        }

        private void handleException(Exception x)
        {
            this.error = true;
            this.Controls.Clear();
            string stack = x.StackTrace.Replace(Environment.NewLine, "<br/>");
            this.Controls.Add(new LiteralControl(string.Format("<span style=\"color: Red\">{0}<br/>{1}</span>",
                x.Message, stack)));
        }

        public override string LoadResource(string id)
        {
            return this.resourceManager.GetString(id);
        }
    }
}
