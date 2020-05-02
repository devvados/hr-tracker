using System.ComponentModel.DataAnnotations;

namespace HR.Model
{
    public class Candidate
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        public string Patronymic { get; set; }

        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
