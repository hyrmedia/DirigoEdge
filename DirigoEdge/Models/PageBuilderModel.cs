using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirigoEdge.Models
{
	public class PageBuilderModel
	{
		public List<ContentModule> Modules;

		public PageBuilderModel()
		{
			using (var context = new DataContext())
			{
				Modules = context.ContentModules.ToList();
			}
		}


	}
}