using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;

        public CommentsController(ICommentRepository commentRepository, IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
        }

        // CREATE (POST)
        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto request)
        {
            await VerifyUserExistsAsync(request.UserId);

            var comment = new Comment(request.Body, request.PostId, request.UserId);
            var createdComment = await _commentRepository.AddAsync(comment);
            var user = await _userRepository.GetSingleAsync(request.UserId);

            var commentDto = new CommentDto
            {
                Id = createdComment.Id,
                Body = createdComment.Body,
                UserName = user.Username
            };

            return CreatedAtAction(nameof(GetComment), new { id = commentDto.Id }, commentDto);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(int id)
        {
            var comment = await _commentRepository.GetSingleAsync(id);
            if (comment == null)
                return NotFound();

            var user = await _userRepository.GetSingleAsync(comment.UserId);
            return new CommentDto
            {
                Id = comment.Id,
                Body = comment.Body,
                UserName = user.Username
            };
        }

        // GET ALL COMMENTS
        [HttpGet]
        public ActionResult<IEnumerable<CommentDto>> GetAllComments()
        {
            var comments = _commentRepository.GetManyAsync()
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Body = c.Body,
                    UserName = _userRepository.GetSingleAsync(c.UserId).Result.Username
                });

            return Ok(comments);
        }

        // GET COMMENTS BY POST ID FILTER
        [HttpGet("filterByPostId")]
        public ActionResult<IEnumerable<CommentDto>> GetCommentsByPostId([FromQuery] int postId)
        {
            var comments = _commentRepository.GetManyAsync()
                .Where(c => c.PostId == postId)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Body = c.Body,
                    UserName = _userRepository.GetSingleAsync(c.UserId).Result.Username
                });

            return Ok(comments);
        }

        // GET COMMENTS BY USER ID FILTER
        [HttpGet("filterByUserId")]
        public ActionResult<IEnumerable<CommentDto>> GetCommentsByUserId([FromQuery] int userId)
        {
            var comments = _commentRepository.GetManyAsync()
                .Where(c => c.UserId == userId)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Body = c.Body,
                    UserName = _userRepository.GetSingleAsync(c.UserId).Result.Username
                });

            return Ok(comments);
        }

        // GET COMMENTS BY USER NAME FILTER
        [HttpGet("filterByUserName")]
        public ActionResult<IEnumerable<CommentDto>> GetCommentsByUserName([FromQuery] string userName)
        {
            var comments = _commentRepository.GetManyAsync()
                .Where(c => _userRepository.GetSingleAsync(c.UserId).Result.Username == userName)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Body = c.Body,
                    UserName = _userRepository.GetSingleAsync(c.UserId).Result.Username
                });

            return Ok(comments);
        }
        
        // GET COMMENTS BY POST TITLE FILTER
        [HttpGet("filterByPostTitle")]
        public ActionResult<IEnumerable<CommentDto>> GetCommentsByPostTitle([FromQuery] string postTitle)
        {
            var comments = _commentRepository.GetManyAsync()
                .Join(_postRepository.GetManyAsync(),  // Join entre comentarios y posts
                    c => c.PostId,
                    p => p.Id,
                    (c, p) => new { Comment = c, Post = p })  // Combina comentarios con posts
                .Where(cp => cp.Post.Title.Contains(postTitle, StringComparison.OrdinalIgnoreCase))  // Filtra por título del post
                .Select(cp => new CommentDto
                {
                    Id = cp.Comment.Id,
                    Body = cp.Comment.Body,
                    UserName = _userRepository.GetSingleAsync(cp.Comment.UserId).Result.Username
                })
                .ToList();

            return Ok(comments);
        }

        // UPDATE (PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CreateCommentDto request)
        {
            var comment = await _commentRepository.GetSingleAsync(id);
            if (comment == null)
                return NotFound();

            comment.Body = request.Body;
            await _commentRepository.UpdateAsync(comment);

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            await _commentRepository.DeleteAsync(id);
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
