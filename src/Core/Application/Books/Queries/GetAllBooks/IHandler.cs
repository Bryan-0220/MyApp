namespace GetAllBooks
{
    public interface IGetAllBooksQueryHandler
    {
        Task<IEnumerable<GetAllBooksQueryOutput>> HandleAsync(GetAllBooksQueryInput query, CancellationToken ct);
    }
}
