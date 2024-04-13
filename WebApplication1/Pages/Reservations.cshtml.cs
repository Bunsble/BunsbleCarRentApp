using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Devart.Data.MySql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Pages
{
    public class ReservationsModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReservationsModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<ReservationViewModel> Reservations { get; set; }

        public void OnGet()
        {
            // Retrieving reservations from the database
            Reservations = GetReservationsFromDatabase();
        }

        [BindProperty]
        public DateTime EndDate { get; set; }

        public IActionResult OnPostRent(int carID, DateTime endDate)
        {
            // Retrieve user ID from cookies
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;
            Console.WriteLine("Debug message: UserID = " + endDate);

            var userID = GetUserIDByUsername(username);

            // Inserting a new reservation record into the database
            InsertReservationIntoDatabase(carID, userID, endDate);

            // Redirecting back to the Reservations page after reservation
            return RedirectToPage("/Reservations");
        }

        private List<ReservationViewModel> GetReservationsFromDatabase()
        {
            List<ReservationViewModel> reservations = new List<ReservationViewModel>();

            // Example connection string, replace with your actual connection string
            string connectionString = "User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;Charset=utf8mb4;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Example query to select reservation data from the database
                string query = "SELECT * FROM Cars";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ReservationViewModel reservation = new ReservationViewModel
                            {
                                // Map data from the database to ReservationViewModel properties
                                CarID = reader.GetInt32("CarID"),
                                Brand = reader.GetString("Brand"),
                                Model = reader.GetString("Model"),
                                Year = reader.GetInt32("Year"),
                                PassengerCapacity = reader.GetInt32("PassengerCapacity"),
                                Description = reader.GetString("Description"),
                                PricePerDay = reader.GetDecimal("PricePerDay")
                            };

                            reservations.Add(reservation);
                        }
                    }
                }
            }

            return reservations;
        }

        private int GetUserIDByUsername(string username)
        {
            int userID = -1; // Default value if user is not found

            // Connect to the database and retrieve the user ID based on username
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;Charset=utf8mb4;"))
            {
                conn.Open();

                string query = "SELECT UserID FROM Users WHERE Username = @Username";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        userID = Convert.ToInt32(result);
                    }
                }
            }

            return userID;
        }


        private void InsertReservationIntoDatabase(int carID, int userID, DateTime endDate)
        {
            // Inserting a new reservation record into the database
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;Charset=utf8mb4;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();


                    cmd.CommandText = @"INSERT INTO Reservations (CarID, UserID, StartDate, EndDate, Approved) 
                                        VALUES (@CarID, @UserID, @StartDate, @EndDate, @Approved)";
                    cmd.Parameters.AddWithValue("@CarID", carID);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@StartDate", DateTime.Today);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    cmd.Parameters.AddWithValue("@Approved", 0); // 0 for not approved
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    public class ReservationViewModel
    {
        public int CarID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int PassengerCapacity { get; set; }
        public string Description { get; set; }
        public decimal PricePerDay { get; set; }
    }
}
