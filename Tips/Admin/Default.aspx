<%@ Page Language="C#" MasterPageFile="~/Admin/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VMTips_2022.Admin.Default" ValidateRequest="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="HTMLEditor" %>

<asp:Content ID="cntHead" runat="server" ContentPlaceHolderID="head">
	<link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="stylesheet" type="text/css"/>
	<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js"></script>
	<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js"></script>
	<script type="text/javascript">

		var countTextBoxes = 0;

		function pageLoad() {

			$(document).ready(function () {
				$("#ctl00_MainContent_tabs_tabCoupon_txtTopScorer").autocomplete({
					source: "../Helpers/TopScorers.ashx" ,
					dataType: "json"
				});

				$("#ctl00_MainContent_tabs_tabCoupon_lnkAddAnswerScorer").click(function() {
					countTextBoxes++;
					$('#ctl00_MainContent_tabs_tabCoupon_pnlScorers').append(AddTextBox("txtAdditionalTopScorers" + countTextBoxes));
					return (false);
				});
			});
		};
		function add(type) {
			//Create an input type dynamically.
			var element = document.createElement("input");
			var foo = document.getElementById("fooBar");
			//Append the element in page (in span).
			foo.appendChild(element);
		}
	
		function AddTextBox(id) {
			var textbox = document.createElement('input');
			$(textbox).attr("id", id);
			$(textbox).attr("style", "display:block");
			$(textbox).autocomplete({ source: "../Helpers/TopScorers.ashx" });
			$(textbox).focus();
			return $(textbox);
		}

		function SetTopScorers() {
			$("#<%=hdnTopScorers.ClientID %>").val("");

			var scorers = "";
			$("[id*='txtAdditionalTopScorers']").each(function () {
				scorers += "|" + $(this).val();
			});
			$("#<%=hdnTopScorers.ClientID %>").val(scorers);
		}
		
	</script>
</asp:Content>

<asp:Content ID="cntMain" runat="server" ContentPlaceHolderID="MainContent">
	
	<div class="logout"><asp:LinkButton ID="btnLogout" runat="server" OnClick="Logout" Text="Logga ut" /></div>
	<cc1:TabContainer ID="tabs" runat="server">
	<cc1:TabPanel ID="tabCoupon" runat="server" HeaderText="Facit">
	<ContentTemplate>
	<div class="wrap">
		<asp:UpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" >
		<ContentTemplate>
	   
		<asp:Label ID="lblError" runat="server" CssClass="error" />
		
		<div class="leftside">
			<div class="editor-field">1. Tipsraden. Tippa resultatet i gruppspelet, t.ex 2-0.</div>
			<table class="tips">
			<asp:Repeater ID="rptMatches" runat="server" >
				<ItemTemplate >
					<tr>
						<td style="display:none;">
							<asp:HiddenField ID="hdnMatchID" runat="server" Value='<%#Eval("ID") %>' />
						</td>
						<td><%# String.Format(new System.Globalization.CultureInfo("en-US"),"{0:d/M}",Eval("Date")) %></td>
						<td><%# Eval("HomeTeam.GroupID")%>&nbsp;</td>
						<td><%# Eval("HomeTeam.TeamName")%></td>
						<td>-</td>
						<td><%# Eval("AwayTeam.TeamName")%></td>
						<td>
							<asp:Textbox ID="txtHomeGoals" runat="server" CssClass="goalInput" Text='<%#Eval("HomeGoals") %>' MaxLength="2" AutoComplete="off" />
						</td>
						<td>-</td>
						<td>
							<asp:Textbox ID="txtAwayGoals" runat="server" CssClass="goalInput" Text='<%#Eval("AwayGoals") %>' MaxLength="2" AutoComplete="off" /> 
							<cc1:FilteredTextBoxExtender ID="fltHomeGoals" runat="server" FilterType="Numbers" TargetControlID="txtHomeGoals" />
							<cc1:FilteredTextBoxExtender ID="fltAwayGoals" runat="server" FilterType="Numbers" TargetControlID="txtAwayGoals" />                                
						</td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
			</table>
		</div>
	
		<div class="rightside">
			<img src="../images/loggacolor.png" alt="Manges VM-tips" width="350px"/>
			<div class="editor-label">
				2. Vidare från gruppen 
			</div>
			<div id="divPlayoffTeams" runat="server">
				<table>
					<thead><tr>
						<td>Grupp A </td>
						<td>Grupp B </td>
						<td>Grupp C </td>
						<td>Grupp D </td>
					</tr></thead>
					<tr>
						<td>
							1. <asp:DropDownList ID="ddlGruppA_Lag1" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							1. <asp:DropDownList ID="ddlGruppB_Lag1" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							1. <asp:DropDownList ID="ddlGruppC_Lag1" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>                            
						<td>
							1. <asp:DropDownList ID="ddlGruppD_Lag1" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td>
							2. <asp:DropDownList ID="ddlGruppA_Lag2" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							2. <asp:DropDownList ID="ddlGruppB_Lag2" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							2. <asp:DropDownList ID="ddlGruppC_Lag2" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							2. <asp:DropDownList ID="ddlGruppD_Lag2" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
					</tr>
				</table>                 
				<table>
					<thead><tr>
						<td>Grupp E</td>
						<td>Grupp F</td>
						<td>Grupp G</td>
						<td>Grupp H</td>
					</tr></thead>
					<tr>
						<td>
							1. <asp:DropDownList ID="ddlGruppE_Lag1" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							1. <asp:DropDownList ID="ddlGruppF_Lag1" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							1. <asp:DropDownList ID="ddlGruppG_Lag1" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>                            
						<td>
							1. <asp:DropDownList ID="ddlGruppH_Lag1" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td>
							2. <asp:DropDownList ID="ddlGruppE_Lag2" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							2. <asp:DropDownList ID="ddlGruppF_Lag2" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							2. <asp:DropDownList ID="ddlGruppG_Lag2" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							2. <asp:DropDownList ID="ddlGruppH_Lag2" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
							</asp:DropDownList>
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
						<td>
							<asp:DropDownList ID="ddlKvart1" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							<asp:DropDownList ID="ddlKvart5" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 5" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td>
							<asp:DropDownList ID="ddlKvart2" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							<asp:DropDownList ID="ddlKvart6" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 6" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td>
							<asp:DropDownList ID="ddlKvart3" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 3" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							<asp:DropDownList ID="ddlKvart7" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 7" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td>
							<asp:DropDownList ID="ddlKvart4" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 4" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							<asp:DropDownList ID="ddlKvart8" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 8" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
					</tr>
				</table>
			</div>
			<br />
			<div class="editor-label">
				3. Vilka fyra lag går till semifinal?
			</div>
			<div id="divSemiFinalTeams" runat="server">
				<table>
					<tr>
						<td>
							<asp:DropDownList ID="ddlSemi1" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							<asp:DropDownList ID="ddlSemi3" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 3" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td>
							<asp:DropDownList ID="ddlSemi2" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							<asp:DropDownList ID="ddlSemi4" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 4" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
					</tr>
				</table>
			</div>
			
			<br />
			<div class="editor-label">
				4. Vilka två lag går till final?
			</div>
			<div id="divFinalTeams" runat="server">
				<table>
					<tr>
						<td>
							<asp:DropDownList ID="ddlFinal1" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 1" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
						<td>
							<asp:DropDownList ID="ddlFinal2" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag 2" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
					</tr>
				</table>
			</div>
			
			<br />
			<div class="editor-label">5. Skyttekung:</div>
			<div id="divTopScorer" runat="server">
				<asp:HiddenField ID="hdnTopScorers" runat="server" />
				<table>
					<tr>
						<td>
							<asp:TextBox ID="txtTopScorer" runat="server" AutoComplete="off" />
							<br />
							<asp:Panel id="pnlScorers" runat="server" />
							<asp:LinkButton ID="lnkAddAnswerScorer" runat="server" Text="En till..." OnClick="AddMoreAnswerScorers" /> 
							
						</td>
				  </tr>
				</table>
			</div>
			<br />
			<div class="editor-label">7. Brons:</div>
			<div id="divBronzeTeam" runat="server">
				<table>
					<tr>
						<td>
							<asp:DropDownList ID="ddlBronze" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
				  </tr>
				</table>
			</div>
			<br />
			<div class="editor-label">6. Silver:</div>
			<div id="divSilverTeam" runat="server">
				<table>
					<tr>
						<td>
							<asp:DropDownList ID="ddlSilver" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
				  </tr>
				</table>
			</div>

			<br />
			<div class="editor-label">7. Guld:</div>
			<div id="divGoldTeam" runat="server">
				<table>
					<tr>
						<td>
							<asp:DropDownList ID="ddlGold" runat="server" AppendDataBoundItems="true"
								DataValueField="ID" DataTextField="TeamName">
								<asp:ListItem Text="Lag" Value="-1" Selected="True" />
							</asp:DropDownList>
						</td>
				  </tr>
				</table>
			</div>
		</div>

		<asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="0">
			<ProgressTemplate>
				<div id="progress" class="progress_long" runat="server">
					<img src="../images/progress_indicator.gif" height="30px" alt="arbetar" style="float:left; padding-right: 5px;" />
					Sparar facit,<br />
					och uppdaterar alla tipsrader... 
				</div>
			</ProgressTemplate>
		</asp:UpdateProgress>
		<cc1:AlwaysVisibleControlExtender ID="test" runat="server" TargetControlID="progress" HorizontalSide="Center" VerticalSide="Bottom" VerticalOffset="200" />

		<div id="footer">
			<asp:Button ID="btnSubmitAnswers" runat="server" Text="Rätta" OnClick="btnSubmitResult_OnClick" CausesValidation="false" OnClientClick="SetTopScorers();" />
		</div>
	 
		</ContentTemplate>
		</asp:UpdatePanel>
	</div>
	</ContentTemplate>
	</cc1:TabPanel>
	<cc1:TabPanel ID="tabUsers" runat="server" HeaderText="Användare">
	<ContentTemplate>
	<asp:UpdatePanel ID="pnlUpdate2" runat="server">
	<ContentTemplate>
		
		<asp:Label ID="lblError2" runat="server" CssClass="error" />
		
		<asp:Repeater ID="rptUsers" runat="server">
			<HeaderTemplate>
				<p><asp:Button ID="btnSaveUsers2" runat="server" Text="Spara" OnClick="SaveUsers" /></p>
				<table class="border">
					<thead>
						<th>Fullständigt namn</th>
						<th>E-post</th>
						<th>Personlig kod</th>
						<th>Har betalt</th>
						<th>Bekräftad</th>
						<th>Vinnare</th>
						<th>Inskickat</th> 
						<th>Kupong</th>
					</thead>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td><%#Eval("DisplayName") %> <asp:HiddenField ID="hdnUserID" runat="server" Value='<%#Eval("ID") %>' /></td>
					<td><a href='<%# String.Format("mailto:{0}", Eval("EmailAddress"))%>'><%#Eval("EmailAddress")%></a></td>
					<td><%#Eval("PayCode")%></td>
					<td><asp:CheckBox ID="chkHasPaid" runat="server" Checked='<%#Eval("HasPaid") %>' /></td>
					<td><asp:CheckBox ID="chkIsConfirmed" runat="server" Checked='<%#Eval("IsConfirmed") %>' /></td>
					<td><asp:CheckBox ID="chkIsWinner" runat="server" Checked='<%#Eval("IsWinner") %>' /></td>
					<td><%#String.Format("{0:dd/MM/yyyy}", Eval("PostedDate")) %></td>
					<td><a href='<%#String.Format("../pdfGenerator.aspx?id={0}",Eval("Guid")) %>' target="_blank">PDF</a></td>                    
				</tr>
			</ItemTemplate>
			<FooterTemplate>
				</table>
				<p><asp:Button ID="btnSaveUsers2" runat="server" Text="Spara" OnClick="SaveUsers" CausesValidation="false" /></p>
			</FooterTemplate>
		</asp:Repeater>
		
		<asp:UpdateProgress ID="UpdateProgress2" runat="server" DisplayAfter="0" DynamicLayout="false">
			<ProgressTemplate>
				<div class="progress">
					<img src="../images/progress_indicator.gif" height="30px" alt="arbetar" style="float:left; padding-right: 5px;" />
					Sparar ... 
				</div>
			</ProgressTemplate>
		</asp:UpdateProgress>
		
	</ContentTemplate>
	</asp:UpdatePanel>
	</ContentTemplate>
	</cc1:TabPanel>
	<cc1:TabPanel ID="tabOther" runat="server" HeaderText="Övrigt">
	<ContentTemplate>
	<asp:UpdatePanel ID="updPanel3" runat="server">
	<Triggers><asp:PostBackTrigger ControlID="ddlCssStyle" /></Triggers>
	<ContentTemplate>
		<asp:Label ID="lblError3" runat="server" CssClass="error" />
		<asp:Label ID="lblDisableNewEntries" runat="server" /><br /><br />
		<asp:Button ID="btnEnableDisableNewEntries" runat="server" OnClick="EnableDisableNewEntries" CausesValidation="false" />
		<br />
		<asp:UpdateProgress ID="UpdateProgress3" runat="server" DisplayAfter="0" DynamicLayout="false">
			<ProgressTemplate>
				<div class="">
					<img src="../images/progress_indicator.gif" height="30px" alt="arbetar" style="float:left; padding-right: 5px;" />
					Vänta lite ... 
				</div>
			</ProgressTemplate>
		</asp:UpdateProgress>
		<hr />
		<p>
			När du trycker på knappen kommer procentsatser räknas ut för varje lag beroende på vad användarna (godkända) tippat. <br />
			<asp:Button ID="btnStats" runat="server" Text="Generera statistik" OnClick="GenerateStats" CausesValidation="false" /> 
			<asp:Label ID="lblStats" runat="server" CssClass="error" />
		</p>
		<hr />
		<p>
			Välj bakgrundsfärg från listan:
			<asp:DropDownList ID="ddlCssStyle" runat="server" CausesValidation="false" 
				OnSelectedIndexChanged="ChangeCSS" AutoPostBack="true" Width="250px">
				<asp:ListItem Text="Blå" Value="Blue" />
				<asp:ListItem Text="Blek orange" Value="FadingOrange" />
				<asp:ListItem Text="UEFA Euro Cup 2012 - Lila" Value="EM2012_Purple" />
				<asp:ListItem Text="UEFA Euro Cup 2012 - Grön" Value="EM2012_Green" />
				<asp:ListItem Text="Grön" Value="Green" />
				<asp:ListItem Text="Blek blå" Value="PaleBlue" />
				<asp:ListItem Text="Silver" Value="Silver" />
				<asp:ListItem Text="VM 2014 - Gul/Grön" Value="VM2014" />
				<asp:ListItem Text="EM 2016" Value="EM2016" />
			</asp:DropDownList>
		</p>
	</ContentTemplate>
	</asp:UpdatePanel>
	</ContentTemplate>
	</cc1:TabPanel>
	<cc1:TabPanel ID="tabComments" runat="server" HeaderText="Blog">
		<ContentTemplate>
		<div class="wrap">
		<asp:UpdatePanel ID="updBlog" runat="server">
		<ContentTemplate>
			<div class="leftside">
			<asp:UpdateProgress ID="UpdateProgress4" runat="server" DisplayAfter="0">
				<ProgressTemplate>
					<div class="progress" >
						<img src="../images/progress_indicator.gif" height="30px" alt="arbetar" style="float:left; padding-right: 5px;" />
						Vänta lite... 
					</div>
				</ProgressTemplate>
			</asp:UpdateProgress>
			
			<p>
			<asp:HiddenField ID="hdnBlogEntryID" runat="server" />
			<asp:Label ID="lblError4" runat="server" CssClass="error" />
			Titel: <asp:TextBox ID="txtBlogTitle" runat="server" />
			<asp:RequiredFieldValidator ID="reqTitle" runat="server" ControlToValidate="txtBlogTitle" ErrorMessage="*" ValidationGroup="blog" />
			<br />
			<asp:TextBox ID="txtBlogText" runat="server" TextMode="MultiLine" Width="400px" Height="300px" />
			<asp:RequiredFieldValidator ID="reqText" runat="server" ControlToValidate="txtBlogText" ErrorMessage="*" ValidationGroup="blog" />
			<br />
			<asp:Button ID="btnSendComment" runat="server" Text="Lägg till" OnClick="SaveBlogEntry" ValidationGroup="blog" />
			</p>
			</div>
			
			<div class="rightside">
			<p>
				Tidigare inlägg:
				<asp:GridView ID="grdBlogEntries" runat="server" AutoGenerateColumns="False" DataKeyNames="ID"
					OnRowDataBound="grdBlogEntries_RowDataBound" OnSelectedIndexChanging="EditBlogEntry" 
					OnRowDeleting="DeleteBlogEntry" ShowHeader="false" >
				<Columns>
					<asp:TemplateField> 
						<ItemTemplate>
							<%# Eval("PostedDate") %>                            
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<ItemTemplate>
							<%# Eval("Title") %>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:CommandField ShowSelectButton="true" SelectImageUrl="~/images/icon-edit.png" ButtonType="Image" CausesValidation="false" />
					<asp:CommandField ShowDeleteButton="true" DeleteImageUrl="~/images/icon_delete.gif" ButtonType="Image" CausesValidation="false" /> 
				</Columns>
			</asp:GridView>
			</p>
			</div>
			
			<div style="clear:both;"></div>
		</ContentTemplate>
		</asp:UpdatePanel>     
		</div>       
		</ContentTemplate>
	</cc1:TabPanel>
	<cc1:TabPanel ID="tabTopScorers" runat="server" HeaderText="Potetiella toppskyttar">
		<ContentTemplate>
		<asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
		<ContentTemplate>
		<div class="wrap">
			<div class="leftside">
			<asp:UpdateProgress ID="UpdateProgress5" runat="server" DisplayAfter="0">
				<ProgressTemplate>
					<div class="progress" >
						<img src="../images/progress_indicator.gif" height="30px" alt="arbetar" style="float:left; padding-right: 5px;" />
						Vänta lite... 
					</div>
				</ProgressTemplate>
			</asp:UpdateProgress>
			
			<p>
				Importera många: <br />
				<asp:TextBox ID="txtTopScorerBatch" runat="server" /> 
				<asp:Button ID="btnBatchImport" runat="server" Text="Importera" OnClick="btnBatchImport_OnClick" /> <br />
				<label style="font-size: 7pt">Separera spelarnamnen med | (ex: Zlatan Ibrahimovic|Rasmus Elm|Tobias Hysen)</label>
			</p>

			<p> 
			<asp:HiddenField ID="hdnTopScorerID" runat="server" />
			<asp:Label ID="lblError5" runat="server" CssClass="error" />
			Förnamn: <asp:TextBox ID="txtFirstName" runat="server" />
			<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtFirstName" ErrorMessage="*" ValidationGroup="topscorer" />
			<br />
			Efternamn: <asp:TextBox ID="txtLastName" runat="server" />
			<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtLastName" ErrorMessage="*" ValidationGroup="topscorer" Enabled="false" />
			<br />
			<asp:Button ID="btnSaveTopScorer" runat="server" Text="Lägg till" OnClick="SaveTopScorer" ValidationGroup="topscorer" />
			</p>
			</div>
			
			<div class="rightside">
			<p>
				Potentiella skyttekungar:
				<asp:GridView ID="grdTopScorer" runat="server" AutoGenerateColumns="False" DataKeyNames="ID"
					OnRowDataBound="grdTopScorer_RowDataBound" OnSelectedIndexChanging="EditTopScorer" 
					OnRowDeleting="DeleteTopScorer" ShowHeader="false">
				<Columns>
					<asp:TemplateField> 
						<ItemTemplate>
							<%# Eval("DisplayName") %>              
						</ItemTemplate>
					</asp:TemplateField>
					<asp:CommandField ShowSelectButton="true" SelectImageUrl="~/images/icon-edit.png" ButtonType="Image" />                    
					<asp:CommandField ShowDeleteButton="true" DeleteImageUrl="~/images/icon_delete.gif" ButtonType="Image" CausesValidation="false"  ValidationGroup="topscorer"/>                    
				</Columns>
			</asp:GridView>
			</p>
			</div>
			<div style="clear:both;"></div>
		</div>    
		</ContentTemplate>
		</asp:UpdatePanel>        
		</ContentTemplate>
	</cc1:TabPanel>
	<cc1:TabPanel ID="tabTeams" runat="server" HeaderText="Lag">
		<ContentTemplate>
		<asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="true">
		<ContentTemplate>
		<div class="wrap">
			<div class="leftside">
			<asp:UpdateProgress ID="UpdateProgress6" runat="server" DisplayAfter="0">
				<ProgressTemplate>
					<div class="progress" >
						<img src="../images/progress_indicator.gif" height="30px" alt="arbetar" style="float:left; padding-right: 5px;" />
						Vänta lite... 
					</div>
				</ProgressTemplate>
			</asp:UpdateProgress>
			
			<p>
				<asp:HiddenField ID="hdnTeamID" runat="server" />
				<asp:Label ID="lblErrorTeam" runat="server" CssClass="error" />
				Lag: <asp:TextBox ID="txtTeam" runat="server" /><br />
				Grupp: <asp:TextBox ID="txtTeamGroup" runat="server" /><br />
				<asp:Button ID="btnSaveTeam" runat="server" Text="Lägg till" OnClick="SaveTeam" />
			</p>
			
			</div>
			
			<div class="rightside">
			<p>
				Registrerade lag:
				<asp:GridView ID="grdTeams" runat="server" AutoGenerateColumns="False" DataKeyNames="ID"
					OnRowDataBound="grdTeams_RowDataBound" OnSelectedIndexChanging="EditTeam" 
					OnRowDeleting="DeleteTeam" ShowHeader="false">
				<Columns>
					<asp:TemplateField> 
						<ItemTemplate>
							<%# Eval("TeamName") %> - Grupp <%# Eval("GroupID") %>            
						</ItemTemplate>
					</asp:TemplateField>
					<asp:CommandField ShowSelectButton="true" SelectImageUrl="~/images/icon-edit.png" ButtonType="Image" />                    
					<asp:CommandField ShowDeleteButton="true" DeleteImageUrl="~/images/icon_delete.gif" ButtonType="Image" CausesValidation="false" />                    
				</Columns>
			</asp:GridView>
			</p>
			</div>
			<div style="clear:both;"></div>
		</div>    
		</ContentTemplate>
		</asp:UpdatePanel>        
		</ContentTemplate>
	</cc1:TabPanel>
	<cc1:TabPanel ID="tabMatches" runat="server" HeaderText="Matcher">
		<ContentTemplate>
		<asp:UpdatePanel ID="UpdatePanel3" runat="server" ChildrenAsTriggers="true">
		<ContentTemplate>
		<div class="wrap">
			<div class="leftside">
			<asp:UpdateProgress ID="UpdateProgress7" runat="server" DisplayAfter="0">
				<ProgressTemplate>
					<div class="progress" >
						<img src="../images/progress_indicator.gif" height="30px" alt="arbetar" style="float:left; padding-right: 5px;" />
						Vänta lite... 
					</div>
				</ProgressTemplate>
			</asp:UpdateProgress>
			
			<p>
				<asp:HiddenField ID="hdnMatchID" runat="server" />
				<asp:Label ID="lblErrorMatch" runat="server" CssClass="error" />
				Hemma lag: <asp:DropDownList ID="ddlMatchHomeTeam" runat="server" DataTextField="TeamName" DataValueField="ID"></asp:DropDownList><br />
				Borta lag: <asp:DropDownList ID="ddlMatchAwayTeam" runat="server" DataTextField="TeamName" DataValueField="ID"></asp:DropDownList><br />
				Datum och tid: <asp:TextBox ID="txtMatchDate" runat="server" />
				<cc1:CalendarExtender ID="dateMatch" runat="server" Format="yyyy/MM/dd HH:mm" TargetControlID="txtMatchDate"></cc1:CalendarExtender>
				<asp:Button ID="btnSaveMatch" runat="server" Text="Lägg till" OnClick="SaveMatch" />
			</p>
			
			</div>
			
			<div class="rightside">
			<p>
				Registrerade matcher:
				<asp:GridView ID="grdMatches" runat="server" AutoGenerateColumns="False" DataKeyNames="ID"
					OnRowDataBound="grdMatches_RowDataBound" OnSelectedIndexChanging="EditMatch" 
					OnRowDeleting="DeleteMatch" ShowHeader="false">
				<Columns>
					<asp:TemplateField> 
						<ItemTemplate>
							<%# String.Format("{0:yyyy/MM/dd HH:mm}", Eval("Date")) %> : <%# Eval("HomeTeam.TeamName") %> - <%# Eval("AwayTeam.TeamName") %>            
						</ItemTemplate>
					</asp:TemplateField>
					<asp:CommandField ShowSelectButton="true" SelectImageUrl="~/images/icon-edit.png" ButtonType="Image" />                    
					<asp:CommandField ShowDeleteButton="true" DeleteImageUrl="~/images/icon_delete.gif" ButtonType="Image" CausesValidation="false" />                    
				</Columns>
			</asp:GridView>
			</p>
			</div>
			<div style="clear:both;"></div>
		</div>    
		</ContentTemplate>
		</asp:UpdatePanel>        
		</ContentTemplate>
	</cc1:TabPanel>
</cc1:TabContainer>
	

</asp:Content>