<%@ Page Title="" Language="C#" MasterPageFile="~/AutoWeb/WebPages/SiteMain.Master" AutoEventWireup="true" CodeBehind="ViewLog.aspx.cs" Inherits="PortalServerWeb.AutoWeb.WebPages.ViewLog" %>
<%@ Register assembly="System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" namespace="System.Web.UI.WebControls" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Logs</h2>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" onpageindexchanging="GridView1_PageIndexChanging" 
        PageSize="20">
        <Columns>
            <asp:BoundField DataField="LogTime" HeaderText="LogTime" 
                SortExpression="LogTime" />
            <asp:BoundField DataField="LogType" HeaderText="LogType" 
                SortExpression="LogType" />
            <asp:BoundField DataField="Message" HeaderText="Message" 
                SortExpression="Message" />
        </Columns>
    </asp:GridView>
</asp:Content>
