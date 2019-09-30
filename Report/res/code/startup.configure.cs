// This method gets called by the runtime.
// Use this method to configure the HTTP request pipeline.
public virtual void Configure(IApplicationBuilder app,
                                 IHostingEnvironment env,
                                 ILoggerFactory loggerFactory)
{
    app.UseMiddleware<ExceptionMiddleware>(); |\label{line:useMiddleware}|

    loggerFactory.AddConsole(Configuration.GetSection("Logging"));
    loggerFactory.AddDebug();

    if (env.IsCustomDevelopment())
        app.UseMySwagger();

    app.UseSpaMiddleware();

    // Add Authentication to the pipeline
    app.UseAuthentication();

    app.UseMvc(); |\label{line:useMvc}|

    app.UseStaticFiles();
}
