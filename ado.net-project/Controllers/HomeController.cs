using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using ado.net_project.model;

namespace ado.net_project.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")
                          ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            return new SqlConnection(connectionString);
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Home | Employees";
            List<Employee> employees = new List<Employee>();

            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                string sql = "SELECT * FROM Employee";
                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Employee emp = new Employee
                    {
                        Id = reader["Id"] as int? ?? 0,
                        Name = reader["Name"] as string ?? string.Empty,
                        Position = reader["Position"] as string ?? string.Empty,
                        Age = reader["Age"] as int? ?? 0,
                        Email = reader["Email"] as string ?? string.Empty
                    };
                    employees.Add(emp);
                }

            }

            return View(employees);
        }

        [HttpPost]
        public IActionResult CreateEmployee(Employee emp)
        {
            using var conn = GetConnection();
            string sql = "INSERT INTO Employee (Name, Position, Age, Email) VALUES (@Name, @Position, @Age, @Email)";
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Name", emp.Name ?? string.Empty);
            cmd.Parameters.AddWithValue("@Position", emp.Position ?? string.Empty);
            cmd.Parameters.AddWithValue("@Age", emp.Age);
            cmd.Parameters.AddWithValue("@Email", emp.Email ?? string.Empty);

            conn.Open();
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            ViewData["Title"] = "Edit Employee";
            Employee? emp = null;
            using (var conn = GetConnection())
            {
                string sql = "SELECT * FROM Employee WHERE Id=@Id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    emp = new Employee
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"] as string ?? string.Empty,
                        Position = reader["Position"] as string ?? string.Empty,
                        Age = reader["Age"] as int? ?? 0,
                        Email = reader["Email"] as string ?? string.Empty
                    };
                }
            }
            return View(emp);
        }

        [HttpPost]
        public IActionResult EditEmployee(Employee emp)
        {
            using var conn = GetConnection();
            string sql = @"UPDATE Employee 
           SET Name=@Name, Position=@Position, Age=@Age, Email=@Email 
           WHERE Id=@Id";
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Id", emp.Id);
            cmd.Parameters.AddWithValue("@Name", emp.Name ?? string.Empty);
            cmd.Parameters.AddWithValue("@Position", emp.Position ?? string.Empty);
            cmd.Parameters.AddWithValue("@Age", emp.Age);
            cmd.Parameters.AddWithValue("@Email", emp.Email ?? string.Empty);

            conn.Open();
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteEmployee(int id)
        {
            using var conn = GetConnection();
            string sql = "DELETE FROM Employee WHERE Id=@Id";
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

    }
}
