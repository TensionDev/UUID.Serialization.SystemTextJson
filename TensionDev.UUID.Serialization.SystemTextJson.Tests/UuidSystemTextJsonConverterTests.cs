using Moq;
using System;
using System.Buffers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace TensionDev.UUID.Serialization.SystemTextJson.Tests
{
    public class UuidSystemTextJsonConverterTests : IDisposable
    {
        private bool disposedValue;

        private readonly UuidSystemTextJsonConverter _converter;

        public UuidSystemTextJsonConverterTests()
        {
            _converter = new UuidSystemTextJsonConverter();
        }

        [Theory]
        [InlineData(true, "00000000-0000-0000-0000-000000000000")]
        [InlineData(true, "ffffffff-ffff-ffff-ffff-ffffffffffff")]
        [InlineData(true, "164a714c-0c79-11ec-82a8-0242ac130003")]
        [InlineData(true, "550e8400-e29b-41d4-a716-446655440000")]
        [InlineData(true, "1bf6935b-49e6-54cf-a9c8-51fb21c41b46")]
        [InlineData(false, "00000000-0000-0000-0000-000000000000")]
        [InlineData(false, "ffffffff-ffff-ffff-ffff-ffffffffffff")]
        [InlineData(false, "164a714c-0c79-11ec-82a8-0242ac130003")]
        [InlineData(false, "550e8400-e29b-41d4-a716-446655440000")]
        [InlineData(false, "1bf6935b-49e6-54cf-a9c8-51fb21c41b46")]
        public void TestWrite(bool useNullOptions, string input)
        {
            // Arrange
            using var ms = new MemoryStream();
            using var writer = new Utf8JsonWriter(ms);
            Uuid value = Uuid.Parse(input);
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

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
        [InlineData("164a714c-0c79-11ec-82a8-0242ac130003")]
        [InlineData("550e8400-e29b-41d4-a716-446655440000")]
        [InlineData("1bf6935b-49e6-54cf-a9c8-51fb21c41b46")]
        public void TestReadString(string input)
        {
            // Arrange
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
        // ~UuidSystemTextJsonConverterTests()
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