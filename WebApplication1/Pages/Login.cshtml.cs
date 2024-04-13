using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using Devart.Data.MySql;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace WebApplication1.Pages
{
    public class LoginRegisterModel : PageModel
    {
        // Initialize the Registration property
        public LoginRegisterModel()
        {
            Registration = new RegisterModel();
        }

        [BindProperty]
        public RegisterModel Registration { get; set; }

        [BindProperty]
        public string LoginUsername { get; set; }

        [BindProperty]
        public string LoginPassword { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostLoginAsync()
        {
            // Validate login credentials
            if (IsValidLogin(LoginUsername, LoginPassword))
            {
                // Check if the user is an admin
                bool isAdmin = IsAdmin(LoginUsername);

                // Create claims for the authenticated user
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, LoginUsername) // Store the username as a claim
        };

                // If user is an admin, add the "Admin" role claim
                if (isAdmin)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                }

                // Create identity from the claims
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Create authentication properties
                var authProperties = new AuthenticationProperties
                {
                    // You can add additional properties if needed
                };

                // Sign in the user with the claims identity and authentication properties
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirect to dashboard or another page
                return RedirectToPage("/Index");
            }
            else
            {
                // Show error message or redirect back to login page with error
                ErrorMessage = "Invalid username or password.";
                return Page();
            }
        }

        private bool IsAdmin(string username)
        {
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND isAdmin = 1";
                    cmd.Parameters.AddWithValue("@Username", username);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }



        // Actual login validation logic
        private bool IsValidLogin(string username, string password)
        {
            // Query the database to check if the username exists and the password matches
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    // Check if the provided username and password match any records in the database
                    cmd.CommandText = $"SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }



        public IActionResult OnPostRegister()
        {
            // Check if registration data is valid
            if (!ModelState.IsValid)
            {
               // return Page();
            }

            // Validate email format
            if (!IsValidEmail(Registration.Email))
            {
                ErrorMessage = "Invalid email address.";
                return Page();
            }

            // Check if username already exists
            if (UsernameExists(Registration.Username))
            {
                ErrorMessage = "Username already exists.";
                return Page();
            }

            // Check if email already exists
            if (EmailExists(Registration.Email))
            {
                ErrorMessage = "Email already exists.";
                return Page();
            }

            // Check if personal ID already exists
            if (PersonalIDExists(Registration.PersonalID))
            {
                ErrorMessage = "Personal ID already exists.";
                Console.WriteLine("SAME EGN");
                return Page();
            }


            // Insert registration data into database
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    cmd.CommandText = $"INSERT INTO Users (Username, Password, FirstName, LastName, EmailAddress, PhoneNumber, PersonalID) VALUES ('{Registration.Username}', '{Registration.Password}', '{Registration.FirstName}', '{Registration.LastName}', '{Registration.Email}', '{Registration.PhoneNumber}', '{Registration.PersonalID}')";
                    try
                    {
                        int aff = cmd.ExecuteNonQuery();
                        Console.WriteLine($"{aff} rows were affected.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error encountered during INSERT operation: {ex.Message}");
                    }
                }
            }

            // Redirect to dashboard or another page after successful registration
            return RedirectToPage("/Index");
        }

        // Method to check if username already exists
        private bool UsernameExists(string username)
        {
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    cmd.CommandText = $"SELECT COUNT(*) FROM Users WHERE Username = '{username}'";
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        // Method to check if email already exists
        private bool EmailExists(string email)
        {
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    cmd.CommandText = $"SELECT COUNT(*) FROM Users WHERE EmailAddress = '{email}'";
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        // Method to check if personal ID already exists
        private bool PersonalIDExists(string personalID)
        {
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    cmd.CommandText = $"SELECT COUNT(*) FROM Users WHERE PersonalID = '{personalID}'";
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        // Validate email address
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
