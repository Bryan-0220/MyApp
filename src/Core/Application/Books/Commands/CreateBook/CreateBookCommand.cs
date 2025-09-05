using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Books.Commands.CreateBook
{
    public class CreateBookCommand
    {
        public string Title { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public int? PublishedYear { get; set; }
        public int CopiesAvailable { get; set; } = 1;
    }
}
