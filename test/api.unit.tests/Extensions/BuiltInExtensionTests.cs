using AGRC.api.Extensions;

namespace api.tests.Extensions;

public class BuiltInExtensionTests {
    [Theory]
    [InlineData("Test", "test")]
    [InlineData("Test Data", "testData")]
    [InlineData("test data", "testData")]
    [InlineData("", "")]
    [InlineData(" ", "")]
    [InlineData(null, "")]
    public void Should_convert_to_camel_case(string input, string result)
        => result.ShouldBe(input.ToCamelCase());
}
