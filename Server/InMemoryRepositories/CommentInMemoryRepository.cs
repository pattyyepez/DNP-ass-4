using Entities;
using RepositoryContracts;
namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    private readonly List<Comment> comments = new();

    public CommentInMemoryRepository()
    {
        _ = AddAsync(new Comment("comment1", 1, 2)).Result;
        _ = AddAsync(new Comment("comment2", 1, 3)).Result;
        _ = AddAsync(new Comment("comment3", 1, 4)).Result;
        _ = AddAsync(new Comment("comment4", 1, 5)).Result;
        _ = AddAsync(new Comment("comment5", 1, 1)).Result;
        _ = AddAsync(new Comment("comment1", 2, 2)).Result;
        _ = AddAsync(new Comment("comment2", 2, 3)).Result;
        _ = AddAsync(new Comment("comment3", 2, 4)).Result;
        _ = AddAsync(new Comment("comment4", 2, 5)).Result;
        _ = AddAsync(new Comment("comment5", 2, 1)).Result;
        _ = AddAsync(new Comment("comment1", 3, 2)).Result;
        _ = AddAsync(new Comment("comment2", 3, 3)).Result;
        _ = AddAsync(new Comment("comment3", 3, 4)).Result;
        _ = AddAsync(new Comment("comment4", 3, 5)).Result;
        _ = AddAsync(new Comment("comment5", 3, 1)).Result;
        _ = AddAsync(new Comment("comment1", 4, 2)).Result;
        _ = AddAsync(new Comment("comment2", 4, 3)).Result;
        _ = AddAsync(new Comment("comment3", 4, 4)).Result;
        _ = AddAsync(new Comment("comment4", 4, 5)).Result;
        _ = AddAsync(new Comment("comment5", 4, 1)).Result;
        _ = AddAsync(new Comment("comment1", 5, 2)).Result;
        _ = AddAsync(new Comment("comment2", 5, 3)).Result;
        _ = AddAsync(new Comment("comment3", 5, 4)).Result;
        _ = AddAsync(new Comment("comment4", 5, 5)).Result;
        _ = AddAsync(new Comment("comment5", 5, 1)).Result;
        
    }

    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = comments.Any() ? comments.Max(c => c.Id) + 1 : 1;
        comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        Comment? commentToUpdate = comments.SingleOrDefault(c=>c.Id == comment.Id);
        if (commentToUpdate is null)
        {
            
            throw new Exceptions.NotFoundException(
                $"Comment '{comment.Id}' does not exist");
        }

        comments.Remove(commentToUpdate);
        comments.Add(comment);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var commentToDelete = comments.SingleOrDefault(c => c.Id == id);
        if (commentToDelete is null)
        {
            throw new Exceptions.NotFoundException(
                $"Comment '{id}' does not exist");
        }
        comments.Remove(commentToDelete);
        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        var comment = comments.SingleOrDefault(c => c.Id == id);
        if (comment is null)
        {
            throw new Exceptions.NotFoundException(
                $"Comment '{comment.Id}' does not exist");
        }
        return Task.FromResult(comment);
    }
    public IQueryable<Comment> GetManyAsync()
    {
        return comments.AsQueryable();
    }
    
}