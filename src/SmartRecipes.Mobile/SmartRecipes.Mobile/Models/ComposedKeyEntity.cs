namespace SmartRecipes.Mobile.Models
{
    public class ComposedKeyEntity
    {
        protected ComposedKeyEntity(string id)
        {
            Id = id;
        }
        
        protected ComposedKeyEntity () { /* Sql lite */}

        public string Id { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ComposedKeyEntity e && Id == e.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}