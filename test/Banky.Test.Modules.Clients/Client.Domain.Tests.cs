using NSubstitute;

namespace Banky.Test.Modules.Clients;

public class Client_Domain_Tests
{
    private readonly TimeProvider _time = Substitute.For<TimeProvider>();

    public Client_Domain_Tests()
    {
        _time.GetUtcNow().Returns(new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void Create_ShouldFail_WhenEmailIsMissing()
    {
        // Arrange & Act
        var result = Client.Create(ClientType.Person, "John", "Doe", string.Empty, new DateTime(1990, 1, 1), _time);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Select(e => e.Message).Should()
            .Contain(Errors.EmailIsRequired().Message);
    }

    [Fact]
    public void Create_Person_ShouldFail_WhenBirthDateInFuture()
    {
        // Arrange
        var future = _time.GetUtcNow().AddDays(1).DateTime;

        // Act
        var result = Client.Create(ClientType.Person, "John", "Doe", "john@doe.com", future, _time);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Select(e => e.Message).Should()
            .Contain(Errors.BirthDateCannotBeInTheFuture().Message);
    }

    [Fact]
    public void Update_Person_ShouldFail_WhenUnderage()
    {
        // Arrange
        var client = Client.Create(ClientType.Person, "John", "Doe", "john@doe.com", new DateTime(1990, 1, 1), _time).Value;
        var underage = _time.GetUtcNow().AddYears(-17).DateTime;

        // Act
        var result = client.Update("John", "Doe", "john@doe.com", underage, _time);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Select(e => e.Message).Should()
            .Contain(Errors.MustBeOfLegalAge(Client.PersonLegalAge).Message);
    }

    [Fact]
    public void Update_Person_ShouldFail_WhenLastNameMissing()
    {
        // Arrange
        var client = Client.Create(ClientType.Person, "John", "Doe", "john@doe.com", new DateTime(1990, 1, 1), _time).Value;

        // Act
        var result = client.Update("John", null, "john@doe.com", client.BirthDate, _time);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Select(e => e.Message).Should()
            .Contain(Errors.LastNameIsRequired().Message);
    }

    [Fact]
    public void Update_ShouldFail_WhenNameEmpty()
    {
        // Arrange
        var client = Client.Create(ClientType.Company, "Acme Co.", null, "acme@co.com", null, _time).Value;

        // Act
        var result = client.Update(string.Empty, null, client.Email, null, _time);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Select(e => e.Message).Should()
            .Contain(Errors.NameIsRequired().Message);
    }
}
