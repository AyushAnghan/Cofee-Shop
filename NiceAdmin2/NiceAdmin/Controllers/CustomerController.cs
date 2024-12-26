using Microsoft.AspNetCore.Mvc;
using NiceAdmin.CommonClasses;
using NiceAdmin.Models;
using System.Data;
using System.Data.SqlClient;
using NiceAdmin.CommonClasses;
using Microsoft.AspNetCore.Routing;
using System.Net.Mail;
using System.Net.Mime;
using NiceAdmin.BAL;

namespace NiceAdmin.Controllers
{
    [CheckAccess]
    public class CustomerController : Controller
    {
        #region Configuration
        private IConfiguration configuration;
        private IEmailSender emailSender;

        public CustomerController(IConfiguration configuration, IEmailSender emailSender)
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
                Stream stream = CommonClass.ExportToExcel("PR_Customer_SelectAll");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CustomersExport.xlsx");

            //}        
        }
        #endregion

        #region Customer List View (CustomerView)
        public IActionResult CustomerView()
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else{
                return View(CommonClass.SelectData("PR_Customer_SelectAll"));
            //}
        }
        #endregion

        #region Add Update CustomerView(AddUpdateCustomerView)
        public IActionResult AddUpdateCustomerView(int? CustomerID)
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else
            //{
                CustomerModel modelCustomer = new CustomerModel();
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

                #region Fetch Data
            
            if (CustomerID != null && CustomerID != 0)
            {

                DataTable table = CommonClass.SelectByPk("PR_Customer_SelectByPK", (int)CustomerID, "CustomerID");

                /*if (table == null)
                {
                    TempData["AlertMessage"] = "User Not Exists!";
                    return View(new UserModel());
                }*/
                try
                {
                    modelCustomer.CustomerID = Convert.ToInt32(table.Rows[0]["CustomerID"]);
                    modelCustomer.UserID = Convert.ToInt32(table.Rows[0]["UserID"]);
                    modelCustomer.CustomerName = table.Rows[0]["CustomerName"].ToString();
                    modelCustomer.Email = table.Rows[0]["Email"].ToString();
                    modelCustomer.HomeAddress = table.Rows[0]["HomeAddress"].ToString();
                    modelCustomer.Email = table.Rows[0]["Email"].ToString();
                    modelCustomer.MobileNo = table.Rows[0]["MobileNo"].ToString();
                    modelCustomer.GST_NO = table.Rows[0]["GST_NO"].ToString();
                    modelCustomer.CityName = table.Rows[0]["CityName"].ToString();
                    modelCustomer.PinCode = table.Rows[0]["PinCode"].ToString();
                    modelCustomer.NetAmount = Convert.ToDecimal(table.Rows[0]["PinCode"]);

                }
                catch (Exception ex)
                {
                    TempData["AlertMessage"] = "User Not Exists!";
                    return RedirectToAction("CustomerView");
                }
            }
                #endregion
                return View(modelCustomer);
            //}
        }
        #endregion

        #region AddUpdateCustomerView(HttpPost)
        [HttpPost]
        public IActionResult AddUpdateCustomerView(CustomerModel modelCustomer)
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
                    if (modelCustomer.CustomerID == 0)
                    {
                        command.CommandText = "PR_Customer_Insert";
                    }
                    else
                    {
                        command.CommandText = "PR_Customer_UpdateByPK";
                        command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = modelCustomer.CustomerID;
                    }
                    command.Parameters.Add("@CustomerName", SqlDbType.VarChar).Value = modelCustomer.CustomerName;
                    command.Parameters.Add("@HomeAddress", SqlDbType.VarChar).Value = modelCustomer.HomeAddress;
                    command.Parameters.Add("@Email", SqlDbType.VarChar).Value = modelCustomer.Email;
                    command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = modelCustomer.MobileNo;
                    command.Parameters.Add("@GST_NO", SqlDbType.VarChar).Value = modelCustomer.GST_NO;
                    command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = modelCustomer.CityName;
                    command.Parameters.Add("@PinCode", SqlDbType.VarChar).Value = modelCustomer.PinCode;
                    command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = modelCustomer.NetAmount;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelCustomer.UserID;

                    if (Convert.ToBoolean(command.ExecuteNonQuery()))
                    {
                        TempData["InsertUpdateMSG"] = modelCustomer.CustomerID == 0 ? "Inserted successfully!" : "Updated successfully!";
                        connection.Close();
                        return RedirectToAction("CustomerView");
                    }
                }
                return View(modelCustomer);
            //}
        }
        #endregion

        #region Delete Customer(DeleteCustomer)
        public IActionResult DeleteCustomer(int CustomerID)
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else
            //    {
                try
                {
                
                    CommonClasses.CommonClass.DeleteRow("PR_Customer_DeleteByPK", CustomerID, "CustomerID");
                    TempData["Message"] = "Delete successfully!";
                }
                catch (Exception ex) {
                    TempData["ErrorMSG"] = ex.Message;
                }

                    return RedirectToAction("CustomerView");
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
            //else
            //{
                //var receiver = BAL.CV.Email();
                var receiver = "";
                var subject = "Test";
                var message = "<html><body><b>Hello welcome Home</b></body></html>";

            
                Stream stream = CommonClass.ExportToExcel("PR_Customer_SelectAll");

                try
                {
                    // Create the attachment from the stream

                    Attachment attachment = new Attachment(stream, "CustomerExcelFile.xlsx", MediaTypeNames.Application.Octet);
               
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

                    return RedirectToAction("CustomerView");
            //}
        }
        #endregion

    }
}