namespace CreateAuthor
{
    public interface ICreateAuthorCommandHandler
    {
        Task<CreateAuthorCommandOutput> Handle(CreateAuthorCommandInput input, CancellationToken ct = default);
    }
}
