using System;

namespace adlcp_v1p3.Altova.Types
{

    public class SchemaLong : ISchemaType
    {
        public long Value;

        public SchemaLong(SchemaLong obj)
        {
            Value = obj.Value;
        }

        public SchemaLong(long Value)
        {
            this.Value = Value;
        }

        public SchemaLong(string Value)
        {
            this.Value = Convert.ToInt64(Value);
        }

        public override string ToString()
        {
            return Convert.ToString(Value);
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is SchemaLong))
                return false;
            return Value == ((SchemaLong)obj).Value;
        }

        public static bool operator ==(SchemaLong obj1, SchemaLong obj2)
        {
            return obj1.Value == obj2.Value;
        }

        public static bool operator !=(SchemaLong obj1, SchemaLong obj2)
        {
            return obj1.Value != obj2.Value;
        }

        public int CompareTo(object obj)
        {
            return Value.CompareTo(((SchemaLong)obj).Value);
        }

        public object Clone()
        {
            return new SchemaLong(Value);
        }
    }
}
