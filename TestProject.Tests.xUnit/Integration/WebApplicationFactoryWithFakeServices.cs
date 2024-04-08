using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TestProject.Models;
using TestProject.Services;

namespace TestProject.Tests.xUnit.Integration;

public class WebApplicationFactoryWithFakeServices<TProgram> :
    WebApplicationFactory<TProgram> where TProgram : class
{
    public List<Product>? Products { get; private set; }
    public List<User>? Users { get; private set; }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // ServiceDescriptor storage =
            //     services.Single(x => x.ServiceType == typeof(StorageService));
            // services.Remove(storage);
            //
            // ServiceDescriptor email =
            //     services.Single(x => x.ServiceType == typeof(ServiceDescriptor));
            // services.Remove(email);

            Products = FakeStorageService.GenerateProducts(10);
            Users = FakeStorageService.GenerateUsers(4);
            
            services.AddScoped<IStorageService, FakeStorageService>(x =>
                new FakeStorageService(Products, Users));
            services.AddTransient<IEmailService, FakeEmailService>();
        });

        builder.UseEnvironment("development");
    }
}