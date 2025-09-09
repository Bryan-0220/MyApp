namespace GetAllAuthors
{
    public interface IGetAllAuthorsQueryHandler
    {
        Task<IEnumerable<GetAllAuthorsQueryOutput>> HandleAsync(GetAllAuthorsQueryInput query, CancellationToken ct = default);
    }
}
