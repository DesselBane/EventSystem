public PersonServiceInterceptor(PersonValidator personValidator)
{
    _personValidator = personValidator;

    BuildUp(new Dictionary<string, Action<IInvocation>>
    {
        {
            nameof(UpdatePersonAsync),
            x => UpdatePersonAsync((Person) x.Arguments[0], (int) x.Arguments[1])
        }
    });
}
