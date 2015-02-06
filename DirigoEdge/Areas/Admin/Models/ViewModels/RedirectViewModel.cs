using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

public class RedirectViewModel : DirigoBaseModel
{
    public List<Redirect> Redirects;
    public List<string> Pages = new List<string>();

    public RedirectViewModel()
    {
        BookmarkTitle = "Configure Redirects";
        Redirects = Context.Redirects.ToList();

        var pages = Context.ContentPages.ToList();
        foreach (var page in pages)
        {
            Pages.Add(NavigationUtils.GetGeneratedUrl(page));
        }
    }
}