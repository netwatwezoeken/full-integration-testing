using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using ContactsApp.Data;
using ContactsApp.Grid;

[assembly: InternalsVisibleTo("FullIntegrationTests")]
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// register factory and configure the options
builder.Services.AddDbContextFactory<ContactContext>(opt =>
    opt.UseSqlServer("Server=localhost;Database=myDataBase;User Id=sa;Password=yourStrong(!)Password;"));

// pager
builder.Services.AddScoped<IPageHelper, PageHelper>();

// filters
builder.Services.AddScoped<IContactFilters, GridControls>();

// query adapter (applies filter to contact request).
builder.Services.AddScoped<GridQueryAdapter>();

// service to communicate success on edit between pages
builder.Services.AddScoped<EditSuccess>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContactContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
