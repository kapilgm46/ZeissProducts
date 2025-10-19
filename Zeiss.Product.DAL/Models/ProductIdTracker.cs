using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeiss.Product.DAL.Models
{
    public class ProductIdTracker
    {
        [Key]
        public int Id { get; set; }
        public int LastId { get; set; }
    }
}
