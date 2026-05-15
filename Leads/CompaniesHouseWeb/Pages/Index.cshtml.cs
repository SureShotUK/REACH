using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CompaniesHouseWeb.Pages;

public class IndexModel : PageModel
{
    public string? ErrorMessage { get; set; }

    public void OnGet([FromQuery] string? error)
    {
        ErrorMessage = error switch
        {
            "notfound" => "Company not found. Please check the company number and try again.",
            "invalid"  => "Please enter a valid 8-digit company number.",
            _          => null
        };
    }
}
