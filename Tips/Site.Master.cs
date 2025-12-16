using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Tips
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetStyleSheet();

            SetCurrentPage();

            ShowHideMenuItems();
        }

        private void ShowHideMenuItems()
        {
            XDocument xDoc = XDocument.Load(Server.MapPath(@"~/Models/SettingsExtensions.xml"));
            XAttribute xattr = xDoc.Root.Element("EnableNewEntries").Attribute("On");
            if (Boolean.Parse(xattr.Value))
            {
                mAnswers.Visible = false;
                mStats.Visible = false;
            }
            else
                mNew.Visible = false;
        }

        private void SetCurrentPage()
        {
            if (Request.Url.AbsolutePath.ToLower().Contains("default.aspx"))
                mHome.Attributes.Add("class", "selected");
            else if (Request.Url.AbsolutePath.ToLower().Contains("answers.aspx"))
                mAnswers.Attributes.Add("class", "selected");
            else if (Request.Url.AbsolutePath.ToLower().Contains("nykupong.aspx"))
                mNew.Attributes.Add("class", "selected");
            else if (Request.Url.AbsolutePath.ToLower().Contains("rules.aspx"))
                mRules.Attributes.Add("class", "selected");
            else if (Request.Url.AbsolutePath.ToLower().Contains("statistics.aspx"))
                mStats.Attributes.Add("class", "selected");
            else if (Request.Url.AbsolutePath.ToLower().Contains("blog.aspx"))
                mBlog.Attributes.Add("class", "selected");
        }

        private void SetStyleSheet()
        {
            XDocument xDoc = XDocument.Load(Server.MapPath(@"~/Models/SettingsExtensions.xml"));
            XAttribute xattr = xDoc.Root.Element("CSSStyle").Attribute("type");
            HtmlLink cssLink = new HtmlLink();
            cssLink.Attributes.Add("rel", "stylesheet");
            cssLink.Attributes.Add("type", "text/css");

            switch (xattr.Value)
            {
                case "Blue":
                    cssLink.Href = @"CSS/Blue.css";
                    break;
                case "PaleBlue":
                    cssLink.Href = @"CSS/PaleBlue.css";
                    break;
                case "FadingOrange":
                    cssLink.Href = @"CSS/FadingOrange.css";
                    break;
                case "Orange":
                    cssLink.Href = @"CSS/Orange.css";
                    break;
                case "Green":
                    cssLink.Href = @"CSS/Green.css";
                    break;
                case "Silver":
                    cssLink.Href = @"CSS/Silver.css";
                    break;
                case "EM2012_Purple":
                    cssLink.Href = @"CSS/EM2012_Purple.css";
                    break;
                case "EM2012_Green":
                    cssLink.Href = @"CSS/EM2012_Green.css";
                    break;
                case "VM2014":
                    cssLink.Href = @"~/CSS/VM2014.css";
                    break;
                case "EM2016":
                    cssLink.Href = @"~/CSS/EM2016.css";
                    break;
                default:
                    cssLink.Href = @"CSS/Blue.css";
                    break;
            }
            Page.Header.Controls.Add(cssLink);
        }
    }
}
