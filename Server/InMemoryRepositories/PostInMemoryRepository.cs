using Entities;
using RepositoryContracts;
namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    private readonly List<Post> posts = new();
    
    public PostInMemoryRepository()
    {
        _ = AddAsync(new Post("ADS", "Algorithms and Data Structures", 1)).Result;
        _ = AddAsync(new Post("DNP", ".NET Programming", 2)).Result;
        _ = AddAsync(new Post("CAO", "Computer Architecture and Organization", 4)).Result;
        _ = AddAsync(new Post("PRO", "SDJ", 3)).Result;
        _ = AddAsync(new Post("SEP", "Semester project: Heterogeneous Systems.", 4)).Result;
    }
    
    public Task<Post> AddAsync(Post post)
    {
        post.Id = posts.Any() ? posts.Max(p => p.Id) + 1 : 1;
        posts.Add(post);
        return Task.FromResult(post);
    }
    
    public Task UpdateAsync(Post post)
    {
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException(
                $"Post '{post.Id}' does not exist");
        }

        posts.Remove(existingPost);
        posts.Add(post);

        return Task.CompletedTask;
    }
    public Task DeleteAsync(int id)
    {
        var postToRemove = posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException(
                $"Post '{id}' does not exist");
        }

        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    public Task<Post> GetSingleAsync(int id)
    {
        var post = posts.SingleOrDefault(p => p.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException(
                $"Post '{id}' does not exist");
        }
        return Task.FromResult(post);
    }
    public IQueryable<Post> GetManyAsync()
    {
        return posts.AsQueryable();
    }


}