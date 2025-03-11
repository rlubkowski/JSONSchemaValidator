using SchemaValidator;

namespace SchemaValidatorTests
{
    [TestFixture]
    public class SchemaReaderTests
    {
        private SchemaReader _schemaReader;

        [SetUp]
        public void Setup()
        {
            _schemaReader = new SchemaReader();
        }

        [Test]
        public async Task GetSchema_ValidSchema_ReturnsSchemaDictionary()
        {
            // Arrange
            string schemaJson = "{\"name\": {\"length\": 50, \"mandatory\": true}}";
            string schemaPath = "test_schema.json";
            await File.WriteAllTextAsync(schemaPath, schemaJson);

            // Act
            var result = await _schemaReader.GetSchema(schemaPath);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.That(result.Value.Count, Is.EqualTo(1));
            Assert.IsTrue(result.Value.ContainsKey("name"));
            Assert.That(result.Value["name"].Length, Is.EqualTo(50));
            Assert.IsTrue(result.Value["name"].Mandatory);

            File.Delete(schemaPath);
        }

        [Test]
        public async Task GetSchema_InvalidSchema_ReturnsError()
        {
            // Arrange
            string schemaJson = "invalid json";
            string schemaPath = "test_schema.json";
            await File.WriteAllTextAsync(schemaPath, schemaJson);

            // Act
            var result = await _schemaReader.GetSchema(schemaPath);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.That(result.FirstError.Code, Is.EqualTo("Schema.ReadFailed"));

            File.Delete(schemaPath);
        }

        [Test]
        public async Task GetSchema_EmptySchema_ReturnsSuccessWithEmptyDictionary()
        {
            // Arrange
            string schemaJson = "{}";
            string schemaPath = "test_schema.json";
            await File.WriteAllTextAsync(schemaPath, schemaJson);

            // Act
            var result = await _schemaReader.GetSchema(schemaPath);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsEmpty(result.Value);

            File.Delete(schemaPath);
        }

        [Test]
        public async Task GetSchema_NullSchema_ReturnsError()
        {
            //Arrange
            string schemaJson = "null";
            string schemaPath = "test_schema.json";
            await File.WriteAllTextAsync(schemaPath, schemaJson);

            //Act
            var result = await _schemaReader.GetSchema(schemaPath);

            //Assert
            Assert.IsTrue(result.IsError);
            Assert.That(result.FirstError.Code, Is.EqualTo("Schema.Invalid"));

            File.Delete(schemaPath);
        }

        [Test]
        public async Task GetSchema_FileDoesNotExist_ReturnsError()
        {
            //Arrange
            string schemaPath = "non_existent_schema.json";

            //Act
            var result = await _schemaReader.GetSchema(schemaPath);

            //Assert
            Assert.IsTrue(result.IsError);
            Assert.That(result.FirstError.Code, Is.EqualTo("Schema.ReadFailed"));
        }
    }
}
