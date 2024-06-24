using Data;
using Data.Models.Interfaces;
using BlazorWebApp.Client.Pages;
using BlazorWebApp.Components;
using BlazorWebApp.Endpoints;

using BlazorWebApp.Client;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

// builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });

builder.Services.AddOptions<BlogApiJsonDirectAccessSetting>().Configure(options =>
{
	options.DataPath = @"..\..\DataBase";
	options.BlogPostsFolder = "BlogPosts";
	options.TagsFolder = "Tags";
	options.CategoriesFolder = "Categories";
	options.CommentsFolder = "Comments";
});

builder.Services.AddScoped<IBlogApi, BlogApiJsonDirectAccess>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingServerAuthenticationSateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuth0WebAppAuthentication(options => 
{
	options.Domain = builder.Configuration["Auth0:Authority"] ?? string.Empty;
	options.ClientId = builder.Configuration["Auth0:ClientId"] ?? string.Empty;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly)
    .AddAdditionalAssemblies(typeof(SharedComponents.Pages.Home).Assembly);
	//.AddAdditionalAssemblies(typeof(BlazorWebApp.Client._Imports).Assembly) // Adding these "WASM" components allows for SSR to work in "hybrid" // Book had typeof(Counter) here as well
	//.AddAdditionalAssemblies(typeof(Counter).Assembly)
	//.AddAdditionalAssemblies(typeof(SharedComponents._Imports).Assembly); // the book suggested using SharedComponents.Pages.Home
	// comment

app.MapBlogPostApi();
app.MapCategoryApi();
app.MapTagApi();
app.MapCommentApi();

app.MapGet("account/login", async (string returnUrl, HttpContext context) => {
	var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
		.WithRedirectUri(returnUrl)
		.Build();

	await context.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapGet("authentication/logout", async (HttpContext context) => {
	var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
		.WithRedirectUri("/")
		.Build();
	await context.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
	await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});



app.Run();