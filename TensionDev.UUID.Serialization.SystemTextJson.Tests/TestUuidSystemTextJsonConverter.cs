using Moq;
using System;
using System.Buffers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace TensionDev.UUID.Serialization.SystemTextJson.Tests
{
    public class TestUuidSystemTextJsonConverter : IDisposable
    {
        private bool disposedValue;

        private readonly UuidSystemTextJsonConverter _converter;

        public TestUuidSystemTextJsonConverter()
        {
            _converter = new UuidSystemTextJsonConverter();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestWrite(bool useNullOptions)
        {
            // Arrange
            using var ms = new MemoryStream();
            using var writer = new Utf8JsonWriter(ms);
            Uuid value = new Uuid();
            JsonSerializerOptions? options = useNullOptions ? null : new JsonSerializerOptions();

            // Act
            _converter.Write(writer, value, options);
            writer.Flush();
            string actual = Encoding.UTF8.GetString(ms.ToArray());

            // Assert
            string expected = JsonSerializer.Serialize(value.ToString());
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestReadNotString()
        {
            // Arrange
            // Use a canonical all-zero UUID representation which is commonly accepted by UUID parsers.
            const string input = "00000000-0000-0000-0000-000000000000";
            string jsonText = "0";
            byte[] json = Encoding.UTF8.GetBytes(jsonText);
            var reader = new Utf8JsonReader(json);
            reader.Read();

            // Act
            JsonException ex = null;
            try
            {
                _converter.Read(ref reader, typeof(Uuid), new JsonSerializerOptions());
            }
            catch (JsonException caught)
            {
                ex = caught;
            }

            // Assert
            Assert.NotNull(ex);
        }

        [Fact]
        public void TestReadEmptyString()
        {
            // Arrange
            // Use a canonical all-zero UUID representation which is commonly accepted by UUID parsers.
            const string input = "00000000-0000-0000-0000-000000000000";
            string jsonText = "\"\"";
            byte[] json = Encoding.UTF8.GetBytes(jsonText);
            var reader = new Utf8JsonReader(json);
            reader.Read();

            // Act
            JsonException ex = null;
            try
            {
                _converter.Read(ref reader, typeof(Uuid), new JsonSerializerOptions());
            }
            catch (JsonException caught)
            {
                ex = caught;
            }

            // Assert
            Assert.NotNull(ex);
        }

        [Fact]
        public void TestReadString()
        {
            // Arrange
            // Use a canonical all-zero UUID representation which is commonly accepted by UUID parsers.
            const string input = "00000000-0000-0000-0000-000000000000";
            string jsonText = "\"" + input + "\"";
            byte[] json = Encoding.UTF8.GetBytes(jsonText);
            var reader = new Utf8JsonReader(json);
            reader.Read();

            // Act
            Uuid result = _converter.Read(ref reader, typeof(Uuid), new JsonSerializerOptions());

            // Assert
            // Compare textual forms to avoid depending on reference equality or unknown equality semantics of Uuid.
            Assert.Equal(input, result.ToString());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TestUuidSystemTextJsonConverter()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}