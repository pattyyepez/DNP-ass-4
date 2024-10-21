using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICommentRepository _commentRepository;

        public PostsController(IPostRepository postRepository, IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        // CREATE (POST)
        [HttpPost]
        public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto request)
        {
            await VerifyUserExistsAsync(request.UserId);

            var post = new Post(request.Title, request.Content, request.UserId);
            var createdPost = await _postRepository.AddAsync(post);
            var user = await _userRepository.GetSingleAsync(request.UserId);

            var postDto = new PostDto
            {
                Id = createdPost.Id,
                Title = createdPost.Title,
                Content = createdPost.Content,
                UserName = user.Username
            };

            return CreatedAtAction(nameof(GetPost), new { id = postDto.Id }, postDto);
        }

        // GET BY ID WITH OPTIONAL COMMENTS
        [HttpGet("{id}")]
        public async Task<ActionResult<PostWithCommentsDto>> GetPost(int id, [FromQuery] bool includeComments = false)
        {
            // Obtener el post por ID
            var post = await _postRepository.GetSingleAsync(id);
            if (post == null)
                return NotFound();

            // Obtener el nombre del usuario que creó el post
            var user = await _userRepository.GetSingleAsync(post.UserId);

            // Crear el objeto DTO que incluye el post y sus comentarios (si se requiere)
            var postWithCommentsDto = new PostWithCommentsDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                UserName = user.Username
            };

            // Si includeComments es verdadero, agregar los comentarios
            if (includeComments)
            {
                var comments = _commentRepository.GetManyAsync()
                    .Where(c => c.PostId == id)
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Body = c.Body,
                        UserName = _userRepository.GetSingleAsync(c.UserId).Result.Username
                    }).ToList();

                postWithCommentsDto.Comments = comments;
            }

            return Ok(postWithCommentsDto);
        }

        // GET ALL POSTS
        [HttpGet]
        public ActionResult<IEnumerable<PostDto>> GetAllPosts()
        {
            var posts = _postRepository.GetManyAsync()
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    UserName = _userRepository.GetSingleAsync(p.UserId).Result.Username
                });

            return Ok(posts);
        }

        // GET POSTS BY TITLE FILTER
        [HttpGet("filterByTitle")]
        public ActionResult<IEnumerable<PostDto>> GetPostsByTitle([FromQuery] string titleContains)
        {
            var posts = _postRepository.GetManyAsync()
                .Where(p => p.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase))
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    UserName = _userRepository.GetSingleAsync(p.UserId).Result.Username
                });

            return Ok(posts);
        }

        // GET POSTS BY USER ID FILTER
        [HttpGet("filterByUserId")]
        public ActionResult<IEnumerable<PostDto>> GetPostsByUserId([FromQuery] int userId)
        {
            var posts = _postRepository.GetManyAsync()
                .Where(p => p.UserId == userId)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    UserName = _userRepository.GetSingleAsync(p.UserId).Result.Username
                });

            return Ok(posts);
        }

        // GET POSTS BY USER NAME FILTER
        [HttpGet("filterByUserName")]
        public ActionResult<IEnumerable<PostDto>> GetPostsByUserName([FromQuery] string userName)
        {
            var posts = _postRepository.GetManyAsync()
                .Where(p => _userRepository.GetSingleAsync(p.UserId).Result.Username == userName)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    UserName = _userRepository.GetSingleAsync(p.UserId).Result.Username
                });

            return Ok(posts);
        }
        
        // GET TOP 10 MOST COMMENTED POSTS
        [HttpGet("topCommented")]
        public ActionResult<IEnumerable<PostDto>> GetTop10MostCommentedPosts()
        {
            var postCommentsCount = _commentRepository.GetManyAsync()
                .GroupBy(c => c.PostId)        // Agrupa comentarios por PostId
                .Select(g => new { PostId = g.Key, CommentCount = g.Count() })  // Cuenta los comentarios de cada post
                .OrderByDescending(g => g.CommentCount)  // Ordena por número de comentarios
                .Take(10)  // Toma los 10 posts con más comentarios
                .ToList();

            var topPosts = postCommentsCount
                .Join(_postRepository.GetManyAsync(),  // Haz un join con los posts
                    pc => pc.PostId,
                    p => p.Id,
                    (pc, p) => new PostDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Content = p.Content,
                        UserName = _userRepository.GetSingleAsync(p.UserId).Result.Username
                    })
                .ToList();

            return Ok(topPosts);
        }


        // UPDATE (PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] CreatePostDto request)
        {
            var post = await _postRepository.GetSingleAsync(id);
            if (post == null)
                return NotFound();

            post.Title = request.Title;
            post.Content = request.Content;
            await _postRepository.UpdateAsync(post);

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _postRepository.DeleteAsync(id);
            return NoContent();
        }

        // Método auxiliar para verificar si el usuario existe
        private async Task VerifyUserExistsAsync(int userId)
        {
            var user = await _userRepository.GetSingleAsync(userId);
            if (user == null)
            {
                throw new Exceptions.ValidationException($"The user with ID '{userId}' does not exist.");
            }
        }
    }
}
