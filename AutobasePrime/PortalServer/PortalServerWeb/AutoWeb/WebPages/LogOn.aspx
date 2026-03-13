<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogOn.aspx.cs" Inherits="PortalServerWeb.AutoWeb.WebPages.LogOn" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="~/Content/w3.css" />
    
    <script type="text/javascript" src="../../src/core.js"></script>
    <script type="text/javascript" src="../../src/sha256.js"></script>
    <script type="text/javascript" >
        function HashPassword() {
            var source = document.getElementById("TextBoxPassword").value;

            if (source.substring(0, 7) == "Hashed_") return;    
            
            var hash = CryptoJS.SHA256(source);
            hash = "Hashed_" + hash;
            document.getElementById("TextBoxPassword").value = hash;
        }
    </script>
</head>

<body>
    <form id="Form1" runat="server">
    <h1>관리자 로그인</h1>
    <br />
    <asp:Label ID="Label1" Runat="server">Username:</asp:Label> 
    <br />
    <asp:TextBox ID="TextBoxUsername" Runat="server">
        </asp:TextBox> 
    <br />
    <asp:Label ID="Label2" Runat="server" >Password:</asp:Label> 
    <br />
    <asp:TextBox ID="TextBoxPassword" Runat="server"  
        TextMode="Password" ></asp:TextBox>&nbsp;
    <asp:TextBox ID="TextBoxPasswordHide" Runat="server" Visible="False"></asp:TextBox> 
    <br />
    <br />
    <asp:CustomValidator ID="CustomValidator1" Runat="server" ErrorMessage="CustomValidator"
            OnServerValidate="CustomValidator1_ServerValidate" >
        </asp:CustomValidator> 
    <br />
    <asp:Button ID="CommandOK" Runat="server" OnClick="CommandOK_Click" 
        Font-Size="Large" Text="OK" OnClientClick="HashPassword()" /></form>
</body>
</html>
