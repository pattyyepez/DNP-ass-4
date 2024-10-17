using RepositoryContracts;
using Entities;

namespace CLI.UI.ManageUsers;

public class ListUsersView
{
    private readonly IUserRepository userRepository;

    public ListUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public void Show()
    {
        Console.WriteLine();
        ViewUsersAsync();
    }

    private void ViewUsersAsync()
    {
        IEnumerable<User> usersFromRepo = userRepository.GetManyAsync();
        List<User> users = usersFromRepo.OrderBy(u=>u.Id).ToList();
        Console.WriteLine("List of users:");
        foreach (User user in users)
        {
            Console.WriteLine($"\t ID: {user.Id} Username: {user.Username}");
        }
        Console.WriteLine();

    }
}