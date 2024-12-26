using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace NiceAdmin.Models
{
    public class CityModel
    {
        public int? CityID { get; set; }

        [Required(ErrorMessage = "City Name is required.")]
        [DisplayName("City Name")]
        public string CityName { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [DisplayName("Country Name")]
        public int CountryID { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [DisplayName("State Name")]
        public int StateID { get; set; }

        [Required(ErrorMessage = "City Code is required.")]
        [DisplayName("City Code")]
        [MaxLength(10, ErrorMessage = "City Code cannot exceed 10 characters.")]
        public string CityCode { get; set; }
    }

    public class CountryDropdownModel
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
    }
    public class StateDropdownModel
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
    }
}
