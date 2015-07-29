<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="updateapp.aspx.cs" Inherits="Esquel.GEK.FI.PDA.updateapp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        更新程序类型：<asp:DropDownList ID="dlUpdateType" runat="server">
            <asp:ListItem Value="0">成检组板PDA</asp:ListItem>
            <asp:ListItem Value="1">坯布挑修PDA</asp:ListItem>
        </asp:DropDownList>&nbsp;&nbsp;
        <asp:Label ID="Label1" runat="server" Font-Names="Arial" Font-Size="Medium" Text="上传文件："></asp:Label>
&nbsp;<asp:FileUpload ID="FileUpload1" runat="server" Width="440px" />
        <asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click" Text="Upload" />
        <br />
        <br />
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
        <br />
        <br />
        <asp:Label ID="lblUploadDetail" runat="server" Font-Names="Arial" Font-Size="Small"></asp:Label>
    
    </div>
    </form>
</body>
</html>
