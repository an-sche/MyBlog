using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Data.Models;
using SharedComponents.Interfaces;

namespace BlazorWebApp.Client.Services;

public class BlazorWebAssemblyBlogNotificationService : IBlogNotificationService, IAsyncDisposable
{
    public event Action<BlogPost>? BlogPostChanged;
    private readonly HubConnection _hub;

    public BlazorWebAssemblyBlogNotificationService(NavigationManager navigation)
    {
        _hub = new HubConnectionBuilder()
            .WithUrl(navigation.ToAbsoluteUri("/BlogNotificationHub"))
            .Build();

        _hub.On<BlogPost>("BlogPostChanged", (post) =>
        {
            BlogPostChanged?.Invoke(post);
        });

        _hub.StartAsync();
    }

    public async Task SendNotification(BlogPost post)
    {
        await _hub.SendAsync("SendNotification", post);
    }

    public async ValueTask DisposeAsync()
    {
        await _hub.DisposeAsync();
    }
}
