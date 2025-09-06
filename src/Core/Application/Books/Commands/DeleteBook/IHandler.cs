namespace DeleteBook
{
    public interface IDeleteBookCommandHandler
    {
        Task<DeleteBookCommandOutput> HandleAsync(DeleteBookCommandInput input, CancellationToken ct = default);
    }
}
