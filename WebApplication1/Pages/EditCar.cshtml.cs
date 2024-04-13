using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Devart.Data.MySql;
using System.Threading.Tasks;

namespace WebApplication1.Pages
{
    public class EditCarModel : PageModel
    {
        [BindProperty]
        public Car Car { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Car = await GetCarByIdAsync(id);
            if (Car == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            UpdateCarInDatabase(Car);

            return RedirectToPage("/ManageCars");
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
    }
}
