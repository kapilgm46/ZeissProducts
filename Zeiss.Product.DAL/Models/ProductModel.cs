using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeiss.Product.DAL.Models
{
    public class ProductModel
    {
        [Key]
        [Required]
        [Range(100000,999999)]
        public int ProductId {  get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string Description { get; set; }
    }
}
