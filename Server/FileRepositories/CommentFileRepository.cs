using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class CommentFileRepository : ICommentRepository
{
    private const string FilePath = "comments.json";
    
    public CommentFileRepository()
    {
        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, "[]"); 
        }
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        List<Comment> comments = await LoadCommentsAsync(); 
        comment.Id = comments.Count > 0 ? comments.Max(c => c.Id) + 1 : 1;
        comments.Add(comment);
        await SaveCommentsAsync(comments); 
        return comment;
    }

    private static Task SaveCommentsAsync(List<Comment> comments)
    {
        string commentsAsJson = JsonSerializer.Serialize(comments, new JsonSerializerOptions { WriteIndented = true }); 
        return File.WriteAllTextAsync(FilePath, commentsAsJson); 
    }

    
    private static async Task<List<Comment>> LoadCommentsAsync()
    {
        string commentsAsJson = await File.ReadAllTextAsync(FilePath);
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!; 
        return comments;
    }

    public async Task UpdateAsync(Comment comment)
    {
        List<Comment> comments = await LoadCommentsAsync();
        Comment existingComment = await GetSingleAsync(comment.Id); 

        comments.Remove(existingComment);
        comments.Add(comment);
        
        await SaveCommentsAsync(comments);
        
    }

    public async Task DeleteAsync(int id)
    {
        List<Comment> comments = await LoadCommentsAsync();
        Comment commentToRemove = await GetSingleAsync(id); 


        comments.Remove(commentToRemove);

        await SaveCommentsAsync(comments);
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        List<Comment> comments = await LoadCommentsAsync();
        Comment? comment = comments.SingleOrDefault(c => c.Id == id);
        if (comment is null)
        {
            throw new Exceptions.NotFoundException($"Comment with ID '{id}' not found");
        }

        return comment;
    }

    public IQueryable<Comment> GetManyAsync()
        => LoadCommentsAsync().Result.AsQueryable();
}