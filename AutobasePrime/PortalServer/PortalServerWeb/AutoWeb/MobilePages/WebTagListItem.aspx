<%@ Page Language="C#" AutoEventWireup="true" Inherits="PortalServerWeb.AutoWeb.MobilePages.WebTagListItem" Codebehind="WebTagListItem.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="Form1" runat="server"><asp:Label ID="Label2" Runat="server" BackColor="#FFE0C0"
        Font-Bold="True" Font-Names="Gulim">웹 그룹 태그</asp:Label> 
    <br />
    <asp:Label ID="LabelGroupName"
            Runat="server" Font-Names="Gulim">Label</asp:Label>
    <br />
    <asp:Label ID="LabelDescription"
                Runat="server" Font-Names="Gulim">Label</asp:Label>
    <br />
    <asp:ListBox ID="List1" Runat="server" Font-Names="Gulim" AutoPostBack="True" 
        Font-Size="Large" onselectedindexchanged="List1_SelectedIndexChanged"></asp:ListBox>&nbsp;<br />&nbsp;</form>
</body>
</html>
