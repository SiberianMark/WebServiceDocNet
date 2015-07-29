<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IPConfig.aspx.cs" Inherits="Esquel.GEK.FI.PDA.IPConfig" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>打印机IP管理界面</title>

    <!-- Framework CSS -->
    <link rel="stylesheet" href="../Css/screen.css" type="text/css" media="screen, projection" />
    <link rel="stylesheet" href="../Css/print.css" type="text/css" media="print" />
    <!--[if lt IE 8]><link rel="stylesheet" href="../blueprint/ie.css" type="text/css" media="screen, projection"><![endif]-->
    <style type="text/css" media="screen">
      p, table, hr, .box { margin-bottom:25px; }
      .box p { margin-bottom:10px; }
    </style>
</head>
<body>
    <form id="form1" runat="server" action="IPConfig.aspx">
    <div style="width:240px; margin:0 auto;">
    
        <table style="font-family:Arial; font-size:13px; text-align:center;">
            <thead>
                <tr>
                    <th>IPNo</th>
                    <th>IP地址</th>
                </tr>
            </thead>
            <tr>
                <td>
                    <label for="IP1" id="No1" name="No1" runat="server"></label></td>
                <td>
                    <input type="text" class="text" id="IP1" name="IP1" runat="server" /></td>
            </tr>
            <tr>
                <td>
                    <label for="IP2" id="No2" name="No2" runat="server"></label>
                </td>
                <td><input type="text" class="text"  id="IP2" name="IP2" runat="server" /></td>
            </tr>
            <tr>
                <td>
                    <label for="IP3" id="No3" name="No3" runat="server"></label>
                </td>
                <td><input type="text" class="text"  id="IP3" name="IP3" runat="server" /></td>
            </tr>
            <tr>
                <td>
                    <label for="IP4" id="No4" name="No4" runat="server"></label>
                </td>
                <td><input type="text" class="text"  id="IP4" name="IP4" runat="server" /></td>
            </tr>
            <tr>
                <td colspan="2" style="text-align:center;">
                    <input type="submit" class="button" value="Save" />&nbsp;&nbsp;&nbsp;&nbsp;
                    <input type="reset" class="button" value="Cancle" />
                </td>
            </tr>
        </table>
    
    </div>

    <div id="info" runat="server" class="info"  style="width:240px; margin:0 auto;">
        
    </div>
    </form>
</body>
</html>
