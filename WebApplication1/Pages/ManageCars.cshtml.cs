using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Devart.Data.MySql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Data;

namespace WebApplication1.Pages
{
    public class ManageCarsModel : PageModel
    {
        [BindProperty]
        public Car NewCar { get; set; }

        public List<Car> Cars { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch existing cars from the database
            Cars = await GetExistingCarsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // If model state is not valid, reload the page
                Cars = await GetExistingCarsAsync();
                return Page();
            }

            // Insert new car into the database
            InsertNewCarIntoDatabase(NewCar);

            // Redirect to GET to refresh the page with updated data
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(int carID)
        {
            var carToUpdate = await GetCarByIdAsync(carID);
            if (carToUpdate == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // If model state is not valid, reload the page
                Cars = await GetExistingCarsAsync();
                return Page();
            }

            // Update existing car in the database
            UpdateCarInDatabase(NewCar);

            // Redirect to GET to refresh the page with updated data
            return RedirectToPage();
        }

        private async Task<Car> GetCarByIdAsync(int carID)
        {
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = "SELECT CarID, Brand, Model, Year, PassengerCapacity, Description, PricePerDay FROM Cars WHERE CarID = @CarID";
                    cmd.Parameters.AddWithValue("@CarID", carID);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Car
                            {
                                CarID = reader.GetInt32(0),
                                Brand = reader.GetString(1),
                                Model = reader.GetString(2),
                                Year = reader.GetInt32(3),
                                PassengerCapacity = reader.GetInt32(4),
                                Description = reader.GetString(5),
                                PricePerDay = reader.GetDecimal(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        private async Task<List<Car>> GetExistingCarsAsync()
        {
            List<Car> cars = new List<Car>();
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = "SELECT CarID, Brand, Model, Year, PassengerCapacity, Description, PricePerDay FROM Cars";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            cars.Add(new Car
                            {
                                CarID = reader.GetInt32(0),
                                Brand = reader.GetString(1),
                                Model = reader.GetString(2),
                                Year = reader.GetInt32(3),
                                PassengerCapacity = reader.GetInt32(4),
                                Description = reader.GetString(5),
                                PricePerDay = reader.GetDecimal(6)
                            });
                        }
                    }
                }
            }
            return cars;
        }

        private void InsertNewCarIntoDatabase(Car car)
        {
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = @"INSERT INTO Cars (Brand, Model, Year, PassengerCapacity, Description, PricePerDay) 
                                        VALUES (@Brand, @Model, @Year, @PassengerCapacity, @Description, @PricePerDay)";
                    cmd.Parameters.AddWithValue("@Brand", car.Brand);
                    cmd.Parameters.AddWithValue("@Model", car.Model);
                    cmd.Parameters.AddWithValue("@Year", car.Year);
                    cmd.Parameters.AddWithValue("@PassengerCapacity", car.PassengerCapacity);
                    cmd.Parameters.AddWithValue("@Description", car.Description);
                    cmd.Parameters.AddWithValue("@PricePerDay", car.PricePerDay);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateCarInDatabase(Car car)
        {
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = @"UPDATE Cars 
                                        SET Brand = @Brand, Model = @Model, Year = @Year, 
                                            PassengerCapacity = @PassengerCapacity, Description = @Description, 
                                            PricePerDay = @PricePerDay 
                                        WHERE CarID = @CarID";
                    cmd.Parameters.AddWithValue("@CarID", car.CarID);
                    cmd.Parameters.AddWithValue("@Brand", car.Brand);
                    cmd.Parameters.AddWithValue("@Model", car.Model);
                    cmd.Parameters.AddWithValue("@Year", car.Year);
                    cmd.Parameters.AddWithValue("@PassengerCapacity", car.PassengerCapacity);
                    cmd.Parameters.AddWithValue("@Description", car.Description);
                    cmd.Parameters.AddWithValue("@PricePerDay", car.PricePerDay);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int carId)
        {
            // Delete the user from the database using the provided userId
            await DeleteCarFromDatabase(carId);

            // Refresh the user list after deletion
            Cars = await GetExistingCarsAsync();

            // Redirect to the same page
            return RedirectToPage();
        }

        private async Task DeleteCarFromDatabase(int carID)
        {
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = "DELETE FROM Cars WHERE CarID = @CarID";
                    cmd.Parameters.AddWithValue("@CarID", carID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
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
}
