namespace DeleteAuthor
{
    public interface IDeleteAuthorCommandHandler
    {
        Task<DeleteAuthorCommandOutput> Handle(DeleteAuthorCommandInput input, CancellationToken ct = default);
    }
}
