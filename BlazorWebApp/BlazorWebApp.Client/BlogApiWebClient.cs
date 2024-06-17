using Data.Models;
using Data.Models.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorWebApp.Client;

public class BlogApiWebClient : IBlogApi
{
    public readonly IHttpClientFactory _factory;
    public BlogApiWebClient(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<BlogPost?> GetBlogPostAsync(string id)
    {
        var client = _factory.CreateClient("Api");
        return await client.GetFromJsonAsync<BlogPost>($"/api/BlogPosts/{id}");
    }

    public async Task<int> GetBlogPostCountAsync()
    {
        var client = _factory.CreateClient("Api");
        return await client.GetFromJsonAsync<int>($"/api/BlogPostsCount");
    }

    public async Task<List<BlogPost>?> GetBlogPostsAsync(int numberOfPosts, int startIndex)
    {
        var client = _factory.CreateClient("Api");
        return await client.GetFromJsonAsync<List<BlogPost>>($"/api/BlogPosts?numberofposts={numberOfPosts}&startindex={startIndex}");
    }

    public async Task<BlogPost?> SaveBlogPostAsync(BlogPost item)
    {
        try 
        {
            var client = _factory.CreateClient("Api");
            var response = await client.PutAsJsonAsync("/api/BlogPosts", item);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BlogPost>(json);
        }
        catch (AccessTokenNotAvailableException e) 
        {
            e.Redirect();
        }
        return null;
    }

    public async Task DeleteBlogPostAsync(string id)
    {
        try
        {
            var client = _factory.CreateClient("Api");
            await client.DeleteAsync($"/api/BlogPosts/{id}");
        }
        catch (AccessTokenNotAvailableException e)
        {
            e.Redirect();
        }
    }

    public async Task<List<Category>?> GetCategoriesAsync()
    {
        var client = _factory.CreateClient("Api");
        return await client.GetFromJsonAsync<List<Category>>($"/api/Categories");
    }

    public async Task<Category?> GetCategoryAsync(string id)
    {
        var client = _factory.CreateClient("Api");
        return await client.GetFromJsonAsync<Category>($"/api/Categories/{id}");
    }

    public async Task DeleteCategoryAsync(string id)
    {
        try
        {
            var client = _factory.CreateClient("Api");
            await client.DeleteAsync($"/api/Categories/{id}");
        }
        catch (AccessTokenNotAvailableException e)
        {
            e.Redirect();
        }
    }

    public async Task<Category?> SaveCategoryAsync(Category item)
    {
        try
        {
            var client = _factory.CreateClient("Api");
            var response = await client.PutAsJsonAsync($"/api/Categories", item);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Category>(json);            
        }
        catch (AccessTokenNotAvailableException e)
        {
            e.Redirect();
        }
        return null;
    }

    public async Task<Tag?> GetTagAsync(string id)
    {
        var client = _factory.CreateClient("Api");
        return await client.GetFromJsonAsync<Tag>($"/api/Tags/{id}");
    }
    public async Task<List<Tag>?> GetTagsAsync()
    {
        var client = _factory.CreateClient("Api");
        return await client.GetFromJsonAsync<List<Tag>>($"/api/Tags");
    }

    public async Task DeleteTagAsync(string id)
    {
        try 
        {
            var client = _factory.CreateClient("Api");
            await client.DeleteAsync($"/api/Tags/{id}");
        }
        catch (AccessTokenNotAvailableException e)
        {
            e.Redirect();
        }
    }

    public async Task<Tag?> SaveTagAsync(Tag item)
    {
        try
        {
            var client = _factory.CreateClient("Api");
            var response = await client.PutAsJsonAsync($"/api/Tags", item);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Tag>(json);
        }
        catch (AccessTokenNotAvailableException e)
        {
            e.Redirect();
        }
        return null;
    }

    public async Task<List<Comment>?> GetCommentsAsync(string blogPostId)
    {
        var client = _factory.CreateClient("Api");
        return await client.GetFromJsonAsync<List<Comment>>($"/api/Comments/{blogPostId}");
    }

    public async Task<Comment?> SaveCommentAsync(Comment item)
    {
        try
        {
            var client = _factory.CreateClient("Api");
            var response = await client.PutAsJsonAsync($"/api/Comments", item);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Comment>(json);            
        }
        catch (AccessTokenNotAvailableException e)
        {
            e.Redirect();
        }
        return null;
    }

    public async Task DeleteCommentAsync(string id)
    {
        try
        {
            var client = _factory.CreateClient("Api");            
            await client.DeleteAsync($"/api/Comments/{id}");
        }
        catch (AccessTokenNotAvailableException e)
        {
            e.Redirect();
        }
    }
}
