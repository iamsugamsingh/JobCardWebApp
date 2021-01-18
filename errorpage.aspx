<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="errorpage.aspx.cs" Inherits="JobCardApplication.errorpage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="background:#ffc357">
    <div style="width:50%; margin:auto; background:#fff; height:300px; border-radius:5px; margin-top:100px; box-shadow:10px 10px 10px gray;">
        <form id="form1" runat="server" style="padding:25px;text-align:center;">
        <h2 style="color:Red;">Error Found...!</h2>
        <hr style="height:2px; background:red; border:none; width:50%"/>
            <div style="padding-top:25px;padding-bottom:25px;">
                <asp:Label ID="errorMsg" runat="server"></asp:Label>
            </div>
            <hr style="height:2px; background:red; border:none;"/>
        </form>
    </div>    
</body>
</html>
