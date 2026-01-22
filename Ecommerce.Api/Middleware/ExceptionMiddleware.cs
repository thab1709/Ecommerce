using Serilog;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next =next;

    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

        }
catch (Exception ex)
        {
            Log.Error(ex, "Lỗi chưa sửa");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                
               message = "Lỗi Server" ,
               detail = ex.Message,
               inner = ex.InnerException?.Message
            });

        }

    }
}