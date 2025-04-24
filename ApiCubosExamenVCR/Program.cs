using ApiCubosExamenVCR.Data;
using ApiCubosExamenVCR.Helpers;
using ApiCubosExamenVCR.Repositories;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));
});

SecretClient secretClient = builder.Services.BuildServiceProvider()
                                            .GetRequiredService<SecretClient>();

KeyVaultSecret secretConnectionString = await secretClient.GetSecretAsync("sqlazure");
KeyVaultSecret secretStorageAccount = await secretClient.GetSecretAsync("storageaccount");


HelperActionServicesOAuth helper = new HelperActionServicesOAuth(builder.Configuration);
builder.Services.AddSingleton<HelperActionServicesOAuth>(helper);
builder.Services.AddAuthentication(helper.GetAuthenticationOptions())
                .AddJwtBearer(helper.GetJwtBearerOptions());

builder.Services.AddTransient<RepositoryCubos>();
builder.Services.AddTransient<BlobServiceClient>(x => new BlobServiceClient(secretStorageAccount.Value));

builder.Services.AddDbContext<CubosContext>(options =>
    options.UseSqlServer(secretConnectionString.Value));



builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.MapOpenApi();

app.UseHttpsRedirection();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/openapi/v1.json", "Cubos API V1");
    c.RoutePrefix = "";
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
