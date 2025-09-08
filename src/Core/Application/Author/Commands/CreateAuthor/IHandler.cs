namespace CreateAuthor
{
    public interface ICreateAuthorCommandHandler
    {
        Task<CreateAuthorCommandOutput> HandleAsync(CreateAuthorCommandInput input, CancellationToken ct = default);
    }
}
