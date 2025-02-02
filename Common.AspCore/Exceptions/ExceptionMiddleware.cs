﻿using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Common.AspCore.Exceptions
{
    public class ExceptionMiddleware
    {
        #region Vars

        private readonly RequestDelegate _next;

        #endregion

        #region Constructors

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidRestOperationException invalidRestOperationException)
            {
                context.Response.StatusCode = invalidRestOperationException.ResponseCode;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ExceptionDTO(invalidRestOperationException)));
            }
        }
    }
}