namespace CreateBook
{
    public interface ICreateBookCommandHandler
    {
        Task<CreateBookCommandOutput> Handle(CreateBookCommandInput input, CancellationToken ct = default);
    }
}
