using Entities;
using RepositoryContracts;
namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    private List<User> users = new List<User>();

    public UserInMemoryRepository()
    {
        _ = AddAsync(new User("pat", "1111")).Result;
        _ = AddAsync(new User("domi", "2222")).Result;
        _ = AddAsync(new User("joan", "3333")).Result;
        _ = AddAsync(new User("seba", "4444")).Result;
        _ = AddAsync(new User("kris", "5555")).Result;
    }

    public Task<User> AddAsync(User user)
    {
        user.Id = users.Any() ? users.Max(x => x.Id) + 1 : 1;
        users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        User? userToUpdate = users.SingleOrDefault(u=>u.Id == user.Id);
        if (userToUpdate is null)
        {
            throw new Exceptions.NotFoundException(
                $"User '{user.Id}' does not exist");
        }
        users.Remove(userToUpdate);
        users.Add(userToUpdate);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var userToDelete = users.SingleOrDefault(u => u.Id == id);
        if (userToDelete is null)
        {
            throw new Exceptions.NotFoundException(
                $"User '{id}' does not exist");
        }
        users.Remove(userToDelete);
        return Task.CompletedTask;
    }

    public Task<User> GetSingleAsync(int id)
    {
        var user = users.SingleOrDefault(u => u.Id == id);
        if (user is null)
        {
            throw new Exceptions.NotFoundException(
                $"User '{id}' does not exist");
        }
        return Task.FromResult(user);
    }
    public IQueryable<User> GetManyAsync()
    {
        return users.AsQueryable();
    }
}