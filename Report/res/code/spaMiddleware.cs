public async Task Invoke(HttpContext context)
{
    if (!context.Request.Path.StartsWithSegments("/api") |\label{line:spa_conditions}|
        && !context.Request.Path.StartsWithSegments("/swagger")
        && !Path.HasExtension(context.Request.Path.Value))
    {
        context.Response.StatusCode = (int) HttpStatusCode.OK;
        await context.Response.WriteAsync(File.ReadAllText(_indexPath));
    }
    else
    {
        await _next(context);
    }
}
