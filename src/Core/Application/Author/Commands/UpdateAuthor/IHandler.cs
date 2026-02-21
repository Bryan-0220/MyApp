namespace UpdateAuthor
{
    public interface IUpdateAuthorCommandHandler
    {
        Task<UpdateAuthorCommandOutput> Handle(UpdateAuthorCommandInput input, CancellationToken ct = default);
    }
}
