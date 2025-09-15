using Microsoft.AspNetCore.Mvc;
using CreateAuthor;
using DeleteAuthor;
using UpdateAuthor;
using GetAuthorById;
using GetAllAuthors;
using FilterAuthors;

namespace EntryPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICreateAuthorCommandHandler _createAuthorHandler;
        private readonly IDeleteAuthorCommandHandler _deleteAuthorHandler;
        private readonly IUpdateAuthorCommandHandler _updateAuthorHandler;
        private readonly IGetAuthorByIdQueryHandler _getAuthorByIdQueryHandler;
        private readonly IGetAllAuthorsQueryHandler _getAllAuthorsQueryHandler;
        private readonly IFilterAuthorsQueryHandler _filterAuthorsQueryHandler;

        public AuthorsController(
            ICreateAuthorCommandHandler createAuthorHandler,
            IDeleteAuthorCommandHandler deleteAuthorHandler,
            IUpdateAuthorCommandHandler updateAuthorHandler,
            IGetAuthorByIdQueryHandler getAuthorByIdQueryHandler,
            IGetAllAuthorsQueryHandler getAllAuthorsQueryHandler,
            IFilterAuthorsQueryHandler filterAuthorsQueryHandler)
        {
            _createAuthorHandler = createAuthorHandler;
            _deleteAuthorHandler = deleteAuthorHandler;
            _updateAuthorHandler = updateAuthorHandler;
            _getAuthorByIdQueryHandler = getAuthorByIdQueryHandler;
            _getAllAuthorsQueryHandler = getAllAuthorsQueryHandler;
            _filterAuthorsQueryHandler = filterAuthorsQueryHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAuthorCommandInput input)
        {
            var result = await _createAuthorHandler.Handle(input, HttpContext.RequestAborted);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateAuthorCommandInput input)
        {
            input.Id = id;
            var updated = await _updateAuthorHandler.Handle(input, HttpContext.RequestAborted);
            if (updated is null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _deleteAuthorHandler.Handle(new DeleteAuthorCommandInput { Id = id }, HttpContext.RequestAborted);
            if (!result.Success) return NotFound();
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var authors = await _getAllAuthorsQueryHandler.Handle(new GetAllAuthorsQueryInput(), HttpContext.RequestAborted);
            return Ok(authors);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] string[]? genres, [FromQuery] string? name, [FromQuery] string? nationality)
        {
            var input = new FilterAuthorsQueryInput { Genres = genres, Name = name, Nationality = nationality };
            var authors = await _filterAuthorsQueryHandler.Handle(input, HttpContext.RequestAborted);
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var author = await _getAuthorByIdQueryHandler.Handle(new GetAuthorByIdQueryInput { Id = id }, HttpContext.RequestAborted);
            if (author == null) return NotFound();
            return Ok(author);
        }
    }
}
