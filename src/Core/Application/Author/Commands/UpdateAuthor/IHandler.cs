namespace UpdateAuthor
{
    public interface IUpdateAuthorCommandHandler
    {
        Task<UpdateAuthorCommandOutput?> HandleAsync(UpdateAuthorCommandInput input, CancellationToken ct = default);
    }
}
