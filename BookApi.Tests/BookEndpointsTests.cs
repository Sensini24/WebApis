using System.Net.Http.Json;
using Xunit;
using DemoMinimalAPI.Entities;
using System.Net;

public class BookEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BookEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetBooks_ReturnsSuccessAndEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/getBooks");

        // Assert
        response.EnsureSuccessStatusCode(); // 200
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("[]", json); // lista vacía en JSON
    }

    [Fact]
    public async Task CreateBook_ReturnTheSameBookCreated()
    {
        var newBook = new Book
        {
            Title = "Pedro Páramo",
            Author = "Juan Rulfo",
            Isbn = "123456789",
            PublishedDate = new DateTime(1955, 1, 1)
        };
        
        var response = await _client.PostAsJsonAsync("/saveBook", newBook);
        var createdBook = await response.Content.ReadFromJsonAsync<Book>();

        //response.StatusCode.Should().Be(HttpStatusCode.Created);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(createdBook);
        Assert.True(createdBook.Id > 0);
        Assert.Equal(newBook.Title, "Perro faldero");
        Assert.Equal(newBook.Author, createdBook.Author);
    }
    // [Fact]
    // public async Task SaveBook_Then_GetById_WorksCorrectly()
    // {
    //     // Arrange
    //     var book = new Book
    //     {
    //         Title = "Pedro Páramo",
    //         Author = "Juan Rulfo",
    //         Isbn = "123456789",
    //         PublishedDate = new DateTime(1955, 1, 1)
    //     };

    //     // Act - Save the book
    //     var response = await _client.PostAsJsonAsync("/saveBook", book);
    //     response.EnsureSuccessStatusCode();

    //     // Act - Get all books
    //     var books = await _client.GetFromJsonAsync<List<Book>>("/getBooks");

    //     // Assert
    //     Assert.Single(books);
    //     Assert.Equal("Pedro Páramo", books[0].Title);
    // }
}
