using System;

namespace adlcp_v1p3.Altova.Types
{

    public class SchemaDateTime : ISchemaType
    {
        public DateTime Value;

        public SchemaDateTime(SchemaDateTime obj)
        {
            Value = obj.Value;
        }

        public SchemaDateTime(DateTime Value)
        {
            this.Value = Value;
        }

        public SchemaDateTime(string Value)
        {
            this.Value = Convert.ToDateTime(Value);
        }

        public override string ToString()
        {
            return Value.GetDateTimeFormats('s')[0];
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is SchemaDateTime))
                return false;
            return Value == ((SchemaDateTime)obj).Value;
        }

        public static bool operator ==(SchemaDateTime obj1, SchemaDateTime obj2)
        {
            return obj1.Value == obj2.Value;
        }

        public static bool operator !=(SchemaDateTime obj1, SchemaDateTime obj2)
        {
            return obj1.Value != obj2.Value;
        }

        public int CompareTo(object obj)
        {
            return Value.CompareTo(((SchemaDateTime)obj).Value);
        }

        public object Clone()
        {
            return new SchemaDateTime(Value);
        }
    }
}
