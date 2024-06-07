using System.Text.Json;
using MyFx.ObjectModel;
namespace ObjectModelTests.Classes
{

    public class Person : FxClass
    {
        private static string _typeName => typeof(Person).Name;

        public Person():this(string.Empty, default(int), null) {}

        public Person(string? instanceJson):base(instanceJson)
        {
            Name = string.Empty;
            Id = default(int);
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Person"/> class with the specified name and ID.
        /// </summary>
        /// <param name="name">The name of the child.</param>
        /// <param name="id">The ID of the child.</param>
        public Person(string name, int id, DateTime? birthdate = null) : base(_typeName)
        {
            Name = name;
            Id = id;
            Birthdate = birthdate;
        }
        
        public string Name { get; set; }

        public int Id { get; set; }

        public DateTime? Birthdate { get; set; }

        
    }
}