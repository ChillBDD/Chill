namespace Chill.Tests.TestSubjects
{
    public class AClass
    {
        public AClass()
        {
            
        }

        public AClass(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        protected bool Equals(AClass other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AClass) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
