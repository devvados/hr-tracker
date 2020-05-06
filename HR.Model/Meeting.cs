using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Model
{
    public class Meeting
    {
        public Meeting() { }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string Status { get; set; }

        public int CandidateId { get; set; }

        public Candidate Candidate { get; set; }
    }
}
