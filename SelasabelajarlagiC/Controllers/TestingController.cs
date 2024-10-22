using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using SelasabelajarlagiC.Models;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using System.Data;
using FastMember;

namespace SelasabelajarlagiC.Controllers
{
    public class TestingController : Controller
    {
        // GET: Testing
        List<UsersModel> lud = new List<UsersModel>();
        CompanyRepo cr = new CompanyRepo();
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult Testing(string username, string password)
        {
            List<string> coba1 = new List<string>() { "nama1", "nama2", "nama3" };
            ViewBag.listData = coba1;
            ViewBag.Jje = "Halloo";
            
            UsersModel ud = new UsersModel();
            ud.Number = 1;
            ud.Username = username;
            ud.Password = password;

            lud.Add(ud);
            //return View();
            return Json(new { lud }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuatData()
        {
            //return Json(new { lud }, JsonRequestBehavior.AllowGet);
            return View();
        }

        public ActionResult SearchCompany(string name)
		{            
            List<Company> lc = cr.SearchCompany(name);
            return Json(new { data = lc }, JsonRequestBehavior.AllowGet);
		}
        public ActionResult getCompany(string id = null)
		{
            List<Company> lc = cr.GetCompany(id);
            return Json(new { data = lc }, JsonRequestBehavior.AllowGet);
            //return View(lc);
        }

       /* public JsonResult insertCompany(Company d)
		{
            Company postData = new Company();
            postData.ID = "ARKSS";
            postData.NAME = "ARKAMAYA";
            postData.DESCRIPTION = "Software House";
            Result res = cr.PostData(postData);            
            return Json(new { result = res }, JsonRequestBehavior.AllowGet);
        }*/

        public ActionResult DeleteCompany(string id)
		{
            var res = cr.DeleteData(id);
            //return Json(new { result = res}, JsonRequestBehavior.AllowGet);
            return RedirectToAction("BuatData", "Testing");
		}

        public JsonResult UpdateCompany(Company d)
		{
            Company updateData = new Company();
            //updateData.ID = "ARKM";
            //updateData.NAME = "ADUHADUH";
            updateData.ID = d.ID;
            updateData.NAME = d.NAME;
            updateData.DESCRIPTION = d.DESCRIPTION;
            var res = cr.UpdateData(updateData);

            return Json(new { result = res, message = "Data berhasil diperbaharui" }, JsonRequestBehavior.AllowGet);
		}

        public JsonResult CobaIsidata(Company d)
		{
            Result res = cr.PostData(d);            
            return Json(new { result = res }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(string id)
		{
            List<Company> detail = cr.GetCompany(id);        
            Company editData = new Company();
            editData.ID = detail[0].ID;
            editData.NAME = detail[0].NAME;
            editData.DESCRIPTION = detail[0].DESCRIPTION;

            ViewBag.ID = editData.ID;
            ViewBag.NAME = editData.NAME;
            ViewBag.DESCRIPTION = editData.DESCRIPTION;

            return View();
        }

        private void CreateCell(IRow CurrentRow, int CellIndex, string Value, HSSFCellStyle Style)
        {
            ICell Cell = CurrentRow.CreateCell(CellIndex);
            Cell.SetCellValue(Value);
            Cell.CellStyle = Style;
        }
        public ActionResult downloadExcel()
		{
            var workbook = new HSSFWorkbook();
            HSSFFont myFont = (HSSFFont)workbook.CreateFont();           
            HSSFCellStyle borderedCellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            borderedCellStyle.SetFont(myFont);
            borderedCellStyle.BorderLeft = BorderStyle.Medium;
            borderedCellStyle.BorderTop = BorderStyle.Medium;
            borderedCellStyle.BorderRight = BorderStyle.Medium;
            borderedCellStyle.BorderBottom = BorderStyle.Medium;
            borderedCellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

            ISheet Sheet = workbook.CreateSheet("Company");
            Sheet.AutoSizeColumn(0);
            Sheet.AutoSizeColumn(1);
            Sheet.AutoSizeColumn(2);
            Sheet.AutoSizeColumn(3);

            //Creat The Headers of the excel
            IRow HeaderRow = Sheet.CreateRow(0);
               //Create The Actual Cells
            CreateCell(HeaderRow, 0, "No", borderedCellStyle);
            CreateCell(HeaderRow, 1, "ID", borderedCellStyle);
            CreateCell(HeaderRow, 2, "NAME", borderedCellStyle);
            CreateCell(HeaderRow, 3, "DESCRIPTION", borderedCellStyle);

            // This Where the Data row starts from
            int RowIndex = 1;

            CompanyRepo cr = new CompanyRepo();
            List<Company> data = cr.GetCompany();

            /*foreach(Company d in data)
			{
                IRow CurrentRow = Sheet.CreateRow(RowIndex);
                //Creating the CurrentDataRow
                CreateCell(CurrentRow, 0, d.ID, borderedCellStyle);
                // This will be used to calculate the merge area
                int NumberOfRules = d.ID.Count();
                if (NumberOfRules > 1)
                {
                    int MergeIndex = (NumberOfRules - 1) + RowIndex;

                    //Merging Cells
                    NPOI.SS.Util.CellRangeAddress MergedBatch = new NPOI.SS.Util.CellRangeAddress(RowIndex, MergeIndex, 0, 0);
                    Sheet.AddMergedRegion(MergedBatch);
                }
            }*/

            int i = 0;
            // Iterate through cub collection
            foreach (Company d in data)
            {
                var number = i + 1;
                //IRow CurrentRow = Sheet.CreateRow(RowIndex);
                    IRow CurrentRow = Sheet.CreateRow(RowIndex);
                CreateCell(CurrentRow, 0, number.ToString(), borderedCellStyle);
                CreateCell(CurrentRow, 1, d.ID, borderedCellStyle);
                CreateCell(CurrentRow, 2, d.NAME, borderedCellStyle);
                CreateCell(CurrentRow, 3, d.DESCRIPTION, borderedCellStyle);

                Sheet.AutoSizeColumn(0);
                Sheet.AutoSizeColumn(1);
                Sheet.AutoSizeColumn(2);
                Sheet.AutoSizeColumn(3);
                RowIndex++;
                i++;
            }
            MemoryStream output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(),
             "application/vnd.ms-excel",
             "ArticleList.xls");
        }
        public ActionResult downloadByPdf()
		{
            var document = new Document()
            {
                PageInfo = new PageInfo { Margin = new MarginInfo(28, 28, 28, 40) }
            };
            var pdfpage = document.Pages.Add();
            Aspose.Pdf.Table table = new Aspose.Pdf.Table
            {
                ColumnWidths = "25% 25% 25% 40%",
                DefaultCellPadding = new MarginInfo(10, 5, 10, 5),
                Border = new BorderInfo(BorderSide.All, .5f, Color.Black),
                DefaultCellBorder = new BorderInfo(BorderSide.All, .2f, Color.Black),
            };
            
            CompanyRepo cr = new CompanyRepo();

            List<Company> data = cr.GetCompany();
            DataTable tb = new DataTable();
            using (var reader = ObjectReader.Create(data, "ID", "NAME", "DESCRIPTION"))
            {
                tb.Load(reader);
            }
            
            table.ImportDataTable(tb, true, 0, 0);
            document.Pages[1].Paragraphs.Add(table);

            using (var streamout = new MemoryStream())
			{
                document.Save(streamout);
                return new FileContentResult(streamout.ToArray(), "application/pdf")
                {
                    FileDownloadName = "Company.pdf"
                };
			}
        }
      
    }
}