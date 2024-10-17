using RepositoryContracts;
using Entities;

namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private readonly IUserRepository userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task Show()
    {
        Console.WriteLine();
        await CreateUserAsync();
    }

    private async Task AddUserAsync(string username, string password)
    {
        User user = new User(username, password);
        User userCreated = await userRepository.AddAsync(user);
        Console.WriteLine($"User {userCreated.Id} created successfully.");
    }
    private async Task CreateUserAsync()
    {
        while (true)
        {
            Console.Write("Insert username: ");
            string? name = null;
            while (string.IsNullOrEmpty(name))
            {
               name = Console.ReadLine();
               if (string.IsNullOrEmpty(name))
               {
                   Console.WriteLine("Insert a valid username.");
               }
            }
            Console.Write("Insert password: ");
            string? password = null;

            while (string.IsNullOrEmpty(password))
            {
                password = Console.ReadLine();
                if (string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Insert a valid password.");
                }
            }
            
            Console.WriteLine($"User name: {name}");
            Console.WriteLine($"Password: {password}");
            await AddUserAsync(name, password);
        }
        
    }
}