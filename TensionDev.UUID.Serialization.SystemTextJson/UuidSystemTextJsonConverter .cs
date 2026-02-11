using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TensionDev.UUID.Serialization.SystemTextJson
{
    /// <summary>
    /// <see cref="JsonConverter"/> for the <see cref="Uuid"/> type that handles serialization and deserialization
    /// </summary>
    public class UuidSystemTextJsonConverter : JsonConverter<Uuid>
    {
        /// <summary>
        /// Reads a JSON value and converts it to a new instance of the Uuid type.
        /// </summary>
        /// <param name="reader">The JsonReader used to read the JSON value to be converted.</param>
        /// <param name="typeToConvert">The type of the object to deserialize. This parameter is not used.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> options for the deserialization.</param>
        /// <returns>A Uuid instance parsed from the JSON value read by the reader.</returns>
        /// <exception cref="JsonException">Thrown when the JSON token is not a string or if parsing fails.</exception>
        public override Uuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Unexpected token {reader.TokenType} when parsing UUID.");
            }
            string s = reader.GetString();
            if (string.IsNullOrEmpty(s))
            {
                throw new JsonException("UUID string value was null or empty.");
            }
            return Uuid.Parse(s);
        }

        /// <summary>
        /// Writes the specified <see cref="Uuid"/> to JSON by writing its string representation
        /// to the provided <paramref name="writer"/>.
        /// The converter serializes the Uuid using its canonical textual form returned by
        /// <see cref="Uuid.ToString()"/>.
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> used to write the JSON value.</param>
        /// <param name="value">The <see cref="Uuid"/> instance to serialize.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> options for the serialization.</param>
        public override void Write(Utf8JsonWriter writer, Uuid value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
