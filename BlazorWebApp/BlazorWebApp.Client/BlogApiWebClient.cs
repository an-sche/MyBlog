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

    public async Task<List<BlogPost>> GetBlogPostsAsync(int numberOfPosts, int startIndex)
    {
        var client = _factory.CreateClient("Api");
        return await client.GetFromJsonAsync<List<BlogPost>>($"/api/BlogPosts?numberofposts={numberOfPosts}&startindex={startIndex}") ?? [];
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





    public Task DeleteBlogPostAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCategoryAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCommentAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTagAsync(string id)
    {
        throw new NotImplementedException();
    }




    public Task<List<Category>> GetCategoriesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Category?> GetCategoryAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Comment>> GetCommentsAsync(string blogPostId)
    {
        throw new NotImplementedException();
    }

    public Task<Tag?> GetTagAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Tag>> GetTagsAsync()
    {
        throw new NotImplementedException();
    }


    public Task<Category?> SaveCategoryAsync(Category item)
    {
        throw new NotImplementedException();
    }

    public Task<Comment?> SaveCommentAsync(Comment item)
    {
        throw new NotImplementedException();
    }

    public Task<Tag?> SaveTagAsync(Tag item)
    {
        throw new NotImplementedException();
    }
}
