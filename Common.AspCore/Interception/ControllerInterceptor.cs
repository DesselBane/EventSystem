// ReSharper disable once RedundantUsingDirective
// Collections Generic and System Refelction are needed by line 18 (the any statement)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.ErrorCodes;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Common.AspCore.Interception
{
    public class ControllerInterceptor : IInterceptor
    {
        #region Implementation of IInterceptor

        public void Intercept(IInvocation invocation)
        {
            if (invocation.MethodInvocationTarget.CustomAttributes.Any(x => x.AttributeType.GetTypeInfo().IsSubclassOf(typeof(HttpMethodAttribute))))
                if (invocation.Arguments.Any(invocationArgument => invocationArgument == null))
                    throw new UnprocessableEntityException("Argument cannot be null", Guid.Parse(GlobalErrorCodes.NO_DATA));

            invocation.Proceed();
        }

        #endregion
    }
}