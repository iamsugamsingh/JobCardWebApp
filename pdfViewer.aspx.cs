using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;

namespace JobCardApplication
{
    public partial class pdfViewer : System.Web.UI.Page
    {
       //String finalFile = "W://Software//Sugam//JobCard WebApp//final pdf//CombinePdfJD.pdf";
       //String finalduplicateFile = "W://Software//Sugam//JobCard WebApp//final pdf//DuplicatePdf.pdf";

        protected void Page_Load(object sender, EventArgs e)
        {
            String finalFile = Server.MapPath(@"~/final pdf/CombinePdfJD.pdf");
            String finalduplicateFile = Server.MapPath(@"~/final pdf/DuplicatePdf.pdf");

            string FilePath = "";
            if (File.Exists(finalFile))
            {
                FilePath = Server.MapPath("~/final pdf/CombinePdfJD.pdf");
            }
            if (File.Exists(finalduplicateFile))
            {
                FilePath = Server.MapPath("~/final pdf/DuplicatePdf.pdf");
            }

            WebClient User = new WebClient();
            Byte[] FileBuffer = User.DownloadData(FilePath);
            if (FileBuffer != null)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", FileBuffer.Length.ToString());
                //Response.AppendHeader("Content-Disposition", "attachment;filename=Generated.pdf");
                Response.BinaryWrite(FileBuffer);
            }
            if (Session["pdfGenerated"] != null)
            {
                Session.Remove("pdfGenerated");
            }
        }
    }
}