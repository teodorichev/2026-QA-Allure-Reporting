using NUnit.Framework;
using RickAndMortyTests.Helpers;
using RickAndMortyTests.Models;

using Allure.Net.Commons;
using Allure.NUnit.Attributes;


namespace RickAndMortyTests.Tests;

[AllureSuite("Rick and Morty API")]
[AllureFeature("Character Endpoints")]

public class CharacterTests : BaseTest
{
    [Test]
    public async Task GetAllCharacters_ReturnsSuccessfully()
    {
        // Arrange
        var url = $"{ApiClient.BaseUrl}/character";

        // Act
        var response = await _apiClient.GetAsync(url);
        var data = await _apiClient.ReadJsonAsync<ApiResponse<Character>>(response);

        // Assert
        Assert.That((int)response.StatusCode, Is.EqualTo(200), 
            $"Expected 200 OK but got {response.StatusCode}");
        Assert.That(data, Is.Not.Null);
        Assert.That(data!.Results, Is.Not.Empty);
        Assert.That(data.Info.Count, Is.GreaterThan(0));
    }

    [Test]
    [AllureDescription("Verify Rick Sanchez character data is correct")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureTag("smoke")]
    [AllureTag("character")]

    public async Task GetCharacter_RickSanchez_ReturnsCorrectData()
    {
        // Arrange
        var url = $"{ApiClient.BaseUrl}/character/1";

        // Act
        var response = await _apiClient.GetAsync(url);
        var character = await _apiClient.ReadJsonAsync<Character>(response);

        // Assert
        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That(character, Is.Not.Null);
        Assert.That(character!.Id, Is.EqualTo(1));
        Assert.That(character.Name, Is.EqualTo("Rick Sanchez"));
        Assert.That(character.Status, Is.EqualTo("Alive"));
        Assert.That(character.Species, Is.EqualTo("Human"));
    }

    [Test]
    public async Task GetCharacter_MortySmith_ReturnsCorrectData()
    {
        // Arrange
        var url = $"{ApiClient.BaseUrl}/character/2";

        // Act
        var response = await _apiClient.GetAsync(url);
        var character = await _apiClient.ReadJsonAsync<Character>(response);

        // Assert
        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That(character, Is.Not.Null);
        Assert.That(character!.Id, Is.EqualTo(2));
        Assert.That(character.Name, Is.EqualTo("Morty Smith"));
        Assert.That(character.Status, Is.EqualTo("Alive"));
    }

    [Test]
    public async Task FilterCharacters_ByNameRick_ReturnsMultipleResults()
    {
        // Arrange
        var url = $"{ApiClient.BaseUrl}/character?name=rick";

        // Act
        var response = await _apiClient.GetAsync(url);
        var data = await _apiClient.ReadJsonAsync<ApiResponse<Character>>(response);

        // Assert
        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That(data, Is.Not.Null);
        Assert.That(data!.Results, Is.Not.Empty);
        
        foreach (var character in data.Results)
        {
            Assert.That(character.Name.ToLower(), Does.Contain("rick"));
        }
    }

    [Test]
    public async Task FilterCharacters_ByStatus_ReturnsAliveCharacters()
    {
        // Arrange
        var url = $"{ApiClient.BaseUrl}/character?status=alive";

        // Act
        var response = await _apiClient.GetAsync(url);
        var data = await _apiClient.ReadJsonAsync<ApiResponse<Character>>(response);

        // Assert
        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That(data, Is.Not.Null);
        Assert.That(data!.Results, Is.Not.Empty);
        
        foreach (var character in data.Results)
        {
            Assert.That(character.Status, Is.EqualTo("Alive"));
        }
    }

    [Test]
    [AllureDescription("Verify filtering by both name and status works correctly")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureTag("regression")]
    [AllureTag("filter")]

    public async Task FilterCharacters_ByNameAndStatus_ReturnsFilteredResults()
    {
        // Arrange
        var url = $"{ApiClient.BaseUrl}/character?name=rick&status=alive";

        // Act
        var response = await _apiClient.GetAsync(url);
        var data = await _apiClient.ReadJsonAsync<ApiResponse<Character>>(response);

        // Assert
        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That(data, Is.Not.Null);
        Assert.That(data!.Results, Is.Not.Empty);
        
        foreach (var character in data.Results)
        {
            Assert.That(character.Name.ToLower(), Does.Contain("rick"));
            Assert.That(character.Status, Is.EqualTo("Alive"));
        }
    }

    [Test]
    public async Task GetMultipleCharacters_Returns3Characters()
    {
        List<Character>? characters = null;

        await AllureApi.Step("Step 1: Request characters 1, 2, 3", async () =>
        {
            var url = $"{ApiClient.BaseUrl}/character/1,2,3";
            var response = await _apiClient.GetAsync(url);
            characters = await _apiClient.ReadJsonAsync<List<Character>>(response);
            Assert.That((int)response.StatusCode, Is.EqualTo(200));
        });

        AllureApi.Step("Step 2: Verify count is 3", () =>
        {
            Assert.That(characters, Is.Not.Null);
            Assert.That(characters!.Count, Is.EqualTo(3));
        });

        AllureApi.Step("Step 3: Verify character IDs", () =>
        {
            Assert.That(characters![0].Id, Is.EqualTo(1));
            Assert.That(characters[1].Id, Is.EqualTo(2));
            Assert.That(characters[2].Id, Is.EqualTo(3));
        });
    }

    [Test]
    public async Task GetCharacter_VerifyOriginAndLocation()
    {
        // Arrange
        var url = $"{ApiClient.BaseUrl}/character/1";

        // Act
        var response = await _apiClient.GetAsync(url);
        var character = await _apiClient.ReadJsonAsync<Character>(response);

        // Assert
        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That(character, Is.Not.Null);
        Assert.That(character!.Origin, Is.Not.Null);
        Assert.That(character.Origin.Name, Is.Not.Empty);
        Assert.That(character.Location, Is.Not.Null);
        Assert.That(character.Location.Name, Is.Not.Empty);
    }
}
