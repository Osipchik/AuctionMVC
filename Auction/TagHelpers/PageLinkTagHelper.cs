using System.Collections.Generic;
using Auction.DTO.Pagination;
using Auction.DTO.SortOptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Auction.TagHelpers
{
    [HtmlTargetElement("ul", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory _urlHelperFactory;
        
        public PageLinkTagHelper(IUrlHelperFactory helperFactory) {
            _urlHelperFactory = helperFactory;
        }
        
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PagingInfo PageModel { get; set; }
        public string PageAction { get; set; }
        
        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }
        
        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();

        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            
            var result = new TagBuilder("ul");
            var countFrom = PageModel.CurrentPage > 6 ? PageModel.CurrentPage - 6 : 1;
            for (var i = countFrom; i <= PageModel.CurrentPage + 6 && i <= PageModel.TotalPages; i++) {
                var item = new TagBuilder("li");
                var tag = new TagBuilder("a");
                PageUrlValues["page"] = i;
                tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                if (PageClassesEnabled) {
                    tag.AddCssClass(PageClass);
                    item.AddCssClass(i == PageModel.CurrentPage
                        ? PageClassSelected : PageClassNormal);
                }
                tag.InnerHtml.Append(i.ToString());
                item.InnerHtml.AppendHtml(tag);
                result.InnerHtml.AppendHtml(item);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}