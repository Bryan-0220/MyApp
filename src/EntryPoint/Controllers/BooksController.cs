using Microsoft.AspNetCore.Mvc;
using CreateBook;
using DeleteBook;
using UpdateBook;
using GetBookById;
using GetAllBooks;
using FilterBooks;

namespace EntryPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ICreateBookCommandHandler _createBookHandler;
        private readonly IDeleteBookCommandHandler _deleteBookHandler;
        private readonly IUpdateBookCommandHandler _updateBookHandler;
        private readonly IGetBookByIdQueryHandler _getBookByIdQueryHandler;
        private readonly IGetAllBooksQueryHandler _getAllBooksQueryHandler;
        private readonly IFilterBooksQueryHandler _filterBooksQueryHandler;

        public BooksController(
            ICreateBookCommandHandler createBookHandler,
            IDeleteBookCommandHandler deleteBookHandler,
            IUpdateBookCommandHandler updateBookHandler,
            IGetBookByIdQueryHandler getBookByIdQueryHandler,
            IGetAllBooksQueryHandler getAllBooksQueryHandler,
            IFilterBooksQueryHandler filterBooksQueryHandler)
        {
            _createBookHandler = createBookHandler;
            _deleteBookHandler = deleteBookHandler;
            _updateBookHandler = updateBookHandler;
            _getBookByIdQueryHandler = getBookByIdQueryHandler;
            _getAllBooksQueryHandler = getAllBooksQueryHandler;
            _filterBooksQueryHandler = filterBooksQueryHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookCommandInput input)
        {
            var result = await _createBookHandler.Handle(input, HttpContext.RequestAborted);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateBookCommandInput input)
        {
            input.Id = id;
            var updated = await _updateBookHandler.Handle(input, HttpContext.RequestAborted);
            if (updated is null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _deleteBookHandler.Handle(new DeleteBookCommandInput { Id = id }, HttpContext.RequestAborted);
            if (!result.Success) return NotFound();
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _getAllBooksQueryHandler.Handle(new GetAllBooksQueryInput(), HttpContext.RequestAborted);
            return Ok(books);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] string? genre)
        {
            var books = await _filterBooksQueryHandler.Handle(new FilterBooksQueryInput { Genre = genre }, HttpContext.RequestAborted);
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var book = await _getBookByIdQueryHandler.Handle(new GetBookByIdQueryInput { Id = id }, HttpContext.RequestAborted);
            if (book == null) return NotFound();
            return Ok(book);
        }
    }
}
