namespace Talabat.API.Extensions
{
    public static class SwaggerExtension
    {
        public static WebApplication AddSwaggerMiddlewares(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
    }
}
