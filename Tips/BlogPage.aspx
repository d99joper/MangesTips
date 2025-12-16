<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BlogPage.aspx.cs" Inherits="VMTips_2022.BlogPage" ValidateRequest="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="wrap">
    
    
    <asp:Label ID="lblError" runat="server" CssClass="error" />
    <asp:Label ID="lblBlogTitle" runat="server" CssClass="blogHeader" />
    <div id="blogEntry">
        <asp:Label ID="lblBlogText" runat="server" />
    </div>
    <div id="blogFooter">
        <asp:Label ID="lblBlogDate" runat="server" />
    </div>
    
    <asp:UpdatePanel ID="updComments" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>    
        <div style="width:350px;">
            <asp:Repeater ID="rptComments" runat="server" OnItemDataBound="rptComments_OnItemDataBound" OnItemCommand="btnDeleteComment_OnClick">
                <ItemTemplate>
                    <div id="commentText">
                        <asp:HiddenField ID="hdnCommentID" runat="server" Value='<%# Eval("ID") %>' />
                        <%# Eval("Text") %>
                        <asp:Panel ID="divDeleteComment" runat="server" style="float:right;" Visible="false">
                            <asp:ImageButton ID="btnDeleteComment" runat="server" ImageUrl="~/images/icon_delete.gif" 
                                OnClick="btnDeleteComment_OnClick" CausesValidation="false" 
                                CommandName="Delete" CommandArgument='<%# Eval("ID") %>' AlternateText="Postback"  />
                        </asp:Panel>
                    </div>
                    <div id="commentFooter">
                        <%# String.Format("{0} : {1}", Eval("PostedBy"), Eval("PostedDate")) %>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <br />
        <!-- Add comments -->
        <div id="divAddComment" runat="server" >
            Namn: <asp:TextBox ID="txtName" runat="server" MaxLength="35" />
            <asp:RequiredFieldValidator ID="reqName" runat="server" ControlToValidate="txtName" ErrorMessage="*" />
            <br />
            De fem första bokstäverna i alfabetet är: 
            <asp:TextBox ID="txtControll" runat="server" Columns="5" Width="40px" MaxLength="10" />
            <asp:RequiredFieldValidator ID="reqControll" runat="server" ControlToValidate="txtControll" ErrorMessage="*" />
            <asp:RegularExpressionValidator ID="regControll" runat="server" ValidationExpression="[a|A][b|B][c|C][d|D][e|E]" 
                ControlToValidate="txtControll" ErrorMessage="*" />
            <br />
            <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine"
                Width="270px" Height="50px" />
            <asp:RequiredFieldValidator ID="reqComment" runat="server" ControlToValidate="txtComment" ErrorMessage="*" />
            <asp:Button ID="btnSend" runat="server" Text="Kommentera" OnClick="btnSendComment" />
        </div>
        
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="0">
            <ProgressTemplate>
                <div class="progress" >
                    <img src="../images/progress_indicator.gif" height="30px" alt="arbetar" style="float:left; padding-right: 5px;" />
                    Vänta lite... 
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </ContentTemplate>
    </asp:UpdatePanel> 
</div>

</asp:Content>
