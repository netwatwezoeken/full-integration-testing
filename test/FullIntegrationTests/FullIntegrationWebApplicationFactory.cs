using ContactsApp.Data;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;
using Xunit.Sdk;

namespace FullIntegrationTests;

internal class FullIntegrationWebApplicationFactory :Nwwz.Mvc.Testing.WebApplicationFactory<Program>
{
    public readonly MsSqlContainer DatabaseContainer;

    public FullIntegrationWebApplicationFactory()
    {
        try
        {
            DatabaseContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .Build();          
        }
        catch (ArgumentException ae)
        { 
            throw new XunitException($"Is docker installed and running? {ae.Message}.");
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("FullIntegrationTest").ConfigureTestServices(services =>
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "FullIntegrationTest");
            
            // replace the DB with docker variant fill it with data
            //https://github.com/dotnet/efcore/issues/27118
            var factoryDescriptor = services
                .FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IDbContextFactory<ContactContext>));
            if(factoryDescriptor != null) services.Remove(factoryDescriptor);
            
            var contextDescriptor = services
                .FirstOrDefault(descriptor => descriptor.ServiceType == typeof(ContactContext));
            if(contextDescriptor != null) services.Remove(contextDescriptor);
            
            var factorySourceDescriptor = services
                .FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IDbContextFactorySource<ContactContext>));
            if(factorySourceDescriptor != null) services.Remove(factorySourceDescriptor);
            
            var dbContextDescriptor = services
                .FirstOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<ContactContext>));
            if(dbContextDescriptor != null) services.Remove(dbContextDescriptor);
            
            services.AddDbContextFactory<ContactContext>(opt =>
                 opt.UseSqlServer(DatabaseContainer.GetConnectionString()));
            
            var db = services.BuildServiceProvider().GetService<ContactContext>();
            db.Database.Migrate();
            
            try
            {
                lock (TestData.LockObject)
                {
                    SeedTestData(db);
                }
            }
            catch (System.ArgumentException)
            {
                // The DataInitializer does not detect that Data is already in the DB.
            }
        });
    }

    private void SeedTestData(ContactContext db)
    {
        if (db.Contacts.Any()) return;
        
        db.Contacts.AddRange(TestData.Contacts);
        db.SaveChanges();
    }
}