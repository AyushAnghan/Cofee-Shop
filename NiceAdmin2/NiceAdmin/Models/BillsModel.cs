using System.ComponentModel.DataAnnotations;

namespace NiceAdmin.Models
{
    public class BillsModel
    {
        [Key]
        public int BillID { get; set; }
        [Required]
        public string BillNumber { get; set; }

        public DateTime BillDate { get; set; }
        [Required]
        public int OrderID { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public decimal Discount { get; set; }
        [Required]
        public decimal NetAmount { get; set; }
        [Required]
        public int UserID { get; set; } 
    }
}
