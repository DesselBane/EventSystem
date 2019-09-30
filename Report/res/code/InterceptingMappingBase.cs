public abstract class InterceptingMappingBase : IInterceptor{
    private IReadOnlyDictionary<string, Action<IInvocation>> _mappings;

    protected void BuildUp(IDictionary<string, Action<IInvocation>> mappings){
        if (_mappings != null)
            throw new InvalidOperationException($"{nameof(BuildUp)} Method can only be called once");
        _mappings = new ReadOnlyDictionary<string, Action<IInvocation>>(mappings);
    }

    public void Intercept(IInvocation invocation){
        if (_mappings.ContainsKey(invocation.Method.Name))
            _mappings[invocation.Method.Name](invocation);

        invocation.Proceed();
    }
}
