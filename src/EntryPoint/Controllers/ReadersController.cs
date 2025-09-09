using Microsoft.AspNetCore.Mvc;
using CreateReader;
using DeleteReader;
using UpdateReader;
using GetReaderById;
using GetAllReaders;
using FilterReaders;

namespace EntryPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReadersController : ControllerBase
    {
        private readonly ICreateReaderCommandHandler _createReaderHandler;
        private readonly IDeleteReaderCommandHandler _deleteReaderHandler;
        private readonly IUpdateReaderCommandHandler _updateReaderHandler;
        private readonly IGetReaderByIdQueryHandler _getReaderByIdQueryHandler;
        private readonly IGetAllReadersQueryHandler _getAllReadersQueryHandler;
        private readonly IFilterReadersQueryHandler _filterReadersQueryHandler;

        public ReadersController(
            ICreateReaderCommandHandler createReaderHandler,
            IDeleteReaderCommandHandler deleteReaderHandler,
            IUpdateReaderCommandHandler updateReaderHandler,
            IGetReaderByIdQueryHandler getReaderByIdQueryHandler,
            IGetAllReadersQueryHandler getAllReadersQueryHandler,
            IFilterReadersQueryHandler filterReadersQueryHandler)
        {
            _createReaderHandler = createReaderHandler;
            _deleteReaderHandler = deleteReaderHandler;
            _updateReaderHandler = updateReaderHandler;
            _getReaderByIdQueryHandler = getReaderByIdQueryHandler;
            _getAllReadersQueryHandler = getAllReadersQueryHandler;
            _filterReadersQueryHandler = filterReadersQueryHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReaderCommandInput input)
        {
            var result = await _createReaderHandler.HandleAsync(input, HttpContext.RequestAborted);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateReaderCommandInput input)
        {
            input.Id = id;
            var updated = await _updateReaderHandler.HandleAsync(input, HttpContext.RequestAborted);
            if (updated is null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _deleteReaderHandler.HandleAsync(new DeleteReaderCommandInput { Id = id }, HttpContext.RequestAborted);
            if (!result.Success) return NotFound();
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var readers = await _getAllReadersQueryHandler.HandleAsync(new GetAllReadersQueryInput(), HttpContext.RequestAborted);
            return Ok(readers);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] string? firstName, [FromQuery] string? lastName)
        {
            var input = new FilterReadersQueryInput { FirstName = firstName, LastName = lastName };
            var readers = await _filterReadersQueryHandler.HandleAsync(input, HttpContext.RequestAborted);
            return Ok(readers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var reader = await _getReaderByIdQueryHandler.HandleAsync(new GetReaderByIdQueryInput { Id = id }, HttpContext.RequestAborted);
            if (reader == null) return NotFound();
            return Ok(reader);
        }
    }
}
