using Microsoft.AspNetCore.Mvc.RazorPages;
using Devart.Data.MySql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication1.Data;

namespace WebApplication1.Pages
{
    public class ManageUsersModel : PageModel
    {
        [BindProperty]
        public UserModel NewUser { get; set; }

        [BindProperty]
        public UserModel EditUser { get; set; }

        public List<UserModel> Users { get; set; }


        // Constructor
        public ManageUsersModel()
        {
            Users = new List<UserModel>(); // Initialize the Users property
        }

        [HttpPost]
        public async Task<IActionResult> OnPostEditAsync()
        {


            if (!ModelState.IsValid)
            {
                // If model state is not valid, reload the page
                Users = await GetExistingUsersAsync();
                return Page();
            }


            // Update the user in the database
            UpdateUserInDatabase(EditUser);

            // Refresh the user list after updating
            Users = await GetExistingUsersAsync();

            // Redirect to GET to refresh the page with updated data
            return RedirectToPage();
        }


        private void UpdateUserInDatabase(UserModel user)
        {

            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = "UPDATE Users SET Username = @Username, Password = @Password, FirstName = @FirstName, LastName = @LastName, PersonalID = @PersonalID, PhoneNumber = @PhoneNumber, EmailAddress = @EmailAddress, IsAdmin = @IsAdmin WHERE UserID = @UserID";
                    cmd.Parameters.AddWithValue("@UserID", user.UserId);
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@PersonalID", user.PersonalID);
                    cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                    cmd.Parameters.AddWithValue("@EmailAddress", user.EmailAddress);
                    cmd.Parameters.AddWithValue("@IsAdmin", user.IsAdmin ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch existing users from the database
            Users = await GetExistingUsersAsync();

            // Ensure that Users property is initialized
            if (Users == null)
            {
                Users = new List<UserModel>();
            }

            // Initialize EditUser
            EditUser = new UserModel(); // Or however you want to initialize it

            return Page();
        }


        public async Task<IActionResult> OnPostCreateAsync()
        {

            /*** Debugging: Log the values of the new user
            Console.WriteLine("New User Details:");
            Console.WriteLine($"Username: {NewUser.Username}");
            Console.WriteLine($"Password: {NewUser.Password}");
            Console.WriteLine($"FirstName: {NewUser.FirstName}");
            Console.WriteLine($"LastName: {NewUser.LastName}");
            Console.WriteLine($"PersonalID: {NewUser.PersonalID}");
            Console.WriteLine($"PhoneNumber: {NewUser.PhoneNumber}");
            Console.WriteLine($"EmailAddress: {NewUser.EmailAddress}");

            if (!ModelState.IsValid)
            {
                // Log validation errors
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        Console.WriteLine($"Validation error: {error.ErrorMessage}");
                    }
                }

                // If model state is not valid, reload the page
                Users = await GetExistingUsersAsync();
                return Page();
            }***/

            if (await IsDuplicateUserAsync(NewUser.Username, NewUser.EmailAddress, NewUser.PersonalID))
            {
                ModelState.AddModelError("", "Username, email or PersonalID already exists.");
                Users = await GetExistingUsersAsync();
                return Page();
            }

            if (!ModelState.IsValid)
            {
                // If model state is not valid, reload the page
                Users = await GetExistingUsersAsync();
                return Page();
            }

            // Insert new user into the database
            InsertNewUserIntoDatabase(NewUser);


            // Refresh the user list after inserting the new user
            Users = await GetExistingUsersAsync();

            // Redirect to GET to refresh the page with updated data
            return RedirectToPage();
        }

        private async Task<bool> IsDuplicateUserAsync(string username, string emailAddress, string personalId)
        {
            Console.WriteLine($"Checking for duplicate user with username: {username}, email: {emailAddress}, and personal ID: {personalId}");

            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @Username OR EmailAddress = @EmailAddress OR PersonalID = @PersonalID";
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@EmailAddress", emailAddress);
                    cmd.Parameters.AddWithValue("@PersonalID", personalId);
                    int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    Console.WriteLine($"Found {count} duplicate users.");
                    return count > 0;
                }
            }
        }



        public async Task<IActionResult> OnPostDeleteAsync(int userId)
        {
            // Delete the user from the database using the provided userId
            await DeleteUserFromDatabaseAsync(userId);

            // Refresh the user list after deletion
            Users = await GetExistingUsersAsync();

            // Redirect to the same page
            return RedirectToPage();
        }

        private async Task DeleteUserFromDatabaseAsync(int userId)
        {
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = "DELETE FROM Users WHERE UserID = @UserID";
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }



        private async Task<List<UserModel>> GetExistingUsersAsync()
        {
            List<UserModel> users = new List<UserModel>();
            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = "SELECT UserID, Username, Password, FirstName, LastName, PersonalID, PhoneNumber, EmailAddress, IsAdmin FROM Users";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new UserModel
                            {
                                UserId = reader.GetInt32("UserID"),
                                Username = reader.GetString("Username"),
                                Password = reader.GetString("Password"),
                                FirstName = reader.GetString("FirstName"),
                                LastName = reader.GetString("LastName"),
                                PersonalID = reader.GetString("PersonalID"),
                                PhoneNumber = reader.GetString("PhoneNumber"),
                                EmailAddress = reader.GetString("EmailAddress"),
                                IsAdmin = reader.GetBoolean("IsAdmin")
                            });
                        }
                    }
                }
            }

            // Log the count of users fetched
            Console.WriteLine("Number of users fetched: " + users.Count);

            return users;
        }

        private void InsertNewUserIntoDatabase(UserModel user)
        {



            using (MySqlConnection conn = new MySqlConnection("User Id=ivan;Password=taraleji##;Host=p.bunsble.com;Database=CarRent;"))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = "INSERT INTO Users (Username, Password, FirstName, LastName, PersonalID, PhoneNumber, EmailAddress, IsAdmin) VALUES (@Username, @Password, @FirstName, @LastName, @PersonalID, @PhoneNumber, @EmailAddress, @IsAdmin)";
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@PersonalID", user.PersonalID);
                    cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                    cmd.Parameters.AddWithValue("@EmailAddress", user.EmailAddress);
                    cmd.Parameters.AddWithValue("@IsAdmin", user.IsAdmin ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    public class UserModel
    {
        public int UserId { get; set; }
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
