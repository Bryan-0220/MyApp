namespace DeleteAuthor
{
    public interface IDeleteAuthorCommandHandler
    {
        Task<DeleteAuthorCommandOutput> HandleAsync(DeleteAuthorCommandInput input, CancellationToken ct = default);
    }
}
