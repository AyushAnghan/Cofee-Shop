using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NiceAdmin.Models
{
    public class OrderModel
    {
        [Key]
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        [Required]
        public int CustomerID { get; set; }
        [Required]
        public string PaymentMode { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public string ShippingAddress { get; set; }
        [Required]
        public int UserID { get; set; }

    }

    public class OrderDropDownModel
    {
        public int OrderID { get; set; }
    }

}
