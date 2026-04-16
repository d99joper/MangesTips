<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Tipset.Details" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="wrap">
    <p><asp:Label ID="lblError" runat="server" CssClass="error" /></p>
    
    <div id="leftside" style="width:530px;">
        <p>
            <asp:Label ID="lblDisplayName" runat="server" /> <br />
            Nuvarande placering: <asp:Label ID="lblPlace" runat="server" /><br />
            Totalpoäng: <asp:Label ID="lblPoints" runat="server" /><br />
            <asp:Label ID="lblvmtips" runat="server" />
            <asp:Label ID="lblemtips" runat="server" />
            <asp:Label ID="lblvmtips2014" runat="server" />
            <asp:Label ID="lblemtips2016" runat="server" />
            Se ursprunglig tipsrad för utskrift (pdf): <asp:HyperLink ID="hplPDF" runat="server" Text="PDF-fil" Target="_blank" />
        </p>
        <asp:Repeater ID="rptUserMatches" runat="server" >
            <HeaderTemplate>
                1. Tipsraden
                <table style="width: 500px;">
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# String.Format(new System.Globalization.CultureInfo("en-US"), "{0:d/M}", Eval("Match.Date"))%></td>
                    <td><%# Eval("Match.HomeTeam.GroupID")%>&nbsp;</td>
                    <td style="width: 160px"><%# Eval("Match.HomeTeam.TeamName")%></td>
                    <td>-</td>
                    <td style="width: 160px"><%# Eval("Match.AwayTeam.TeamName")%></td>
                    <td style="width: 35px"><%# Eval("HomeGoals")%>-<%# Eval("AwayGoals")%></td>
                    <td style="padding:0 0 0 20px;"><%# Eval("Match.ResultMark") != null ? String.Format("{0}p",Eval("Points")) : "" %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <br /><br />
        <div class="editor-label">
            2. Vidare från gruppen 
        </div>
        <div id="divPlayoffTeams">
            <table class="playoff">
                <thead>
                <tr>
                    <td>Grupp A <asp:Label ID="lblBonusA" runat="server" /></td>
                    <td>Grupp B <asp:Label ID="lblBonusB" runat="server" /></td>
                    <td>Grupp C <asp:Label ID="lblBonusC" runat="server" /></td>
                    <td>Grupp D <asp:Label ID="lblBonusD" runat="server" /></td>
                </tr>
                </thead>
                <tr>
                    <td>1. <asp:Label ID="lblPO1A" runat="server" /></td>
                    <td>1. <asp:Label ID="lblPO1B" runat="server" /></td>
                    <td>1. <asp:Label ID="lblPO1C" runat="server" /></td>
                    <td>1. <asp:Label ID="lblPO1D" runat="server" /></td>
                </tr>
                <tr>
                    <td>2. <asp:Label ID="lblPO2A" runat="server" /></td>
                    <td>2. <asp:Label ID="lblPO2B" runat="server" /></td>
                    <td>2. <asp:Label ID="lblPO2C" runat="server" /></td>
                    <td>2. <asp:Label ID="lblPO2D" runat="server" /></td>
                </tr> 
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td align="center">Grupp E <asp:Label ID="lblBonusE" runat="server" /></td>
                    <td align="center">Grupp F <asp:Label ID="lblBonusF" runat="server" /></td>
                    <td align="center">Grupp G <asp:Label ID="lblBonusG" runat="server" /></td>
                    <td align="center">Grupp H <asp:Label ID="lblBonusH" runat="server" /></td>
                </tr>
                <tr>
                    <td>1. <asp:Label ID="lblPO1E" runat="server" /></td>
                    <td>1. <asp:Label ID="lblPO1F" runat="server" /></td>
                    <td>1. <asp:Label ID="lblPO1G" runat="server" /></td>
                    <td>1. <asp:Label ID="lblPO1H" runat="server" /></td>
                </tr>
                <tr>
                    <td>2. <asp:Label ID="lblPO2E" runat="server" /></td>
                    <td>2. <asp:Label ID="lblPO2F" runat="server" /></td>
                    <td>2. <asp:Label ID="lblPO2G" runat="server" /></td>
                    <td>2. <asp:Label ID="lblPO2H" runat="server" /></td>
                </tr>
            </table>
        </div>
         
        <br /><br />
        <div class="editor-label">
            3. Vilka lag går till kvartsfinal? 
        </div>
        <div id="divQuarterFinalTeams">
            <table id="tblQuarterFinal" runat="server" class="playoff">
                
            </table>
        </div>
        
        <br /><br />
        <div class="editor-label">
            4. Vilka lag går till semifinal? 
        </div>
        <div id="div1">
            <table id="tblSemiFinalTeams" runat="server" class="playoff">
                
            </table>
        </div>
        
        <br /><br />
        <div class="editor-label">
            5. Vilka lag går till final? 
        </div>
        <div id="div2">
            <table id="tblFinalTeams" runat="server" class="playoff">
                
            </table>
        </div>
        
        <br /><br />
        6. Skytteligan: <asp:Label ID="lblTopScorer" runat="server" />
        
        <br /><br />
        7. Brons: <asp:Label ID="lblBronze" runat="server" />
        
        <br /><br />
        8. Silver: <asp:Label ID="lblSilver" runat="server" />
        
        <br /><br />
        9. Guld: <asp:Label ID="lblGold" runat="server" />
    </div>
    
    <div id="rightside" style="width: 330px;">
        <img src="images/overstlogga2022small3.png" alt="Manges VM-tips" />
    </div>
    
</div>
</asp:Content>
