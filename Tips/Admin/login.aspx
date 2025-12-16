<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Tips.login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head>
<title>Manges VM-tips :: Login</title>
</head>

<body bgcolor="#FFFFFF" text="#000000">
<form id="Form1" runat="server">
<table width="400" border="0" cellspacing="0" cellpadding="0">
  <tr>
    <td width="100">Användarnamn : </td>
    <td width="10"> </td>
    <td><asp:TextBox Id="txtUser" width="150" runat="server"/></td>
  </tr>
  <tr>
    <td>Lösenord : </td>
    <td width="10"> </td>
    <td><asp:TextBox Id="txtPassword" width="150" TextMode="Password" runat="server"/></td>
  </tr>
  <tr>
    <td> </td>
    <td width="10"> </td>
    <td><asp:Button Id="cmdLogin" OnClick="ProcessLogin" Text="Logga in" runat="server" /></td>
  </tr>
</table>
<br>
<br>
<div id="ErrorMessage" runat="server" />
</form>
</body>
</html>
