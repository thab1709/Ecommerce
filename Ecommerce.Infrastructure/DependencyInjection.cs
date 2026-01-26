namespace Ecommerce.Infrastructure;
public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)   
    
    {
        var connectionString =  configuration.GetConnectionString("PostgresConnection");
        services.AddDbContext<AppDbContext>(options => 
        options.UseNpgsql(connectionString));
       services.AddScoped<IPromotionRepository, PromotionRepository>();
       services.AddScoped<IOrderRepository, OrderRepository>();
       services.AddScoped<ICustomerRepository, CustomerRepository>();
       services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
       services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
      return services;
}
}