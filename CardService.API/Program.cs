using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using CardService.Application;
using CardService.Application.Common.Extensions;
using CardService.Application.Common.Filters;
using CardService.Application.Common.Mapper;
using CardService.Application.Common.Models.Configurations;
using CardService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add(typeof(ModelStateValidationFilterAttribute));
        options.Filters.Add(typeof(ServiceExceptionFilter));
    }).AddNewtonsoftJson(options =>
    {
        // Use the default property (Pascal) casing
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

    builder.Services.AddCors(c =>
    {
        c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    });

    // Auto Mapper Configurations
    var mapperConfig = new MapperConfiguration(mc =>
    {
        mc.AddProfile(new MappingProfile());
    });

    IMapper mapper = mapperConfig.CreateMapper();
    builder.Services.AddSingleton(mapper);

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(options =>
    {
        options.EnableAnnotations();
    });

    builder.Services.AddSwagger();
    builder.Services.AddApplication(builder.Configuration);
    builder.Services.AddDbConnection(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddInfrastructure();
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

    AppSettings appSettings = new();
    builder.Configuration.GetSection("AppSettings").Bind(appSettings);

    var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
        .WriteTo.Console()
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Cards", "Service")
        .WriteTo.Seq(appSettings.LogUrl)
                        .CreateLogger();
    builder.Host.UseSerilog(logger);

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();
    app.ConfigureExceptionHandler();
    app.UseCors(x => x
     .SetIsOriginAllowed(origin => true)
     .AllowAnyMethod()
     .AllowAnyHeader()
     .AllowCredentials());

    await app.SeedUserData();
    app.RunMigration();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("../swagger/v1/swagger.json", "Card Service V1");
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception occurred in Card Service");
}
finally
{
    Log.Information("Shut down complete Card Service");
    Log.CloseAndFlush();
}