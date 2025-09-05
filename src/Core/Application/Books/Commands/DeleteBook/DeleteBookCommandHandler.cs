using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Application.Books.Commands.DeleteBook
{
    public class DeleteBookCommandHandler
    {
        private readonly IBookRepository _bookRepository;

        public DeleteBookCommandHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<bool> HandleAsync(DeleteBookCommand command, CancellationToken ct = default)
        {
            var existing = await _bookRepository.GetByIdAsync(command.Id, ct);
            if (existing is null) return false;

            await _bookRepository.DeleteAsync(command.Id, ct);
            return true;
        }
    }
}
