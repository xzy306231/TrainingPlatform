namespace Altova.Types
{

    public class SchemaBoolean : ISchemaType
    {
        public bool Value;

        public SchemaBoolean(SchemaBoolean obj)
        {
            Value = obj.Value;
        }

        public SchemaBoolean(bool Value)
        {
            this.Value = Value;
        }

        public SchemaBoolean(string Value)
        {
            this.Value = Value == "true" || Value == "1";
        }

        public override string ToString()
        {
            return Value ? "true" : "false";
        }

        public override int GetHashCode()
        {
            return Value ? 1231 : 1237;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is SchemaBoolean))
                return false;
            return Value == ((SchemaBoolean)obj).Value;
        }

        public static bool operator ==(SchemaBoolean obj1, SchemaBoolean obj2)
        {
            return obj1.Value == obj2.Value;
        }

        public static bool operator !=(SchemaBoolean obj1, SchemaBoolean obj2)
        {
            return obj1.Value != obj2.Value;
        }

        public int CompareTo(object obj)
        {
            return Value.CompareTo(((SchemaBoolean)obj).Value);
        }

        public object Clone()
        {
            return new SchemaBoolean(Value);
        }
    }
}
