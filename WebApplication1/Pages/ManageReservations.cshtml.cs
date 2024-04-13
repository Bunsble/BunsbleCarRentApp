using Microsoft.AspNetCore.Mvc.RazorPages;
using Devart.Data.MySql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace WebApplication1.Pages
{
    public class ManageReservationsModel : PageModel
    {
        public List<ReservationModel> Reservations { get; set; }

        // Constructor
        public ManageReservationsModel()
        {
            Reservations = new List<ReservationModel>(); // Initialize the Reservations property
        }

        public void OnGet()
        {
            // Fetch reservations with approved = 0 from the database
            Reservations = GetPendingReservations();
        }

        // Method to get pending reservations from the database
        private List<ReservationModel> GetPendingReservations()
        {
            List<ReservationModel> reservations = new List<ReservationModel>();
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = @"SELECT r.ReservationID, r.StartDate, r.EndDate, r.Approved, 
                                        c.CarID, c.Brand, c.Model, c.Year, c.PassengerCapacity, c.Description, c.PricePerDay,
                                        u.UserID, u.Username, u.Password, u.FirstName, u.LastName, u.PersonalID, 
                                        u.PhoneNumber, u.EmailAddress, u.IsAdmin
                                 FROM Reservations r
                                 INNER JOIN Users u ON r.UserID = u.UserID
                                 INNER JOIN Cars c ON r.CarID = c.CarID
                                 WHERE r.Approved = 0";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Car car = new Car
                            {
                                CarID = reader.GetInt32("CarID"),
                                Brand = reader.GetString("Brand"),
                                Model = reader.GetString("Model"),
                                Year = reader.GetInt32("Year"),
                                PassengerCapacity = reader.GetInt32("PassengerCapacity"),
                                Description = reader.GetString("Description"),
                                PricePerDay = reader.GetDecimal("PricePerDay")
                            };

                            UserModel user = new UserModel
                            {
                                UserID = reader.GetInt32("UserID"),
                                Username = reader.GetString("Username"),
                                Password = reader.GetString("Password"),
                                FirstName = reader.GetString("FirstName"),
                                LastName = reader.GetString("LastName"),
                                PersonalID = reader.GetString("PersonalID"),
                                PhoneNumber = reader.GetString("PhoneNumber"),
                                EmailAddress = reader.GetString("EmailAddress"),
                                IsAdmin = reader.GetBoolean("IsAdmin")
                            };

                            reservations.Add(new ReservationModel
                            {
                                ReservationID = reader.GetInt32("ReservationID"),
                                StartDate = reader.GetDateTime("StartDate"),
                                EndDate = reader.GetDateTime("EndDate"),
                                Approved = reader.GetBoolean("Approved"),
                                Car = car,
                                User = user
                            });
                        }
                    }
                }
            }
            return reservations;
        }

        // Add methods to handle approving and disapproving reservations

        public IActionResult OnPostApproveReservation(int reservationId)
        {
            UpdateReservationStatus(reservationId, true);
            return RedirectToPage();
        }

        public IActionResult OnPostDisapproveReservation(int reservationId)
        {
            UpdateReservationStatus(reservationId, false);
            return RedirectToPage();
        }

        private void UpdateReservationStatus(int reservationId, bool approved)
        {
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = "UPDATE Reservations SET Approved = @Approved WHERE ReservationID = @ReservationID";
                    cmd.Parameters.AddWithValue("@Approved", approved ? 1 : 2);
                    cmd.Parameters.AddWithValue("@ReservationID", reservationId);
                    cmd.ExecuteNonQuery();
                }
            }
        }




        public class ReservationModel
        {
            public int ReservationID { get; set; }
            public Car Car { get; set; }
            public UserModel User { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public bool Approved { get; set; }
        }

        public class Car
        {
            public int CarID { get; set; }
            public string Brand { get; set; }
            public string Model { get; set; }
            public int Year { get; set; }
            public int PassengerCapacity { get; set; }
            public string Description { get; set; }
            public decimal PricePerDay { get; set; }
        }

        public class UserModel
        {
            public int UserID { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PersonalID { get; set; }
            public string PhoneNumber { get; set; }
            public string EmailAddress { get; set; }
            public bool IsAdmin { get; set; }
        }
    }
}
