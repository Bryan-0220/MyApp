namespace GetBookById
{
    public interface IGetBookByIdQueryHandler
    {
        Task<GetBookByIdQueryOutput?> Handle(GetBookByIdQueryInput query, CancellationToken ct);
    }
}
