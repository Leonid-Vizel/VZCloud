using System.Net;
using VZCloud.Instrument;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc(options =>
{
    options.InputFormatters.Insert(0,new RawJsonBodyInputFormatter());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = ctx =>
    {
        if (ctx.File.PhysicalPath.Contains("storage"))
        {
            ctx.Context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            ctx.Context.Response.ContentLength = 0;
            ctx.Context.Response.Body = Stream.Null;
            ctx.Context.Response.Headers.Add("Cache-Control", "no-store");
        }
    }
});

app.MapControllerRoute("default", "{controller}/{action}");

app.Run();