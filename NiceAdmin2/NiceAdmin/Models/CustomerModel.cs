using System.ComponentModel.DataAnnotations;

namespace NiceAdmin.Models
{
    public class CustomerModel
    {
        [Key]
        public int CustomerID { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string HomeAddress { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string MobileNo { get; set; }
        [Required]
        public string GST_NO { get; set; }
        [Required]
        public string CityName { get; set; }
        [Required]
        public string PinCode { get; set; }
        [Required]
        public decimal NetAmount { get; set; }
        [Required]
        public int UserID { get; set; }
    }

    public class CustomerDropDownModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}
