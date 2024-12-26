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
    public class OrderDetailController : Controller
    {
        #region Configuration
        private IConfiguration configuration;
        private IEmailSender emailSender;
        public OrderDetailController(IConfiguration configuration, IEmailSender emailSender)
        {
            this.configuration = configuration;
            this.emailSender = emailSender;
        }
        #endregion

        #region ExportToExcel
        public IActionResult ExportToExcel()
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else{
            Stream stream = CommonClass.ExportToExcel("PR_OrderDetail_SelectAll");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OrderDetailsExport.xlsx");
            //}

        }
        #endregion

        #region OrderDetail View(OrderDetailView)
        public IActionResult OrderDetailView()
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else{
                return View(CommonClass.SelectData("PR_OrderDetail_SelectAll"));
            //}
        }
        #endregion

        #region Add Update Order Detail View(AddOrderDetailView)
        public IActionResult AddUpdateOrderDetailView(int? OrderDetailID)
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else{
                OrderDetailModel modelOrderDetail = new OrderDetailModel();
                #region Drop Down Product
                DataTable dataTable1 = CommonClass.SelectData("PR_Product_DropDown");

                List<ProductDropDownModel> productList = new List<ProductDropDownModel>();
                foreach (DataRow data in dataTable1.Rows) { 
                    ProductDropDownModel productDropDownModel = new ProductDropDownModel();
                    productDropDownModel.ProductID = Convert.ToInt32(data["ProductID"]);
                    productDropDownModel.ProductName = data["ProductName"].ToString();
                    productList.Add(productDropDownModel);
                }
                ViewBag.ProductList = productList;
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
            
                #region Fetch Data

                if (OrderDetailID != null && OrderDetailID != 0)
                {

                    DataTable table = CommonClass.SelectByPk("PR_OrderDetail_SelectByPK", (int)OrderDetailID, "OrderDetailID");

                    /*if (table == null)
                    {
                        TempData["AlertMessage"] = "User Not Exists!";
                        return View(new UserModel());
                    }*/
                    try
                    {
                        modelOrderDetail.OrderDetailID = Convert.ToInt32(table.Rows[0]["OrderDetailID"]);
                        modelOrderDetail.UserID = Convert.ToInt32(table.Rows[0]["UserID"]);
                        modelOrderDetail.OrderID = Convert.ToInt32(table.Rows[0]["OrderID"]);
                        modelOrderDetail.ProductID = Convert.ToInt32(table.Rows[0]["ProductID"]);
                        modelOrderDetail.Quantity = Convert.ToInt32(table.Rows[0]["Quantity"]);
                        modelOrderDetail.TotalAmount = Convert.ToDecimal(table.Rows[0]["TotalAmount"]);
                        modelOrderDetail.Amount = Convert.ToDecimal(table.Rows[0]["Amount"]);

                    }
                    catch (Exception ex)
                    {
                        TempData["AlertMessage"] = "Detail of Order Not Exists!";
                        return RedirectToAction("OrderDetailView");
                    }
                }
                #endregion

                return View(modelOrderDetail);
            //}
        }
        #endregion

        #region AddUpdateOrderDetailView(HttpPost)
        [HttpPost]
        public IActionResult AddUpdateOrderDetailView(OrderDetailModel modelOrderDetailView)
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
                if (modelOrderDetailView.OrderDetailID == 0)
                {
                    command.CommandText = "PR_OrderDetail_Insert";
                }
                else
                {
                    command.CommandText = "PR_OrderDetail_UpdateByPK";
                    command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = modelOrderDetailView.OrderDetailID;
                }

                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = modelOrderDetailView.OrderID;
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = modelOrderDetailView.ProductID;
                command.Parameters.Add("@Quantity", SqlDbType.Int).Value = modelOrderDetailView.Quantity;
                command.Parameters.Add("@Amount", SqlDbType.Decimal).Value = modelOrderDetailView.Amount;
                command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = modelOrderDetailView.TotalAmount;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelOrderDetailView.UserID;

                if (Convert.ToBoolean(command.ExecuteNonQuery()))
                {
                    TempData["InsertUpdateMSG"] = modelOrderDetailView.OrderDetailID == 0 ? "Inserted successfully!" : "Updated successfully!";
                    connection.Close();
                    return RedirectToAction("OrderDetailView");
                }
            }
                return View(modelOrderDetailView);
            //}         
        }
        #endregion

        #region Delete OrderDetail
        public IActionResult DeleteOrderDetail (int OrderDetailID) {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else{
                try
                {
                
                    CommonClasses.CommonClass.DeleteRow("PR_OrderDetail_DeleteByPK", OrderDetailID, "OrderDetailID");
                    TempData["Message"] = "Delete successfully!";
                }
                catch (Exception ex) {
                    TempData["ErrorMSG"] = ex.Message;
                }

                return RedirectToAction("OrderDetailView");
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

            
                Stream stream = CommonClass.ExportToExcel("PR_OrderDetail_SelectAll");

                try
                {
                    // Create the attachment from the stream

                    Attachment attachment = new Attachment(stream, "OrderDetailExcelFile.xlsx", MediaTypeNames.Application.Octet);

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

                return RedirectToAction("OrderDetailView");
            //}
        }
        #endregion
    }
}
