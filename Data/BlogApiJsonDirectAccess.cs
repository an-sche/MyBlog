using Data.Models;
using Data.Models.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Data;

public class BlogApiJsonDirectAccess : IBlogApi
{
    private readonly BlogApiJsonDirectAccessSetting _settings;

    public BlogApiJsonDirectAccess(IOptions<BlogApiJsonDirectAccessSetting> options)
    {
        _settings = options.Value;
        
        ManageDataPaths();
    }

    private void ManageDataPaths() 
    {
        CreateDirectoryIfNotExists(_settings.DataPath);
        CreateDirectoryIfNotExists($@"{_settings.DataPath}\{_settings.BlogPostsFolder}");
        CreateDirectoryIfNotExists($@"{_settings.DataPath}\{_settings.CategoriesFolder}");
        CreateDirectoryIfNotExists($@"{_settings.DataPath}\{_settings.TagsFolder}");
        CreateDirectoryIfNotExists($@"{_settings.DataPath}\{_settings.CommentsFolder}");
    }

    private static void CreateDirectoryIfNotExists(string path) 
    {
        if (!Directory.Exists(path)) 
        {
            Directory.CreateDirectory(path);
        }
    }

    private async Task<List<T>> LoadAsync<T>(string folder) 
    {
        var list = new List<T>();
        foreach (var f in Directory.GetFiles($@"{_settings.DataPath}\{folder}")) 
        {
            var json = await File.ReadAllTextAsync(f);
            var item = JsonSerializer.Deserialize<T>(json);
            if (item is not null) 
            {
                list.Add(item);
            }
        }
        return list;
    }

    private async Task SaveAsync<T>(string folder, string fileName, T item) 
    {
        var filepath = $@"{_settings.DataPath}\{folder}\{fileName}";
        await File.WriteAllTextAsync(filepath, JsonSerializer.Serialize(item));
    }

    private Task DeleteAsync(string folder, string fileName) 
    {
        var filepath = $@"{_settings.DataPath}\{folder}\{fileName}";
        if (File.Exists(filepath)) 
        {
            File.Delete(filepath);
        }
        return Task.CompletedTask;
    }

    public async Task<int> GetBlogPostCountAsync()
    {
        var list = await LoadAsync<BlogPost>(_settings.BlogPostsFolder);
        return list.Count;
    }

    public async Task<List<BlogPost>> GetBlogPostsAsync(int numberOfPosts, int startIndex)
    {
        var list = await LoadAsync<BlogPost>(_settings.BlogPostsFolder);
        return list.Skip(startIndex).Take(numberOfPosts).ToList();
    }

    public async Task<BlogPost?> GetBlogPostAsync(string id)
    {
        var list = await LoadAsync<BlogPost>(_settings.BlogPostsFolder);
        return list.FirstOrDefault(x => x.Id == id);
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await LoadAsync<Category>(_settings.CategoriesFolder);
    }

    public async Task<Category?> GetCategoryAsync(string id)
    {
        var list = await LoadAsync<Category>(_settings.CategoriesFolder);
        return list.FirstOrDefault(x => x.Id == id);
    }

    public async Task<List<Tag>> GetTagsAsync()
    {
        return await LoadAsync<Tag>(_settings.TagsFolder);
    }

    public async Task<Tag?> GetTagAsync(string id)
    {
        var list = await LoadAsync<Tag>(_settings.TagsFolder);
        return list.FirstOrDefault(x => x.Id == id);
    }

    public async Task<List<Comment>> GetCommentsAsync(string blogPostId)
    {
        var list = await LoadAsync<Comment>(_settings.CommentsFolder);
        return list.Where(x => x.BlogPostId == blogPostId).ToList();
    }

    public async Task<BlogPost?> SaveBlogPostAsync(BlogPost item)
    {
        item.Id ??= Guid.NewGuid().ToString();
        await SaveAsync(_settings.BlogPostsFolder, item.Id, item);
        return item;
    }

    public async Task<Category?> SaveCategoryAsync(Category item)
    {
        item.Id ??= Guid.NewGuid().ToString();
        await SaveAsync(_settings.CategoriesFolder, item.Id, item);
        return item;
    }

    public async Task<Comment?> SaveCommentAsync(Comment item)
    {
        item.Id ??= Guid.NewGuid().ToString();
        await SaveAsync(_settings.CommentsFolder, item.Id, item);
        return item;
    }

    public async Task<Tag?> SaveTagAsync(Tag item)
    {
        item.Id ??= Guid.NewGuid().ToString();
        await SaveAsync(_settings.TagsFolder, item.Id, item);
        return item;
    }

    public async Task DeleteBlogPostAsync(string id)
    {
        await DeleteAsync(_settings.BlogPostsFolder, id);
        var comments = await GetCommentsAsync(id);
        foreach (var c in comments) 
        {
            if (c.Id != null) 
            {
                await DeleteAsync(_settings.CommentsFolder, c.Id);
            }
        }
    }

    public async Task DeleteCategoryAsync(string id)
    {
        await DeleteAsync(_settings.CategoriesFolder, id);
    }

    public async Task DeleteCommentAsync(string id)
    {
        await DeleteAsync(_settings.CommentsFolder, id);
    }

    public async Task DeleteTagAsync(string id)
    {
        await DeleteAsync(_settings.TagsFolder, id);
    }
}