public async Task Invoke(HttpContext context)
{
    try
    {
        await _next(context);
    }
    catch (InvalidRestOperationException invalidRestOperationException)
    {
        context.Response.StatusCode = invalidRestOperationException.ResponseCode; |\label{line:em_httpStatusCode}|
        var dto = new ExceptionDTO(invalidRestOperationException);
        await context.Response.WriteAsync(JsonConvert.SerializeObject(dto)); |\label{line:em_exceptionDTO}|
    }
}
