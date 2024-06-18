using Data;
using Data.Models.Interfaces;
using BlazorWebApp.Client.Pages;
using BlazorWebApp.Components;
using BlazorWebApp.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddOptions<BlogApiJsonDirectAccessSetting>().Configure(options =>
{
	options.DataPath = @"..\..\DataBase\";
	options.BlogPostsFolder = "BlogPosts";
	options.TagsFolder = "Tags";
	options.CategoriesFolder = "Categories";
	options.CommentsFolder = "Comments";
});
builder.Services.AddScoped<IBlogApi, BlogApiJsonDirectAccess>();

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

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(BlazorWebApp.Client._Imports).Assembly) // Adding these "WASM" components allows for SSR to work in "hybrid" // Book had typeof(Counter) here as well
	// .AddAdditionalAssemblies(typeof(Counter).Assembly)
	.AddAdditionalAssemblies(typeof(SharedComponents._Imports).Assembly); // the book suggested using SharedComponents.Pages.Home
	// comment

app.MapBlogPostApi();
app.MapCategoryApi();
app.MapTagApi();
app.MapCommentApi();

app.Run();
