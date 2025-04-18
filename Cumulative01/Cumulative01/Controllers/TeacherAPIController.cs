using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative01.Models;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Cumulative01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;


        //constructor assign connection to the database and private variable.
        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        //setting up the API method to receveive a GET request to the endpoint /api/Teacher
        [HttpGet(template:"Teacher")]

        //calling database and returning a list of teachers.

        /// <summary>
        /// Returns a list of Teachers in the system
        /// </summary>
        /// 
        /// <example>
        /// GET api/Teacher -> [{"TeacherId":1,"TeacherFirstName":"Katrina","TeacherLastName":"Bernadett",...},..]
        /// </example>
        /// 
        /// <returns>
        /// A list of Teacher objects containing ID, Name, EmployeeID, JoinedOn, and Income
        /// </returns>
        public List<Teacher> ListTeacherNames()
        {
            //list of a list type of Teacher which hold the teachers instances in as objects in the list.
            List<Teacher> teachers = new List<Teacher>();
            MySqlConnection Connection = _context.GetConnection();
            Connection.Open();
            Debug.WriteLine("DbConnected");
            string SQLQuery = "SELECT * FROM teachers";
            MySqlCommand Command = Connection.CreateCommand();
            Command.CommandText = SQLQuery;
            MySqlDataReader DataReader = Command.ExecuteReader();
              while (DataReader.Read())
            { 
                int TeacherId = Convert.ToInt32(DataReader["teacherid"]);
                string TeacherFName = DataReader["teacherfname"].ToString();
                string TeacherLName = DataReader["teacherlname"].ToString();
                string EmployeeID = DataReader["employeenumber"].ToString();
                DateTime HireDate = Convert.ToDateTime(DataReader["hiredate"]);
                double Salary = Convert.ToDouble(DataReader["salary"]);

                Teacher newTeacher = new Teacher();
                newTeacher.TeacherId = TeacherId;
                newTeacher.TeacherFirstName = TeacherFName;
                newTeacher.TeacherLastName = TeacherLName;
                newTeacher.EmployeeID = EmployeeID;
                newTeacher.HireDate = HireDate;
                newTeacher.Salary = Salary;
                teachers.Add(newTeacher);
            }
            Connection.Close();
            return teachers;
        }

        /// <summary>
        /// Finding a teacher by their ID
        /// </summary>
        /// 
        /// <example>
        /// GET api/FindTeacher/1 -> {"TeacherId":1,"TeacherFirstName":"Katrina","TeacherLastName":"Bernadett",...}
        /// </example>
        /// 
        /// <param name="id">The ID of the teacher</param>
        /// 
        /// <returns>
        /// A Teacher object containing ID, Name, EmployeeID, HireDate, and Salary
        /// </returns>
        //APIto recieve a GET request to the endpoint /api/FindTeacher/{id}
        [HttpGet]
        [Route(template: "FindTeacher/{id}")]

        //calling the database and returning a teacher object by teacher's ID.
        public Teacher FindTeacher(int id)
        {
            Teacher teacher = new Teacher();
            MySqlConnection Connection = _context.GetConnection();
            Connection.Open();

            string SQL = "Select * FROM teachers Where Teacherid = "+id.ToString();

            MySqlCommand Command = Connection.CreateCommand();
            Command.CommandText = SQL;
            
            MySqlDataReader DataReader = Command.ExecuteReader();

            while (DataReader.Read())
            {
                int TeacherId = Convert.ToInt32(DataReader["teacherid"]);
                string TeacherFName = DataReader["teacherfname"].ToString();
                string TeacherLName = DataReader["teacherlname"].ToString();
                string EmployeeID = DataReader["employeenumber"].ToString();
                DateTime HireDate = Convert.ToDateTime(DataReader["hiredate"]);
                double Salary = Convert.ToDouble(DataReader["salary"]);

                teacher.TeacherId = TeacherId;
                teacher.TeacherFirstName = TeacherFName;
                teacher.TeacherLastName = TeacherLName;
                teacher.EmployeeID = EmployeeID;
                teacher.HireDate = HireDate;
                teacher.Salary = Salary;
            }

            Connection.Close(); 

            return teacher;
        }

        /// <summary>
        /// Adds a new teacher to the database
        /// </summary>
        /// <example>
        /// POST api/TeacherAPI/AddTeacher
        /// {
        ///     "TeacherFirstName": "Katrina",
        ///     "TeacherLastName": "Bernadett",
        ///     "EmployeeID": "T123",
        ///     "HireDate": "2023-01-15",
        ///     "Salary": 75000
        /// }
        /// </example>
        /// <returns>
        /// Success message with created teacher data or error message
        /// </returns>
        [HttpPost]
        [Route("AddTeacher")]
        public ActionResult AddTeacher([FromBody] Teacher teacher)
        {
            // Input validation
            if (string.IsNullOrEmpty(teacher.TeacherFirstName) || string.IsNullOrEmpty(teacher.TeacherLastName))
            {
                return BadRequest(new { message = "Teacher name cannot be empty" });
            }

            if (teacher.HireDate > DateTime.Now)
            {
                return BadRequest(new { message = "Hire date cannot be in the future" });
            }

            if (string.IsNullOrEmpty(teacher.EmployeeID) || !teacher.EmployeeID.StartsWith("T") 
                || !teacher.EmployeeID.Substring(1).All(char.IsDigit))
            {
                return BadRequest(new { message = "Employee number must start with 'T' followed by digits" });
            }

            // Checking if employee number is already taken
            MySqlConnection Connection = _context.GetConnection();
            Connection.Open();
            
            string checkSQL = "SELECT COUNT(*) FROM teachers WHERE employeenumber = @employeeNumber";
            MySqlCommand checkCommand = Connection.CreateCommand();
            checkCommand.CommandText = checkSQL;
            checkCommand.Parameters.AddWithValue("@employeeNumber", teacher.EmployeeID);
            
            int count = Convert.ToInt32(checkCommand.ExecuteScalar());
            
            if (count > 0)
            {
                Connection.Close();
                return BadRequest(new { message = "Employee number is already taken" });
            }

            try
            {
                string insertSQL = "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) " +
                    "VALUES (@fname, @lname, @employeeNumber, @hireDate, @salary)";
                
                MySqlCommand insertCommand = Connection.CreateCommand();
                insertCommand.CommandText = insertSQL;
                
                insertCommand.Parameters.AddWithValue("@fname", teacher.TeacherFirstName);
                insertCommand.Parameters.AddWithValue("@lname", teacher.TeacherLastName);
                insertCommand.Parameters.AddWithValue("@employeeNumber", teacher.EmployeeID);
                insertCommand.Parameters.AddWithValue("@hireDate", teacher.HireDate.ToString("yyyy-MM-dd"));
                insertCommand.Parameters.AddWithValue("@salary", teacher.Salary);
                
                insertCommand.ExecuteNonQuery();
                
                // Geting the last inserted ID
                insertCommand.CommandText = "SELECT LAST_INSERT_ID()";
                int newId = Convert.ToInt32(insertCommand.ExecuteScalar());
                teacher.TeacherId = newId;
                
                Connection.Close();
                
                return Ok(new { message = "Teacher added successfully", data = teacher });
            }
            catch (Exception ex)
            {
                Connection.Close();
                return BadRequest(new { message = $"Error adding teacher: {ex.Message}" });
            }
        }

        /// <summary>
        /// Deletes a teacher from the database
        /// </summary>
        /// <param name="id">The ID of the teacher to delete</param>
        /// <example>
        /// DELETE api/TeacherAPI/DeleteTeacher/1
        /// </example>
        /// <returns>Success message or error message</returns>
        [HttpDelete]
        [Route("DeleteTeacher/{id}")]
        public ActionResult DeleteTeacher(int id)
        {
            try
            {
                MySqlConnection Connection = _context.GetConnection();
                Connection.Open();
                
                // First check if the teacher exists
                string checkSQL = "SELECT COUNT(*) FROM teachers WHERE teacherid = @id";
                MySqlCommand checkCommand = Connection.CreateCommand();
                checkCommand.CommandText = checkSQL;
                checkCommand.Parameters.AddWithValue("@id", id);
                
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                
                if (count == 0)
                {
                    Connection.Close();
                    return NotFound(new { message = $"Teacher with ID {id} not found" });
                }

                // Delete the teacher
                string deleteSQL = "DELETE FROM teachers WHERE teacherid = @id";
                MySqlCommand deleteCommand = Connection.CreateCommand();
                deleteCommand.CommandText = deleteSQL;
                deleteCommand.Parameters.AddWithValue("@id", id);
                
                deleteCommand.ExecuteNonQuery();
                Connection.Close();
                
                return Ok(new { message = $"Teacher with ID {id} deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error deleting teacher: {ex.Message}" });
            }
        }
    }
}