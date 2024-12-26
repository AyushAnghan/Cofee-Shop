using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using NiceAdmin.Models;
using NiceAdmin.CommonClasses;

namespace NiceAdmin.Controllers
{
    public class CityController : Controller
    {
        private readonly IConfiguration _configuration;

        public CityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult CityView()
        {
            string connectionstr = this._configuration.GetConnectionString("ConnectionString");
            //PrePare a connection
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(connectionstr);
            conn.Open();

            //Prepare a Command
            SqlCommand objCmd = conn.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "PR_LOC_City_SelectAll";

                SqlDataReader objSDR = objCmd.ExecuteReader();
            dt.Load(objSDR);
            conn.Close();
            return View("CityView", dt);

        }

        public IActionResult DeleteCity(int cityId)
        {
            string connectionstr = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionstr))
            {
                conn.Open();
                using (SqlCommand sqlCommand = conn.CreateCommand())
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_LOC_City_Delete";
                    sqlCommand.Parameters.AddWithValue("@CityID", cityId);
                    sqlCommand.ExecuteNonQuery();
                }
            }
            return RedirectToAction("CityView");
        }

        public IActionResult AddUpdateCityView(int? cityId)
        {
            
            ViewBag.CityList = countyDropDown();

            if (cityId == null) { 
                return View();
            }

            string connectionstr = this._configuration.GetConnectionString("ConnectionString");
            //PrePare a connection
            SqlConnection conn = new SqlConnection(connectionstr);
            conn.Open();
            //Prepare a Command
            SqlCommand objCmd = conn.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "PR_LOC_City_SelectByPK";
            //objCmd.Parameters.Add("@CountryId",sqlDbType)
            objCmd.Parameters.Add("@CityID", SqlDbType.Int).Value = cityId;
            SqlDataReader reader = objCmd.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader);
            
                CityModel city = new CityModel();

            foreach (DataRow data in dataTable1.Rows)
            {
                city.CityID = Convert.ToInt32(data["CityID"]);
                city.CityName = (data["CityName"]).ToString();
                city.StateID = Convert.ToInt32(data["StateID"]);
                city.CountryID = Convert.ToInt32(data["CountryID"]);
                city.CityCode = (data["CityCode"].ToString());
            }


            return View(city);
        }

        [HttpPost]
        public IActionResult AddUpdateCityView(CityModel city)
         {
            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");

                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (city.CityID==null||city.CityID == 0)
                {
                    command.CommandText = "PR_LOC_City_Insert";
                   
                }
                else
                {
                    command.CommandText = "PR_LOC_City_Update";
                    command.Parameters.Add("@CityID", SqlDbType.Int).Value = city.CityID;
     

                }
                command.Parameters.Add("@CityName", SqlDbType.NVarChar).Value = city.CityName;
                command.Parameters.Add("@CityCode", SqlDbType.NVarChar).Value = city.CityCode;
                command.Parameters.Add("@StateID", SqlDbType.Int).Value = city.StateID;
                command.Parameters.Add("@CountryID ", SqlDbType.Int).Value = city.CountryID;

                //command.Parameters.Add("@@CityID", SqlDbType.Int).Value = city.CityID;
                //command.Parameters.Add("@CityName", SqlDbType.NVarChar).Value = city.CityName;
                //command.Parameters.Add("@CityCode", SqlDbType.NVarChar).Value = city.CityCode;
                //command.Parameters.Add("@StateID", SqlDbType.Int).Value = city.StateID;
                //command.Parameters.Add("@CountryID ", SqlDbType.Int).Value = city.CountryID;


                if (Convert.ToBoolean(command.ExecuteNonQuery()))
                {
                    
                    connection.Close();
                    return RedirectToAction("CityView");
                }
            }
            ViewBag.CityList = countyDropDown();
            return View(city);
            //}
        }

        [Route("/statedropdown/{countryId}")]
        public IActionResult StateDropdown(int countryId)
        {
            string connectionstr = this._configuration.GetConnectionString("ConnectionString");
            //PrePare a connection
            SqlConnection conn = new SqlConnection(connectionstr);
            conn.Open();
            //Prepare a Command
            SqlCommand objCmd = conn.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "Pr_dropDownState";
            //objCmd.Parameters.Add("@CountryId",sqlDbType)
            objCmd.Parameters.Add("@CountryId", SqlDbType.Int).Value = countryId;
            SqlDataReader reader = objCmd.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader);
            List<StateDropdownModel> countryList = new List<StateDropdownModel>();

            foreach (DataRow data in dataTable1.Rows)
            {
                StateDropdownModel cityDropdown = new StateDropdownModel();
                cityDropdown.StateID = Convert.ToInt32(data["StateID"]);
                cityDropdown.StateName = data["StateName"].ToString();

                countryList.Add(cityDropdown);
            }
            ////ViewBag.CityList = countryList;

            return new JsonResult(countryList);
        }


        [NonAction]
        public List<CountryDropdownModel> countyDropDown()
        {
            string connectionstr = this._configuration.GetConnectionString("ConnectionString");
            //PrePare a connection
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(connectionstr);
            conn.Open();
            //Prepare a Command
            SqlCommand objCmd = conn.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "Pr_dropDownCountry";
            SqlDataReader reader = objCmd.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader);
            List<CountryDropdownModel> countryList = new List<CountryDropdownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                CountryDropdownModel cityDropdown = new CountryDropdownModel();
                cityDropdown.CountryID = Convert.ToInt32(data["CountryID"]);
                cityDropdown.CountryName = data["CountryName"].ToString();
                countryList.Add(cityDropdown);
            }
            return countryList;
        }


    }
}
