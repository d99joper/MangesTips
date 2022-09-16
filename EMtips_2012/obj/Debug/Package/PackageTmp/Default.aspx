<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VMTips_2018.Default" ValidateRequest="true" %>
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
        
        <div id="rightside" runat="server" style="float:right;">
            <center><img src="images/loggo2018.png" alt="Manges VM-tips" /></center>
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="0">
                <ProgressTemplate>
                    <div class="progress" >
                        <img src="../images/progress_indicator.gif" height="30px" alt="arbetar" style="float:left; padding-right: 5px;" />
                        Vänta lite... 
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <p>
            Aktuell ställning vid datum: <asp:DropDownList ID="ddlUpdates" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ChangeStandings"
                 AppendDataBoundItems="true" DataTextField="UpdateDate" DataValueField="guid" CssClass="longdate" DataTextFormatString="{0:dd\/MM HH:mm}" />
            </p>
        
            <asp:UpdatePanel ID="updPanel" runat="server" ChildrenAsTriggers="true">
            <Triggers><asp:AsyncPostBackTrigger ControlID="ddlUpdates" EventName="SelectedIndexChanged" /></Triggers>
            <ContentTemplate>
            
            <asp:GridView ID="grdStandings" runat="server" AutoGenerateColumns="False" OnRowDataBound="grdStandings_RowDataBound"
                AllowSorting="true" OnSorting="SortUsers" OnSelectedIndexChanged="ViewUserDetails" DataKeyNames="UserID">
                
                <Columns>
                    <asp:CommandField ShowSelectButton="true" SelectText="Select" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                    <asp:TemplateField ShowHeader="false" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn">
                        <ItemTemplate><asp:HiddenField ID="hdnID" runat="server" Value='<%# Eval("UserID") %>' /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField SortExpression="Position" HeaderText="Plats"> 
                        <ItemTemplate>
                            <%# Eval("Position") %>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField SortExpression="FirstName" HeaderText="Förnamn">
                        <ItemTemplate>
                            <%# Eval("User.FirstName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField SortExpression="LastName" HeaderText="Efternamn">
                        <ItemTemplate>
                            <%# Eval("User.LastName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField SortExpression="TotalPoints" HeaderText="Poäng">
                        <ItemTemplate>
                            <%# Eval("TotalPoints") %>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            </ContentTemplate>
            </asp:UpdatePanel>    
            
        </div>    
    </div>        
    
</asp:Content>

