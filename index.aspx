<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="JobCardApplication.index" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Job Card Application</title>
    <link rel="icon" type="image/jpg" href="images/AnutechLogo.jpg">

    <link href="../Styles/bootstrap.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/Style.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="http://ajax.microsoft.com/ajax/jquery/jquery-1.4.2.min.js"></script>
        <script type="text/javascript">
            $(function () {
                $('#TextBox1').focus();
                var $inp = $('.cls');
                $inp.bind('keydown', function (e) {
                    //var key = (e.keyCode ? e.keyCode : e.charCode);
                    var key = e.which;
                    if (key == 13) {
                        e.preventDefault();
                        var nxtIdx = $inp.index(this) + 1;
                        $(".cls:eq(" + nxtIdx + ")").focus();
                        $(".cls:eq(" + nxtIdx + ")").click();
//                        if (nxtIdx % 14 == 0) {
//                            $(".cls:eq(" + nxtIdx + ")").focus();
//                            $(".cls:eq(" + nxtIdx + ")").click();
//                        }
                    }
                    if (key == 38) {
                        e.preventDefault();
                        var nxtIdx = $inp.index(this) - 1;
                        $(".cls:eq(" + nxtIdx + ")").focus();
                    }
                    if (key == 40) {
                        e.preventDefault();
                        var nxtIdx = $inp.index(this) + 1;
                        $(".cls:eq(" + nxtIdx + ")").focus();
                    }
                });
            });
    </script>
    <script type = "text/javascript">

        function Check_Click(objRef) 
        {
            var row = objRef.parentNode.parentNode
            if (objRef.checked) {
                row.style.backgroundColor = "#3390FF";
                row.style.color = "#fff";
            }
            else 
            {
                row.style.backgroundColor = "#FFF";
                row.style.color = "#000";
            }
            var GridView = row.parentNode;
            var inputList = GridView.getElementsByTagName("input");
            for (var i = 0; i < inputList.length; i++) 
            {
                var headerCheckBox = inputList[0];
                var checked = true;
                if (inputList[i].type == "checkbox" && inputList[i] != headerCheckBox) 
                {
                    if (!inputList[i].checked) 
                    {
                        checked = false;
                        break;
                    }
                }
            }
            headerCheckBox.checked = checked;
        }

</script>

<script type = "text/javascript">

    function checkAll(objRef)
    {
        var GridView = objRef.parentNode.parentNode.parentNode;
        var inputList = GridView.getElementsByTagName("input");
        for (var i = 0; i < inputList.length; i++) 
        {
            var row = inputList[i].parentNode.parentNode;
            if (inputList[i].type == "checkbox" && objRef != inputList[i]) 
            {
                if (objRef.checked) 
                {
                    row.style.backgroundColor = "#3390FF";
                    row.style.color = "#fff";
                    inputList[i].checked = true;

                }

                else 
                {
                    row.style.backgroundColor = "#FFF";
                    row.style.color = "#000";
                    inputList[i].checked = false;
                }
            }
        }
    }
</script> 
    
</head>
<body>
    <div class="header">
        <div class="logo">
            <img src="images/AnutechLogo.jpg" alt="ANU WORLES LOGO" />
        </div>
        <center>
            <p class="heading-text">
                Job Card Application
            </p>
        </center>
        <div class="version">
            <p>
                Version: 2.0
            </p>
        </div>
    </div>
    <div class="container-fluid">
        <div >
            <form class="form-inline" runat="server">
                <div class="form-group">
                      <asp:Label ID="Label1" runat="server" class="lbl" Text="From"></asp:Label><br />
                      <asp:TextBox ID="fromTextBox" class="form-control" style="width:100px; margin-left:25px;" runat="server"></asp:TextBox>
                </div>

                <div class="form-group">
                    <asp:Label ID="Label2" runat="server" class="lbl" Text="To"></asp:Label><br />
                    <asp:TextBox ID="toTextBox" class="form-control" style="width:100px;" runat="server"></asp:TextBox>
                </div>
                <asp:Button ID="enterBtn" class="btn btn-primary" runat="server" Text="Enter" 
                  onclick="enterBtn_Click" />

                <div class="checkbox">
                    <asp:Label ID="Label3" class="lbl" runat="server" Text="Print Prefrence"></asp:Label><br />
                   
                        <asp:Panel ID="Panel2" class="checkBoxArea" runat="server">
                        
                        <asp:CheckBox ID="printJobCardCheckBox" class="checkboxfield" runat="server" text="Print Job Card"/>
                        &emsp;
                        <asp:CheckBox ID="printDrawingCheckBox" class="checkboxfield" runat="server" text="Print Drawing"/>
                        &emsp;
                        <asp:CheckBox ID="duplicatejobCardCheckBox" class="checkboxfield" runat="server" text="Duplicate Job Card" oncheckedchanged="duplicatejobCardCheckBox_CheckedChanged" AutoPostBack="true"/>
                    </asp:Panel>
                </div>
                <div class="form-group">
                    <asp:Label ID="repeatQuantityLabel" runat="server" class="lbl" style="margin-left:40px;" Text="Repeat Quantity" visible="false"></asp:Label><br />
                    <asp:TextBox ID="repeatQuantityTextBox" class="form-control" style="margin-left:40px; width:100px;" visible="false" runat="server"></asp:TextBox>
                </div>             
                <asp:Button ID="printBtn" class="btn btn-primary" runat="server" Text="Print" 
                    onclick="printBtn_Click" />   
                <%--<div class="form-group browse">
                    <label for="exampleInputFile">Browse</label><br />
                    <asp:FileUpload ID="FileUpload1" runat="server" class="btn btn-danger" style="margin-top:0px; margin-left:0px; color:white;"/>
              </div>--%>
            <hr />
                <div class="row" style="margin-top:25px;">
                    <div class="col-lg-2">
                       
                        
                        <div class="checkbox">
                            <asp:Label ID="Label4" class="lbl" runat="server" Text="Search via"></asp:Label><br />
                           
                             <asp:Panel ID="Panel1" class="checkBoxArea" runat="server">
                                <asp:RadioButton class="checkboxfield" ID="uidRadioButton" runat="server" 
                                    text="UID" GroupName="printPreference" 
                                    oncheckedchanged="uidRadioButton_CheckedChanged"  AutoPostBack="true"/><br />
                                <asp:RadioButton class="checkboxfield" ID="orderNumberRadioButton" 
                                    runat="server" text="Order Number" GroupName="printPreference" 
                                    oncheckedchanged="orderNumberRadioButton_CheckedChanged" AutoPostBack="true"/> 
                           </asp:Panel>
                        </div>                      
                        <div>
                            <asp:Button ID="singleSearchBtn" runat="server" Text="Search Randomly" 
                                class="btn btn-success" style="margin-left:25px; width:72%;" 
                                onclick="SingleSearch_Click"/>
                            
                            <asp:Button ID="insertBtn" runat="server" Text="Insert >>" 
                                class="btn btn-success" style="margin-left:25px; width:72%;" 
                                onclick="insertBtn_Click" Visible="false"/>
                        </div>
                        <div style="margin-top:25px; margin-left:25px;">
                            <asp:GridView ID="dataGridView2" runat="server" width="82%" AutoGenerateColumns="False" ShowFooter="true" Visible="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="UID / Order Number">
                                        <ItemTemplate>
                                            <asp:TextBox ID="TextBox1" class="cls" runat="server" Width="100%"></asp:TextBox>
                                        </ItemTemplate>
                                        <FooterStyle HorizontalAlign="Right" />
                                            <FooterTemplate>
                                             <asp:Button ID="ButtonAdd" class="cls btn btn-danger btn-xs" runat="server" style="margin:0px;color:White; width:100%;" Text="Add New Row" onClick="ButtonAdd_Click" BorderStyle="Outset" />
                                            </FooterTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle BackColor="#286090" ForeColor="White" />
                        </asp:GridView>
                            <asp:Label ID="label8" runat="server" ></asp:Label>
                            <asp:Label ID="labelPageqty" runat="server" ></asp:Label>
                        </div>
                        <div style="background: red; color:White; width:80%; margin:auto; margin-top:20px; border-radius:5px 5px 5px 5px; font-weight: bold; font-size: 16px; text-align:center; box-shadow:10px 10px 10px gray;">
                            <asp:Label ID="msgLabel" runat="server" Visible="true" Font-Bold="True" 
                                Font-Size="16pt"></asp:Label>
                            <asp:LinkButton ID="LinkButton1" runat="server" Visible="false"
                                onclick="LinkButton1_Click" Font-Bold="True" Font-Size="16pt" 
                                ForeColor="White">click here to see.</asp:LinkButton>
                         </div>
                    </div>
                    <div class="col-lg-10" style="float:right; border:1px solid #cccccc; background:#fff; height:375px; width:80%; float:left; overflow:auto; border-radius:4px; padding:0px;"> 
                        <asp:Label ID="alertMsg" runat="server" Visible="false"></asp:Label>                       
                        <asp:GridView ID="dataGridView1" runat="server" AutoGenerateColumns="False" Width="100%" Font-Size="12px" HorizontalAlign="Center" CssClass="dataViewerArea" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Height="25px" HeaderStyle-BorderColor="Black" HeaderStyle-BorderStyle="Solid" HeaderStyle-BorderWidth="1px" AllowSorting="true" onsorting="GridView1_Sorting">
                            <Columns>                                 
                                <asp:BoundField DataField="NumOrd" HeaderText="UID" ItemStyle-Width="50" SortExpression="NumOrd">
                                </asp:BoundField>
                                <asp:BoundField DataField="ArtOrd" HeaderText="Article" ItemStyle-Width="100" SortExpression="ArtOrd">
                                </asp:BoundField>
                                <asp:BoundField DataField="EntOrd" HeaderText="Delivery Date" 
                                    ItemStyle-Width="150" DataFormatString = "{0:dd-MMM-yyyy}" SortExpression="EntOrd">
                                </asp:BoundField>
                                <asp:BoundField DataField="PinOrd" HeaderText="Order Number" 
                                    ItemStyle-Width="150" SortExpression="PinOrd">
                                </asp:BoundField>
                                <asp:BoundField DataField="LanOrd" HeaderText="Start Date" ItemStyle-Width="150" 
                                    DataFormatString = "{0:dd-MMM-yyyy}" SortExpression="LanOrd">
                                </asp:BoundField>
                                <asp:BoundField DataField="Datos" HeaderText="Datos" ItemStyle-Width="200" SortExpression="Datos">
                                </asp:BoundField>
                                <asp:BoundField DataField="MarPie" HeaderText="Marking" ItemStyle-Width="250" SortExpression="MarPie">
                                </asp:BoundField>
                                <asp:BoundField DataField="PlaOrd" HeaderText="PlaOrd" ItemStyle-Width="250" SortExpression="PlaOrd">
                                </asp:BoundField>
                                <asp:TemplateField ItemStyle-Width="50px">
                                    <HeaderTemplate>
                                      <asp:CheckBox ID="checkAll" runat="server" onclick = "checkAll(this);"/>
                                    </HeaderTemplate>
                                   <ItemTemplate>
                                     <asp:CheckBox ID="CheckBox1" runat="server" onclick = "Check_Click(this)" />
                                   </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle BackColor="#3390ff" ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" Width="100%" />
                        </asp:GridView>
                    </div>
                </div>
                <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                <asp:PlaceHolder ID="PlaceHolder2" runat="server"></asp:PlaceHolder>
            </form>
        </div>
    </div>

    <div class="footer">
        <p style="line-height:50px;">
            Anu Worles &#169; 2020
        </p>
    </div>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <script src="Scripts/bootstrap.js" type="text/javascript"></script>
</body>
</html>
