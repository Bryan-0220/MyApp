namespace DeleteBook
{
    public interface IDeleteBookCommandHandler
    {
        Task<DeleteBookCommandOutput> Handle(DeleteBookCommandInput input, CancellationToken ct = default);
    }
}
