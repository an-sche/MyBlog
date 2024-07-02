using System;
using Data.Models;
using Data.Models.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBlog.Tests;

public class BlogApiMock : IBlogApi
{
    public Task DeleteBlogPostAsync(string id)
    {
        return Task.CompletedTask;
    }

    public Task DeleteCategoryAsync(string id)
    {
        return Task.CompletedTask;
    }

    public Task DeleteCommentAsync(string id)
    {
        return Task.CompletedTask;
    }

    public Task DeleteTagAsync(string id)
    {
        return Task.CompletedTask;
    }

    public async Task<BlogPost?> GetBlogPostAsync(string id)
    {
        var post = new BlogPost()
        {
            Id = id,
            Text = $"This is blog post no {id}",
            Title = $"Blogpost {id}",
            PublishDate = DateTime.Now,
            Category = await GetCategoryAsync("1")
        };
        var t1 = await GetTagAsync("1");
        if (t1 is not null)
            post.Tags.Add(t1);
        var t2 = await GetTagAsync("2");
        if (t2 is not null)
            post.Tags.Add(t2);
        return post;
    }

    public Task<int> GetBlogPostCountAsync()
    {
        return Task.FromResult(10);
    }

    public async Task<List<BlogPost>> GetBlogPostsAsync(int numberOfPosts, int startIndex)
    {
        var posts = new List<BlogPost>();
        for (int i = 0; i < numberOfPosts; i++)
        {
            posts.Add((await GetBlogPostAsync($"{startIndex + i}"))!);
        }
        return posts;
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        var categories = new List<Category>();
        for (int i = 0; i < 10; i++)
        {
            categories.Add((await GetCategoryAsync($"{i}"))!);
        }
        return categories;
    }

    public Task<Category?> GetCategoryAsync(string id)
    {
        var category = new Category()
        {
            Id = id,
            Name = $"Category {id}"
        };
        return Task.FromResult<Category?>(category);
    }

    public Task<List<Comment>> GetCommentsAsync(string blogPostId)
    {
        var comments = new List<Comment>
        {
            new Comment() { Id = "comment1", BlogPostId = blogPostId, Date = DateTime.Now, Name = "William", Text = "Woof!" },
            new Comment() { Id = "comment2", BlogPostId = blogPostId, Date = DateTime.Now, Name = "Koda", Text = "Bark!" }
        };

        return Task.FromResult(comments);
    }

    public Task<Tag?> GetTagAsync(string id)
    {
        var tag = new Tag()
        {
            Id = id,
            Name = $"Tag {id}"
        };
        return Task.FromResult<Tag?>(tag);
    }

    public async Task<List<Tag>> GetTagsAsync()
    {
        var tags = new List<Tag>();

        for (int i = 0; i < 10; i++)
        {
            var t = await GetTagAsync($"{i}");
            tags.Add(t!);
        }

        return tags;
    }

    public Task<BlogPost?> SaveBlogPostAsync(BlogPost item)
    {
        return Task.FromResult<BlogPost?>(item);
    }

    public Task<Category?> SaveCategoryAsync(Category item)
    {
        return Task.FromResult<Category?>(item);
    }

    public Task<Comment?> SaveCommentAsync(Comment item)
    {
        return Task.FromResult<Comment?>(item);
    }

    public Task<Tag?> SaveTagAsync(Tag item)
    {
        return Task.FromResult<Tag?>(item);
    }
}
