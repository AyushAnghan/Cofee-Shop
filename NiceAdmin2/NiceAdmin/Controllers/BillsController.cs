using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;
using System.Data.SqlClient;
using System.Data;
using NiceAdmin.CommonClasses;
using System.Net.Mail;
using System.Net.Mime;
using NiceAdmin.BAL;

namespace NiceAdmin.Controllers
{
    [CheckAccess]
    public class BillsController : Controller
    {
        #region Configuration
        private IConfiguration configuration;
        private IEmailSender emailSender;
        public BillsController(IEmailSender emailSender,IConfiguration configuration)
        {
            this.configuration = configuration;
            this.emailSender = emailSender;
        }
        #endregion

        #region Bills list View(BillsView)
        public IActionResult BillsView()
        {

            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else
            //{
                return View(CommonClass.SelectData("PR_Bills_SelectAll"));
            //}
        }
        #endregion

        #region Add Update Bills View(AddUpdateBillsView)
        public IActionResult AddUpdateBillsView(int? BillID)
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else
            //{

                #region Drop Down User
                DataTable table1 = CommonClass.SelectData("PR_User_DropDown");

                List<UserDropDownModel> userList = new List<UserDropDownModel>();
                foreach (DataRow data in table1.Rows)
                {
                    UserDropDownModel userDropDownModel = new UserDropDownModel();
                    userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                    userDropDownModel.UserName = data["UserName"].ToString();
                    userList.Add(userDropDownModel);
                }
                ViewBag.UserList = userList;
                #endregion

                #region Drop Down Order 
                DataTable dataTable3 = CommonClass.SelectData("PR_Order_DropDown");

                List<OrderDropDownModel> orderList = new List<OrderDropDownModel>();
                foreach (DataRow data in dataTable3.Rows)
                {
                    OrderDropDownModel orderDropDownModel = new OrderDropDownModel();
                    orderDropDownModel.OrderID = Convert.ToInt32(data["OrderID"]);
                    orderList.Add(orderDropDownModel);
                }
                ViewBag.OrderList = orderList;
                #endregion

                BillsModel modelBills = new BillsModel();

                #region Fetch Data
                if (BillID != null && BillID != 0)
                {
                    DataTable table = CommonClass.SelectByPk("PR_Bills_SelectByPK", (int)BillID, "BillID");

                    try
                    {
                        modelBills.BillID = Convert.ToInt32(table.Rows[0]["BillID"]);
                        modelBills.BillNumber = table.Rows[0]["BillNumber"].ToString();
                        modelBills.BillDate = Convert.ToDateTime(table.Rows[0]["BillDate"]);
                        modelBills.OrderID = Convert.ToInt32(table.Rows[0]["OrderID"]);
                        modelBills.TotalAmount = Convert.ToDecimal(table.Rows[0]["TotalAmount"]);
                        modelBills.Discount = Convert.ToDecimal(table.Rows[0]["Discount"]);
                        modelBills.NetAmount = Convert.ToDecimal(table.Rows[0]["NetAmount"]);
                        modelBills.UserID = Convert.ToInt32(table.Rows[0]["UserID"]);
                    }
                    catch (Exception ex)
                    {
                        TempData["AlertMessage"] = "Bill Not Exists!";
                        return RedirectToAction("BillsView");
                    }
                }
                #endregion

                return View("AddUpdateBillsView", modelBills);
            //}
            }
            #endregion

        #region AddBillsView(HttpPost)

        [HttpPost]
        public IActionResult AddUpdateBillsView(BillsModel modelBills)
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else
            //{
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");

                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    if (modelBills.BillID == 0)
                    {
                        command.CommandText = "PR_Bills_Insert";
                    }
                    else
                    {
                        command.CommandText = "PR_Bills_UpdateByPK";
                        command.Parameters.Add("@BillID", SqlDbType.Int).Value = modelBills.BillID;
                        command.Parameters.Add("@BillDate", SqlDbType.DateTime).Value = modelBills.BillDate;
                    }
                    command.Parameters.Add("@BillNumber", SqlDbType.VarChar).Value = modelBills.BillNumber;

                    command.Parameters.Add("@OrderID", SqlDbType.Int).Value = modelBills.OrderID;
                    command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = modelBills.TotalAmount;
                    command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = modelBills.Discount;
                    command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = modelBills.NetAmount;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelBills.UserID;

                    if (Convert.ToBoolean(command.ExecuteNonQuery()))
                    {
                        TempData["InsertUpdateMSG"] = modelBills.BillID == 0 ? "Inserted successfully!" : "Updated successfully!";
                        connection.Close();
                        return RedirectToAction("BillsView");
                    }
                }
                return View("AddUpdateBillsView", modelBills);
            //}
        }
        #endregion

        #region DeleteBills
        [HttpPost]
        public IActionResult DeleteBills(int BillID)

        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else
            //{
                try
                {

                    CommonClasses.CommonClass.DeleteRow("PR_Bills_DeleteByPK", BillID, "BillID");
                    TempData["Message"] = "Delete successfully!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMSG"] = ex.Message;
                }
                return RedirectToAction("BillsView");
            //}
        }
        #endregion

        #region Export To Excel
        public IActionResult ExportToExcel()
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else
            //{
                Stream stream = CommonClass.ExportToExcel("PR_Bills_SelectAll");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BillsExport.xlsx");
            //}
        }
        #endregion

        #region Mail
        public async Task<IActionResult> Mail()
        {
            if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            {
                return RedirectToAction("Index", "SEC_User");
            }
            else
            {
                //var receiver = BAL.CV.Email();
                var receiver = "";
                var subject = "Test";
                var message = "<html><body><b>Hello welcome Home</b></body></html>";



                Stream stream = CommonClass.ExportToExcel("PR_Bills_SelectAll");

                try
                {
                    // Create the attachment from the stream

                    Attachment attachment = new Attachment(stream, "BillsExcelFile.xlsx", MediaTypeNames.Application.Octet);

                    await emailSender.SendEmailAsync(receiver, subject, message, new List<Attachment> { attachment });

                    /*await emailSender.SendEmailAsync(receiver, subject, message);*/
                    /*ViewBag.Message = "Email sent successfully!";*/
                    TempData["Message"] = "Email sent successfully!";
                }
                catch (Exception ex)
                {
                    // Log the error (you can log this to a file or database)
                    TempData["Error"] = $"Error sending email: {ex.Message}";
                }

                return RedirectToAction("BillsView");
            }
        }
        #endregion
    }
}
