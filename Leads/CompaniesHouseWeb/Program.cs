using CompaniesHouseSearch.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Register Companies House client as a singleton (shared HttpClient)
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var apiKey = config["CompaniesHouse:ApiKey"]
        ?? throw new InvalidOperationException("CompaniesHouse:ApiKey not set in appsettings.json");
    var baseUrl = config["CompaniesHouse:BaseUrl"]
        ?? "https://api.company-information.service.gov.uk";
    return new CompaniesHouseClient(apiKey, baseUrl);
});

builder.Services.AddSingleton<LeadScorer>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.Run();
