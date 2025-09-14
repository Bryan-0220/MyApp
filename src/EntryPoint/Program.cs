using MongoDB.Driver;
using Application.Interfaces;
using Infrastructure.Repositories;
using MongoDB.Bson.Serialization;
using Infrastructure.Serialization;
using FluentValidation;
using CreateBook;
using DeleteBook;
using UpdateBook;
using GetAllBooks;
using GetBookById;
using FilterBooks;
using CreateLoan;
using DeleteLoan;
using UpdateLoan;
using GetLoanById;
using GetAllLoans;
using FilterLoans;
using CreateAuthor;
using DeleteAuthor;
using UpdateAuthor;
using GetAllAuthors;
using GetAuthorById;
using FilterAuthors;
using CreateReader;
using DeleteReader;
using UpdateReader;
using GetAllReaders;
using GetReaderById;
using FilterReaders;

var builder = WebApplication.CreateBuilder(args);


BsonSerializer.RegisterSerializer(typeof(string), new GuidAsStringSerializer());

var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoSettings>() ?? new MongoSettings();
builder.Services.AddSingleton(mongoSettings);

builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoSettings.ConnectionString));
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoSettings.DatabaseName);
});

builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

builder.Services.AddScoped<IReaderRepository, ReaderRepository>();

builder.Services.AddScoped<ILoanRepository, LoanRepository>();

builder.Services.AddScoped<Application.Books.Services.IBookDeletionService, Application.Books.Services.BookDeletionService>();

builder.Services.AddScoped<ICreateLoanCommandHandler, CreateLoanCommandHandler>();
builder.Services.AddScoped<IGetLoanByIdQueryHandler, GetLoanByIdQueryHandler>();
builder.Services.AddScoped<IGetAllLoansQueryHandler, GetAllLoansQueryHandler>();
builder.Services.AddScoped<IDeleteLoanCommandHandler, DeleteLoanCommandHandler>();
builder.Services.AddScoped<IUpdateLoanCommandHandler, UpdateLoanCommandHandler>();
builder.Services.AddScoped<IFilterLoansQueryHandler, FilterLoansQueryHandler>();
builder.Services.AddScoped<IValidator<CreateLoanCommandInput>, CreateLoanCommandValidator>();
builder.Services.AddScoped<IValidator<DeleteLoanCommandInput>, DeleteLoanCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateLoanCommandInput>, UpdateLoanCommandValidator>();
builder.Services.AddScoped<IValidator<GetLoanByIdQueryInput>, GetLoanByIdQueryValidator>();
builder.Services.AddScoped<IValidator<FilterLoansQueryInput>, FilterLoansQueryValidator>();

builder.Services.AddScoped<ICreateReaderCommandHandler, CreateReaderCommandHandler>();
builder.Services.AddScoped<IGetReaderByIdQueryHandler, GetReaderByIdQueryHandler>();
builder.Services.AddScoped<IGetAllReadersQueryHandler, GetAllReadersQueryHandler>();
builder.Services.AddScoped<IDeleteReaderCommandHandler, DeleteReaderCommandHandler>();
builder.Services.AddScoped<IUpdateReaderCommandHandler, UpdateReaderCommandHandler>();
builder.Services.AddScoped<IFilterReadersQueryHandler, FilterReadersQueryHandler>();
builder.Services.AddScoped<IValidator<CreateReaderCommandInput>, CreateReaderCommandValidator>();
builder.Services.AddScoped<IValidator<DeleteReaderCommandInput>, DeleteReaderCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateReaderCommandInput>, UpdateReaderCommandValidator>();
builder.Services.AddScoped<IValidator<GetReaderByIdQueryInput>, GetReaderByIdQueryValidator>();
builder.Services.AddScoped<IValidator<FilterReadersQueryInput>, FilterReadersQueryValidator>();

builder.Services.AddScoped<ICreateAuthorCommandHandler, CreateAuthorCommandHandler>();
builder.Services.AddScoped<IGetAuthorByIdQueryHandler, GetAuthorByIdQueryHandler>();
builder.Services.AddScoped<IGetAllAuthorsQueryHandler, GetAllAuthorsQueryHandler>();
builder.Services.AddScoped<IDeleteAuthorCommandHandler, DeleteAuthorCommandHandler>();
builder.Services.AddScoped<IUpdateAuthorCommandHandler, UpdateAuthorCommandHandler>();
builder.Services.AddScoped<IFilterAuthorsQueryHandler, FilterAuthorsQueryHandler>();
builder.Services.AddScoped<IValidator<CreateAuthorCommandInput>, CreateAuthorCommandValidator>();
builder.Services.AddScoped<IValidator<DeleteAuthorCommandInput>, DeleteAuthorCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateAuthorCommandInput>, UpdateAuthorCommandValidator>();
builder.Services.AddScoped<IValidator<GetAuthorByIdQueryInput>, GetAuthorByIdQueryValidator>();
builder.Services.AddScoped<IValidator<FilterAuthorsQueryInput>, FilterAuthorsQueryValidator>();

builder.Services.AddScoped<ICreateBookCommandHandler, CreateBookCommandHandler>();
builder.Services.AddScoped<IGetBookByIdQueryHandler, GetBookByIdQueryHandler>();
builder.Services.AddScoped<IGetAllBooksQueryHandler, GetAllBooksQueryHandler>();
builder.Services.AddScoped<IDeleteBookCommandHandler, DeleteBookCommandHandler>();
builder.Services.AddScoped<IUpdateBookCommandHandler, UpdateBookCommandHandler>();
builder.Services.AddScoped<IFilterBooksQueryHandler, FilterBooksQueryHandler>();
builder.Services.AddScoped<IValidator<CreateBookCommandInput>, CreateBookCommandValidator>();
builder.Services.AddScoped<IValidator<DeleteBookCommandInput>, DeleteBookCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateBookCommandInput>, UpdateBookCommandValidator>();
builder.Services.AddScoped<IValidator<GetBookByIdQueryInput>, GetBookByIdQueryValidator>();
builder.Services.AddScoped<IValidator<FilterBooksQueryInput>, FilterBooksQueryValidator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
