using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            var user = HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                // Get the claims for the authenticated user
                var claims = user.Claims;

                // Print the claims
                foreach (var claim in claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }
            }
            else
            {
                Console.WriteLine("User is not authenticated.");
            }

            return Page();
        }


    }
}
