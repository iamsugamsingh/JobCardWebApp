using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using Excel = Microsoft.Office.Interop.Excel;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing; //merging pdf 
using Spire.Barcode;
using System.Drawing.Imaging;
using System.Net;

namespace JobCardApplication
{
    public partial class index : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        OleDbConnection con;
        DataTable dt = new DataTable();
        String UID, marking, data;
        String pdfGenerated, finalFile, finalduplicateFile;
        public String filepath;
        protected void Page_Load(object sender, EventArgs e)
        {
            fromTextBox.Focus();
            con = new OleDbConnection(connectionString);
            if (Session["pdfGenerated"] != null)
            {
                pdfGenerated = Session["pdfGenerated"].ToString();
            }
            if (pdfGenerated == "successfull")
            {
                finalFile = Server.MapPath(@"~/final pdf/CombinePdfJD.pdf");
                finalduplicateFile = Server.MapPath(@"~/final pdf/DuplicatePdf.pdf");

                if (File.Exists(finalFile)||File.Exists(finalduplicateFile))
                {
                    filepath = finalFile;
                    msgLabel.Text = "Pdf generated succesfully...!";
                    msgLabel.Visible = true;
                    LinkButton1.Visible = true;
                }
                else
                {
                    filepath = finalduplicateFile;
                    msgLabel.Text = "Pdf not generated...!";
                    msgLabel.Visible = true;
                    LinkButton1.Visible = false;
                }
            }
            if (!Page.IsPostBack)
            {                
                SetInitialRow();
                uidRadioButton.Checked = true;

                if (Session["GridViewMaintain"] != null)
                {
                    DataTable GridViewMaintainDataTable = (DataTable)Session["GridViewMaintain"];

                    dataGridView1.DataSource = GridViewMaintainDataTable;
                    dataGridView1.DataBind();

                    for (int i = 0; i < GridViewMaintainDataTable.Rows.Count; i++)
                    {
                        System.Web.UI.WebControls.CheckBox chkoc = (System.Web.UI.WebControls.CheckBox)dataGridView1.Rows[i].FindControl("CheckBox1");
                        if (GridViewMaintainDataTable.Rows[i].ItemArray[0].ToString() == dataGridView1.Rows[i].Cells[0].Text)
                        {
                            if (GridViewMaintainDataTable.Rows[i].ItemArray[8].ToString() == "check")
                            {
                                chkoc.Style["checked"]="true";
                            }
                        }

                    }

                    refreshdata();

                    if (Session["GridViewMaintain"] != null)
                    {
                        Session.Remove("GridViewMaintain");
                    }
                }
            }
        }
        private void SetInitialRow()
        {
            DataTable dt = new DataTable();
            DataRow dr = null;
            dt.Columns.Add(new DataColumn("UID", typeof(string)));

            dr = dt.NewRow();
            dr["UID"] = string.Empty;

            dt.Rows.Add(dr);
            ViewState["CurrentTable"] = dt;
            dataGridView2.DataSource = dt;
            dataGridView2.DataBind();
        }
        private void AddNewRowToGrid()
        {
            int rowIndex = 0;
            if (ViewState["CurrentTable"] != null)
            {
                DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];
                DataRow drCurrentRow = null;

                if (dtCurrentTable.Rows.Count > 0)
                {
                    for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)
                    {
                        System.Web.UI.WebControls.TextBox box1 = (System.Web.UI.WebControls.TextBox)dataGridView2.Rows[rowIndex].Cells[0].FindControl("TextBox1");
                        drCurrentRow = dtCurrentTable.NewRow();
                        dtCurrentTable.Rows[i - 1]["UID"] = box1.Text;
                        rowIndex++;
                    }
                    dtCurrentTable.Rows.Add(drCurrentRow);
                    ViewState["CurrentTable"] = dtCurrentTable;                    
                    dataGridView2.DataSource = dtCurrentTable;
                    dataGridView2.DataBind();
                }
            }
            else
            {
                Response.Write("ViewState is null");
            }
            SetPreviousData();
        }

        private void SetPreviousData()
        {
            int rowIndex = 0;
            if (ViewState["CurrentTable"] != null)
            {
                DataTable dt = (DataTable)ViewState["CurrentTable"];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        System.Web.UI.WebControls.TextBox box1 = (System.Web.UI.WebControls.TextBox)dataGridView2.Rows[rowIndex].Cells[0].FindControl("TextBox1");
                        box1.Text = dt.Rows[i]["UID"].ToString();
                        rowIndex++;
                    }
                }
            }
        }

        protected void ButtonAdd_Click(object sender, EventArgs e)
        {
            AddNewRowToGrid();
            GridViewRow row = dataGridView2.Rows[dataGridView2.Rows.Count - 1];
            System.Web.UI.WebControls.TextBox uIdTextBox = (System.Web.UI.WebControls.TextBox)row.FindControl("TextBox1");
            uIdTextBox.Focus();
        }
        
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = dataGridView2.Rows[rowIndex];
                string name = (row.FindControl("TextBox1") as System.Web.UI.WebControls.TextBox).Text;
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Name: " + name + "');", true);
            }
        }
        
        protected void uidRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (uidRadioButton.Checked == true)
            {
                System.Web.UI.WebControls.TextBox uidTextBox = (System.Web.UI.WebControls.TextBox)dataGridView2.Rows[0].FindControl("TextBox1");
                //uidTextBox.Focus();
                fromTextBox.Text = string.Empty;
                toTextBox.Text = string.Empty;
                insertBtn.Visible = false;
                dataGridView2.Visible = false;
                dataGridView1.Visible = false;
            }
        }

        protected void orderNumberRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (orderNumberRadioButton.Checked == true)
            {
                fromTextBox.Focus();
                //insertBtn.Visible = false;
                fromTextBox.ReadOnly = false;
                toTextBox.ReadOnly = false;
                enterBtn.Visible=true;
                //dataGridView2.Visible = false;
                dataGridView1.Visible = false;
                if (Session["pdfGenerated"] != null)
                {
                    Session.Remove("pdfGenerated");
                }
                msgLabel.Visible = false;
                LinkButton1.Visible = false;
            }
        }



        //-----------------    Button2   Starts--------------------------------------------



        protected void insertBtn_Click(object sender, EventArgs e)
        {
            if (Session["pdfGenerated"] != null)
            {
                Session.Remove("pdfGenerated");
            }
            msgLabel.Visible = false;
            LinkButton1.Visible = false;

            DataTable dt = new DataTable();
            int counter = 0;
            string query="";
            if (uidRadioButton.Checked == true)
            {
                foreach (GridViewRow row in dataGridView2.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        System.Web.UI.WebControls.TextBox uidTextBox = (System.Web.UI.WebControls.TextBox)row.FindControl("TextBox1");
                        if (uidTextBox.Text != string.Empty)
                        {
                            try
                            {
                                dataGridView1.Visible = true;
                                if (uidTextBox.Text.Contains("-"))
                                {
                                    string[] uids = uidTextBox.Text.Split('-');
                                    query = "SELECT NumOrd, ArtOrd, EntOrd, PinOrd, LanOrd, Datos, MarPie, PlaOrd FROM  [Ordenes de fabricación] WHERE NumOrd >= " + uids[0] + " AND NumOrd <=" + uids[1] + "";
                                }
                                else
                                {
                                    query = "SELECT NumOrd, ArtOrd, EntOrd, PinOrd, LanOrd, Datos, MarPie, PlaOrd FROM  [Ordenes de fabricación] WHERE NumOrd = " + uidTextBox.Text + "";                                     
                                }
                                OleDbConnection conn = new OleDbConnection(connectionString);
                                OleDbCommand cmd = new OleDbCommand(query, conn);
                                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                                adapter.Fill(dt);

                                foreach (DataRow dr in dt.Rows)
                                {
                                    int count = 0;
                                    foreach (DataRow drnew in dt.Rows)
                                    {
                                        if (Convert.ToInt32(dr["NumOrd"]) == Convert.ToInt32(drnew["NumOrd"]))
                                        {
                                            count++;
                                            if (count >= 2)
                                            {
                                                Response.Write("<script>alert('Duplicate UID : " + dr["NumOrd"].ToString() + "');</script>");
                                            }
                                        }
                                    }
                                }
                                dataGridView1.DataSource = dt;
                                dataGridView1.DataBind();
                            }
                            catch (Exception ex)
                            {
                                string error = "Exception Throws From Insert Button Click Event:- " + ex.Message;
                                Response.Redirect("~/errorpage.aspx?error=" + error);
                            }
                        }
                        else
                        {
                            if (counter == 0)
                            {
                                Response.Write("<script>alert('Please enter uid numbers...!');</script>");
                                uidTextBox.BackColor = Color.Red;
                                uidTextBox.Focus();
                            }
                        }
                    }
                    counter++;
                }

                refreshdata();
            }
            else if(orderNumberRadioButton.Checked==true)
            {
                foreach (GridViewRow row in dataGridView2.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        System.Web.UI.WebControls.TextBox uidTextBox = (System.Web.UI.WebControls.TextBox)row.FindControl("TextBox1");
                        if (uidTextBox.Text != string.Empty)
                        {
                            try
                            {
                                dataGridView1.Visible = true;
                                if (uidTextBox.Text.Contains("-"))
                                {
                                    string[] orderNum = uidTextBox.Text.Split('-');
                                    query = "SELECT NumOrd, ArtOrd, EntOrd, PinOrd, LanOrd, Datos, MarPie, PlaOrd FROM  [Ordenes de fabricación] WHERE PinOrd >= " + orderNum[0] + " AND PinOrd <=" + orderNum[1] + "";
                                    
                                    //SELECT NumOrd, ArtOrd, EntOrd, PinOrd, LanOrd, Datos, MarPie, PlaOrd FROM  [Ordenes de fabricación] WHERE PinOrd >= " + fromTextBox.Text + " AND PinOrd <=" + toTextBox.Text + ";
                                }
                                else
                                {
                                    query = "SELECT NumOrd, ArtOrd, EntOrd, PinOrd, LanOrd, Datos, MarPie, PlaOrd FROM  [Ordenes de fabricación] WHERE PinOrd = " + uidTextBox.Text + "";
                                }
                                OleDbConnection conn = new OleDbConnection(connectionString);
                                OleDbCommand cmd = new OleDbCommand(query, conn);
                                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                                adapter.Fill(dt);

                                //foreach (DataRow dr in dt.Rows)
                                //{
                                //    int count = 0;
                                //    foreach (DataRow drnew in dt.Rows)
                                //    {
                                //        if (Convert.ToInt32(dr["PinOrd"]) == Convert.ToInt32(drnew["PinOrd"]))
                                //        {
                                //            count++;
                                //            if (count >= 2)
                                //            {
                                //                Response.Write("<script>alert('Duplicate OrderNum : " + dr["PinOrd"].ToString() + "');</script>");
                                //            }
                                //        }
                                //    }
                                //}
                                dataGridView1.DataSource = dt;
                                dataGridView1.DataBind();
                            }
                            catch (Exception ex)
                            {
                                string error = "Exception Throws From Insert Button Click Event:- " + ex.Message;
                                Response.Redirect("~/errorpage.aspx?error=" + error);
                            }

                            refreshdata();
                        }
                        else
                        {
                            if (counter == 0)
                            {
                                Response.Write("<script>alert('Please enter order numbers...!');</script>");
                                uidTextBox.BackColor = Color.Red;
                                uidTextBox.Focus();
                            }
                        }
                    }
                    counter++;
                }   
            }
            else
            {
                Response.Write("<script>alert('Please, Choose Print Preference First...!');</script>");
                Panel1.BackColor = Color.Red;
                Panel1.ForeColor = Color.White;
            }
        }


        //-----------------    Button2  Ends--------------------------------------------






        //-----------------    Button1   Starts--------------------------------------------


        protected void enterBtn_Click(object sender, EventArgs e)
        {
            if (fromTextBox.Text == "")
            {
                Response.Write("<script>alert('Please enter the value in `From` TextBox...!');</script>");
                fromTextBox.BackColor = Color.Red;
                fromTextBox.Focus();
            }
            else if (toTextBox.Text == "")
            {
                Response.Write("<script>alert('Please enter the value in `To` TextBox...!');</script>");
                toTextBox.BackColor = Color.Red;
                toTextBox.Focus();
            }
            else
            {
                if (Session["pdfGenerated"] != null)
                {
                    Session.Remove("pdfGenerated");
                }
                msgLabel.Visible = false;
                LinkButton1.Visible = false;

                dataGridView1.Visible = true;
                string query = "";
                int from = Convert.ToInt32(fromTextBox.Text);
                int to = Convert.ToInt32(toTextBox.Text);
                if (from < to)
                {
                    if (orderNumberRadioButton.Checked == true || uidRadioButton.Checked == true)
                    {
                        if (fromTextBox.Text != string.Empty)
                        {
                            if (toTextBox.Text != string.Empty)
                            {
                                if (orderNumberRadioButton.Checked == true)
                                {
                                    query = "SELECT NumOrd, ArtOrd, EntOrd, PinOrd, LanOrd, Datos, MarPie, PlaOrd FROM  [Ordenes de fabricación] WHERE PinOrd >= " + fromTextBox.Text + " AND PinOrd <=" + toTextBox.Text + "";
                                }
                                if (uidRadioButton.Checked == true)
                                {
                                    query = "SELECT NumOrd, ArtOrd, EntOrd, PinOrd, LanOrd, Datos, MarPie, PlaOrd FROM  [Ordenes de fabricación] WHERE NumOrd >= " + fromTextBox.Text + " AND NumOrd <=" + toTextBox.Text + "";
                                }
                                try
                                {
                                    dataGridView1.Visible = true;
                                    DataTable dt = new DataTable();
                                    OleDbConnection conn = new OleDbConnection(connectionString);
                                    OleDbCommand cmd = new OleDbCommand(query, conn);
                                    OleDbDataAdapter adepter = new OleDbDataAdapter(cmd);
                                    adepter.Fill(dt);
                                    if (dt.Rows.Count != 0)
                                    {
                                        alertMsg.Visible = false;
                                        dataGridView1.DataSource = dt;
                                        dataGridView1.DataBind();
                                    }
                                    else
                                    {
                                        dataGridView1.Visible = false;
                                        alertMsg.Visible = true;
                                        alertMsg.Text = "Data Not Found...!";
                                    }
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        if (dr["MarPie"] == DBNull.Value)
                                        {
                                            Response.Write("<script>alert('" + dr["NumOrd"].ToString() + " have no Marking...!');</script>");
                                            //alert.Show(dr["UID"].ToString() + " have no Marking ....!");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string error = "Exception Throws From Enter Button Click Event:- " + ex.Message;
                                    Response.Redirect("~/errorpage.aspx?error=" + error);
                                }

                                refreshdata();
                            }
                            else
                            {
                                Response.Write("<script>alert('Please, fill out the order number in to field...!');</script>");
                                toTextBox.BackColor = Color.Red;
                                toTextBox.Focus();
                            }
                        }
                        else
                        {
                            Response.Write("<script>alert('Please, fill out the order number in from field...!');</script>");
                            fromTextBox.BackColor = Color.Red;
                            fromTextBox.Focus();
                        }
                    }
                    else
                    {
                        Response.Write("<script>alert('Please, Choose Print Preference First...!');</script>");
                        Panel1.BackColor = Color.Red;
                        Panel1.ForeColor = Color.White;
                    }
                }
                else
                {
                    Response.Write("<script>alert('`To` textbox value must be greater than `from` textbox!...');</script>");
                    toTextBox.BackColor = Color.Red;
                    toTextBox.Focus();
                }
            }
        }



        //-----------------    Button1   Ends--------------------------------------------


        protected void duplicatejobCardCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (duplicatejobCardCheckBox.Checked == true)
            {
                repeatQuantityTextBox.Visible = true;
                repeatQuantityLabel.Visible = true;
                repeatQuantityTextBox.Focus();
            }
            else
            {
                repeatQuantityTextBox.Visible = false;
                repeatQuantityLabel.Visible = false;
            }
        }



        //-----------------    Button3   Starts--------------------------------------------



        protected void printBtn_Click(object sender, EventArgs e)
        {
            msgLabel.Visible = false;
            LinkButton1.Visible = false;

            string checkboxStatus = "";
            DataTable uidtable = new DataTable();
            uidtable.Columns.Add("uid");
            uidtable.Columns.Add("marking");
            uidtable.Columns.Add("orderNum");
            uidtable.Columns.Add("DrawingNum");


            DataTable Duplicateuidtable = new DataTable();
            Duplicateuidtable.Columns.Add("uid");
            Duplicateuidtable.Columns.Add("marking");
            Duplicateuidtable.Columns.Add("DrawingNum");

            DataTable GridViewMaintainState = new DataTable();
            GridViewMaintainState.Columns.Add("NumOrd");
            GridViewMaintainState.Columns.Add("ArtOrd");
            GridViewMaintainState.Columns.Add("EntOrd");
            GridViewMaintainState.Columns.Add("PinOrd");
            GridViewMaintainState.Columns.Add("LanOrd");
            GridViewMaintainState.Columns.Add("Datos");
            GridViewMaintainState.Columns.Add("MarPie");
            GridViewMaintainState.Columns.Add("PlaOrd");
            GridViewMaintainState.Columns.Add("CheckBox");

            foreach (GridViewRow grow in dataGridView1.Rows)
            {
                System.Web.UI.WebControls.CheckBox chkoc = (System.Web.UI.WebControls.CheckBox)grow.FindControl("CheckBox1");
                if (chkoc.Checked)
                {
                    checkboxStatus = "";
                    msgLabel.Visible = false;
            LinkButton1.Visible = false;
                    break;
                }
                else 
                {
                    grow.Cells[8].BackColor = System.Drawing.Color.Red;
                    checkboxStatus = "unchecked";
                }
            }
            if (checkboxStatus == "unchecked")
            {
                Response.Write("<script>alert('You have not selected any checkbox from Table...!');</script>");
            }
            else if (printJobCardCheckBox.Checked==false && printDrawingCheckBox.Checked==false && duplicatejobCardCheckBox.Checked==false)
            {
                Panel2.BackColor = Color.Red;
                Response.Write("<script>alert('You have not selected any checkbox from Print Preference...!');</script>");
            }
            else 
            {
               if (printJobCardCheckBox.Checked == true && printDrawingCheckBox.Checked==false)
               {
                foreach (GridViewRow grow in dataGridView1.Rows)
                {
                    System.Web.UI.WebControls.CheckBox chkoc = (System.Web.UI.WebControls.CheckBox)grow.FindControl("CheckBox1");
                    if (chkoc.Checked)
                    {
                        uidtable.Rows.Add(grow.Cells[0].Text, grow.Cells[6].Text, grow.Cells[3],grow.Cells[1].Text);
                        GridViewMaintainState.Rows.Add(grow.Cells[0].Text, grow.Cells[1].Text, grow.Cells[2].Text, grow.Cells[3].Text, grow.Cells[4].Text, grow.Cells[5].Text, grow.Cells[6].Text, grow.Cells[7].Text, "check");
                    }
                    else
                    {
                        GridViewMaintainState.Rows.Add(grow.Cells[0].Text, grow.Cells[1].Text, grow.Cells[2].Text, grow.Cells[3].Text, grow.Cells[4].Text, grow.Cells[5].Text, grow.Cells[6].Text, grow.Cells[7].Text, "uncheck");
                    }
                }
                Session["GridViewMaintain"] = GridViewMaintainState;
                Session["checkedJobCardCheckBox"] = "true";
                Session["uidtable"] = uidtable;
                Response.Redirect("/print.aspx"); 
               }

                if (printDrawingCheckBox.Checked == true && printJobCardCheckBox.Checked == false && duplicatejobCardCheckBox.Checked==false)
                {
                    foreach (GridViewRow grow in dataGridView1.Rows)
                    {
                        System.Web.UI.WebControls.CheckBox chkoc = (System.Web.UI.WebControls.CheckBox)grow.FindControl("CheckBox1");
                        if (chkoc.Checked)
                        {
                            uidtable.Rows.Add(grow.Cells[0].Text, grow.Cells[6].Text, grow.Cells[3], grow.Cells[1].Text);
                            GridViewMaintainState.Rows.Add(grow.Cells[0].Text, grow.Cells[1].Text, grow.Cells[2].Text, grow.Cells[3].Text, grow.Cells[4].Text, grow.Cells[5].Text, grow.Cells[6].Text, grow.Cells[7].Text, "check");
                        }
                        else
                        {
                            GridViewMaintainState.Rows.Add(grow.Cells[0].Text, grow.Cells[1].Text, grow.Cells[2].Text, grow.Cells[3].Text, grow.Cells[4].Text, grow.Cells[5].Text, grow.Cells[6].Text, grow.Cells[7].Text, "uncheck");
                        }
                    }
                    Session["GridViewMaintain"] = GridViewMaintainState;
                    Session["CheckedprintDrawingCheckBox"] = "true";
                    Session["uidtable"] = uidtable;
                    Response.Redirect("/print.aspx"); 
                }

                if (printDrawingCheckBox.Checked == true && printJobCardCheckBox.Checked == true)
                {
                    foreach (GridViewRow grow in dataGridView1.Rows)
                    {
                        System.Web.UI.WebControls.CheckBox chkoc = (System.Web.UI.WebControls.CheckBox)grow.FindControl("CheckBox1");
                        if (chkoc.Checked)
                        {
                            uidtable.Rows.Add(grow.Cells[0].Text, grow.Cells[6].Text, grow.Cells[3], grow.Cells[1].Text);
                            GridViewMaintainState.Rows.Add(grow.Cells[0].Text, grow.Cells[1].Text, grow.Cells[2].Text, grow.Cells[3].Text, grow.Cells[4].Text, grow.Cells[5].Text, grow.Cells[6].Text, grow.Cells[7].Text, "check");
                        }
                        else
                        {
                            GridViewMaintainState.Rows.Add(grow.Cells[0].Text, grow.Cells[1].Text, grow.Cells[2].Text, grow.Cells[3].Text, grow.Cells[4].Text, grow.Cells[5].Text, grow.Cells[6].Text, grow.Cells[7].Text, "uncheck");
                        }
                    }
                    Session["GridViewMaintain"] = GridViewMaintainState;
                    Session["checkedBothCheckBox"] = "true";
                    Session["uidtable"] = uidtable;
                    Response.Redirect("/print.aspx"); 
                }

                if(duplicatejobCardCheckBox.Checked==true && printDrawingCheckBox.Checked==false)
                {
                    foreach (GridViewRow grow in dataGridView1.Rows)
                    {
                        System.Web.UI.WebControls.CheckBox chkoc = (System.Web.UI.WebControls.CheckBox)grow.FindControl("CheckBox1");
                        if (chkoc.Checked)
                        {
                            Duplicateuidtable.Rows.Add(grow.Cells[0].Text, grow.Cells[6].Text, grow.Cells[1].Text);
                            GridViewMaintainState.Rows.Add(grow.Cells[0].Text, grow.Cells[1].Text, grow.Cells[2].Text, grow.Cells[3].Text, grow.Cells[4].Text, grow.Cells[5].Text, grow.Cells[6].Text, grow.Cells[7].Text, "check");
                        }
                        else
                        {
                            GridViewMaintainState.Rows.Add(grow.Cells[0].Text, grow.Cells[1].Text, grow.Cells[2].Text, grow.Cells[3].Text, grow.Cells[4].Text, grow.Cells[5].Text, grow.Cells[6].Text, grow.Cells[7].Text, "uncheck");
                        }
                    }
                    Session["GridViewMaintain"] = GridViewMaintainState;
                    Session["repeatQuantityTextBox"] = repeatQuantityTextBox.Text;
                    Session["Duplicateuidtable"] = Duplicateuidtable;
                    Response.Redirect("/DuplicateJobCardViewer.aspx");
                }

                if(duplicatejobCardCheckBox.Checked==true && printDrawingCheckBox.Checked==true)
                {                
                    foreach (GridViewRow grow in dataGridView1.Rows)
                    {
                        System.Web.UI.WebControls.CheckBox chkoc = (System.Web.UI.WebControls.CheckBox)grow.FindControl("CheckBox1");
                        if (chkoc.Checked)
                        {
                            Duplicateuidtable.Rows.Add(grow.Cells[0].Text, grow.Cells[6].Text, grow.Cells[1].Text);
                            GridViewMaintainState.Rows.Add(grow.Cells[0].Text, grow.Cells[1].Text, grow.Cells[2].Text, grow.Cells[3].Text, grow.Cells[4].Text, grow.Cells[5].Text, grow.Cells[6].Text, grow.Cells[7].Text, "check");
                        }
                        else
                        {
                            GridViewMaintainState.Rows.Add(grow.Cells[0].Text, grow.Cells[1].Text, grow.Cells[2].Text, grow.Cells[3].Text, grow.Cells[4].Text, grow.Cells[5].Text, grow.Cells[6].Text, grow.Cells[7].Text, "uncheck");
                        }
                    }
                    Session["GridViewMaintain"] = GridViewMaintainState;
                    Session["printDrawing"] = "true";                
                    Session["repeatQuantityTextBox"] = repeatQuantityTextBox.Text;
                    Session["Duplicateuidtable"] = Duplicateuidtable;
                    Response.Redirect("/DuplicateJobCardViewer.aspx");
                }
            }
             
            
            
                //if (printDrawingCheckBox.Checked == true)
                //{
                //    Session["printDrawing"] = "true";
                //}

                    //int RepeatQty = Convert.ToInt32(repeatQuantityTextBox.Text);
                    //int ordQty = Convert.ToInt32(dt.Rows[0][3]);
                    //if (RepeatQty < ordQty)
                    //{
                    //    repeatQuantityTextBox.Text = RepeatQty + " OF " + ordQty;
                    //}
                    //else
                    //{
                    //    repeatQuantityTextBox.Text = ordQty.ToString(); ;
                    //}
                //if (Session["pdfGenerated"] != null)
                //{
                //    Session.Remove("pdfGenerated");
                //}
                //msgLabel.Visible = false;
                //LinkButton1.Visible = false;            
        }
 
        //
        //        

        protected void SingleSearch_Click(object sender, EventArgs e)
        {
            singleSearchBtn.Visible = false;
            insertBtn.Visible = true;
            dataGridView2.Visible = true;
            System.Web.UI.WebControls.TextBox uidTextBox = (System.Web.UI.WebControls.TextBox)dataGridView2.Rows[0].FindControl("TextBox1");
            uidTextBox.Focus();
            if (Session["pdfGenerated"] != null)
            {
                Session.Remove("pdfGenerated");
            }
            msgLabel.Visible = false;
            LinkButton1.Visible = false;
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            //Response.Redirect("~/pdfViewer.aspx");
            //Response.Write("<script>window.open ('pdfViewer.aspx','_blank');</script>");


            finalFile = Server.MapPath(@"~/final pdf/CombinePdfJD.pdf");
            finalduplicateFile = Server.MapPath(@"~/final pdf/DuplicatePdf.pdf");

            if (File.Exists(finalFile))
            {
                Response.Write("<script>");
                Response.Write("window.open('/final pdf/CombinePdfJD.pdf', '_newtab');");
                Response.Write("</script>");
            }
            if (File.Exists(finalduplicateFile))
            {
                Response.Write("<script>");
                Response.Write("window.open('/final pdf/DuplicatePdf.pdf', '_newtab');");
                Response.Write("</script>");
            }

            



            //string FilePath = "";
            //if (File.Exists(finalFile))
            //{
            //    FilePath = Server.MapPath("~/final pdf/CombinePdfJD.pdf");
            //}
            //if (File.Exists(finalduplicateFile))
            //{
            //    FilePath = Server.MapPath("~/final pdf/DuplicatePdf.pdf");
            //}

            //WebClient User = new WebClient();
            //Byte[] FileBuffer = User.DownloadData(FilePath);
            //if (FileBuffer != null)
            //{
            //    Response.ContentType = "application/pdf";
            //    Response.AddHeader("content-length", FileBuffer.Length.ToString());
            //    Response.BinaryWrite(FileBuffer);
            //}
            //if (Session["pdfGenerated"] != null)
            //{
            //    Session.Remove("pdfGenerated");
            //}
        }


        //-----------------    Button3 Ends--------------------------------------------
        public void refreshdata()
        {
            //string firstValue = dataGridView1.Rows[0].Cells[0].Text;
            //int lastindex= dataGridView1.Rows.Count-1;
            //string lastvalue = dataGridView1.Rows[lastindex].Cells[0].Text;
            DataTable dt = new DataTable();
            dt.Columns.Add("NumOrd");
            dt.Columns.Add("ArtOrd");
            dt.Columns.Add("EntOrd");
            dt.Columns.Add("PinOrd");
            dt.Columns.Add("LanOrd");
            dt.Columns.Add("Datos");
            dt.Columns.Add("MarPie");
            dt.Columns.Add("PlaOrd");

            OleDbConnection connnn = new OleDbConnection(connectionString);
            try
            {
                foreach (GridViewRow row in dataGridView1.Rows)
                {

                    string uid = row.Cells[0].Text;


                    OleDbCommand cmd = new OleDbCommand("SELECT NumOrd, ArtOrd, EntOrd, PinOrd, LanOrd, Datos, MarPie, PlaOrd FROM  [Ordenes de fabricación] WHERE NumOrd=" + uid, connnn);
                    connnn.Open();
                    OleDbDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows == true)
                    {
                        while (dr.Read())
                        {
                            String numord = dr["NumOrd"].ToString();
                            String artord = dr["ArtOrd"].ToString();

                            String entord = "";
                            if (dr["EntOrd"].ToString() != "")
                            {
                                entord = Convert.ToDateTime(dr["EntOrd"]).ToString("dd-MMM-yyyy");
                            }
                            String pinord = dr["PinOrd"].ToString();

                            String lanord = "";
                            if (dr["LanOrd"].ToString() != "")
                            {
                                lanord = Convert.ToDateTime(dr["LanOrd"]).ToString("dd-MMMM-yyyy");
                            }
                            String datos = dr["Datos"].ToString();
                            String marpie = dr["MarPie"].ToString();
                            String plaord = dr["PlaOrd"].ToString();


                            dt.Rows.Add(numord, artord, entord, pinord, lanord, datos, marpie, plaord);
                        }
                    }
                    connnn.Close();

                }

                dataGridView1.DataSource = dt;
                dataGridView1.DataBind();
                ViewState["dirState"] = dt;
                ViewState["sortdr"] = "Asc";


            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dtrslt = (DataTable)ViewState["dirState"];
            if (dtrslt.Rows.Count > 0)
            {
                if (Convert.ToString(ViewState["sortdr"]) == "Asc")
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sortdr"] = "Desc";
                }
                else
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sortdr"] = "Asc";
                }
                dataGridView1.DataSource = dtrslt;
                dataGridView1.DataBind();
            }
        }
    }
}



















