<%@ Page Language="C#" AutoEventWireup="true" Inherits="PortalServerWeb.AutoWeb.MobilePages.MobileLogOn" Codebehind="MobileLogOn.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="Form1" runat="server"><asp:Label ID="Label3" Runat="server" BackColor="#FFE0C0" Font-Bold="True" Font-Names="Gulim">사용자 로그인</asp:Label> 
    <br />
    <asp:Label ID="Label1" Runat="server" Font-Names="Gulim">사용자명</asp:Label> 
    <br />
    <asp:TextBox ID="TextBoxUsername" Runat="server" Font-Names="Gulim" 
        ontextchanged="TextBoxUsername_TextChanged">
        </asp:TextBox> 
    <br />
    <asp:Label ID="Label2" Runat="server" Font-Names="Gulim">암호</asp:Label> 
    <br />
    <asp:TextBox ID="TextBoxPassword" Runat="server" Font-Names="Gulim" 
        TextMode="Password" ></asp:TextBox>&nbsp;<asp:CustomValidator ID="CustomValidator1" Runat="server" ErrorMessage="CustomValidator"
            OnServerValidate="CustomValidator1_ServerValidate">
        </asp:CustomValidator> 
    <br />
    <asp:Button ID="CommandOK" Runat="server" Font-Names="Gulim" OnClick="CommandOK_Click" Font-Size="Large" Text="확인"/></form>
</body>
</html>
