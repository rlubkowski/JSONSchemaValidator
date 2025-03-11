using Newtonsoft.Json.Linq;
using System.Collections.Immutable;

namespace SchemaValidator.Mapping
{
    internal class JObjectToDictionaryMapper
    {
        internal ImmutableDictionary<string, object?> Map(JObject jObject)
        {
            var dictionary = new Dictionary<string, object?>();

            foreach (var property in jObject.Properties())
            {
                if (property.Value.Type == JTokenType.Null)
                {
                    dictionary[property.Name] = null;
                }
                else if (property.Value.Type == JTokenType.Array && !property.Value.HasValues)
                {
                    dictionary[property.Name] = null;
                }
                else
                {
                    dictionary[property.Name] = property.Value.ToObject<object>();
                }
            }

            return dictionary.ToImmutableDictionary();
        }
    }
}
