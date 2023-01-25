using Xunit;

namespace Bumbo.Tests;

public class TestClassTests
{
    //Temporary test to make sure that tests are working and show general test structure
    [Fact]
    public void FunctionName_ExpectedShouldEqualActual()
    {
        // Arrange
        var expected = 0;

        // Act
        var actual = 0;

        // Assert
        Assert.True(expected == actual);
    }
}