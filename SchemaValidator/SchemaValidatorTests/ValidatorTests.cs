using SchemaValidator;

namespace SchemaValidatorTests
{
    [TestFixture]
    public class ValidatorTests
    {
        private Validator _validator;
        private string _tempDirectory;

        [SetUp]
        public void Setup()
        {
            _validator = new Validator();
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        [TearDown]
        public void Cleanup()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }

        [Test]
        public async Task ValidateAsync_WithValidInput_ReturnsValidResult()
        {
            //Arrange
            var schemaPath = CreateTempJsonFile("schema.json", "{\"name\": {\"length\": 50, \"mandatory\": true}, \"age\": {\"length\": 3, \"mandatory\": false}, \"email\": {\"length\": 100, \"mandatory\": true}}");
            var inputPath = CreateTempJsonFile("input.json", "[{ \"name\": \"John Doe\", \"email\": \"john.doe@example.com\", \"age\": 30 }]");
            var outputPath = Path.Combine(_tempDirectory, "output.json");

            //Act
            var result = await _validator.ValidateAsync(inputPath, schemaPath, _tempDirectory);

            //Assert
            Assert.IsTrue(result);
            var expectedJson = "[{\"Record\":{\"name\":\"John Doe\",\"email\":\"john.doe@example.com\",\"age\":30},\"IsValid\":true,\"ErrorMessages\":[]}]";
            var actualJson = File.ReadAllText(outputPath);
            actualJson = Newtonsoft.Json.JsonConvert.SerializeObject(Newtonsoft.Json.JsonConvert.DeserializeObject(actualJson));
            Assert.That(actualJson.Trim(), Is.EqualTo(expectedJson.Trim()));
        }

        [Test]
        public async Task ValidateAsync_WithMissingMandatoryField_ReturnsInvalidResult()
        {
            //Arrange
            var schemaPath = CreateTempJsonFile("schema.json", "{\"name\": {\"length\": 50, \"mandatory\": true}, \"age\": {\"length\": 3, \"mandatory\": false}, \"email\": {\"length\": 100, \"mandatory\": true}}");
            var inputPath = CreateTempJsonFile("input.json", "[{ \"name\": \"John Doe\" }]");
            var outputPath = Path.Combine(_tempDirectory, "output.json");

            //Act
            var result = await _validator.ValidateAsync(inputPath, schemaPath, _tempDirectory);

            //Assert
            Assert.IsFalse(result);
            var expectedJson = "[{\"Record\":{\"name\":\"John Doe\"},\"IsValid\":false,\"ErrorMessages\":[{\"ErrorType\":\"Field.MandatoryError\",\"ErrorMessage\":\"Field 'email' is mandatory but missing.\",\"Field\":\"email\"}]}]";
            var actualJson = File.ReadAllText(outputPath);
            actualJson = Newtonsoft.Json.JsonConvert.SerializeObject(Newtonsoft.Json.JsonConvert.DeserializeObject(actualJson));
            Assert.That(actualJson.Trim(), Is.EqualTo(expectedJson.Trim()));
        }

        private string CreateTempJsonFile(string fileName, string content)
        {
            var filePath = Path.Combine(_tempDirectory, fileName);
            File.WriteAllText(filePath, content);
            return filePath;
        }
    }
}