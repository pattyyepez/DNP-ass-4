using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class DeleteUsersView
{
    private readonly IUserRepository userRepository;

    public DeleteUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task Show()
    {
        Console.WriteLine();
        await DeleteUserAsync();
    }

    private async Task DeleteUserAsync()
    {
        while (true)
        {
            Console.Write("Insert user ID to delete: ");
            string? input = Console.ReadLine(); 
            if (int.TryParse(input, out int userId) && userId > 0) 
            {
                try
                {
                    await userRepository.DeleteAsync(userId); 
                    Console.WriteLine($"User {userId} removed successfully.");
                    break;
                }
                catch (Exceptions.NotFoundException ex)
                {
                    Console.WriteLine(ex.Message); 
                }
            }
            else
            {
                Console.WriteLine("Insert a valid numeric ID."); 
            }
        }
    }

        
    
}