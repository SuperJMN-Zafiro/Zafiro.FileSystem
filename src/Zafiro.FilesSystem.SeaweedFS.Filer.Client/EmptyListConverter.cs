using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public class EmptyListConverter<T> : JsonConverter<List<T>>
{
    public override bool HandleNull => true;

    public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            // Si el token actual es un inicio de array '[', deserializamos normalmente.
            return JsonSerializer.Deserialize<List<T>>(ref reader, options);
        }
        else
        {
            // Si no es un inicio de array, creamos una lista vacía.
            return new List<T>();
        }
    }

    public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteStartArray();
            writer.WriteEndArray();
        }
        else
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}

public class EmptyListConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        // Verificar si el tipo es una lista genérica
        var isGenericType = typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(List<>);
        return isGenericType;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        // Obtener el tipo de elemento de la lista
        var elementType = typeToConvert.GetGenericArguments()[0];

        // Crear una instancia del convertidor personalizado con el tipo de elemento
        var converterType = typeof(EmptyListConverter<>).MakeGenericType(elementType);
        var jsonConverter = (JsonConverter) Activator.CreateInstance(converterType);
        return jsonConverter;
    }
}