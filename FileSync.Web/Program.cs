using FileSync.Application;
using FileSync.Infrastructure;
using FileSync.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

builder.Services.AddSignalR();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    connectionString: builder.Configuration.GetConnectionString("DefaultConnection") 
                      ?? "Data Source=filesync.db",
    fileStoragePath: builder.Configuration["FileStorage:Path"] ?? "uploads",
    baseUrl: builder.Configuration["BaseUrl"] ?? "https://localhost:5001"
);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<FileSync.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.MapHub<SpaceHub>("/spacehub");

app.Run();
    