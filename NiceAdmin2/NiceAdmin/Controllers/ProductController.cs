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
    public class ProductController : Controller
    {
        #region Configuration
        private IConfiguration configuration;
        private IEmailSender emailSender;
        public ProductController (IConfiguration configuration, IEmailSender emailSender)
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
                Stream stream = CommonClass.ExportToExcel("PR_Product_SelectAll");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProductsExport.xlsx");
            //}
        }
        #endregion

        #region Product List View(ProductView)
        public IActionResult ProductView()
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else
                return View(CommonClass.SelectData("PR_Product_SelectAll"));
        }
        #endregion

        #region Delete Product Row(DeleteProduct)
        public IActionResult DeleteProduct(int ProductID)

        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else{
                try
                {
                    CommonClasses.CommonClass.DeleteRow("PR_Product_DeleteByPK", ProductID, "ProductID");
                    TempData["Message"] = "Delete successfully!";

                }
                catch (Exception ex) {
                    TempData["ErrorMSG"] = ex.Message;
                }
                return RedirectToAction("ProductView");
            //}
        }
        #endregion

        #region Add Update ProductView(AddUpdateProductView)
        public IActionResult AddUpdateProductView(int? ProductID)

        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else{
                ProductModel modelProduct = new ProductModel();
                #region Drop Down User
                DataTable table1 = CommonClass.SelectData("PR_User_DropDown");  
            
                List<UserDropDownModel> userList = new List<UserDropDownModel>();
                foreach (DataRow data in table1.Rows) {
                    UserDropDownModel userDropDownModel = new UserDropDownModel();
                    userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                    userDropDownModel.UserName = data["UserName"].ToString();
                    userList.Add(userDropDownModel);
                }
                ViewBag.UserList = userList;
                #endregion

                #region Fetch Data

                if (ProductID != null && ProductID != 0)
                {

                    DataTable table = CommonClass.SelectByPk("PR_Product_SelectByPK", (int)ProductID, "ProductID");

                    try
                    {
                        modelProduct.ProductID = Convert.ToInt32(table.Rows[0]["ProductID"]);
                        modelProduct.UserID = Convert.ToInt32(table.Rows[0]["UserID"]);
                        modelProduct.ProductName = table.Rows[0]["ProductName"].ToString();
                        modelProduct.ProductPrice = Convert.ToDecimal(table.Rows[0]["ProductPrice"]);
                        modelProduct.ProductCode = table.Rows[0]["ProductCode"].ToString();
                        modelProduct.Description = table.Rows[0]["Description"].ToString();
                    
                    }
                    catch (Exception ex)
                    {
                        TempData["AlertMessage"] = "Product Not Exists!";
                        return RedirectToAction("ProductView");
                    }
                }
                #endregion
                return View(modelProduct);
            //}
        }
        #endregion

        #region AddUpdateProductView(HttpPost)
        [HttpPost]
        public IActionResult AddUpdateProductView(ProductModel modelProduct)
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
                    if (modelProduct.ProductID == 0)
                    {
                        command.CommandText = "PR_Product_Insert";
                    }
                    else
                    {
                        command.CommandText = "PR_Product_UpdateByPK";
                        command.Parameters.Add("@ProductID", SqlDbType.Int).Value = modelProduct.ProductID;
                    }
                    command.Parameters.Add("@ProductName", SqlDbType.VarChar).Value = modelProduct.ProductName;
                    command.Parameters.Add("@ProductPrice", SqlDbType.Decimal).Value = modelProduct.ProductPrice;
                    command.Parameters.Add("@ProductCode", SqlDbType.VarChar).Value = modelProduct.ProductCode;
                    command.Parameters.Add("@Description", SqlDbType.VarChar).Value = modelProduct.Description;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelProduct.UserID;

                    if (Convert.ToBoolean(command.ExecuteNonQuery()))
                    {
                        TempData["InsertUpdateMSG"] = modelProduct.ProductID == 0 ? "Inserted successfully!" : "Updated successfully!";
                        connection.Close();
                        return RedirectToAction("ProductView");
                    }
                }
                return View(modelProduct);
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
            //    else{
            //var receiver = BAL.CV.Email();
            var receiver = "";
            var subject = "Test";
                var message = "<html><body><b>Hello welcome Home</b></body></html>";

            
                Stream stream = CommonClass.ExportToExcel("PR_Product_SelectAll");

                try
                {
                    // Create the attachment from the stream

                    Attachment attachment = new Attachment(stream, "ProductExcelFile.xlsx", MediaTypeNames.Application.Octet);

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

                return RedirectToAction("ProductView");
            //}
        }
        #endregion

    }
}
