using Shouldly;

namespace TransferBatch.Common.UnitTests
{
    public class ExtensionsTests
    {
        [Fact]
        public void ToParameters_ShouldReturnCorrectDictionary_ForValidArgs()
        {
            // Arrange
            var args = new string[] { "-param1", "value1", "-param2", "value2" };
            var expected = new Dictionary<string, string>
            {
                { "PARAM1", "value1" },
                { "PARAM2", "value2" }
            };

            // Act
            var result = args.ToParameters();

            // Assert
            result.ShouldBe(expected);
        }

        [Fact]
        public void ToParameters_ShouldHandleSwitchParameters()
        {
            // Arrange
            var args = new string[] { "-switch1", "-param1", "value1" };
            var switchParameters = new string[] { "switch1" };
            var expected = new Dictionary<string, string>
            {
                { "SWITCH1", bool.TrueString },
                { "PARAM1", "value1" }
            };

            // Act
            var result = args.ToParameters(switchParameters);

            // Assert
            result.ShouldBe(expected);
        }

        [Fact]
        public void ToParameters_ShouldHandleLastArgumentWithoutParameter()
        {
            // Arrange
            var args = new string[] { "-param1", "value1", "-param2" };
            var expected = new Dictionary<string, string>
            {
                { "PARAM1", "value1" },
                { string.Empty, "param2" }
            };

            // Act
            var result = args.ToParameters();

            // Assert
            result.ShouldBe(expected);
        }

        [Fact]
        public void ToParameters_ShouldHandleEmptyArgs()
        {
            // Arrange
            var args = new string[] { };
            var expected = new Dictionary<string, string>();

            // Act
            var result = args.ToParameters();

            // Assert
            result.ShouldBe(expected);
        }
    }
}
