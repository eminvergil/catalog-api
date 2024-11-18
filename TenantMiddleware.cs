public class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, TenantProvider tenantProvider)
    {
        if (context.Request.Headers.TryGetValue("x-tenant-id", out var tenantIdStr) && 
            int.TryParse(tenantIdStr, out var tenantId))
        {
            tenantProvider.TenantId = tenantId;
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Tenant ID is missing.");
            return;
        }

        await next(context);
    }
}
