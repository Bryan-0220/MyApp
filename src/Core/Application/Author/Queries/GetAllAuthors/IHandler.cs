namespace GetAllAuthors
{
    public interface IGetAllAuthorsQueryHandler
    {
        Task<IEnumerable<GetAllAuthorsQueryOutput>> Handle(GetAllAuthorsQueryInput query, CancellationToken ct = default);
    }
}
