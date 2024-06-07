using System.Text.Json.Serialization;

namespace ObjectModelTests.Classes;

public sealed class Student:Person
{

    [JsonConstructor]
    public Student():this(new Person(), string.Empty, default(int)){}

    public Student(string? instanceJson):base(instanceJson)
    {
        School = string.Empty;
        Grade = default(int);
        
    }

    public Student(Person person, string school, int grade) : base(person.Name, person.Id, person.Birthdate)
    {
        School = school;
        Grade = grade;
    }

    public string School { get; set; }
    public int Grade { get; set; }

}

