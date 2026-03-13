<%@ Page Language="C#" AutoEventWireup="true" Inherits="PortalServerWeb.AutoWeb.MobilePages.TagView" Codebehind="TagView.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="Form1" runat="server">
        <asp:Label ID="Label2" Runat="server" BackColor="#FFE0C0" Font-Bold="True" Font-Name="Gulim">태그 상태 보기</asp:Label>
        <br />
        <asp:Label ID="LabelTag" Runat="server" Font-Name="Gulim">Label</asp:Label>
        <br />
        <asp:Label ID="LabelDescription" Runat="server" Font-Name="Gulim">Label</asp:Label> 
        <br />
        <asp:Label ID="LabelValue" Runat="server" Font-Name="Gulim">Label</asp:Label> 
        <br />
        <asp:Label ID="Label1" Runat="server" Font-Name="Gulim"></asp:Label>
        <br />
        <asp:HyperLink ID="LinkWrite" Runat="server" Font-Name="Gulim" Font-Size="Large">출력화면</asp:HyperLink>
        <br />
        <asp:HyperLink ID="LinkList" Runat="server" Font-Name="Gulim" Font-Size="Large">목록</asp:HyperLink>
    </form>
</body>
</html>
