using Devart.Data.MySql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using WebApplication1.Data;
using static WebApplication1.Pages.ManageReservationsModel;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Pages
{
    public class MyProfileModel : PageModel
    {
        public UserModel User { get; set; }
        public List<ReservationModel> PendingReservations { get; set; }
        public List<ReservationModel> DisapprovedReservations { get; set; }

        public IActionResult OnGet()
        {
            string username = GetLoggedInUsername();
            User = GetUserByUsername(username);

            if (User != null)
            {
                PendingReservations = GetReservationsByStatus(User.UserId, 0);
                DisapprovedReservations = GetReservationsByStatus(User.UserId, 2);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            // Check if ModelState is valid
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch the current user
            string username = GetLoggedInUsername();
            User = GetUserByUsername(username);

            // Check if the user exists
            if (User == null)
            {
                // Handle the case where the user is not found
                // You can log an error message or handle it according to your application's logic
                return RedirectToPage("/Error");
            }

            // Update the user model with the form data
            User.Username = Request.Form["Username"];
            User.FirstName = Request.Form["FirstName"];
            User.LastName = Request.Form["LastName"];
            User.PersonalID = Request.Form["PersonalID"];
            User.PhoneNumber = Request.Form["PhoneNumber"];
            User.EmailAddress = Request.Form["EmailAddress"];

            // Update user information in the database
            UpdateUser(User);

            // Redirect to GET to refresh the page with updated data
            return RedirectToPage();
        }


        private void UpdateUser(UserModel user)
        {
            
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = "UPDATE Users SET Username = @Username, FirstName = @FirstName, LastName = @LastName, PersonalID = @PersonalID, PhoneNumber = @PhoneNumber, EmailAddress = @EmailAddress WHERE UserID = @UserID";
                    cmd.Parameters.AddWithValue("@UserID", user.UserId);
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@PersonalID", user.PersonalID);
                    cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                    cmd.Parameters.AddWithValue("@EmailAddress", user.EmailAddress);
                    Console.WriteLine("Executing SQL command:");
                    Console.WriteLine(cmd.CommandText);
                    foreach (MySqlParameter parameter in cmd.Parameters)
                    {
                        Console.WriteLine($"{parameter.ParameterName}: {parameter.Value}");
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }





        private string GetLoggedInUsername()
        {
            // Check if the user is authenticated
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                // Retrieve the username from the user's identity
                string username = HttpContext.User.Identity.Name;
                return username;
            }
            else
            {
                // Handle the case where the user is not authenticated
                // You can log an error message or return a default value
                Console.WriteLine("User is not authenticated!");
                return null; // or return a default username
            }
        }



        private UserModel GetUserByUsername(string username)
        {
            UserModel user = null;

            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = "SELECT * FROM Users WHERE Username = @Username";
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new UserModel
                            {
                                UserId = reader.GetInt32("UserID"),
                                Username = reader.GetString("Username"),
                                FirstName = reader.GetString("FirstName"),
                                LastName = reader.GetString("LastName"),
                                PersonalID = reader.GetString("PersonalID"),
                                PhoneNumber = reader.GetString("PhoneNumber"),
                                EmailAddress = reader.GetString("EmailAddress"),
                                IsAdmin = reader.GetBoolean("IsAdmin")
                            };
                        }
                    }
                }
            }

            return user;
        }

        private List<ReservationModel> GetReservationsByStatus(int userId, int status)
        {
            List<ReservationModel> reservations = new List<ReservationModel>();

            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = "SELECT * FROM Reservations WHERE UserID = @UserID AND Approved = @Status";
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Status", status);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservations.Add(new ReservationModel
                            {
                                ReservationID = reader.GetInt32("ReservationID"),
                                CarID = reader.GetInt32("CarID"),
                                UserID = reader.GetInt32("UserID"),
                                StartDate = reader.GetDateTime("StartDate"),
                                EndDate = reader.GetDateTime("EndDate"),
                                Approved = reader.GetBoolean("Approved")
                            });
                        }
                    }
                }
            }

            return reservations;
        }

        public class ReservationModel
        {
            public int ReservationID { get; set; }
            public int CarID { get; set; }
            public int UserID { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public bool Approved { get; set; }
        }
    }
}
