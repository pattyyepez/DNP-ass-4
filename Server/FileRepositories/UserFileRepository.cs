using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class UserFileRepository : IUserRepository
{
    private const string FilePath = "users.json";

    public UserFileRepository()
    {
        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, "[]");
        }
    }

    public async Task<User> AddAsync(User user)
    {
        List<User> users = await LoadUsersAsync();
        user.Id = users.Count > 0 ? users.Max(c => c.Id) + 1 : 1;
        users.Add(user);
        await SaveListAsync(users);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        List<User> users = await LoadUsersAsync();
        User? existingUser = users.SingleOrDefault(c => c.Id == user.Id);
        if (existingUser is null)
        {
            throw new Exceptions.NotFoundException($"User with ID '{user.Id}' not found");
        }
        
        users.Remove(existingUser);
        users.Add(user);
        
        await SaveListAsync(users);
    }

    public async Task DeleteAsync(int id)
    {
        List<User> users = await LoadUsersAsync();
        User? userToRemove = users.SingleOrDefault(c => c.Id == id);
        if (userToRemove is null)
        {
            throw new Exceptions.NotFoundException($"User with ID '{id}' not found");
        }
        
        users.Remove(userToRemove);
        await SaveListAsync(users);
    }

    public async Task<User> GetSingleAsync(int id)
    {
        List<User> users = await LoadUsersAsync();
        User? user = users.SingleOrDefault(c => c.Id == id);

        if (user is null) throw new Exceptions.NotFoundException($"User with ID '{id}' not found");
        
        return user;
    }

    public IQueryable<User> GetManyAsync()
        => LoadUsersAsync().Result.AsQueryable();

    private static async Task SaveListAsync(List<User> users)
    {
        string usersAsJson = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(FilePath, usersAsJson);
    }

    private static async Task<List<User>> LoadUsersAsync()
    {
        if (!File.Exists(FilePath)) return new List<User>(); 

        string usersAsJson = await File.ReadAllTextAsync(FilePath);
        return JsonSerializer.Deserialize<List<User>>(usersAsJson) ?? new List<User>();  }

}