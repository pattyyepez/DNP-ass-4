using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class DeletePostsView
{
        private readonly IPostRepository postRepository;

        public DeletePostsView(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        public async Task ShowAsync()
        {
            Console.WriteLine();
            await DeletePostAsync();
        }

        public async Task DeletePostAsync()
        { 
            while (true)
        {
            Console.WriteLine("Insert Post ID to delete (or type 'Exit' to go back):");
            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Please insert a valid Post ID.");
                continue;
            }

            if (input.ToLower() == "exit")
            {
                return;
            }
            
            if (int.TryParse(input, out int postId))
            {
                try
                {
                    Post post = await postRepository.GetSingleAsync(postId);
                    Console.WriteLine($"Are you sure you want to delete the post '{post.Title}'? (y/n)");

                    string? confirmation = Console.ReadLine()?.ToLower();
                    if (confirmation == "y")
                    {
                        await postRepository.DeleteAsync(postId);
                        Console.WriteLine($"Post with ID {postId} deleted successfully.");
                        return;
                    }
                    else if (confirmation == "n")
                    {
                        Console.WriteLine("Post deletion canceled.");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid option, please try again.");
                    }
                }
                catch (Exceptions.NotFoundException)
                {
                    Console.WriteLine($"Post with ID {postId} does not exist. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format. Please enter a numeric Post ID.");
            }
        }
        }
}