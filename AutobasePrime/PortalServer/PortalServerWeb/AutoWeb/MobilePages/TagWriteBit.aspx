<%@ Page Language="C#" AutoEventWireup="true" Inherits="PortalServerWeb.AutoWeb.MobilePages.TagWriteBit" Codebehind="TagWriteBit.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="Form1" runat="server">
        <asp:Label ID="Label2" Runat="server" BackColor="#FFE0C0" Font-Bold="True" Font-Names="Gulim">디지털(비트) 출력</asp:Label>
        <br />
        <asp:Label ID="LabelTag" Runat="server" Font-Names="Gulim">Label</asp:Label>
        <br />
        <asp:Label ID="LabelDescription" Runat="server" Font-Names="Gulim">Label</asp:Label>
        <br />
        <asp:Label ID="LabelValue" Runat="server" Font-Names="Gulim">Label</asp:Label>
        <br />
        <asp:Button ID="CommandON" Runat="server" OnClick="CommandON_Click" Font-Names="Gulim" Font-Size="Large" Text="O N"/>
        <br />
        <asp:Button ID="CommandOFF" Runat="server" OnClick="CommandOFF_Click" Font-Names="Gulim" Font-Size="Large" Text="OFF"/>

    </form>
</body>
</html>
