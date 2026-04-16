<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NyKupong.aspx.cs" Inherits="Tipset.NyKupong" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>VM-tipset : Skapa ny kupong</title> 
    <link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="stylesheet" type="text/css"/>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js"></script>
    <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js"></script>
    <script>
        function pageLoad() {
            $(document).ready(function() {
                $("#ctl00_MainContent_txtTopScorer").autocomplete({
                source: "Helpers/TopScorers.ashx", delay: 100
            });
        })
        };
    </script>
    <script type="text/javascript">
        function findValue(li) {
            if (li == null) return alert("No match!");

            // if coming from an AJAX call, let's use the CityId as the value
            if (!!li.extra) var sValue = li.extra[0];

            // otherwise, let's just display the value in the text box test testtest
            else var sValue = li.selectValue;

            //alert("The value you selected was: " + sValue);
        }

        function selectItem(li) {
            findValue(li);
        }

        function formatItem(row) {
            return row[0];
        }

        function lookupAjax() {
            var oSuggest = $("#ctl00_MainContent_txtTopScorer")[0].autocompleter;
            oSuggest.findValue();
            return false;
        }

        function lookupLocal() {
            var oSuggest = $("#ctl00_MainContent_txtTopScorer")[0].autocompleter;

            oSuggest.findValue();

            return false;
        }
    </script>
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divPassedDeadline" runat="server" visible="false">Anmälningstiden till Manges VM-tips har gått ut.</div>
    
    <div id="wrap" runat="server">
        <%--<asp:UpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" >
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSubmit" />
        </Triggers>
        <ContentTemplate>--%>
        
        <h2>Skapa tipskupong</h2>
    
        <fieldset>
            <legend>Kupong</legend>
                          
            <div id="header">
            
                <asp:Label ID="lblError" runat="server" CssClass="error" />
                
                <div style="float:right;padding:5px 5px 5px 5px;width:400px;">
                    Vill du ladda ner tipset för att sitta och klura i lugn och ro innan du återvänder hit för att lämna in?
                    <a href="Docs/vmtipset2022.docx">Klicka här!</a>   
                    <br /><br />
                    <div id="rightside">
                        <center><img src="images/overstlogga2022small3.png" alt="Manges VM-tips"/></center>                                   
                    </div>
                </div>
            </div>        
            
            <div id="leftside">
                

                <div class="editor-label">
                    <label>Förnamn</label>
                </div>
                <div class="editor-field">
                    <asp:TextBox ID="txtFirstName" runat="server" MaxLength="45" />
                    <asp:RequiredFieldValidator ID="reqFirstName" runat="server" ControlToValidate="txtFirstName" 
                        ErrorMessage="*" SetFocusOnError="true" />
                </div>
                
                <div class="editor-label">
                    <label>Efternamn</label>
                </div>
                <div class="editor-field">
                    <asp:TextBox ID="txtLastName" runat="server" MaxLength="45" />
                    <asp:RequiredFieldValidator ID="reqLastName" runat="server" ControlToValidate="txtLastName" 
                        ErrorMessage="*" SetFocusOnError="true" />  
                </div>
                
                <div class="editor-label">
                    <label>E-post</label>
                </div>
                <div class="editor-field">
                    <asp:TextBox ID="txtEmail" runat="server" MaxLength="95" />
                    <asp:RequiredFieldValidator ID="reqEmail" runat="server" ControlToValidate="txtEmail" 
                        ErrorMessage="*" SetFocusOnError="true" />
                    <asp:RegularExpressionValidator ID="regEmail" runat="server" ControlToValidate="txtEmail" 
                        ErrorMessage="* Ogiltig E-post Address."  SetFocusOnError="true"/>
                </div>
                <br />                
                <div class="editor-label">1. Tipsraden. Tippa resultatet i gruppspelet, t.ex 2-0.</div>
                <table class="tips">
                <asp:Repeater ID="rptUserMatches" runat="server" >
                    <ItemTemplate >
                        <tr>
                            <td style="display:none;">
                                <asp:HiddenField ID="hdnMatchID" runat="server" Value='<%#Eval("ID") %>' />
                            </td>
                            <td><%# String.Format(new System.Globalization.CultureInfo("en-US"), "{0:d/M}", Eval("Date"))%></td>
                            <td><%# Eval("HomeTeam.GroupID")%>&nbsp;</td>
                            <td><%# Eval("HomeTeam.TeamName")%></td>
                            <td>-</td>
                            <td><%# Eval("AwayTeam.TeamName")%></td>
                            <td>
                                <asp:Textbox ID="txtHomeGoals" runat="server" CssClass="goalInput" MaxLength="2" AutoComplete="off" />
                            </td>
                            <td>-</td>
                            <td style="width:60px">
                                <asp:Textbox ID="txtAwayGoals" runat="server" CssClass="goalInput" MaxLength="2" AutoComplete="off" /> 
                                <asp:RequiredFieldValidator ID="reqAwayGoals" runat="server" ErrorMessage="*" Display="Dynamic"
                                    ControlToValidate="txtAwayGoals" SetFocusOnError="true" />
                                <cc1:FilteredTextBoxExtender ID="fltHomeGoals" runat="server" FilterType="Numbers" TargetControlID="txtHomeGoals" />
                                <cc1:FilteredTextBoxExtender ID="fltAwayGoals" runat="server" FilterType="Numbers" TargetControlID="txtAwayGoals" />                                
                                <asp:RequiredFieldValidator ID="reqHomeGoals" runat="server" ErrorMessage="*" Display="Dynamic"
                                    ControlToValidate="txtHomeGoals" SetFocusOnError="true" />
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
                
                    <table>
                        <thead><tr>
                            <td>Grupp A <asp:CompareValidator ID="CompareValidator17" runat="server" ControlToValidate="ddlGruppA_Lag2" ControlToCompare="ddlGruppA_Lag1" ErrorMessage="*" SetFocusOnError="true" Operator="NotEqual" /></td>
                            <td>Grupp B <asp:CompareValidator ID="CompareValidator16" runat="server" ControlToValidate="ddlGruppB_Lag2" ControlToCompare="ddlGruppB_Lag1" ErrorMessage="*" SetFocusOnError="true" Operator="NotEqual" /></td>
                            <td>Grupp C <asp:CompareValidator ID="CompareValidator15" runat="server" ControlToValidate="ddlGruppC_Lag2" ControlToCompare="ddlGruppC_Lag1" ErrorMessage="*" SetFocusOnError="true" Operator="NotEqual" /></td>
                            <td>Grupp D <asp:CompareValidator ID="CompareValidator4" runat="server" ControlToValidate="ddlGruppD_Lag2" ControlToCompare="ddlGruppD_Lag1" ErrorMessage="*" SetFocusOnError="true" Operator="NotEqual" /></td>
                        </tr></thead>
                        <tr>
                            <td>
                                1:a <asp:DropDownList ID="ddlGruppA_Lag1" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
                                </asp:DropDownList>                                
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppA_Lag1" ID="RequiredFieldValidator46" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>
                            <td>
                                1:a <asp:DropDownList ID="ddlGruppB_Lag1" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
                                </asp:DropDownList>                                
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppB_Lag1" ID="RequiredFieldValidator45" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>
                            <td>
                                1:a <asp:DropDownList ID="ddlGruppC_Lag1" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppC_Lag1" ID="RequiredFieldValidator44" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>                            
                            <td>
                                1:a <asp:DropDownList ID="ddlGruppD_Lag1" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppD_Lag1" ID="RequiredFieldValidator43" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                2:a <asp:DropDownList ID="ddlGruppA_Lag2" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppA_Lag2" ID="RequiredFieldValidator42" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>
                            <td>
                                2:a <asp:DropDownList ID="ddlGruppB_Lag2" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppB_Lag2" ID="RequiredFieldValidator41" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>
                            <td>
                                2:a <asp:DropDownList ID="ddlGruppC_Lag2" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppC_Lag2" ID="RequiredFieldValidator40" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>
                            <td>
                                2:a <asp:DropDownList ID="ddlGruppD_Lag2" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppD_Lag2" ID="RequiredFieldValidator39" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>
                        </tr>
                    </table>
                    <table>
                        <thead><tr>
                            <td>Grupp E <asp:CompareValidator ID="CompareValidator21" runat="server" ControlToValidate="ddlGruppE_Lag2" ControlToCompare="ddlGruppE_Lag1" ErrorMessage="*" SetFocusOnError="true" Operator="NotEqual" /></td>
                            <td>Grupp F <asp:CompareValidator ID="CompareValidator20" runat="server" ControlToValidate="ddlGruppF_Lag2" ControlToCompare="ddlGruppF_Lag1" ErrorMessage="*" SetFocusOnError="true" Operator="NotEqual" /></td>
                            <td>Grupp G <asp:CompareValidator ID="CompareValidator19" runat="server" ControlToValidate="ddlGruppG_Lag2" ControlToCompare="ddlGruppG_Lag1" ErrorMessage="*" SetFocusOnError="true" Operator="NotEqual" /></td>
                            <td>Grupp H <asp:CompareValidator ID="CompareValidator18" runat="server" ControlToValidate="ddlGruppH_Lag2" ControlToCompare="ddlGruppH_Lag1" ErrorMessage="*" SetFocusOnError="true" Operator="NotEqual" /></td>
                        </tr></thead>
                        <tr>
                            <td>
                                1:a <asp:DropDownList ID="ddlGruppE_Lag1" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppE_Lag1" ID="RequiredFieldValidator38" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>
                            <td>
                                1:a <asp:DropDownList ID="ddlGruppF_Lag1" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppF_Lag1" ID="RequiredFieldValidator37" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>
                            <td>
                                1:a <asp:DropDownList ID="ddlGruppG_Lag1" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppG_Lag1" ID="RequiredFieldValidator36" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>                            
                            <td>
                                1:a <asp:DropDownList ID="ddlGruppH_Lag1" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppH_Lag1" ID="RequiredFieldValidator35" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                2:a <asp:DropDownList ID="ddlGruppE_Lag2" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppE_Lag2" ID="RequiredFieldValidator34" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>
                            <td>
                                2:a <asp:DropDownList ID="ddlGruppF_Lag2" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppF_Lag2" ID="RequiredFieldValidator33" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>
                            <td>
                                2:a <asp:DropDownList ID="ddlGruppG_Lag2" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppG_Lag2" ID="RequiredFieldValidator14" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />                
                            </td>
                            <td>
                                2:a <asp:DropDownList ID="ddlGruppH_Lag2" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGruppH_Lag2" ID="RequiredFieldValidator15" runat="server" ErrorMessage="*" InitialValue="-1" SetFocusOnError="true" />
                            </td>
                        </tr>
                    </table>
                </div>
                <br />
                
                <div class="editor-label">
                    3. Vilka åtta lag går till kvartsfinal?
                    <asp:Label ID="lblErrorQuarter" runat="server" Text="* Samma lag två gånger." CssClass="error" Visible="false" />
                </div>
                <div id="divQuarterFinalTeams" runat="server">
                    <table>
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddlKvart1" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlKvart1" ID="RequiredFieldValidator16" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlKvart5" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 5" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlKvart5" ID="RequiredFieldValidator17" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddlKvart2" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlKvart2" ID="RequiredFieldValidator18" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlKvart6" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 6" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlKvart6" ID="RequiredFieldValidator19" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddlKvart3" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 3" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlKvart3" ID="RequiredFieldValidator20" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlKvart7" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 7" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlKvart7" ID="RequiredFieldValidator21" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddlKvart4" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 4" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlKvart4" ID="RequiredFieldValidator22" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlKvart8" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 8" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlKvart8" ID="RequiredFieldValidator23" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                        </tr>
                    </table>
                </div>
                <br />
                <div class="editor-label">
                    4. Vilka fyra lag går till semifinal?
                    <asp:Label ID="lblErrorSemi" runat="server" Text="* Samma lag två gånger." CssClass="error" Visible="false" />
                </div>
                <div id="divSemiFinalTeams" runat="server">
                    <table>
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddlSemi1" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlSemi1" ID="RequiredFieldValidator24" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlSemi3" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 3" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlSemi3" ID="RequiredFieldValidator25" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlSemi2" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlSemi2" ID="RequiredFieldValidator26" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlSemi4" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 4" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlSemi4" ID="RequiredFieldValidator27" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                        </tr>
                    </table>
                </div>
                
                <br />
                 <%--<div class="editor-label">
                    4. Vilka två lag går till final?
                    <asp:Label ID="lblErrorFinals" runat="server" Text="* Samma lag två gånger." CssClass="error" Visible="false" />
                </div>--%>
                <div class="editor-label">&nbsp;</div>
                <div id="divFinalTeams" runat="server">
                    <table>
                        <tr>
                            <td>5. Vilka två lag går till final?
                                <asp:DropDownList ID="ddlFinal1" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlFinal1" ID="RequiredFieldValidator28" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlFinal2" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlFinal2" ID="RequiredFieldValidator29" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                        </tr>
                    </table>
                    <asp:Label ID="lblErrorFinal" runat="server" Text="* Samma lag två gånger." CssClass="error" Visible="false" />
                </div>
                    
                <br />
                <%-- <div class="editor-label">7. Skyttekung:</div>--%>
                <div class="editor-label">&nbsp;</div>
                <div id="divTopScorer" runat="server">
                    <table>
                        <tr>
                            <td>6. Skyttekung: 
                                <asp:TextBox ID="txtTopScorer" runat="server" AutoComplete="off" />
                                <asp:Label ID="lblErrorScorer" runat="server" CssClass="error" />
                            </td>
                        </tr>
                    </table>
                </div>
                
                <br />
                <div class="editor-label">&nbsp;</div>
                <div id="divBronzeTeam" runat="server">
                    <table>
                        <tr>
                            <td>
                                7. Brons:
                                <asp:DropDownList ID="ddlBronze" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlBronze" ID="RequiredFieldValidator30" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                        </tr>
                    </table>
                </div>
                <%-- <div class="editor-label">6. Silver:</div> --%>
                <div class="editor-label">&nbsp;</div>
                <div id="divSilverTeam" runat="server">
                    <table>
                        <tr>
                            <td>
                                8. Silver: <asp:DropDownList ID="ddlSilver" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlSilver" ID="RequiredFieldValidator31" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>
                        </tr>
                    </table>
                </div>
                 
                <br />                
                <div id="divGoldTeam" runat="server">
                    <table>
                        <tr>
                            <td>
                                9. Guld: <asp:DropDownList ID="ddlGold" runat="server" AppendDataBoundItems="true"
                                    DataValueField="ID" DataTextField="TeamName">
                                    <asp:ListItem Text="Lag" Value="-1" Selected="True" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlGold" ID="RequiredFieldValidator32" runat="server" InitialValue="-1" ErrorMessage="*" />
                            </td>                                    
                        </tr>
                    </table>
                </div>
                
                <br />
                        
            </div>
           
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="0">
                <ProgressTemplate>
                    <div id="progress" runat="server" class="progress">
                        <img src="images/progress_indicator.gif" height="30px" alt="arbetar" />Vänta lite... 
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <cc1:AlwaysVisibleControlExtender ID="test" runat="server" TargetControlID="progress" HorizontalSide="Center" VerticalSide="Bottom" VerticalOffset="100" />
            
        </fieldset>
        
       <%-- </ContentTemplate>
        </asp:UpdatePanel>--%>

        <div id="footer">
            <asp:Button ID="btnSubmit" runat="server" Text="Skicka" OnClick="btnSubmit_OnClick" CausesValidation="true" />
        </div>
            
           
        
         
    </div>


</asp:Content>
