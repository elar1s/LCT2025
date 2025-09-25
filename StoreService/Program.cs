using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StoreService.Database;
using StoreService.Middleware;
using StoreService.Repositories;
using StoreService.Services;
using StoreService.Extensions; // added for DI extension
using Newtonsoft.Json; // added

var builder = WebApplication.CreateBuilder(args);

#region Serilog
// Use new logging extension
builder.Host.AddSerilogLogging();
#endregion

#region Services
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        options.SerializerSettings.Formatting = Formatting.None;
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

// Granular DI registration chain
builder.Services
    .AddDatabase(builder.Configuration)
    .AddRepositories()
    .AddDomainServices();
#endregion

var app = builder.Build();

#region Middleware
app.UseSerilogRequestLogging();
app.UseGlobalErrorHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
#endregion

app.Run();