using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirigoEdge.Areas.Admin.Models.ModuleModels
{
	public interface IModule
	{

		string RenderHtml();

		string GetDefaultHtml();



	}
}
