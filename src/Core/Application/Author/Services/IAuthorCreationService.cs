namespace Application.Authors.Services
{
    public interface IAuthorCreationService
    {
        Task EnsureCanCreateAsync(string name, CancellationToken ct = default);
    }
}
