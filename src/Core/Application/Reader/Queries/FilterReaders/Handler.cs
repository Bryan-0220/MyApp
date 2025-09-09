using Application.Interfaces;
using Application.Filters;
using FluentValidation;

namespace FilterReaders
{
    public class FilterReadersQueryHandler : IFilterReadersQueryHandler
    {
        private readonly IReaderRepository _readerRepository;
        private readonly IValidator<FilterReadersQueryInput> _validator;

        public FilterReadersQueryHandler(IReaderRepository readerRepository, IValidator<FilterReadersQueryInput> validator)
        {
            _readerRepository = readerRepository;
            _validator = validator;
        }

        public async Task<IEnumerable<FilterReadersQueryOutput>> HandleAsync(FilterReadersQueryInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var rf = new ReaderFilter
            {
                FirstName = input.FirstName,
                LastName = input.LastName
            };

            var readers = await _readerRepository.FilterAsync(rf, ct);

            return readers.Select(r => new FilterReadersQueryOutput
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Email = r.Email,
                MembershipDate = r.MembershipDate
            });
        }
    }
}
