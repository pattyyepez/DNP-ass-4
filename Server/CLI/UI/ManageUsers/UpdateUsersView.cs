using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class UpdateUsersView
{
   private readonly IUserRepository userRepository;

   public UpdateUsersView(IUserRepository userRepository)
   {
      this.userRepository = userRepository;
   }
   public async Task Show()
   {
      Console.WriteLine();
      await UpdateUserAsync();
   }
   private async Task UpdateUserAsync()
    {
        int userId = 0;
        while (userId <= 0)
        {
            Console.Write("Insert user ID to update: ");
            string? input = Console.ReadLine();
            
            if (int.TryParse(input, out userId) && userId > 0)
            {
                try
                {
                    User user = await userRepository.GetSingleAsync(userId);
                    
                    Console.WriteLine($"User found: ID = {user.Id}, Username = {user.Username}");

                    Console.Write("Insert new username: ");
                    string? newUserName = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newUserName))
                    {
                        user.Username = newUserName; 
                    }
                    Console.Write("Insert new password");
                    string? newPassword = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newPassword))
                    {
                        user.Password = newPassword;
                    }
                    await userRepository.UpdateAsync(user);
                    Console.WriteLine($"User {user.Id} updated successfully.");

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