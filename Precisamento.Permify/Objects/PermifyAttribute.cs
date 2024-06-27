using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Defines an additional property of an entity.
    /// </summary>
    public class PermifyAttribute
    {
        /// <summary>
        /// The entity that this attribute belongs to.
        /// </summary>
        [JsonPropertyName("entity")]
        public PermifyEntity Entity { get; set; }

        /// <summary>
        /// The name of the attribute.
        /// </summary>
        [JsonPropertyName("attribute")]
        public string Name { get; set; }

        /// <summary>
        /// The value of the attribute.
        /// </summary>
        [JsonPropertyName("value")]
        public IAttributeValue Value { get; set; }

        [JsonIgnore]
        public bool IsString => Value is PermifyStringAttributeValue;

        [JsonIgnore]
        public string String => ((PermifyStringAttributeValue)Value).Value;

        [JsonIgnore]
        public bool IsBoolean => Value is PermifyBooleanAttributeValue;

        [JsonIgnore]
        public bool Boolean => ((PermifyBooleanAttributeValue)Value).Value;

        [JsonIgnore]
        public bool IsInteger => Value is PermifyIntegerAttributeValue;

        [JsonIgnore]
        public int Integer => ((PermifyIntegerAttributeValue)Value).Value;

        [JsonIgnore]
        public bool IsDouble => Value is PermifyDoubleAttributeValue;

        [JsonIgnore]
        public double Double => ((PermifyDoubleAttributeValue)Value).Value;

        [JsonIgnore]
        public bool IsStringArray => Value is PermifyStringArrayAttributeValue;

        [JsonIgnore]
        public List<string> StringArray => ((PermifyStringArrayAttributeValue)Value).Value;

        [JsonIgnore]
        public bool IsBooleanArray => Value is PermifyBooleanArrayAttributeValue;

        [JsonIgnore]
        public List<bool> BooleanArray => ((PermifyBooleanArrayAttributeValue)Value).Value;

        [JsonIgnore]
        public bool IsIntegerArray => Value is PermifyIntegerArrayAttributeValue;

        [JsonIgnore]
        public List<int> IntegerArray => ((PermifyIntegerArrayAttributeValue)Value).Value;

        [JsonIgnore]
        public bool IsDoubleArray => Value is PermifyDoubleArrayAttributeValue;

        [JsonIgnore]
        public List<double> DoubleArray => ((PermifyDoubleArrayAttributeValue)Value).Value;

        public PermifyAttribute()
        {
        }

        public PermifyAttribute(PermifyEntity entity, string name, IAttributeValue value)
        {
            Entity = entity;
            Name = name;
            Value = value;
        }

        public PermifyAttribute(PermifyEntity entity, string name, string value)
        {
            Entity = entity;
            Name = name;
            Value = new PermifyStringAttributeValue { Value = value };
        }

        public PermifyAttribute(PermifyEntity entity, string name, bool value)
        {
            Entity = entity;
            Name = name;
            Value = new PermifyBooleanAttributeValue { Value = value };
        }

        public PermifyAttribute(PermifyEntity entity, string name, int value)
        {
            Entity = entity;
            Name = name;
            Value = new PermifyIntegerAttributeValue { Value = value };
        }

        public PermifyAttribute(PermifyEntity entity, string name, double value)
        {
            Entity = entity;
            Name = name;
            Value = new PermifyDoubleAttributeValue { Value = value };
        }

        public PermifyAttribute(PermifyEntity entity, string name, IEnumerable<string> value)
        {
            Entity = entity;
            Name = name;
            Value = new PermifyStringArrayAttributeValue { Value = value.ToList() };
        }

        public PermifyAttribute(PermifyEntity entity, string name, IEnumerable<bool> value)
        {
            Entity = entity;
            Name = name;
            Value = new PermifyBooleanArrayAttributeValue { Value = value.ToList() };
        }

        public PermifyAttribute(PermifyEntity entity, string name, IEnumerable<int> value)
        {
            Entity = entity;
            Name = name;
            Value = new PermifyIntegerArrayAttributeValue { Value = value.ToList() };
        }

        public PermifyAttribute(PermifyEntity entity, string name, IEnumerable<double> value)
        {
            Entity = entity;
            Name = name;
            Value = new PermifyDoubleArrayAttributeValue { Value = value.ToList() };
        }

        public override string ToString() => $"{Entity}${Name}|{Value.TypeName}:{Value}";
    }
}