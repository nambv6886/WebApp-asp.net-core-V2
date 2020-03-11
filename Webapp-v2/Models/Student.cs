using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Webapp_v2.Models
{
    public class Student
    {
        public int ID { get; set; }

        // max length is 50 with no space
        [Required]
        [StringLength(50)]
        [Display(Name ="Last Name")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Fisrt Name")]
        [StringLength(50)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        public string FirstName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name ="Enrollment Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true )]
        public DateTime EnrollmentDate { get; set; }

        [Display(Name ="Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }

        public List<Enrollment> Enrollments { get; set; }

    }
}
