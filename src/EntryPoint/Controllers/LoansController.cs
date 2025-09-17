using Microsoft.AspNetCore.Mvc;
using CreateLoan;
using DeleteLoan;
using UpdateLoan;
using GetLoanById;
using GetAllLoans;
using FilterLoans;

namespace EntryPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ICreateLoanCommandHandler _createLoanHandler;
        private readonly IDeleteLoanCommandHandler _deleteLoanHandler;
        private readonly IUpdateLoanCommandHandler _updateLoanHandler;
        private readonly IGetLoanByIdQueryHandler _getLoanByIdQueryHandler;
        private readonly IGetAllLoansQueryHandler _getAllLoansQueryHandler;
        private readonly IFilterLoansQueryHandler _filterLoansQueryHandler;

        public LoansController(
            ICreateLoanCommandHandler createLoanHandler,
            IDeleteLoanCommandHandler deleteLoanHandler,
            IUpdateLoanCommandHandler updateLoanHandler,
            IGetLoanByIdQueryHandler getLoanByIdQueryHandler,
            IGetAllLoansQueryHandler getAllLoansQueryHandler,
            IFilterLoansQueryHandler filterLoansQueryHandler)
        {
            _createLoanHandler = createLoanHandler;
            _deleteLoanHandler = deleteLoanHandler;
            _updateLoanHandler = updateLoanHandler;
            _getLoanByIdQueryHandler = getLoanByIdQueryHandler;
            _getAllLoansQueryHandler = getAllLoansQueryHandler;
            _filterLoansQueryHandler = filterLoansQueryHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLoanCommandInput input)
        {
            var result = await _createLoanHandler.Handle(input, HttpContext.RequestAborted);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateLoanCommandInput input)
        {
            input.Id = id;
            var updated = await _updateLoanHandler.Handle(input, HttpContext.RequestAborted);
            if (updated is null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _deleteLoanHandler.Handle(new DeleteLoanCommandInput { Id = id }, HttpContext.RequestAborted);
            if (!result.Success)
                return NotFound(new { success = result.Success, message = result.Message, loanId = result.LoanId });
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var loans = await _getAllLoansQueryHandler.Handle(new GetAllLoansQueryInput(), HttpContext.RequestAborted);
            return Ok(loans);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] string? bookId, [FromQuery] string? readerId, [FromQuery] string? status)
        {
            var input = new FilterLoansQueryInput { BookId = bookId, ReaderId = readerId, Status = status };
            var loans = await _filterLoansQueryHandler.Handle(input, HttpContext.RequestAborted);
            return Ok(loans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var loan = await _getLoanByIdQueryHandler.Handle(new GetLoanByIdQueryInput { Id = id }, HttpContext.RequestAborted);
            if (loan == null) return NotFound();
            return Ok(loan);
        }
    }
}
