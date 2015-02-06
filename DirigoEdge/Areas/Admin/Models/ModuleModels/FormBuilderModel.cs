using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DirigoEdge.Entities;

namespace DirigoEdge.Areas.Admin.Models.ModuleModels
{
	public class FormBuilderModel : IModule
	{
		public DynamicForm TheForm;

		private const string _defaultPostUrl = "/home/submitform/";

		/// <summary>
		/// Creates a new Dynamic Form and returns the html output for the newly created form
		/// </summary>
		/// <returns>The html output for the newly created form</returns>
		public string RenderHtml()
		{
			return GetDefaultHtml();
		}

		public string RenderHtml(int id)
		{
			using (var context = new DataContext())
			{
				return context.DynamicForms.Single(x => x.DynamicFormId == id).FormHtml;
			}
		}

		public string GetDefaultHtml()
		{
			var defaultForm = new DynamicForm()
				{
					ActionUrl = _defaultPostUrl,
					IsAjaxForm = true,
					SaveFormToDatabase = true,
					SendEmailOnSubmit = false
				};

			using (var context = new DataContext())
			{
				context.DynamicForms.Add(defaultForm);
				context.SaveChanges();

				// Now we have an id to generat a label off of
				defaultForm.FormLabel = "Custom Form " + defaultForm.DynamicFormId;

				// Use the id for the generated id
				defaultForm.FormHtml = getDefaultFormHtml(defaultForm.DynamicFormId);

				context.SaveChanges();
			}

			return defaultForm.FormHtml;
		}

		private string getDefaultFormHtml(int moduleId)
		{
			var sb = new StringBuilder();
			sb.Append(String.Format("<div class='contactFormModule module dynamic' data-id='{0}'>", moduleId));

			sb.Append(String.Format("<form class='custom customForm' action='{0}' method='POST'> ", _defaultPostUrl));

			// First Name, Last Name, Message
			sb.Append("<div class='fieldContainer' data-type='text'><label><span class='requiredToken'>* </span> First Name</label><input name='firstName' type='text' placeholder='First Name' class='required'></div>");
			sb.Append("<div class='fieldContainer' data-type='text'><label><span class='requiredToken'>* </span>Last Name</label><input name='lastName' type='text' placeholder='Last Name' class='required'></div>");
			sb.Append("<div class='fieldContainer' data-type='text'><label><span class='requiredToken'>* </span>E-mail</label><input name='email' type='email' placeholder='you@email.com' class='required email'></div>");
			sb.Append("<div class='fieldContainer' data-type='textarea'><label>Message</label> <textarea name='message' rows='4'></textarea> </div>");

			// Submit Button
			sb.Append("<div class='fieldContainer' data-type='submit'><input type='submit' class='button' value='Send'>");

			sb.Append("</form></div>");

			return sb.ToString();
		}
	}
}