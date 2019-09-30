//Creating Global Authorization Filter
var globalAuthFilter = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

serviceCollection.AddMvc(options => { options.Filters.Add(new AuthorizeFilter(globalAuthFilter)); });
