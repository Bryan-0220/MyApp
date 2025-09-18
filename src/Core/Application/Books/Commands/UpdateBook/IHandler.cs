namespace UpdateBook
{
    public interface IUpdateBookCommandHandler
    {
        Task<UpdateBookCommandOutput> Handle(UpdateBookCommandInput input, CancellationToken ct = default);
    }
}
