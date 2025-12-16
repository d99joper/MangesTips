<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Statistics.aspx.cs" Inherits="Tips.Statistics" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.1/themes/base/jquery-ui.css" /> 
 
	<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"></script> 
	<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.1/jquery-ui.min.js"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            $.fx.speeds._default = 1000;
            $('td a').each(function () {
                var $link = $(this);

                $link.click(function () {
                    var $dialog = $('<div></div>')
				    .load($link.attr('href') + ' #resultView')
				    .dialog({
				        autoOpen: false,
				        title: $link.attr('title'),
				        width: 400,
				        height: 400,
				        modal: true,
				        buttons: { "Stäng": function () { $(this).dialog("close"); } },
				        show: 'explode',
				        hide: 'explode'
				    });

                    $dialog.dialog('open');

                    return false;
                });
            });
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="allContent" runat="server">
    <cc1:TabContainer ID="TabContainer1" runat="server">
    <cc1:TabPanel ID="tabMatches" runat="server" HeaderText="Matchstatistik">
    <ContentTemplate>
    <div class="wrap">        
        <p style="font-size:8pt;">Tabellen nedan beskriver hur användarna tippat på de olika matcherna. </p>
        <asp:Repeater ID="rptMatches" runat="server">
            <HeaderTemplate>
                <table id="game-stats">
                    <tr>
                        <th>&nbsp;</th>
                        <th>&nbsp;</th>
                        <th style="color:Black;"></th>
                        <th>&nbsp;</th>
                        <th style="color:Black;"></th>
                        <th style="width:50px; color:Black;">1</th>
                        <th style="width:50px; color:Black;">X</th>
                        <th style="width:50px; color:Black;">2</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr onmouseover="this.className='highlightrow';this.style.cursor='hand';"
                    onmouseout="this.className='normalrow';this.style.cursor='cursor';">
                    <td><%# String.Format(new System.Globalization.CultureInfo("en-US"), "{0:d/M}", Eval("Date"))%></td>
                    <td><%# Eval("HomeTeam.GroupID")%>&nbsp;</td>
                    <td><%# Eval("HomeTeam.TeamName")%></td>
                    <td>-</td>
                    <td><%# Eval("AwayTeam.TeamName")%></td>
                    <td><a href='StatDetails.aspx?type=Match&id=<%# Eval("ID")%>&Result=1' title='<%#String.Format("{0} - {1} : 1", Eval("HomeTeam.TeamName"), Eval("AwayTeam.TeamName")) %>' ><%# String.Format("{0:P0}", Eval("HomeWinPercent"))%></a></td>
                    <td><a href='StatDetails.aspx?type=Match&id=<%# Eval("ID")%>&Result=X' title='<%#String.Format("{0} - {1} : X", Eval("HomeTeam.TeamName"), Eval("AwayTeam.TeamName")) %>' ><%# String.Format("{0:P0}", Eval("DrawPercent"))%></a></td>
                    <td><a href='StatDetails.aspx?type=Match&id=<%# Eval("ID")%>&Result=2' title='<%#String.Format("{0} - {1} : 2", Eval("HomeTeam.TeamName"), Eval("AwayTeam.TeamName")) %>' ><%# String.Format("{0:P0}", Eval("AwayWinPercent"))%></a></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></table></FooterTemplate>
        </asp:Repeater>        
    </div>
    </ContentTemplate>
    </cc1:TabPanel>
    <cc1:TabPanel ID="tabTeams" runat="server" HeaderText="Lagstatistik">
    <ContentTemplate>
        <p>
        <asp:UpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <p style="font-size:8pt;">Tabellen nedan beskriver de olika lagen samt hur många procent av användarna som tippat att laget ska gå till slutspel, kvartsfinal, osv. </p>
            <asp:GridView ID="grdTeamStats" runat="server" AutoGenerateColumns="False" OnRowDataBound="grdTeamStats_RowDataBound"
                AllowSorting="true" CssClass="stats" OnSorting="SortTeams" OnSelectedIndexChanged="ViewTeamDetails" DataKeyNames="ID"
                HeaderStyle-HorizontalAlign="Center" RowStyle-HorizontalAlign="Center" RowStyle-Width="100px">
                <Columns>
                    <asp:CommandField ShowSelectButton="true" SelectText="Select" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                        <asp:TemplateField SortExpression="TeamName" HeaderText="Land" > 
                            <ItemTemplate>
                                <%# Eval("TeamName") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="Playoff" HeaderText="Slutspel" ControlStyle-Width="200px">
                            <ItemTemplate>
                                <a href='<%#String.Format("StatDetails.aspx?type=playoffs&stage=playoff&teamid={0}", Eval("ID")) %>' title='<%#Eval("TeamName") %> tar sig vidare från gruppen'><%# String.Format("{0:P0}",Eval("TeamStats.PlayoffPercent")) %></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                         
                        <asp:TemplateField SortExpression="QuarterFinals" HeaderText="Kvartsfinal">
                            <ItemTemplate>
                                <a href='<%#String.Format("StatDetails.aspx?type=playoffs&stage=quarterfinals&teamid={0}", Eval("ID")) %>' title='<%#Eval("TeamName") %> går till kvartsfinal'><%# String.Format("{0:P0}",Eval("TeamStats.QuarterFinalPercent"))%></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField SortExpression="SemiFinals" HeaderText="Semifinal">
                            <ItemTemplate>
                                <a href='<%#String.Format("StatDetails.aspx?type=playoffs&stage=semifinals&teamid={0}", Eval("ID")) %>' title='<%#Eval("TeamName") %> går till semifinal'><%# String.Format("{0:P0}",Eval("TeamStats.SemiFinalPercent"))%></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="Finals" HeaderText="Final">
                            <ItemTemplate>
                                <a href='<%#String.Format("StatDetails.aspx?type=playoffs&stage=final&teamid={0}", Eval("ID")) %>' title='<%#Eval("TeamName") %> går till final'><%# String.Format("{0:P0}",Eval("TeamStats.FinalPercent"))%></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField SortExpression="Bronze" HeaderText="Brons">
                            <ItemTemplate>
                                <a href='<%#String.Format("StatDetails.aspx?type=playoffs&stage=bronze&teamid={0}", Eval("ID")) %>' title='<%#Eval("TeamName") %> tar brons'><%# String.Format("{0:P0}", Eval("TeamStats.BronzePercent"))%></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField SortExpression="Silver" HeaderText="Silver">
                            <ItemTemplate>
                                <a href='<%#String.Format("StatDetails.aspx?type=playoffs&stage=silver&teamid={0}", Eval("ID")) %>' title='<%#Eval("TeamName") %> tar silver'><%# String.Format("{0:P0}", Eval("TeamStats.SilverPercent"))%></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="Gold" HeaderText="Guld">
                            <ItemTemplate>
                                <a href='<%#String.Format("StatDetails.aspx?type=playoffs&stage=gold&teamid={0}", Eval("ID")) %>' title='<%#Eval("TeamName") %> tar guld'><%# String.Format("{0:P0}", Eval("TeamStats.GoldPercent"))%></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate> 
        </asp:UpdatePanel>
    </p>    
    </ContentTemplate>
    </cc1:TabPanel>
    <cc1:TabPanel ID="tabTopScorer" runat="server" HeaderText="Skytteligan">
    <ContentTemplate>
    <div class="wrap">        
        <p style="font-size:8pt;">Tabellen nedan beskriver de olika potentiella målskyttar samt hur många procent av användarna som tippat att vinna. </p>
        <asp:Repeater ID="rptTopScorer" runat="server">
            <HeaderTemplate>
                <table id="topscorer-stats">
                    <tr>
                        <th align="center">Namn</td>
                        <th align="center">Procent</td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr onmouseover="this.className='highlightrow';this.style.cursor='hand';"
                    onmouseout="this.className='normalrow';this.style.cursor='cursor';">
                    <td><%# Eval("DisplayName") %></td>
                    <td align="center"><a href='<%#String.Format("StatDetails.aspx?type=topscorer&id={0}", Eval("ID")) %>' title='<%# Eval("DisplayName") %> vinner skytteligan' ><%# String.Format("{0:P0}", Eval("WinPercent")) %></a></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></table></FooterTemplate>
        </asp:Repeater>        
    </div>
    </ContentTemplate>
    </cc1:TabPanel>
</cc1:TabContainer>
</div>
</asp:Content>
