using Microsoft.AspNetCore.Mvc;
using NiceAdmin.Models;
using System.Data.SqlClient;
using System.Data;
/*using OfficeOpenXml.Table;
using OfficeOpenXml;
using System.IO;*/
using NiceAdmin.CommonClasses;
using System.Net.Mail;
using System.Net.Mime;
using NiceAdmin.BAL;
//using System.Net;
namespace NiceAdmin.Controllers
{
    
    public class UserController : Controller
    {
        public static string MaskPassword(string password)
        {
            if (password.Length <= 4)
            {
                return password; // If the password is too short, show it as it is.
            }

            string firstTwo = password.Substring(0, 2); // Get the first 2 characters
            string lastTwo = password.Substring(password.Length - 2); // Get the last 2 characters
            string masked = new string('*', password.Length - 4); // Mask the middle part with '*'

            return firstTwo + masked + lastTwo;
        }


        #region Configuration
        private IConfiguration configuration;
        private IEmailSender emailSender;
        public UserController(IConfiguration configuration,IEmailSender emailSender)
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
            //else{
                Stream stream = CommonClass.ExportToExcel("PR_User_SelectAll");

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UsersExport.xlsx");
            //}
            
        }
        #endregion


        /*using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "PR_User_SelectAll";
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
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OrdersExport.xlsx");
                }
            }*/


        #region User List View (UserView)

        [CheckAccess]
        public IActionResult UserView()
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else
                return View(CommonClass.SelectData("PR_User_SelectAll"));
        }

        #endregion

        [CheckAccess]
        #region Add Update UserView(AddUpdateUserView)
        public IActionResult AddUpdateUserView(int? UserId)
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else
            //{
                UserModel modelUser = new UserModel();

                #region Drop Down Role
                //DataTable dataTable1 = CommonClass.SelectData("PR_Role_DropDown");
                //List<RoleModel> roleList = new List<RoleModel>();
                //foreach (DataRow data in dataTable1.Rows)
                //{
                //    RoleModel roleModel = new RoleModel();
                //    roleModel.RoleID = Convert.ToInt32(data["RoleID"]);
                //    roleModel.Role = data["Role"].ToString();
                //    roleList.Add(roleModel);
                //}
                //ViewBag.RoleList = roleList;
                #endregion

                #region Fetch Data
                if (UserId != null && UserId != 0)
                {
             

                    DataTable table = CommonClass.SelectByPk("PR_User_SelectByPK", (int)UserId, "UserId");

                    try
                    {
                    foreach (DataRow row in table.Rows)
                    {
                        modelUser.UserId = Convert.ToInt32(table.Rows[0]["UserID"]);
                        modelUser.UserName = table.Rows[0]["UserName"].ToString();
                        modelUser.Password = table.Rows[0]["Password"].ToString();
                        modelUser.Address = table.Rows[0]["Address"].ToString();
                        modelUser.Email = table.Rows[0]["Email"].ToString();
                        modelUser.MobileNo = table.Rows[0]["MobileNo"].ToString();
                        modelUser.IsActive = Convert.ToBoolean(table.Rows[0]["IsActive"]);
                    }
                        //modelUser.RoleID = Convert.ToInt32(table.Rows[0]["RoleID"]);
                        //modelUser.PhotoPath = table.Rows[0]["PhotoPath"].ToString();
                    }
                    catch(Exception ex)
                    {
                        TempData["AlertMessage"] = "User Not Exists!";
                        return RedirectToAction("UserView");
                    }
                }
                #endregion

                return View(modelUser);
            //}
        }
        #endregion


        #region AddUpdateUserView(HttpPost)
        [HttpPost]
        public IActionResult AddUpdateUserView(UserModel modelUser)
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
                    if(modelUser.UserId == 0)
                    {
                        command.CommandText = "PR_User_Insert";
                    }
                    else
                    {
                        command.CommandText = "PR_User_UpdateByPK";
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = modelUser.UserId;
                    }
                    command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = modelUser.UserName;
                    command.Parameters.Add("@Email", SqlDbType.VarChar).Value = modelUser.Email;
                    command.Parameters.Add("@Password", SqlDbType.VarChar).Value = modelUser.Password;
                    command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = modelUser.MobileNo;
                    command.Parameters.Add("@Address", SqlDbType.VarChar).Value = modelUser.Address;
                    command.Parameters.Add("@IsActive", SqlDbType.VarChar).Value = modelUser.IsActive;


                    //if (modelUser.RoleID != 0)
                    //{
                    //    command.Parameters.Add("@RoleID", SqlDbType.Int).Value = modelUser.RoleID;
                    //}

                     
                    //if(modelUser.file != null)
                    //{
                    //    modelUser.PhotoPath = CommonClass.GetSavePic(modelUser.file);
                    //    command.Parameters.Add("@PhotoPath", SqlDbType.VarChar).Value = modelUser.PhotoPath;
                        
                    //}
                    //else if (modelUser.PhotoPath != null)
                    //{
                    //    modelUser.PhotoPath = modelUser.PhotoPath;
                    //}
                    

                    if (Convert.ToBoolean(command.ExecuteNonQuery()))
                    {
                        TempData["InsertUpdateMSG"] = modelUser.UserId == 0 ? "Inserted successfully!" : "Updated successfully!";
                        connection.Close();
                        //if (CV.UserID() == modelUser.UserId)
                        //{
                        //    if (modelUser.PhotoPath != null)
                        //    {
                        //        HttpContext.Session.SetString("PhotoPath", modelUser.PhotoPath.ToString() ?? "~/assets/img/profile-img.jpg");
                        //    }
                        //    else
                        //    {
                        //        HttpContext.Session.SetString("PhotoPath", "~/assets/img/profile-img.jpg");
                        //    }
                        //}
                        return RedirectToAction("UserView");
                    }
                }
                return View(modelUser);
            //}           
        }
        #endregion

        #region Delete User(DeleteUser)
        public IActionResult DeleteUser(int UserID)
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else{
                try
                {
                    CommonClasses.CommonClass.DeleteRow("PR_User_DeleteByPK", UserID, "UserID");
                    TempData["Message"] = "Delete successfully!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMSG"] = ex.Message;
                }
                return RedirectToAction("UserView");
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

            
                Stream stream = CommonClass.ExportToExcel("PR_User_SelectAll");

                try
                {
                    // Create the attachment from the stream

                    Attachment attachment = new Attachment(stream, "UserExcelFile.xlsx", MediaTypeNames.Application.Octet);
                    Attachment attachment2 = new Attachment(CommonClass.ExportToExcel("PR_User_SelectAll"), "UserExcelFile2.xlsx", MediaTypeNames.Application.Octet);
                    Attachment attachment3 = new Attachment(CommonClass.ExportToExcel("PR_User_SelectAll"), "UserExcelFile3.xlsx", MediaTypeNames.Application.Octet);
                
                    await emailSender.SendEmailAsync(receiver, subject, message, new List<Attachment> { attachment, attachment2, attachment3 });

                    /*await emailSender.SendEmailAsync(receiver, subject, message);*/
                    /*ViewBag.Message = "Email sent successfully!";*/
                    TempData["Message"] = "Email sent successfully!";
                }
                catch (Exception ex)
                {
                    // Log the error (you can log this to a file or database)
                    TempData["Error"] = $"Error sending email: {ex.Message}";
                }

                return RedirectToAction("UserView");
            //}
        }
        #endregion

        #region Notes
        /*public async Task<IActionResult> Mail()
        {
            var receiver = "anshpadalia2004@gmail.com";
            var subject = "Test";
            var message = "Hello welcome Home";


            await emailSender.SendEmailAsync(receiver, subject, message);

            return View("AddUserView");
        }*/
        #endregion

       
        public IActionResult UserLogin()
        {
            return View("Login");
        }

        [HttpPost]
      
        public IActionResult UserLogin(UserLoginModel userLoginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_Login";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userLoginModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userLoginModel.Password;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);


                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                            HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // No data returned, set an error message
                        TempData["ErrorMessage"] = "Invalid username or password.";
                        return View("Login" , userLoginModel);
                    }
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("UserLogin", "User");
        }
    }
}