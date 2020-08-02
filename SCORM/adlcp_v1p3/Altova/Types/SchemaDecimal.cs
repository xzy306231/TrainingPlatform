using System;

namespace adlcp_v1p3.Altova.Types
{

    public class SchemaDecimal : ISchemaType
    {
        public decimal Value;

        public SchemaDecimal(SchemaDecimal obj)
        {
            Value = obj.Value;
        }

        public SchemaDecimal(decimal Value)
        {
            this.Value = Value;
        }

        public SchemaDecimal(string Value)
        {
            this.Value = Convert.ToDecimal(Value);
        }

        public override string ToString()
        {
            return Convert.ToString(Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is SchemaDecimal))
                return false;
            return Value == ((SchemaDecimal)obj).Value;
        }

        public static bool operator ==(SchemaDecimal obj1, SchemaDecimal obj2)
        {
            return obj1.Value == obj2.Value;
        }

        public static bool operator !=(SchemaDecimal obj1, SchemaDecimal obj2)
        {
            return obj1.Value != obj2.Value;
        }

        public int CompareTo(object obj)
        {
            return Value.CompareTo(((SchemaDecimal)obj).Value);
        }

        public object Clone()
        {
            return new SchemaDecimal(Value);
        }
    }
}
