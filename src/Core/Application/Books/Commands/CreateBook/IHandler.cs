namespace CreateBook
{
    public interface ICreateBookCommandHandler
    {
        Task<CreateBookCommandOutput> HandleAsync(CreateBookCommandInput input, CancellationToken ct = default);
    }
}
