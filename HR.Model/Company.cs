using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Model
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }
    }
}
