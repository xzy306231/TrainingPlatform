namespace Altova.Types
{

    public class SchemaString : ISchemaType
    {
        public string Value;

        public SchemaString(SchemaString obj)
        {
            Value = obj.Value;
        }

        public SchemaString(string Value)
        {
            this.Value = Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is SchemaString))
                return false;
            return Value == ((SchemaString)obj).Value;
        }

        public static bool operator ==(SchemaString obj1, SchemaString obj2)
        {
            return obj1.Value == obj2.Value;
        }

        public static bool operator !=(SchemaString obj1, SchemaString obj2)
        {
            return obj1.Value != obj2.Value;
        }

        public int CompareTo(object obj)
        {
            return Value.CompareTo(((SchemaString)obj).Value);
        }

        public object Clone()
        {
            return new SchemaString(Value);
        }
    }
}
