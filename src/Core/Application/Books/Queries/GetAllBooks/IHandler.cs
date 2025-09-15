namespace GetAllBooks
{
    public interface IGetAllBooksQueryHandler
    {
        Task<IEnumerable<GetAllBooksQueryOutput>> Handle(GetAllBooksQueryInput query, CancellationToken ct);
    }
}
