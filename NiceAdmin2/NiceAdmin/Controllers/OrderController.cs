using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using NiceAdmin.CommonClasses;
using System;
using System.Net.Mail;
using System.Net.Mime;
using NiceAdmin.BAL;

namespace NiceAdmin.Controllers
{
    [CheckAccess]
    public class OrderController : Controller
    {
        #region Configuration
        private IConfiguration configuration;
        private IEmailSender emailSender;
        public OrderController(IConfiguration configuration, IEmailSender emailSender)
        {
            this.configuration = configuration;
            this.emailSender = emailSender;
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
                Stream stream = CommonClass.ExportToExcel("PR_Order_SelectAll");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OrdersExport.xlsx");
            //}

        }
        #endregion

        #region Order List View(OrderView)
        public IActionResult OrderView()
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else
                return View(CommonClass.SelectData("PR_Order_SelectAll"));
        }
        #endregion

        #region Add Update OrderView(AddUpdateOrderView)
        public IActionResult AddUpdateOrderView(int? OrderID)
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else
            //{
                OrderModel modelOrder = new OrderModel();
                #region Drop Down Customer
                DataTable dataTable1 = CommonClass.SelectData("PR_Customer_DropDown");
                List<CustomerDropDownModel> customerList = new List<CustomerDropDownModel>();
                foreach (DataRow data in dataTable1.Rows)
                {
                    CustomerDropDownModel customerDropDownModel = new CustomerDropDownModel();
                    customerDropDownModel.CustomerID = Convert.ToInt32(data["CustomerID"]);
                    customerDropDownModel.CustomerName = data["CustomerName"].ToString();
                    customerList.Add(customerDropDownModel);
                }
                ViewBag.CustomerList = customerList;
                #endregion

                #region Drop Down User
                DataTable dataTable2 = CommonClass.SelectData("PR_User_DropDown");

                List<UserDropDownModel> userList = new List<UserDropDownModel>();
                foreach (DataRow data in dataTable2.Rows)
                {
                    UserDropDownModel userDropDownModel = new UserDropDownModel();
                    userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                    userDropDownModel.UserName = data["UserName"].ToString();
                    userList.Add(userDropDownModel);
                }
                ViewBag.UserList = userList;
                #endregion

                #region Fetch Data 
                if (OrderID != null && OrderID != 0)
                {

                    DataTable table = CommonClass.SelectByPk("PR_Order_SelectByPK", (int)OrderID, "OrderID");

                    /*if (table == null)
                    {
                        TempData["AlertMessage"] = "User Not Exists!";
                        return View(new UserModel());
                    }*/
                    try
                    {
                        modelOrder.OrderID= Convert.ToInt32(table.Rows[0]["OrderID"]);
                        modelOrder.UserID = Convert.ToInt32(table.Rows[0]["UserID"]);
                        modelOrder.CustomerID = Convert.ToInt32(table.Rows[0]["CustomerID"]);
                        modelOrder.OrderDate = Convert.ToDateTime(table.Rows[0]["OrderDate"]);
                        modelOrder.PaymentMode = table.Rows[0]["PaymentMode"].ToString();
                        modelOrder.TotalAmount = Convert.ToDecimal(table.Rows[0]["TotalAmount"]);
                        modelOrder.ShippingAddress = table.Rows[0]["ShippingAddress"].ToString();
                    
                    }
                    catch (Exception ex)
                    {
                        TempData["AlertMessage"] = "Order Not Exists!";
                        return RedirectToAction("OrderView");
                    }
                }
                    #endregion
                    return View(modelOrder);
            //}
        }
        #endregion

        #region AddUpdateOrderView(HttpPost)
        [HttpPost]
        public IActionResult AddUpdateOrderView(OrderModel modelOrder)
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else{
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");

                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (modelOrder.OrderID == 0)
                {
                    command.CommandText = "PR_Order_Insert";
                }
                else
                {
                    command.CommandText = "PR_Order_UpdateByPK";
                    command.Parameters.Add("@OrderID", SqlDbType.Int).Value = modelOrder.OrderID;
                    command.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = modelOrder.OrderDate;
                }
                
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = modelOrder.CustomerID;
                command.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = modelOrder.PaymentMode;
                command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = modelOrder.TotalAmount;
                command.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = modelOrder.ShippingAddress;
                command.Parameters.Add("@UserID", SqlDbType.VarChar).Value = modelOrder.UserID;
                

                if (Convert.ToBoolean(command.ExecuteNonQuery()))
                {
                    TempData["InsertUpdateMSG"] = modelOrder.OrderID == 0 ? "Inserted successfully!" : "Updated successfully!";
                    connection.Close();
                    return RedirectToAction("OrderView");
                }
            }
                return View(modelOrder);
            //}
        }
        #endregion

        #region Delete Order(DeleteOrder)
        public IActionResult DeleteOrder(int OrderID)
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else{
                try
                {
                
                    CommonClasses.CommonClass.DeleteRow("PR_Order_DeleteByPK", OrderID, "OrderID");
                    TempData["Message"] = "Delete successfully!";
                    TempData["SuccessMSG"] = true;

                }
                catch (Exception ex)
                {
                    TempData["ErrorMSG"] = ex.Message;
                }
                return RedirectToAction("OrderView");
            //}
        }
        #endregion

        #region Mail
        public async Task<IActionResult> Mail()
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else{
            //var receiver = BAL.CV.Email();
            var receiver = "";
            var subject = "Test";
            var message = "<html><body><b>Hello welcome Home</b></body></html>";

            
            Stream stream = CommonClass.ExportToExcel("PR_Order_SelectAll");

            try
            {
                // Create the attachment from the stream

                Attachment attachment = new Attachment(stream, "OrderExcelFile.xlsx", MediaTypeNames.Application.Octet);

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

                return RedirectToAction("OrderView");
            //}
        }
        #endregion

        #region Note
        /*public IActionResult ExportToExcel(string name)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "PR_"+name+"_SelectAll";
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(sqlDataReader);

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    // Load data into the worksheet
                    worksheet.Cells["A1"].LoadFromDataTable(dataTable, true, TableStyles.Medium9);

                    // Generate the Excel file
                    var stream = new MemoryStream(package.GetAsByteArray());

                    // Return the file as a downloadable response
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", name + "sExport.xlsx");
                }
            }
        }*/
        #endregion
    }
}
