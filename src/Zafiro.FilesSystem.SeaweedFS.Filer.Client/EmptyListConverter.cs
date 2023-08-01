using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public class EmptyListConverter<T> : JsonConverter<List<T>>
{
    public override bool HandleNull => true;

    public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            // Si el valor es nulo, devolvemos una lista vacía.
            return new List<T>();
        }

        // Si el valor no es nulo, dejamos que System.Text.Json realice la deserialización normalmente.
        return JsonSerializer.Deserialize<List<T>>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}

public class EmptyListConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        try
        {
            // Verificar si el tipo es una lista genérica
            var isGenericType = typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(List<>);
            return isGenericType;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            // Obtener el tipo de elemento de la lista
            var elementType = typeToConvert.GetGenericArguments()[0];

            // Crear una instancia del convertidor personalizado con el tipo de elemento
            var converterType = typeof(EmptyListConverter<>).MakeGenericType(elementType);
            var jsonConverter = (JsonConverter) Activator.CreateInstance(converterType);
            return jsonConverter;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}