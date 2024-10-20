using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ManageUsersView
{
    private readonly IUserRepository userRepository;

    public ManageUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    public async Task ShowAsync()
    {
        Console.WriteLine();
        while (true)
        {
            Console.WriteLine("Please select:\n" +
                              "1) Create new user\n" +
                              "2) Update user\n" +
                              "3) Delete user\n " +
                              "4) View users\n" +
                              "5) Exit");
            string number = Console.ReadLine() ?? "";
            if (string.IsNullOrEmpty(number))
            {
                Console.WriteLine("Please select an option.\n\n");
                continue;
            }

            if ("5".Equals(number))
            {
                return;
            }

            switch (number)
            {
                case "1":
                    await new CreateUserView(userRepository).Show();
                    break;
                case "2":
                    await new UpdateUsersView(userRepository).Show();
                    break;
                case "3":
                    await new DeleteUsersView(userRepository).Show();
                    break;
                case "4":
                    new ListUsersView(userRepository).Show();
                    break;
                default:
                    Console.WriteLine("Invalid option, please try again.\n\n");
                    break;
            }
        }
    }
}