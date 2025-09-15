namespace Application.Authors.Services
{
    public interface IAuthorService
    {
        Task EnsureCanCreate(string name, CancellationToken ct = default);
        Task EnsureCanDelete(string authorId, CancellationToken ct = default);
    }
}
