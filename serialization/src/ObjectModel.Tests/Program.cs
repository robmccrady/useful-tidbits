// See https://aka.ms/new-console-template for more information
using ObjectModel.Tests.Classes;
using ObjectModel.Tests.Processors;

// Using simple DTOs derived from FxClass, let's test our ability to serialize, deserialize, and 
// pass then through each other.
var person = new Person("Rob", 1, new DateTime(year: 1973, month: 9, day: 10 ));
var student = new Student(person, "not mit", 1);

IProcessor processor = new TypedProcessor();
Console.WriteLine("Displaying before normalization");
processor.Display(student);

var pipelineResult = processor.Normalize(student);
Console.WriteLine("Displaying after normalization");
processor.Display(pipelineResult);

Console.WriteLine("================");

// This is just here to visually verify that we can cast from Student to Person and Back again.
 static void TestStrategyPattern(Person person, Student student){

    IProcessor processor = new TypedProcessor();
    // Process the Person instance normally.
    Console.WriteLine("Process our Person as a Person.");
    processor.Display(person);
    Console.WriteLine("================");
    // Process the Student instance normally.
    Console.WriteLine("Process our Student as a Student.");
    processor.Display(student);
    Console.WriteLine("================");
    // Process the Student as a person.
    Console.WriteLine("Process our Student as a Person using the Processor's 'ProcessAs' method that allows us to choose the Type Strategy to use.");
    processor.DisplayAs(student, nameof(Person));
    Console.WriteLine("================");
    
 }