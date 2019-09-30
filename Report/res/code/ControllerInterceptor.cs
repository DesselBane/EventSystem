public void Intercept(IInvocation invocation)
{
    if (invocation.MethodInvocationTarget.CustomAttributes.Any(x => x.AttributeType.GetTypeInfo().IsSubclassOf(typeof(HttpMethodAttribute))))
        if (invocation.Arguments.Any(invocationArgument => invocationArgument == null))
            throw new UnprocessableEntityException("Argument cannot be null", Guid.Parse(GlobalErrorCodes.NO_DATA));

    invocation.Proceed();
}
