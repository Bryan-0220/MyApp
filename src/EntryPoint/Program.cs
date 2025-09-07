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
