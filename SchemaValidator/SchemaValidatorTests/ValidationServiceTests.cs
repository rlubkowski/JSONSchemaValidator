using SchemaValidator.Model.Schema;
using SchemaValidator.Validation;
using System.Collections.Immutable;

namespace SchemaValidatorTests
{
    [TestFixture]
    public class ValidationServiceTests
    {
        private ValidationService _validationService;

        [SetUp]
        public void Setup()
        {
            _validationService = new ValidationService();
        }

        [Test]
        public void Validate_ValidInput_ReturnsSuccess()
        {
            // Arrange
            var input = ImmutableDictionary.Create<string, object?>().Add("name", "John Doe").Add("age", 30);
            var schema = ImmutableDictionary.Create<string, Rule>().Add("name", new Rule { Mandatory = true, Length = 50 }).Add("age", new Rule { Mandatory = false, Length = 3 });

            // Act
            var result = _validationService.Validate(input, schema);

            // Assert
            Assert.IsFalse(result.IsError);
        }

        [Test]
        public void Validate_MissingMandatoryField_ReturnsError()
        {
            // Arrange
            var input = ImmutableDictionary.Create<string, object?>().Add("age", 30);
            var schema = ImmutableDictionary.Create<string, Rule>().Add("name", new Rule { Mandatory = true, Length = 50 }).Add("age", new Rule { Mandatory = false, Length = 3 });

            // Act
            var result = _validationService.Validate(input, schema);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.That(result.Errors, Has.Some.Property("Code").EqualTo("Field.MandatoryError"));
        }

        [Test]
        public void Validate_LengthExceeded_ReturnsError()
        {
            // Arrange
            var input = ImmutableDictionary.Create<string, object?>().Add("name", "John Doe Very Long Name");
            var schema = ImmutableDictionary.Create<string, Rule>().Add("name", new Rule { Mandatory = true, Length = 5 });

            // Act
            var result = _validationService.Validate(input, schema);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.That(result.Errors, Has.Some.Property("Code").EqualTo("Field.LengthExceeded"));
        }

        [Test]
        public void Validate_MissingOptionalField_ReturnsSuccess()
        {
            // Arrange
            var input = ImmutableDictionary.Create<string, object?>().Add("name", "John Doe");
            var schema = ImmutableDictionary.Create<string, Rule>().Add("name", new Rule { Mandatory = true, Length = 50 }).Add("age", new Rule { Mandatory = false, Length = 3 });

            // Act
            var result = _validationService.Validate(input, schema);

            // Assert
            Assert.IsFalse(result.IsError);
        }

        [Test]
        public void Validate_EmptyStringMandatory_ReturnsError()
        {
            //Arrange
            var input = ImmutableDictionary.Create<string, object?>().Add("name", "");
            var schema = ImmutableDictionary.Create<string, Rule>().Add("name", new Rule { Mandatory = true });

            //Act
            var result = _validationService.Validate(input, schema);

            //Assert
            Assert.IsTrue(result.IsError);
            Assert.That(result.Errors, Has.Some.Property("Code").EqualTo("Field.MandatoryError"));
        }

        [Test]
        public void Validate_NullMandatory_ReturnsError()
        {
            //Arrange
            var input = ImmutableDictionary.Create<string, object?>().Add("name", null);
            var schema = ImmutableDictionary.Create<string, Rule>().Add("name", new Rule { Mandatory = true });

            //Act
            var result = _validationService.Validate(input, schema);

            //Assert
            Assert.IsTrue(result.IsError);
            Assert.That(result.Errors, Has.Some.Property("Code").EqualTo("Field.MandatoryError"));
        }

        [Test]
        public void Validate_ExtraFieldNotInSchema_ReturnsSuccess()
        {
            //Arrange
            var input = ImmutableDictionary.Create<string, object?>().Add("name", "Test").Add("extraField", "extraValue");
            var schema = ImmutableDictionary.Create<string, Rule>().Add("name", new Rule { Mandatory = true });

            //Act
            var result = _validationService.Validate(input, schema);

            //Assert
            Assert.IsFalse(result.IsError);
        }
    }
}
