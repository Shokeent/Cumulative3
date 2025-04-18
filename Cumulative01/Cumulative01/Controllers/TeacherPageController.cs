using Cumulative01.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Cumulative01.Controllers
{
    public class TeacherPageController : Controller
    {
        private readonly TeacherAPIController _api;
        private readonly SchoolDbContext _context;

        public TeacherPageController(TeacherAPIController api, SchoolDbContext context)
        {
            _api = api;
            _context = context;
        }

        public IActionResult List()
        {
            List<Teacher> Teach = _api.ListTeacherNames();
            return View(Teach);
        }

        public IActionResult Show(int Id)
        {
            Teacher teach1 = _api.FindTeacher(Id);
            return View(teach1);
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult New(Teacher teacher)
        {
            // Input validation
            if (string.IsNullOrEmpty(teacher.TeacherFirstName) || string.IsNullOrEmpty(teacher.TeacherLastName))
            {
                ModelState.AddModelError("", "Teacher name cannot be empty");
                return View(teacher);
            }

            if (teacher.HireDate > DateTime.Now)
            {
                ModelState.AddModelError("", "Hire date cannot be in the future");
                return View(teacher);
            }

            if (string.IsNullOrEmpty(teacher.EmployeeID) || !teacher.EmployeeID.StartsWith("T") 
                || !teacher.EmployeeID.Substring(1).All(char.IsDigit))
            {
                ModelState.AddModelError("", "Employee number must start with 'T' followed by digits");
                return View(teacher);
            }

            // Use the API to add the teacher
            var result = _api.AddTeacher(teacher) as ObjectResult;
            
            // Check if the operation was successful
            if (result != null && result.StatusCode == 200) 
            {
                TempData["Message"] = "Teacher added successfully!";
                return RedirectToAction("List");
            }
            else
            {
                // Extract error message if available
                if (result != null && result.Value != null)
                {
                    // Convert to string and add as model error
                    var resultString = result.Value.ToString();
                    ModelState.AddModelError("", resultString ?? "An error occurred while adding the teacher");
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while adding the teacher");
                }
                return View(teacher);
            }
        }

        public IActionResult DeleteConfirm(int id)
        {
            Teacher teacher = _api.FindTeacher(id);
            
            // Checking if the teacher exists
            if (teacher == null || teacher.TeacherId == 0)
            {
                TempData["ErrorMessage"] = $"Teacher with ID {id} not found";
                return RedirectToAction("List");
            }
            
            return View(teacher);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var result = _api.DeleteTeacher(id) as ObjectResult;
            
            // Check if the operation was successful
            if (result != null && result.StatusCode == 200) 
            {
                TempData["Message"] = "Teacher deleted successfully!";
            }
            else
            {
                // Extract error message if available
                if (result != null && result.Value != null)
                {
                    // Convert to string and add as temp data
                    var resultString = result.Value.ToString();
                    TempData["ErrorMessage"] = resultString ?? "An error occurred while deleting the teacher";
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while deleting the teacher";
                }
            }
            
            return RedirectToAction("List");
        }
    }
}