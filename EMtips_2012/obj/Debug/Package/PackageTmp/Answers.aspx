<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Answers.aspx.cs" Inherits="VMTips_2018.Answers" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="wrap">
    
        <div id="leftside">
            <div class="editor-field">1. Tipsraden. Tippa resultatet i gruppspelet, t.ex 2-0.</div>
            <table class="tips">
            <asp:Repeater ID="rptMatches" runat="server" >
                <ItemTemplate >
                    <tr onmouseover="this.className='highlightrow';this.style.cursor='hand';"
                        onmouseout="this.className='normalrow';this.style.cursor='cursor';">
                        <td style="display:none;">
                            <asp:HiddenField ID="hdnMatchID" runat="server" Value='<%#Eval("ID") %>' />
                        </td>
                        <td><%# String.Format(new System.Globalization.CultureInfo("en-US"), "{0:d/M}", Eval("Date"))%></td>
                        <td><%# Eval("HomeTeam.GroupID")%>&nbsp;</td>
                        <td><%# Eval("HomeTeam.TeamName")%></td>
                        <td>-</td>
                        <td><%# Eval("AwayTeam.TeamName")%></td>
                        <td>
                            <%#Eval("HomeGoals") %>
                        </td>
                        <td>-</td>
                        <td>
                            <%#Eval("AwayGoals") %>         
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            </table>
            <br /><br />
            <div class="editor-label">
                2. Vidare från gruppen 
            </div>
            <div id="divPlayoffTeams" runat="server">
                <table class="facit">
                    <thead><tr>
                        <td>Grupp A </td>
                        <td>Grupp B </td>
                        <td>Grupp C </td>
                        <td>Grupp D </td>
                    </tr></thead>
                    <tr>
                        <td>
                            1. <asp:Label ID="ddlGruppA_Lag1" runat="server" />
                        </td>
                        <td>
                            1. <asp:Label ID="ddlGruppB_Lag1" runat="server" />
                        </td>
                        <td>
                            1. <asp:Label ID="ddlGruppC_Lag1" runat="server" />
                        </td>                            
                        <td>
                            1. <asp:Label ID="ddlGruppD_Lag1" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            2. <asp:Label ID="ddlGruppA_Lag2" runat="server" />
                        </td>
                        <td>
                            2. <asp:Label ID="ddlGruppB_Lag2" runat="server" />
                        </td>
                        <td>
                            2. <asp:Label ID="ddlGruppC_Lag2" runat="server" />
                        </td>
                        <td>
                            2. <asp:Label ID="ddlGruppD_Lag2" runat="server" />
                        </td>
                    </tr>
                
                    <tr>
                        <td align="center">Grupp E</td>
                        <td align="center">Grupp F</td>
                        <td align="center">Grupp G</td>
                        <td align="center">Grupp H</td>
                    </tr></thead>
                    <tr>
                        <td>
                            1. <asp:Label ID="ddlGruppE_Lag1" runat="server" />
                        </td>
                        <td>
                            1. <asp:Label ID="ddlGruppF_Lag1" runat="server" />
                        </td>
                        <td>
                            1. <asp:Label ID="ddlGruppG_Lag1" runat="server" />
                        </td>                            
                        <td>
                            1. <asp:Label ID="ddlGruppH_Lag1" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            2. <asp:Label ID="ddlGruppE_Lag2" runat="server" />
                        </td>
                        <td>
                            2. <asp:Label ID="ddlGruppF_Lag2" runat="server" />
                        </td>
                        <td>
                            2. <asp:Label ID="ddlGruppG_Lag2" runat="server" />
                        </td>
                        <td>
                            2. <asp:Label ID="ddlGruppH_Lag2" runat="server" />
                        </td>
                    </tr>
                
                </table>
            </div>
        
            <br />
            <div class="editor-label">
                3. Vilka åtta lag går till kvartsfinal?
            </div>
            <div id="divQuarterFinalTeams" runat="server">
                <table>
                    <tr>
                        <td style="width:120px;">
                            <asp:Label ID="ddlKvart1" runat="server" />
                        </td>
                        <td style="width:120px;">
                            <asp:Label ID="ddlKvart5" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="ddlKvart2" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="ddlKvart6" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="ddlKvart3" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="ddlKvart7" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="ddlKvart4" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="ddlKvart8" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        
            <br />
            <div class="editor-label">
                4. Vilka fyra lag går till semifinal?
            </div>
            <div id="divSemiFinalTeams" runat="server">
                <table>
                    <tr>
                        <td style="width:120px;">
                            <asp:Label ID="ddlSemi1" runat="server" />
                        </td>
                        <td style="width:120px;">
                            <asp:Label ID="ddlSemi3" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="ddlSemi2" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="ddlSemi4" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
            
            <br />
            <div class="editor-label">
                5. Vilka två lag går till final?
            </div>
            <div id="divFinalTeams" runat="server">
                <table>
                    <tr>
                        <td style="width:120px;">
                            <asp:Label ID="ddlFinal1" runat="server" />
                        </td>
                        <td style="width:120px;">
                            <asp:Label ID="ddlFinal2" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
            
            <br />
            <div id="divTopScorer" runat="server">
                <table>
                    <tr>
                        <td>
                            <div class="editor-label">6. Skyttekung: <asp:Label ID="lblTopScorer" runat="server" /></div> 
                        </td>
                  </tr>
                </table>
            </div>
        
            <br />
            <div id="divBronzeTeam" runat="server">
                <table>
                    <tr>
                        <td>
                            <div class="editor-label">7. Brons: <asp:Label ID="ddlBronze" runat="server" /></div> 
                        </td>
                  </tr>
                </table>
            </div>
        
            <br />
            
            <div id="divSilverTeam" runat="server">
                <table>
                    <tr>
                        <td>
                            <div class="editor-label">8. Silver: <asp:Label ID="ddlSilver" runat="server" /></div>
                        </td>
                  </tr>
                </table>
            </div>

            <br />
            
            <div id="divGoldTeam" runat="server">
                <table>
                    <tr>
                        <td>
                            <div class="editor-label">9. Guld: <asp:Label ID="ddlGold" runat="server" /></div>
                        </td>
                  </tr>
                </table>
            </div>
        </div>
    
        <div id="rightside">
            <center><img src="images/loggo2018.png" alt="Manges VM-tips" /></center>
            
        </div>
    
    </div>
</asp:Content>
