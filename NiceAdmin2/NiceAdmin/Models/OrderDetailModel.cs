
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NiceAdmin.Models
{
    public class OrderDetailModel
    {

        [Key]
        public int OrderDetailID { get; set; }
        [Required]
        public int OrderID { get; set; }
        [Required]
        public int ProductID { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public int UserID { get; set; }

}
}
