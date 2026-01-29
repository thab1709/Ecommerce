

using Swashbuckle.AspNetCore.SwaggerGen;

Log.Logger = new LoggerConfiguration()
   .MinimumLevel.Information()
   .Enrich.FromLogContext()
   .WriteTo.Console(outputTemplate:"[{Time:HH:mm:ss} {level:u3} {Message:lj} {properties:j} {Newline} {Exception}]")
   .WriteTo.File("logs/log -.txt", rollingInterval:RollingInterval.Day,rollOnFileSizeLimit:true,fileSizeLimitBytes:100*1024*1024)
   .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc();
builder.Services.AddHttpContextAccessor();
DevExpress.Xpo.DB.PostgreSqlConnectionProvider.Register();

builder.Services.AddDevExpressControls();
builder.Services.ConfigureReportingServices(configurator =>
{
    configurator.ConfigureReportDesigner(designerConfigurator =>
    {
        designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider();

    });

  configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
});




builder.Host.UseSerilog();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; 
});

builder.Services.AddApplication();                      
builder.Services.AddInfrastructure(builder.Configuration);


var jwtSetting = builder.Configuration.GetSection("jwtSetting");
var key = Encoding.UTF8.GetBytes(jwtSetting["Secrect"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidIssuer = jwtSetting["Issuer"],
            ValidAudience = jwtSetting["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();


builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("Fixed" , opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 5;
        opt.QueueLimit = 0;
    }
    );
    options.AddTokenBucketLimiter("AuthLimited", opt =>
    {
        opt.TokenLimit =10;
    });
});
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
});
builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();

builder.Services.AddCarter(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>  {options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
       {
           Description = "Nhap ma Token",
           Name = "Authorization",
           In = ParameterLocation.Header,
           Type = SecuritySchemeType.Http,
           Scheme = "bearer",
           BearerFormat = "JWT"

       });
     options.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme 
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer" 
            }
        },
        new string[] {} 
   }
    });
    options.DocInclusionPredicate((docName, apiDesc) => {
        if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return false;
        // Bỏ qua các Controller thuộc namespace DevExpress
        return !methodInfo.DeclaringType.FullName.StartsWith("DevExpress");
    });
   });



var app = builder.Build();


app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseDevExpressControls();
app.MapCarter(); 
app.MapControllers();
app.Run();