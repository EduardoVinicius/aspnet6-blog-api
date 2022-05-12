using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class PostController : ControllerBase
    {
        [HttpGet("v1/posts")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 25)
        {
            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();
                var posts = await context
                    .Posts
                    .AsNoTracking()
                    .Include(p => p.Author)
                    .Include(p => p.Category)
                    .Select(p => new ListPostsViewModel
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Slug = p.Slug,
                        LastUpdateDate = p.LastUpdateDate,
                        Category = p.Category.Name,
                        Author = $"{p.Author.Name} ({p.Author.Email})"
                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(x => x.LastUpdateDate)
                    .ToListAsync();
                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<List<Post>>("05X04 - Internal server fault."));
            }
        }
    
        [HttpGet("v1/posts/{id:int}")]
        public async Task<IActionResult> DetailsAsync(
            [FromServices] BlogDataContext context,
            [FromRoute] int id)
        {
            try
            {
                var post = await context
                    .Posts
                    .AsNoTracking()
                    .Include(p => p.Author)
                    .ThenInclude(a => a.Roles)
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (post == null)
                    return NotFound(new ResultViewModel<Post>("Content not found!"));

                return Ok(new ResultViewModel<Post>(post));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Post>("05X04 - Internal server error."));
            }
        }

        [HttpGet("v1/posts/category/{categories}")]
        public async Task<IActionResult> GetByCategoryAsync(
            [FromRoute] string category,
            [FromServices] BlogDataContext context,
            [FromQuery] int page,
            [FromQuery] int pageSize)
        {
            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();
                var posts = await context
                    .Posts
                    .AsNoTracking()
                    .Include(p => p.Author)
                    .Include(p => p.Category)
                    .Where(p => p.Category.Slug == category)
                    .Select(p => new ListPostsViewModel
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Slug = p.Slug,
                        LastUpdateDate = p.LastUpdateDate,
                        Category = p.Category.Name,
                        Author = $"{p.Author.Name} ({p.Author.Email})"
                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(x => x.LastUpdateDate)
                    .ToListAsync();
                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<List<Post>>("05X04 - Internal server fault."));
            }
        }
    }
}
