<%@ Page Language="C#" AutoEventWireup="true" Inherits="PortalServerWeb.AutoWeb.MobilePages.TagWriteWord" Codebehind="TagWriteWord.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="Form1" runat="server">
        <asp:Label ID="Label2" Runat="server" BackColor="#FFE0C0" Font-Bold="True" Font-Names="Gulim">아날로그 출력</asp:Label>
        <br />
        <asp:Label ID="LabelTag" Runat="server" Font-Names="Gulim">Label</asp:Label>
        <br />
        <asp:Label ID="LabelDescription" Runat="server" Font-Names="Gulim">Label</asp:Label> 
        <br />
        <asp:Label ID="LabelValue" Runat="server" Font-Names="Gulim">Label</asp:Label>
        <br />
        <asp:Label ID="Label1" Runat="server" Font-Names="Gulim">
        </asp:Label>
        <br />
        <asp:TextBox
        ID="TextBoxValue" Runat="server" Font-Names="Gulim" Font-Size="Large">
    </asp:TextBox> 
        <br />
        <asp:Button ID="CommandOK" Runat="server" OnClick="CommandOFF_Click" Font-Names="Gulim" Font-Size="Large" Text="변경"/></form>
</body>
</html>
