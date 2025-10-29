using CRUDExample.MiddleWare;
using CRUDExample.StartUpExtenstions;
using Entities;
using IRepositoryContract;
using Microsoft.EntityFrameworkCore;
using Repository;
using Serilog;
using ServiceContracts;
using Services;
var builder = WebApplication.CreateBuilder(args);


builder.Services.ConfigureServices(builder.Configuration, builder.Host);
var app = builder.Build();
app.UseStaticFiles();
app.UseSerilogRequestLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseUseCustomExceptionHandleMiddleWare();
}
if (builder.Environment.IsEnvironment("Test")==false)
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();   
app.UseAuthorization();
app.MapControllers();
app.UseEndpoints(Endpoint =>
{
    Endpoint.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller}/{action}");
    Endpoint.MapControllerRoute(name: "Default", pattern: "{Controller}/{action}");
});
app.Logger.LogCritical("the error is critical");
app.Logger.LogDebug("the error need to be debug");
app.UseHttpLogging();
app.UseHsts();//if the browser send request the Server send back that we need to create secure channel first 
app.UseHttpsRedirection();
app.Run();

//middleware for logging request and response details in the log providers 

public partial class Program { }