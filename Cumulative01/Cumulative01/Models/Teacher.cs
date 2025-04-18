using System;
using System.ComponentModel.DataAnnotations;

namespace Cumulative01.Models
{
    /// <summary>
    /// Represents a teacher in the school database
    /// </summary>
    public class Teacher
    {
        /// <summary>
        /// Unique identifier for the teacher
        /// </summary>
        public int TeacherId { get; set; }

        /// <summary>
        /// First name of the teacher
        /// </summary>
        [Required(ErrorMessage = "First name is required")]
        public string TeacherFirstName { get; set; }

        /// <summary>
        /// Last name of the teacher
        /// </summary>
        [Required(ErrorMessage = "Last name is required")]
        public string TeacherLastName { get; set; }

        /// <summary>
        /// Employee number in format T followed by digits
        /// </summary>
        [Required(ErrorMessage = "Employee number is required")]
        [RegularExpression(@"^T\d+$", ErrorMessage = "Employee number must start with 'T' followed by digits")]
        public string EmployeeID { get; set; }

        /// <summary>
        /// Date the teacher was hired
        /// </summary>
        [Required(ErrorMessage = "Hire date is required")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        /// <summary>
        /// Teacher's salary
        /// </summary>
        [DataType(DataType.Currency)]
        public double Salary { get; set; }
    }
}