<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Blog.aspx.cs" Inherits="VMTips_2022.Blog" ValidateRequest="true" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <div id="wrap">
        
        <div id="leftside">
                    
            <asp:Repeater ID="rptBlog" runat="server">
                <ItemTemplate>
                    <asp:Label ID="lblTitle" runat="server" CssClass="blogHeader">
                        <%# String.Format("<a href=\"BlogPage.aspx?id={0}\"> {1}</a>", Eval("ID"), Eval("Title")) %>
                    </asp:Label>
                    <div id="blogEntry">
                        <%# Eval("Text") %>
                    </div>
                    <div id="blogFooter">
                        <%# String.Format("Postat {0}.", Eval("PostedDate")) %>
                        <%# Eval("Comments.Count").ToString() == "0" ? String.Format(" <a href=\"BlogPage.aspx?id={0}\"> Kommentera</a>", Eval("ID")) : String.Format("Det finns <a href=\"BlogPage.aspx?id={0}\"> {1} kommentarer.</a>", Eval("ID"), Eval("Comments.Count"))%>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <p>
                <asp:HyperLink ID="hplPrevious" runat="server" Visible="false" Text="<< Tidigare inlägg" />
                 | 
                <asp:HyperLink ID="hplNext" runat="server" Visible="false" Text="Senare inlägg >>" />
            </p>
        </div>
        
      
    </div>        
    
</asp:Content>

