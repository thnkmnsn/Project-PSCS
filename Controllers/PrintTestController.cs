using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Common;
using PSCS.Models;
using PSCS.Services;
using System.Text;
using System.IO;
using System.Drawing;
using OfficeOpenXml;
using PSCS.Excels;
using System.Drawing.Imaging;
using System.Configuration;
using System.Net;
using PSCS.ModelsScreen;
using System.Management;

namespace PSCS.Controllers
{
    public class PrintTestController : BaseController
    {
        //public class Printer
        //{
        //    public string Name { get; set; }
        //}

        public PrintTestScreen myModel
        {
            get
            {
                if (Session["ModelPrintTestScreen"] == null)
                {
                    Session["ModelPrintTestScreen"] = new PrintTestScreen();
                    return (PrintTestScreen)Session["ModelPrintTestScreen"];
                }
                else
                {
                    return (PrintTestScreen)Session["ModelPrintTestScreen"];
                }
            }
            set { Session["ModelPrintTestScreen"] = value; }
        }

        

        // GET: PrintTest
        public ActionResult Index()
        {
            //Printer[] arrPrintername = null;
            //Printer objPrintername = null;

            try
            {
                InitializeActionName = "Index";
                QueryStringList = new Dictionary<string, string>();
                this.myModel.AlertsType = Common.Constants.AlertsType.None;
                this.myModel.Message = string.Empty;
                //this.myModel.InputPrinter = "\\\\192.168.200.20\\ZDesigner ZT230-200dpi ZPL";
                this.myModel.InputPrinter = "ZDesigner ZT230-200dpi ZPL on 192.168.200.20";

                List<SelectListItem> objPrinterList = new List<SelectListItem>();
                //foreach (string printerName in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                //{
                //    objPrinterList.Add(new SelectListItem { Text = printerName, Value = printerName.ToString() });
                //}
                //this.myModel.PrinterList = objPrinterList;


                String pkInstalledPrinters;
                for (int i = 0; i < System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count; i++)
                {
                    pkInstalledPrinters = System.Drawing.Printing.PrinterSettings.InstalledPrinters[i];
                    objPrinterList.Add(new SelectListItem { Text = pkInstalledPrinters, Value = pkInstalledPrinters });
                }
                this.myModel.PrinterList = objPrinterList;
            }
            catch (Exception ex)
            {
                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.myModel);
            }

            return View(this.myModel);
        }

        [HttpPost]   
        public ActionResult Index(string submitButton, PrintTestScreen model)
        {
            try
            {
                switch (submitButton)
                {
                    case "Print":
                        //return (Print_OnClick(model.FilterPrinter));
                        return (Print_OnClick(model.InputPrinter));
                    default:
                        return View(this.myModel);
                }
            }
            catch (Exception ex)
            {
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.myModel);
            }
        }


        private ActionResult Print_(string pPrinterName)
        {
            string rawString = string.Empty;
            string nl = System.Environment.NewLine;

            try
            {

                string barcode = "123456";
                MemoryStream memoryStream = new MemoryStream();

                Bitmap bitMap = new Bitmap(barcode.Length * 40, 80);

                using (Graphics graphics = Graphics.FromImage(bitMap))
                {
                    Font oFont = new Font("Microsoft Sans Serif", 16);
                    oFont = new Font("Chiller", 16);
                    oFont = new Font("Code 128", 16);
                    oFont = new Font("IDAutomationHC39M", 16);
                    //PointF point = new PointF(2f, 2f);
                    PointF point = new PointF(2f, 2f);
                    SolidBrush whiteBrush = new SolidBrush(Color.White);
                    graphics.FillRectangle(whiteBrush, 0, 0, bitMap.Width, bitMap.Height);
                    SolidBrush blackBrush = new SolidBrush(Color.DarkBlue);
                    graphics.DrawString("*" + barcode + "*", oFont, blackBrush, point);
                }

                bitMap.Save(@"C:\Temp\test2.jpg");
                ////b = new Bitmap(@"C:\Documents and Settings\Desktop\7506.jpg");
                //b.Save(@"C:\Extract\test.jpg");
                Image logo = bitMap;

                //Image logo = Image.FromFile(@"C:\Temp\test2.jpg");
                using (ExcelPackage package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Test Page");
                    int a = 1;

                    ws.Row(a * 5).Height = 39.00D;
                    var picture = ws.Drawings.AddPicture(a.ToString(), logo);
                    // row 5 col 2 (b) => Real row 6 col 3 (c)
                    picture.SetPosition(a * 5, 0, 2, 0);
                    //picture.SetPosition(20, 0, 5, 0);

                    package.SaveAs(memoryStream);
                    memoryStream.Position = 0;
                }

                this.myModel.AlertsType = Common.Constants.AlertsType.Success;
                this.myModel.Message = "Print Success";

                //return View("Index", this.myModel);

                DateTime dt = DateTime.Today;
                Response.AddHeader("Content-Disposition", "attachment; filename= Test1" + dt.ToString(Common.Constants.DATE_FILENAME) + Common.Constants.EXCEL_EXTENSION);

                return new FileStreamResult(memoryStream, Common.Constants.EXCEL_CONTENTTYPE);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //private StringBuilder command = new StringBuilder();

        private ActionResult Print_OnClick(string pPrinterName)
        {
            StringBuilder command = new StringBuilder();
            bool result = false;
            string strBarcode = string.Empty;

            try
            {
                //ReceivePlanService objReceivePlanService = new ReceivePlanService(this.dbEntities);

                //var list = objReceivePlanService.GetReceivePlanList(null, "", "");
                //command = CreateCommand();
                ////string Printer = "ZDesigner ZT230-200dpi ZPL";
                ////Print
                //result = PrintTest(pPrinterName, command);

                // 1. Convert Image to monochrome bmp
                string bitmapFilePath = @"D:\WORKS\Awaji\Awaji_PSCS\PSCS\bin\somepath.bmp";
                Bitmap imageToConvert = new Bitmap(bitmapFilePath);
                var rectangle = new Rectangle(0, 0, imageToConvert.Width, imageToConvert.Height);
                Bitmap monochromeImage = imageToConvert.Clone(rectangle, PixelFormat.Format1bppIndexed);

                // Mirror image
                monochromeImage.RotateFlip(RotateFlipType.Rotate180FlipX);

                // Save mono image  
                monochromeImage.Save(@"D:\WORKS\Awaji\Awaji_PSCS\PSCS\bin\somePathMono.bmp", ImageFormat.Bmp);

                // 2. Convert to ZPL
                ConvertImage();
            }
            catch (Exception ex)
            {
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.myModel);
            }

            return View(this.myModel);
        }


        private StringBuilder CreateCommand()
        {
            StringBuilder command = new StringBuilder();

            try
            {
                string nl = Environment.NewLine;

                command.Append("^XA" + nl);
                command.Append("^FX Third section with barcode. " + nl);
                command.Append("^BY2,2,50" + nl);
                command.Append("^FO15,30^BC" + nl);
                command.Append("^FD C11100353020^FS" + nl);
                command.Append("^XZ" + nl);

                var rwString = command.ToString();
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return command;
        }

        public bool Print(StringBuilder command)
        {
            bool result = false;
            string rawString = string.Empty;
            string nl = Environment.NewLine;
            string url = "http://api.labelary.com/v1/printers/8dpmm/labels/4x6/0/";

            if (command.Length > 0)
            {
                rawString = command.ToString();
                byte[] zpl = Encoding.UTF8.GetBytes(rawString);

                // adjust print density (8dpmm), label width (4 inches), label height (3 inches), and label index (0) as necessary
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Accept = "image/*"; // omit this line to get PNG images back
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = zpl.Length;

                var requestStream = request.GetRequestStream();
                requestStream.Write(zpl, 0, zpl.Length);
                requestStream.Close();

                try
                {
                    // Get the response.
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    // Get the stream containing content returned by the server.
                    Stream respStream = response.GetResponseStream();

                    Bitmap bmp = new Bitmap(respStream);
                    respStream.Dispose();

                    // Clean up the streams and the response.
                    response.Close();

                    MemoryStream ms = new MemoryStream();

                    var cropped = Common.Common.CropWhiteSpace(bmp);

                    int width = cropped.Width / 2;
                    int height = cropped.Height / 2;

                    Bitmap bitMap = new Bitmap(cropped, width, height);

                    //The Bitmap is saved to Memory Stream.
                    bitMap.Save(ms, ImageFormat.Png);

                    ViewBag.BarcodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());

                }
                catch (WebException e)
                {
                    Console.WriteLine("Error: {0}", e.Status);
                }

            }
            return result;
        }

        public bool PrintTest(string pPrinter, StringBuilder command)
        {
            bool result = false;
            string rawString = string.Empty;
            
            if (command.Length > 0)
            {
                rawString = command.ToString();

                try
                {
                    result = RawPrinterHelper.SendStringToPrinter(pPrinter, rawString);

                    if(result)
                    {
                        this.myModel.AlertsType = Constants.AlertsType.Success;
                        this.myModel.Message = "Print Success.";
                    }
                    else
                    {
                        this.myModel.AlertsType = Constants.AlertsType.Danger;
                        this.myModel.Message = "Print Error.";
                    }
                }
                catch (WebException e)
                {
                    Console.WriteLine("Error: {0}", e.Status);
                }
            }
            return result;
        }

        [HttpPost]
        public ActionResult Export()
        {
            Test();

            this.myModel.AlertsType = Common.Constants.AlertsType.None;
            this.myModel.Message = string.Empty;

            UserExcel excel = new UserExcel(this.dbEntities, this.LoginUser);
            MemoryStream ms = new MemoryStream();

            List<User> result = new List<User>();

            HttpCookie langCookie = Request.Cookies["PSCS_culture"];
            string lang = langCookie != null ? langCookie.Value : "En";

            if (!excel.Export(1, ms, lang))
            {
                Response.Clear();
                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModel.Message = "Unable to export Inbound Tally Sheet. " + excel.GetErrorMesssage();
                PrintError(this.myModel.Message);

                return View("Index", this.myModel);
            }

            DateTime dt = DateTime.Today;
            Response.AddHeader("Content-Disposition", "attachment; filename= Test1" + dt.ToString(Common.Constants.DATE_FILENAME) + Common.Constants.EXCEL_EXTENSION);

            return new FileStreamResult(ms, Common.Constants.EXCEL_CONTENTTYPE);
        }


        public void Test()
        {
            var inputPath = @"C:\Temp\test2.png";
            var outputPath = inputPath.Replace(".png", "-out.png");

            var bitmap = new Bitmap(inputPath);
            var cropped = Common.Common.CropWhiteSpace(bitmap);
            cropped.Save(outputPath, ImageFormat.Png);
        }


        public static void ConvertImage()
        {
            string bitmapFilePath = @"D:\WORKS\Awaji\Awaji_PSCS\PSCS\bin\somePathMono.bmp";
            int w, h;
            Bitmap b = new Bitmap(bitmapFilePath);
            w = b.Width; h = b.Height;
            byte[] bitmapFileData = System.IO.File.ReadAllBytes(bitmapFilePath);
            int fileSize = bitmapFileData.Length;

            int bitmapDataOffset = int.Parse(bitmapFileData[10].ToString()); ;
            int width = w; // int.Parse(bitmapFileData[18].ToString()); ;
            int height = h; // int.Parse(bitmapFileData[22].ToString()); ;
            int bitsPerPixel = int.Parse(bitmapFileData[28].ToString());
            int bitmapDataLength = bitmapFileData.Length - bitmapDataOffset;
            double widthInBytes = Math.Ceiling(width / 8.0);

            while (widthInBytes % 4 != 0)
            {
                widthInBytes++;
            }

            // Copy over the actual bitmap data without header data  
            byte[] bitmap = new byte[bitmapDataLength];

            Buffer.BlockCopy(bitmapFileData, bitmapDataOffset, bitmap, 0, bitmapDataLength);

            // Invert bitmap colors
            for (int i = 0; i < bitmapDataLength; i++)
            {
                bitmap[i] ^= 0xFF;
            }

            // Create ASCII ZPL string of hexadecimal bitmap data
            string ZPLImageDataString = BitConverter.ToString(bitmap);
            ZPLImageDataString = ZPLImageDataString.Replace("-", string.Empty);

            // Add new line every 1023 chars characters
            string ZPLImageDataStringWithNewLine = SpliceText(ZPLImageDataString, 1023);

            // Create ZPL command to print image
            string ZPLCommand = string.Empty;

            ZPLCommand += "^XA";
            ZPLCommand += "^FO20,20";
            ZPLCommand +=
            "^GFA," +
           bitmapDataLength.ToString() + "," +
           bitmapDataLength.ToString() + "," +
           widthInBytes.ToString() + "," +
            System.Environment.NewLine +
            ZPLImageDataStringWithNewLine;

            ZPLCommand += "^XZ";

            System.IO.StreamWriter sr = new System.IO.StreamWriter(@"D:\WORKS\Awaji\Awaji_PSCS\PSCS\bin\zplCodePath", false, System.Text.Encoding.Default);

            sr.Write(ZPLCommand);
            sr.Close();
        }

        public static string SpliceText(string text, int lineLength)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, "(.{" + lineLength + "})", "$1" + Environment.NewLine);
        }
    }

}