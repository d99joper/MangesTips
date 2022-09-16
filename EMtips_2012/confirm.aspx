<%@ Page Title="Manges VM-tips: Bekräfta anmälan" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="confirm.aspx.cs" Inherits="VMTips_2022.confirm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table >
        <tr>
            <td>
                <h2 id="lblMessage" runat="server" class="error" >
                    Tack! Din kupong är nu registrerad.
                </h2>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblMoreInfo" runat="server" />
            </td>
        </tr>
    </table>
    
</asp:Content>
