using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.Data.OleDb;
using System.IO;
using System.Data;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Spire.Barcode;
using System.Drawing;
using System.Net;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Globalization;

namespace JobCardApplication
{
    public partial class print : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        OleDbConnection con;
        String data;
        String CheckedprintDrawingCheckBox, checkedBothCheckBox, checkedJobCardCheckBox, drawingFolder;
        DataTable dataTable = new DataTable();
        DataTable stepsTable = new DataTable();
        DataTable datasetTable = new DataTable();
        ReportDataSource rd, rd1, rd2, rd3,rd4;
        ReportParameter parameter1, parameter2, parameter3, parameter4, parameter5, weekOfYear, orderDate, startDate, deliveryDate, plapie;
        DataTable imagedata = new DataTable();
                                    
        protected void Page_Load(object sender, EventArgs e)
        {
            stepsTable.Columns.Add("NumFas");
            stepsTable.Columns.Add("CodPie");
            stepsTable.Columns.Add("Operac");
            stepsTable.Columns.Add("CodPro");
            stepsTable.Columns.Add("FasExt");
            datasetTable.Columns.Add("NumOrd");
            datasetTable.Columns.Add("ArtOrd");
            datasetTable.Columns.Add("PieOrd");
            datasetTable.Columns.Add("PinOrd");
            datasetTable.Columns.Add("FecPed");
            datasetTable.Columns.Add("NomArt");
            datasetTable.Columns.Add("PlaOrd");
            datasetTable.Columns.Add("Observaciones");
            datasetTable.Columns.Add("PreOrd");
            datasetTable.Columns.Add("EntOrd");
            datasetTable.Columns.Add("Datos");
            datasetTable.Columns.Add("PedPed");
            datasetTable.Columns.Add("LanOrd");
            datasetTable.Columns.Add("MarPie");

            List<String> pdflist = new List<String>();
            con = new OleDbConnection(connectionString);
            if (!IsPostBack)
            {
                String[] dltFiles = new String[6] { Server.MapPath(@"~/GeneratePDF/"), Server.MapPath(@"~/Marking QR/"), Server.MapPath(@"~/UID QR/"), Server.MapPath(@"~/combinePdf/"), Server.MapPath(@"~/final pdf/"), Server.MapPath(@"~/HiltiDataMatrixQR/")};
                try
                {
                    foreach (String files in dltFiles)
                    {
                        var filePaths = Directory.GetFiles(files, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".pdf") || s.EndsWith(".Jpeg"));
                        foreach (string filePath in filePaths)
                        {
                            if (filePath != null)
                            {
                                File.Delete(filePath);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Redirect("errorpage.aspx?error="+ex.Message);
                }

                DataTable dt = (DataTable)Session["uidtable"];
                if (Session["CheckedprintDrawingCheckBox"] != null)
                {
                    CheckedprintDrawingCheckBox = Session["CheckedprintDrawingCheckBox"].ToString();
                }

                if (Session["checkedBothCheckBox"] != null)
                {
                    checkedBothCheckBox = Session["checkedBothCheckBox"].ToString();
                }

                if (Session["checkedJobCardCheckBox"] != null)
                {
                    checkedJobCardCheckBox = Session["checkedJobCardCheckBox"].ToString();
                }              
                
                foreach (DataRow dtrow in dt.Rows)
                {
                    qrCode(dtrow["uid"].ToString());
                    QRCodeMarking(dtrow["marking"].ToString(), dtrow["uid"].ToString());

                    con.Open();
                    OleDbCommand commands = new OleDbCommand("Select NumOrd, LanOrd from [Ordenes de fabricación] Where NumOrd="+dtrow["uid"].ToString(), con);
                    OleDbDataReader readr = commands.ExecuteReader();

                    if (readr.HasRows == true)
                    {
                        while (readr.Read())
                        {
                            UpdateStartDate(readr["NumOrd"].ToString(), readr["LanOrd"].ToString());
                        }
                    }

                    con.Close();

                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/pdf.rdlc"); 
                    ReportViewer1.LocalReport.EnableExternalImages = true;

                    DataSet1 dataset = new DataSet1();
                    con.Open();
                    string query = "SELECT [Ordenes de fabricación].NumOrd,[Ordenes de fabricación].ArtOrd,[Ordenes de fabricación].PieOrd,[Ordenes de fabricación].PinOrd,[Pedidos de clientes].FecPed,[Artículos de clientes].NomArt,[Ordenes de fabricación].PlaOrd,[Ordenes de fabricación].Observaciones,[Ordenes de fabricación].PreOrd,[Ordenes de fabricación].EntOrd,[Ordenes de fabricación].Datos,[Pedidos de clientes].PedPed,[Ordenes de fabricación].LanOrd,[Ordenes de fabricación].MarPie FROM [Pedidos de clientes] INNER JOIN ([Artículos de clientes] INNER JOIN [Ordenes de fabricación] ON [Artículos de clientes].CodArt = [Ordenes de fabricación].ArtOrd) ON [Pedidos de clientes].NumPed = [Ordenes de fabricación].PinOrd WHERE [Ordenes de fabricación].NumOrd =" + dtrow["uid"];
                    
                    OleDbDataAdapter adapt = new OleDbDataAdapter(query, con);
                    adapt.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        string custcode = row[1].ToString().Substring(0, 6);

                        if (!(custcode.Contains("494938")) && !(custcode.Contains("343420")))
                        {
                            string markingQr = new Uri(Server.MapPath("~/Marking QR//mar-" + dtrow["uid"].ToString() + ".Jpeg")).AbsoluteUri;
                            parameter2 = new ReportParameter("markingpath", markingQr);
                            
                        }

                        if (!(custcode.Contains("323205")) && !(custcode.Contains("494931")) && !(custcode.Contains("494938")) && !(custcode.Contains("343420")) && !(custcode.Contains("494946")))
                        {
                            string imagePath = new Uri(Server.MapPath("~/UID QR//" + dtrow["uid"].ToString() + ".Jpeg")).AbsoluteUri;
                            parameter1 = new ReportParameter("ImagePath", imagePath);

                            if (custcode.Contains("333325") || custcode.Contains("494908") || custcode.Contains("343403"))
                            {
                                string cylenderImagePath = new Uri(Server.MapPath("~/cylenderImage//MARKING.png")).AbsoluteUri;
                                parameter3 = new ReportParameter("CylenderImage", cylenderImagePath);
                            }
                            if (/*custcode.Contains("494923") ||*/ custcode.Contains("494905"))
                            {
                                string calenderImagePath = new Uri(Server.MapPath("~/494923//494923.png")).AbsoluteUri;
                                parameter4 = new ReportParameter("calender", calenderImagePath);
                            }

                        }
                        if (parameter1 != null)
                            ReportViewer1.LocalReport.SetParameters(parameter1);
                        if (parameter2 != null)
                            ReportViewer1.LocalReport.SetParameters(parameter2);
                        if (parameter3 != null)
                            ReportViewer1.LocalReport.SetParameters(parameter3);
                        if (parameter4 != null)
                            ReportViewer1.LocalReport.SetParameters(parameter4);
                        
                    }

                    if (datasetTable.Rows.Count != 0)
                    {
                        datasetTable.Clear();
                    }

                    try
                    {
                        OleDbCommand cmds = new OleDbCommand(query, con);
                        OleDbDataReader reader = cmds.ExecuteReader();
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                string numord = reader["NumOrd"].ToString();
                                string artord = reader["ArtOrd"].ToString();
                                string pieord = reader["PieOrd"].ToString();
                                string pinord = reader["PinOrd"].ToString();
                                string fecped = reader["FecPed"].ToString();
                                string nomart = reader["NomArt"].ToString();
                                string plaord = reader["PlaOrd"].ToString();
                                string observaciones = reader["Observaciones"].ToString();
                                string preord = reader["PreOrd"].ToString();
                                string entord = reader["EntOrd"].ToString();
                                string datos = reader["Datos"].ToString();
                                string pedped = reader["PedPed"].ToString();
                                string lanord = reader["LanOrd"].ToString();
                                string marpie = reader["MarPie"].ToString();

                                marpie = plaord;

                                if (!(artord.Substring(0, 6).Contains("393903")))
                                {
                                    if (marpie != "")
                                    {
                                        if ((artord.Substring(0, 6).Contains("494938")) || (artord.Substring(0, 6).Contains("343420")))
                                        {
                                            marpie = "No Marking";
                                        }
                                        else
                                        {
                                            marpie = plaord;
                                        }

                                        if ((artord.Substring(0, 6).Contains("323205")) || (artord.Substring(0, 6).Contains("494931")) || (artord.Substring(0, 6).Contains("494938")) || (artord.Substring(0, 6).Contains("343420")) || (artord.Substring(0, 6).Contains("494946")))
                                        {

                                        }
                                        else
                                        {
                                            numord = "W " + numord;
                                        }
                                    }
                                    else
                                    {
                                        if ((artord.Substring(0, 6).Contains("494938")) || (artord.Substring(0, 6).Contains("343420")) || (artord.Substring(0, 6).Contains("494946")))
                                        {
                                            marpie = "No Marking";
                                        }
                                        else
                                        {
                                            marpie = plaord;
                                        }

                                        if ((artord.Substring(0, 6).Contains("323205")) || (artord.Substring(0, 6).Contains("494931")) || (artord.Substring(0, 6).Contains("494938")) || (artord.Substring(0, 6).Contains("343420")) || (artord.Substring(0, 6).Contains("494946")))
                                        {

                                        }
                                        else
                                        {
                                            numord = "W " + numord;
                                        }
                                    }
                                }

                                if (artord.Substring(0, 6).Contains("393903"))
                                {
                                    marpie = plaord;
                                }

                                if (artord.Substring(0, 6).Contains("515101"))
                                {
                                    imagedata.Columns.Add("imageName");
                                    imagedata.Columns.Add("imagePath");

                                    for (int i = 1; i <= Convert.ToInt32(pieord); i++)
                                    {
                                        String HiltiDataMatrixCode = plaord.Replace('/', '-') + "-" + pedped.Split(' ').GetValue(0) + "-" + i;

                                        HiltiDataMatrixQrCode(HiltiDataMatrixCode, numord, i);

                                        string HiltiDataMatrixQrCodeImage = new Uri(Server.MapPath("~/HiltiDataMatrixQR/HiltiDataMatrixQR-" + numord + "-"+i+".Jpeg")).AbsoluteUri;

                                        imagedata.Rows.Add(HiltiDataMatrixCode, HiltiDataMatrixQrCodeImage);
                                    }

                                }


                                if(numord.Contains("W"))
                                    parameter5 = new ReportParameter("uidnumber", numord.Split(' ').GetValue(1).ToString());
                                else
                                    parameter5 = new ReportParameter("uidnumber", numord);                                    
                                if (parameter5 != null)
                                    ReportViewer1.LocalReport.SetParameters(parameter5);
                                datasetTable.Rows.Add(numord, artord, pieord, pinord, fecped, nomart, plaord, observaciones, preord, entord, datos, pedped, lanord, marpie);
                                string weekNum=null, sDate=null, oDate=null, dDate=null;
                                if (entord != "")
                                { 
                                    CultureInfo cul = CultureInfo.CurrentCulture;  
                                    weekNum = cul.Calendar.GetWeekOfYear(Convert.ToDateTime(entord), CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
                                    dDate = Convert.ToDateTime(entord).ToString("dd-MMM-yyyy");

                                }
                                else
                                {
                                    weekNum="";
                                }

                                weekOfYear = new ReportParameter("weekOfYear", weekNum);
                                ReportViewer1.LocalReport.SetParameters(weekOfYear);

                                deliveryDate = new ReportParameter("deliveryDate",dDate);
                                ReportViewer1.LocalReport.SetParameters(deliveryDate);

                                if (lanord != "")
                                {
                                    sDate = Convert.ToDateTime(lanord).ToString("dd-MMM-yyyy");
                                }
                                else
                                {
                                    sDate = "";
                                }
                                startDate = new ReportParameter("startDate", sDate);
                                ReportViewer1.LocalReport.SetParameters(startDate);

                                if (fecped != "")
                                {
                                    oDate = Convert.ToDateTime(fecped).ToString("dd-MMM-yyyy");
                                }
                                else 
                                {
                                    oDate = "";
                                }

                                orderDate = new ReportParameter("orderDate", oDate);
                                ReportViewer1.LocalReport.SetParameters(orderDate);
                            }
                        }


                    }
                    catch(Exception ex)
                    {
                        Response.Redirect("errorpage.aspx?error=" + ex.Message);                        
                    }

                    OleDbDataAdapter ada = new OleDbDataAdapter("select NumFas,CodPie,Operac,CodPro,FasExt from [Ordenes de fabricación (fases)] WHERE NumOrd=" + dtrow["uid"], con);
                    ada.Fill(dataset, "DataTable2");

                    DataTable dtable = new DataTable();
                    OleDbDataAdapter ad = new OleDbDataAdapter("SELECT  [Ordenes de fabricación].NumOrd,[Ordenes de fabricación].ArtOrd, [Artículos de clientes (piezas)].CodPie,[Artículos de clientes (piezas)].CtdPie,[Artículos de clientes (piezas)].MatPie,[Artículos de clientes (piezas)].CalPie,[Artículos de clientes (piezas)].DurPie,[Artículos de clientes (piezas)].DiaExt,[Artículos de clientes (piezas)].Longit,[Artículos de clientes (piezas)].DiaInt,[Artículos de clientes (piezas)].Dimen4,[Artículos de clientes (piezas)].Dimen5, [Ordenes de fabricación].Location" +
                                " FROM ([Ordenes de fabricación] LEFT JOIN [Artículos de clientes (piezas)] ON [Ordenes de fabricación].ArtOrd = [Artículos de clientes (piezas)].Codart )" +
                                "  WHERE [Ordenes de fabricación].NumOrd = " + dtrow["uid"], con);
                    ad.Fill(dataset, "DataTable3");


                    DataTable dstable = new DataTable();
                    OleDbDataAdapter adrr = new OleDbDataAdapter("SELECT ProPie,Plapie,CanRec,PedPro,CodPie  FROM [Ordenes de fabricación (piezas)] WHERE NumOrd = " + dtrow["uid"], con);
                    adrr.Fill(dataset, "DataTable4");
                    adrr.Fill(dtable);

                    string plapieDate = null;

                    if (dtable.Rows.Count > 0)
                    {
                        if (dtable.Rows[0]["PlaPie"].ToString() != "")
                        {
                            plapieDate = Convert.ToDateTime(dtable.Rows[0]["PlaPie"]).ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            plapieDate = "";
                        }
                    }

                    plapie = new ReportParameter("plapie", plapieDate);
                    ReportViewer1.LocalReport.SetParameters(plapie);

                    OleDbCommand command = new OleDbCommand("select NumOrd, NumFas, CodPie, Operac, CodPro, FasExt from [Ordenes de fabricación (fases)] WHERE NumOrd=" + dtrow["uid"], con);
                    OleDbDataReader dr = command.ExecuteReader();
                    if (dr.HasRows == false)
                    {
                        ReportViewer1.LocalReport.ReportPath = "Report1.rdlc";
                        //rd = new ReportDataSource("DataSet1", dataset.Tables[0]);
                        rd = new ReportDataSource("DataSet1", datasetTable);
                        rd1 = new ReportDataSource("DataSet2", dataset.Tables[1]);
                        rd2 = new ReportDataSource("DataSet3", dataset.Tables[2]);
                        rd3 = new ReportDataSource("DataSet4",dataset.Tables[3]);
                    }
                    else
                    {
                        if (stepsTable.Rows.Count != 0)
                        {
                            stepsTable.Clear();
                        }
                        
                        while (dr.Read())
                        {
                            string operac = "";
                            string numfas = dr["NumFas"].ToString();
                            string codpie = dr["CodPie"].ToString();

                            if (dr["Operac"].ToString().Contains("&") == true)
                            {
                                string temp = dr["Operac"].ToString();
                                string[] sc = temp.Split(' ');

                                for(int i=0;i<sc.Length;i++)
                                {
                                    if (sc[i].Contains("&"))
                                    {
                                        sc[i] = WebUtility.HtmlDecode(sc[i]);
                                    }

                                    operac = operac + " " + sc[i];
                                }
                            }
                            else
                            {
                                operac = dr["Operac"].ToString();
                            }                            
                            string codpro = dr["CodPro"].ToString();
                            string fasext = dr["FasExt"].ToString();
                            stepsTable.Rows.Add(numfas,codpie,operac.Trim(),codpro,fasext);
                        }

                        
                        ReportViewer1.LocalReport.ReportPath = "pdf.rdlc";
                        rd = new ReportDataSource("DataSet1", datasetTable);
                        rd1 = new ReportDataSource("DataSet2",stepsTable);
                        rd2 = new ReportDataSource("DataSet3", dataset.Tables[2]);
                        rd3 = new ReportDataSource("DataSet4",dataset.Tables[3]);
                        rd4 = new ReportDataSource("DataSet5", imagedata);
                    }

                    con.Close();

                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(rd);
                    ReportViewer1.LocalReport.DataSources.Add(rd1);
                    ReportViewer1.LocalReport.DataSources.Add(rd2);
                    ReportViewer1.LocalReport.DataSources.Add(rd3);
                    ReportViewer1.LocalReport.DataSources.Add(rd4);
                    
                    SavePDF(ReportViewer1, Server.MapPath(@"~/GeneratePDF/" + dtrow["uid"] + ".pdf"));

                    int columnCount=imagedata.Columns.Count;
                    for (int i = 0; i < columnCount;)
                    {
                        imagedata.Columns.RemoveAt(i);
                        columnCount--;
                    }
                    imagedata.Rows.Clear();
                    
                    drawingFolder = dtrow["DrawingNum"].ToString();
                    if (checkedBothCheckBox == "true")
                    {

                        pdflist.Add(Server.MapPath(@"~/GeneratePDF/" + dtrow["uid"].ToString() + ".pdf"));
                        pdflist.Add("W://test//Access//Planos//" + drawingFolder.Substring(0, 6) + "/" + dtrow["DrawingNum"].ToString() + ".PC.pdf");

                    }
                    else if(checkedJobCardCheckBox=="true")
                    {
                        pdflist.Add(Server.MapPath(@"~/GeneratePDF/"+dtrow["uid"].ToString()+".pdf"));
                    }
                    else if (CheckedprintDrawingCheckBox == "true")
                    {
                        pdflist.Add("W://test//Access//Planos//" + drawingFolder.Substring(0, 6) + "/" + dtrow["DrawingNum"].ToString() + ".PC.pdf");
                    }
                }

                String[] file=new String[pdflist.Count];

                for (int i = 0; i < pdflist.Count; i++)
                {
                    file[i] = pdflist[i];
                }

                CombineMultiplePDFs(file, Server.MapPath(@"~/final pdf/CombinePdfJD.pdf"));

                pdflist.Clear();
            }
            Session.Remove("uidtable");
            if (Session["CheckedprintDrawingCheckBox"] != null)
            {
                Session.Remove("CheckedprintDrawingCheckBox");
            }
            if (Session["checkedJobCardCheckBox"] != null)
            {
                Session.Remove("checkedJobCardCheckBox");
            }

            if (Session["checkedBothCheckBox"] != null)
            {
                Session.Remove("checkedBothCheckBox");
            }

            Session["pdfGenerated"] = "successfull";
            //Response.Redirect("javascript:history.go(-1)");
            Response.Redirect("index.aspx");
            
        }

        public static void CombineMultiplePDFs(string[] fileNames, string outFile)
        {
            // step 1: creation of a document-object
            Document document = new Document();
            //create newFileStream object which will be disposed at the end
            using (FileStream newFileStream = new FileStream(outFile, FileMode.Create))
            {
                // step 2: we create a writer that listens to the document
                PdfCopy writer = new PdfCopy(document, newFileStream);
                if (writer == null)
                {
                    return;
                }

                // step 3: we open the document
                document.Open();

                foreach (string fileName in fileNames)
                {
                    // we create a reader for a certain document
                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(fileName);
                    reader.ConsolidateNamedDestinations();

                    // step 4: we add content
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        writer.AddPage(page);
                    }

                    PRAcroForm form = reader.AcroForm;
                    if (form != null)
                    {
                        writer.CopyAcroForm(reader);
                    }

                    reader.Close();
                }

                // step 5: we close the document and writer
                writer.Close();
                document.Close();
            }//disposes the newFileStream object
        }

        void UpdateStartDate(String uid, String StartDate)
        {
            //string UID = dt.Rows[0][0].ToString();
            if (StartDate=="")
            {
                try
                {
                    DateTime todayDate = DateTime.Today.Date;
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "UPDATE [Ordenes de fabricación] SET LanOrd = '" + todayDate.ToString("dd/MM/yyyy") + "' Where NumOrd = " + uid ;

                    cmd.ExecuteNonQuery();
                    

                }
                catch (Exception exception)
                {
                    //MessageBox.Show(exception.ToString());
                }
            }
            else
            {

            }
        }

        public void SavePDF(ReportViewer viewer, string savePath)
        {
            string deviceInfo = "<DeviceInfo>" +
                    "  <OutputFormat>PDF</OutputFormat>" +
                    "  <PageWidth>8.5in</PageWidth>" +
                    "  <PageHeight>11in</PageHeight>" +
                    "  <MarginTop>0cm</MarginTop>" +
                    "  <MarginLeft>0cm</MarginLeft>" +
                    "  <MarginRight>0cm</MarginRight>" +
                    "  <MarginBottom>0cm</MarginBottom>" +
                    "  <HumanReadablePDF>True</HumanReadablePDF>" +
                    "</DeviceInfo>";
            byte[] Bytes = viewer.LocalReport.Render(format: "PDF", deviceInfo: deviceInfo);

            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                stream.Write(Bytes, 0, Bytes.Length);
            }
        }
        public void qrCode(String UID)
        {
            BarcodeSettings.ApplyKey("NRL6F-JPPT7-7KA4Z-BBKUA-FP3RM");//you need a key from e-iceblue, otherwise the watermark 'E-iceblue' will be shown in barcode
            BarcodeSettings settings = new BarcodeSettings();
            settings.Type = BarCodeType.QRCode;
            settings.Unit = GraphicsUnit.Pixel;
            settings.ShowText = false;
            settings.ResolutionType = ResolutionType.UseDpi;
            settings.Data = data;
            data = "W "+UID;
            settings.Data = data;
            settings.ForeColor = System.Drawing.Color.Black;
            settings.BackColor = System.Drawing.Color.White;
            short barWidth;
            settings.X = 3;
            short leftMargin = 1;
            
            settings.LeftMargin = leftMargin;

            short rightMargin = 1;
            
            settings.RightMargin = rightMargin;
            short topMargin = 1;
            settings.TopMargin = topMargin;
            short bottomMargin = 1;
            settings.BottomMargin = bottomMargin;

            BarCodeGenerator generator = new BarCodeGenerator(settings);
            System.Drawing.Image QRbarcode = generator.GenerateImage();
            QRbarcode.Save(Server.MapPath(@"~/UID QR/" + UID + ".Jpeg"), System.Drawing.Imaging.ImageFormat.Jpeg);
            
        }
        void QRCodeMarking(String marking, String UID)
        {
            BarcodeSettings.ApplyKey("NRL6F-JPPT7-7KA4Z-BBKUA-FP3RM");//you need a key from e-iceblue, otherwise the watermark 'E-iceblue' will be shown in barcode
            BarcodeSettings settings = new BarcodeSettings();
            settings.Type = BarCodeType.QRCode;
            settings.Unit = GraphicsUnit.Pixel;
            settings.ShowText = false;
            settings.ResolutionType = ResolutionType.UseDpi;
            //input data
            string data = "12345";
            settings.Data = data;
           
            data = "1";
            
            data = marking;
            settings.Data = data;
            
            settings.ForeColor = System.Drawing.Color.Black;
            
            settings.BackColor = System.Drawing.Color.White;
            
            short barWidth;
            
            settings.X = 3;

            short leftMargin = 1;
            
            settings.LeftMargin = leftMargin;

            short rightMargin = 1;
            
            settings.RightMargin = rightMargin;

            short topMargin = 1;
            
            settings.TopMargin = topMargin;

            short bottomMargin = 1;
            
            settings.BottomMargin = bottomMargin;

            //generate QR code
            BarCodeGenerator generator = new BarCodeGenerator(settings);
            System.Drawing.Image QRbarcode = generator.GenerateImage();
            QRbarcode.Save(Server.MapPath(@"~/Marking QR/mar-" + UID + ".Jpeg"), System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        void HiltiDataMatrixQrCode(String HiltiDataMatrixCode, String UID, int k)
        {
            BarcodeSettings.ApplyKey("NRL6F-JPPT7-7KA4Z-BBKUA-FP3RM");//you need a key from e-iceblue, otherwise the watermark 'E-iceblue' will be shown in barcode
            BarcodeSettings settings = new BarcodeSettings();
            settings.Type = BarCodeType.QRCode;
            settings.Unit = GraphicsUnit.Pixel;
            settings.ShowText = false;
            settings.ResolutionType = ResolutionType.UseDpi;
            //input data
            string data = "12345";
            settings.Data = data;

            data = "1";

            data = HiltiDataMatrixCode;
            settings.Data = data;

            settings.ForeColor = System.Drawing.Color.Black;

            settings.BackColor = System.Drawing.Color.White;

            short barWidth;

            settings.X = 3;

            short leftMargin = 1;

            settings.LeftMargin = leftMargin;

            short rightMargin = 1;

            settings.RightMargin = rightMargin;

            short topMargin = 1;

            settings.TopMargin = topMargin;

            short bottomMargin = 1;

            settings.BottomMargin = bottomMargin;

            //generate QR code
            BarCodeGenerator generator = new BarCodeGenerator(settings);
            System.Drawing.Image QRbarcode = generator.GenerateImage();
            QRbarcode.Save(Server.MapPath(@"~/HiltiDataMatrixQR//HiltiDataMatrixQR-" + UID  + "-"+k+".Jpeg"), System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}