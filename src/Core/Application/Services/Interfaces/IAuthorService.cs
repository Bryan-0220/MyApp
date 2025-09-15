namespace Application.Authors.Services
{
    public interface IAuthorService
    {
        Task EnsureCanCreateAsync(string name, CancellationToken ct = default);
        Task EnsureCanDeleteAsync(string authorId, CancellationToken ct = default);
    }
}
