using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using DirigoEdge.Utils;
using DirigoEdgeCore.Business;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Models.ViewModels;
using DirigoEdgeCore.Utils;


namespace DirigoEdge.Controllers
{
	public class HomeController : DirigoBaseController
	{
		public ActionResult Index()
		{
		    var model = new ContentViewViewModel {ThePage = ContentLoader.GetDetailsByTitle(String.Empty)};
            model.TheTemplate = ContentLoader.GetContentTemplate(model.ThePage.Template);
            model.PageData = ContentUtils.GetFormattedPageContentAndScripts(model.ThePage.HTMLContent, Context);

		    if (model.ThePage != null)
            {
                return View(model.TheTemplate.ViewLocation, model);
            }
            
            HttpContext.Response.StatusCode = 404;
            return View("~/Views/Home/Error404.cshtml");
		}

		public ActionResult About()
		{
			return View();
		}

		public ActionResult Blog()
		{
			return View();
		}

		public ActionResult Contact()
		{
			return View();
		}

        public ActionResult Tweet()
        {
            var model = new TweetViewModel(1);
            return View(model);
        }

		public XmlSitemapResult SitemapXML()
		{
			string hostUrl = HTTPUtils.GetFullyQualifiedApplicationPath();

			var items = new List<SitemapItem>();

			// Add generated blogs, public content pages, etc.
			items.AddRange(Utils.Sitemap.GetGeneratedSiteMapItems(hostUrl));

			return new XmlSitemapResult(items);
		}

		#region AjaxControllers

		public JsonResult ContactVerify(string qaptcha_key)
		{
			bool error = true;
			if (!String.IsNullOrEmpty(qaptcha_key))
			{
				error = false;
				Session["qaptcha_key"] = qaptcha_key;
			}


			return new JsonResult() { Data = new { error = error } };
		}

		public JsonResult ContactFormPost(ContactForm form)
		{
			string subject = "New Contact Form Notification";
			string sendMessageTo = "you@email.com";
			string messageFrom = "no-reply@myaddress.com";
			string smtpCredential = "mail.domain.com";
			string smtpCredentialLogin = "noreply@mydomain.com";
			string smtpPassword = "myPassword";

			var result = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };

			// Check Captcha
			if (Session["qaptcha_key"] != null)
			{
				var sb = new StringBuilder();

				sb.Append("Name : " + form.name + "<br />");
				sb.Append("Contact Email : " + form.contactEmail + "<br />");
				sb.Append("Subject : " + form.subject + "<br /><br />");
				sb.Append("Comments : " + form.message + "<br />");

				// Send Form Data
				try
				{
					var msg = new MailMessage();
					msg.Body = sb.ToString();
					msg.IsBodyHtml = true;
					msg.From = new MailAddress(messageFrom);
					msg.Subject = subject;
					msg.To.Add(sendMessageTo);

					var smtp = new SmtpClient(smtpCredential);
					smtp.Credentials = new NetworkCredential(smtpCredentialLogin, smtpPassword);
					smtp.Port = 25;
					smtp.Send(msg);
				}
				catch (Exception e)
				{
                    Log.Warn("Failed to send email", e);
				}
			}

			return result;
		}

		#endregion

		public class ContactForm
		{
			public string message { get; set; }
			public string contactEmail { get; set; }
			public string name { get; set; }
			public string subject { get; set; }

			public string captchaId { get; set; }
		}

	}
}
