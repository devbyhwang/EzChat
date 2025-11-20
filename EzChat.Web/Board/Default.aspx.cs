using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using EzChat.Web.App_Code.BLL;

namespace EzChat.Web.Board
{
    public partial class DefaultPage : System.Web.UI.Page
    {
        private const int PageSize = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string search = Request.QueryString["search"];
                txtSearch.Text = search;

                int page = 1;
                if (!string.IsNullOrEmpty(Request.QueryString["page"]))
                {
                    int.TryParse(Request.QueryString["page"], out page);
                }

                LoadPosts(page, search);
            }
        }

        private void LoadPosts(int page, string searchTerm)
        {
            DataTable posts = BoardBLL.GetPosts(page, PageSize, searchTerm);
            gvPosts.DataSource = posts;
            gvPosts.DataBind();

            int totalCount = BoardBLL.GetTotalPostCount(searchTerm);
            int totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            // 페이저 생성
            List<PageItem> pages = new List<PageItem>();
            for (int i = 1; i <= totalPages; i++)
            {
                pages.Add(new PageItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    IsCurrent = (i == page)
                });
            }

            rptPager.DataSource = pages;
            rptPager.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();
            Response.Redirect($"/Board/Default.aspx?search={Server.UrlEncode(search)}");
        }

        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string search = txtSearch.Text.Trim();
            string page = e.CommandArgument.ToString();

            string url = $"/Board/Default.aspx?page={page}";
            if (!string.IsNullOrEmpty(search))
            {
                url += $"&search={Server.UrlEncode(search)}";
            }

            Response.Redirect(url);
        }

        private class PageItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public bool IsCurrent { get; set; }
        }
    }
}
