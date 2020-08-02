using System;

namespace adlcp_v1p3.Altova.Types
{

    public class SchemaInt : ISchemaType
    {
        public int Value;

        public SchemaInt(SchemaInt obj)
        {
            Value = obj.Value;
        }

        public SchemaInt(int Value)
        {
            this.Value = Value;
        }

        public SchemaInt(string Value)
        {
            this.Value = Convert.ToInt32(Value);
        }

        public override string ToString()
        {
            return Convert.ToString(Value);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is SchemaInt))
                return false;
            return Value == ((SchemaInt)obj).Value;
        }

        public static bool operator ==(SchemaInt obj1, SchemaInt obj2)
        {
            return obj1.Value == obj2.Value;
        }

        public static bool operator !=(SchemaInt obj1, SchemaInt obj2)
        {
            return obj1.Value != obj2.Value;
        }

        public int CompareTo(object obj)
        {
            return Value.CompareTo(((SchemaInt)obj).Value);
        }

        public object Clone()
        {
            return new SchemaInt(Value);
        }
    }
}
