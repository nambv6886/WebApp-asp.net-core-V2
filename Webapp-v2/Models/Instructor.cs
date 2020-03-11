using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webapp_v2.Models
{
    public class Instructor
    {
        public int ID { get; set; }

        [Required]
        [Display(Name ="Last Name")]
        [Column("LastName")]
        [StringLength(50)]
        public string LastName { get; set; }
        
        [Required]
        [Display(Name ="Fisrt Name")]
        [Column("FirstName")]
        [StringLength(50)]
        public string FirstName { get; set; }


        [Required]
        [Display(Name ="Hire Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true )]
        public DateTime HireDate { get; set; }

        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }

        public OfficeAssignment OfficeAssignment { get; set; }

        //public List<CourseAssignment> CourseAssignments { get; set; }

        public ICollection<CourseAssignment> CourseAssignments { get; set; }

    }
}
