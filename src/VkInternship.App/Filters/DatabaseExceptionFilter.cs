using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace VkInternship.App.Filters;

public class DatabaseExceptionFilter : Attribute, IExceptionFilter
{
    private readonly IHostEnvironment _hostEnvironment;

    public DatabaseExceptionFilter(IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }
    
    public void OnException(ExceptionContext context)
    {
        var contentResult = new ContentResult
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity,
        };
        
        switch (context.Exception)
        {
            // Ошибка при конфликте транзакций
            case InvalidOperationException
            {
                InnerException: DbUpdateException
                {
                    InnerException: PostgresException
                    {
                        SqlState: "40001"
                    } postgresException
                }
            }:
            {
                if (_hostEnvironment.IsDevelopment())
                {
                    contentResult.Content = postgresException.Detail;
                }

                context.Result = contentResult;
                context.ExceptionHandled = true;
                break;
            }
        }
    }
}