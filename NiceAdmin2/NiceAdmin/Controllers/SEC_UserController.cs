using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NiceAdmin.BAL;
using NiceAdmin.CommonClasses;
using NiceAdmin.Models;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace NiceAdmin.Controllers
{
    public class SEC_UserController : Controller
    {
        #region Configuration
        private IConfiguration configuration;
        private IEmailSender emailSender;
        public SEC_UserController(IConfiguration configuration, IEmailSender emailSender)
        {
            this.configuration = configuration;
            this.emailSender = emailSender;
        }
        #endregion

        #region Login(View)
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region Login(HttpPost)
        [HttpPost]
        public IActionResult Login(UserModel modelUser)
        {
            /*string connectionString = this.configuration.GetConnectionString("ConnectionString");*/
            string error = null;

            if (modelUser.UserName == null) {
                error += "UserName is required!";
            }
            if(modelUser.Password == null)
            {
                error += "\n Password is reqired!";
            }

            if (error != null) {
                TempData["Error"] = error;
                return RedirectToAction("Index");
            }
            else
            {
                DataTable dt = CommonClasses.CommonClass.dbo_PR_SEC_User_SelectByUserNamePassword(modelUser.UserName, modelUser.Password);
                if (dt.Rows.Count > 0) {
                    foreach (DataRow dr in dt.Rows)
                    {
                        HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                        HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                        HttpContext.Session.SetString("Email", dr["Email"].ToString());
                        HttpContext.Session.SetString("MobileNo", dr["MobileNo"].ToString());
                        HttpContext.Session.SetString("Address", dr["Address"].ToString());
                        HttpContext.Session.SetString("IsActive", dr["IsActive"].ToString());
                        HttpContext.Session.SetString("Password", dr["Password"].ToString());
                        HttpContext.Session.SetString("PhotoPath", dr["PhotoPath"].ToString());
                        HttpContext.Session.SetString("Role", dr["Role"].ToString());
                        break;
                    }
                }
                else
                {
                    TempData["Error"] = "User name or Password is invalid!";
                    return RedirectToAction("Index");
                }

                if(HttpContext.Session.GetString("UserName") != null && HttpContext.Session.GetString("Password") != null)
                {
                    return RedirectToAction("Index","Home");
                }
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Logout(HttpPost)
        [HttpPost]
        public IActionResult Logout()
        {
            // Clear the session data
            HttpContext.Session.Clear();

            // Optionally, clear authentication cookies if used
            // await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); 
            // (use the above if you are using cookie-based authentication)

            // Redirect to the login page after logging out
            return RedirectToAction("Index", "SEC_User"); 
        }
        #endregion


        public IActionResult Register()
        {
            UserModel modelUser = new UserModel();
            return View(modelUser);
        }
        #region AddUpdateUserView(HttpPost)
        [HttpPost]
        public IActionResult Register(UserModel modelUser)
        {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");

                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    if (modelUser.UserId == 0)
                    {
                        command.CommandText = "PR_User_Insert";
                    }
                    
                    command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = modelUser.UserName;
                    command.Parameters.Add("@Email", SqlDbType.VarChar).Value = modelUser.Email;
                    command.Parameters.Add("@Password", SqlDbType.VarChar).Value = modelUser.Password;
                    command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = modelUser.MobileNo;
                    command.Parameters.Add("@Address", SqlDbType.VarChar).Value = modelUser.Address;
                    command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = true;


                    

                    //if (modelUser.file != null)
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
                       
                        return RedirectToAction("Index");
                    }
                }
                return View(modelUser);
            
        }
        #endregion
    }
}
