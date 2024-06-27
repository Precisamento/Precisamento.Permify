using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Represents a Permify Attribute value. Only the following types are supported by default: string, boolean, integer, double, string[], boolean[], integer[], double[].
    /// If additional types are required, see <see cref="PermifyAttributeTypeResolver"/>.
    /// </summary>
    [JsonPolymorphic(IgnoreUnrecognizedTypeDiscriminators = false, TypeDiscriminatorPropertyName = "@type")]
    [JsonDerivedType(typeof(PermifyStringAttributeValue), "type.googleapis.com/base.v1.StringValue")]
    [JsonDerivedType(typeof(PermifyBooleanAttributeValue), "type.googleapis.com/base.v1.BooleanValue")]
    [JsonDerivedType(typeof(PermifyIntegerAttributeValue), "type.googleapis.com/base.v1.IntegerValue")]
    [JsonDerivedType(typeof(PermifyDoubleAttributeValue), "type.googleapis.com/base.v1.DoubleValue")]
    [JsonDerivedType(typeof(PermifyStringArrayAttributeValue), "type.googleapis.com/base.v1.StringArrayValue")]
    [JsonDerivedType(typeof(PermifyBooleanArrayAttributeValue), "type.googleapis.com/base.v1.BooleanArrayValue")]
    [JsonDerivedType(typeof(PermifyIntegerArrayAttributeValue), "type.googleapis.com/base.v1.IntegerArrayValue")]
    [JsonDerivedType(typeof(PermifyDoubleArrayAttributeValue), "type.googleapis.com/base.v1.DoubleArrayValue")]
    public interface IAttributeValue
    {
        string TypeName { get; }
    }

    public class PermifyStringAttributeValue : IAttributeValue
    {
        [JsonPropertyName("data")]
        public string Value { get; set; }

        [JsonIgnore]
        public string TypeName => "string";

        public override string ToString() => Value;
    }

    public class PermifyBooleanAttributeValue : IAttributeValue
    {
        [JsonPropertyName("data")]
        public bool Value { get; set; }

        [JsonIgnore]
        public string TypeName => "boolean";

        public override string ToString() => Value.ToString().ToLower();
    }

    public class PermifyIntegerAttributeValue : IAttributeValue
    {
        [JsonPropertyName("data")]
        public int Value { get; set; }

        [JsonIgnore]
        public string TypeName => "integer";

        public override string ToString() => Value.ToString();
    }

    public class PermifyDoubleAttributeValue : IAttributeValue
    {
        [JsonPropertyName("data")]
        public double Value { get; set; }

        [JsonIgnore]
        public string TypeName => "double";

        public override string ToString() => Value.ToString();
    }

    public class PermifyStringArrayAttributeValue : IAttributeValue
    {
        [JsonPropertyName("data")]
        public List<string> Value { get; set; }

        [JsonIgnore]
        public string TypeName => "string[]";

        public override string ToString() => $"[{string.Join(", ", Value)}]";
    }

    public class PermifyBooleanArrayAttributeValue : IAttributeValue
    {
        [JsonPropertyName("data")]
        public List<bool> Value { get; set; }

        [JsonIgnore]
        public string TypeName => "boolean[]";

        public override string ToString() => $"[{string.Join(", ", Value)}]";
    }

    public class PermifyIntegerArrayAttributeValue : IAttributeValue
    {
        [JsonPropertyName("data")]
        public List<int> Value { get; set; }

        [JsonIgnore]
        public string TypeName => "integer[]";

        public override string ToString() => $"[{string.Join(", ", Value)}]";
    }

    public class PermifyDoubleArrayAttributeValue : IAttributeValue
    {
        [JsonPropertyName("data")]
        public List<double> Value { get; set; }

        [JsonIgnore]
        public string TypeName => "double[]";

        public override string ToString() => $"[{string.Join(", ", Value)}]";
    }

    public class PermifyAttributeTypeResolver : DefaultJsonTypeInfoResolver
    {
        private Type _attributeType = typeof(IAttributeValue);
        private JsonPolymorphismOptions _options = new JsonPolymorphismOptions()
        {
            TypeDiscriminatorPropertyName = "@type",
            IgnoreUnrecognizedTypeDiscriminators = false,
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
            DerivedTypes =
            {
                new JsonDerivedType(typeof(PermifyStringAttributeValue), "type.googleapis.com/base.v1.StringValue"),
                new JsonDerivedType(typeof(PermifyBooleanAttributeValue), "type.googleapis.com/base.v1.BooleanValue"),
                new JsonDerivedType(typeof(PermifyIntegerAttributeValue), "type.googleapis.com/base.v1.IntegerValue"),
                new JsonDerivedType(typeof(PermifyDoubleAttributeValue), "type.googleapis.com/base.v1.DoubleValue"),
                new JsonDerivedType(typeof(PermifyStringArrayAttributeValue), "type.googleapis.com/base.v1.StringArrayValue"),
                new JsonDerivedType(typeof(PermifyBooleanArrayAttributeValue), "type.googleapis.com/base.v1.BooleanArrayValue"),
                new JsonDerivedType(typeof(PermifyIntegerArrayAttributeValue), "type.googleapis.com/base.v1.IntegerArrayValue"),
                new JsonDerivedType(typeof(PermifyDoubleArrayAttributeValue), "type.googleapis.com/base.v1.DoubleArrayValue"),
            }
        };

        public PermifyAttributeTypeResolver(IEnumerable<JsonDerivedType> derivedTypes)
        {
            _options = new JsonPolymorphismOptions()
            {
                TypeDiscriminatorPropertyName = "@type",
                IgnoreUnrecognizedTypeDiscriminators = false,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
            };

            foreach(var type in derivedTypes.Concat(DefaultTypes()))
            {
                _options.DerivedTypes.Add(type);
            }
        }

        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo info = base.GetTypeInfo(type, options);

            if (info.Type == _attributeType)
            {
                info.PolymorphismOptions = _options;
            }

            return info;
        }

        private IEnumerable<JsonDerivedType> DefaultTypes()
        {
            return new List<JsonDerivedType>() {
                new JsonDerivedType(typeof(PermifyStringAttributeValue), "type.googleapis.com/base.v1.StringValue"),
                new JsonDerivedType(typeof(PermifyBooleanAttributeValue), "type.googleapis.com/base.v1.BooleanValue"),
                new JsonDerivedType(typeof(PermifyIntegerAttributeValue), "type.googleapis.com/base.v1.IntegerValue"),
                new JsonDerivedType(typeof(PermifyDoubleAttributeValue), "type.googleapis.com/base.v1.DoubleValue"),
                new JsonDerivedType(typeof(PermifyStringArrayAttributeValue), "type.googleapis.com/base.v1.StringArrayValue"),
                new JsonDerivedType(typeof(PermifyBooleanArrayAttributeValue), "type.googleapis.com/base.v1.BooleanArrayValue"),
                new JsonDerivedType(typeof(PermifyIntegerArrayAttributeValue), "type.googleapis.com/base.v1.IntegerArrayValue"),
                new JsonDerivedType(typeof(PermifyDoubleArrayAttributeValue), "type.googleapis.com/base.v1.DoubleArrayValue")
            };
        }
    }
}