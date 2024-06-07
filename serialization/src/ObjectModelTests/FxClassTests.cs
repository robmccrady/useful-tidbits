using MyFx.ObjectModel;
using ObjectModelTests.Classes;

namespace ObjectModelTests;

/// <summary>
/// Tests for <see cref="FxClass"/>.
///
/// The tests in this class are meant to test the functionality of <see cref="FxClass"/>.
/// In particular, the Json serializationa and deserialization methods are exercised, to ensure
/// that classes along an inheritance tree can be used interchangeably, as long as the operations
/// are occurring on the current branch or one of its ancestors.
/// </summary>
public class FxClassTests
{
    // Starter instances
    private Person _person;
    private Student _student;

    [SetUp]
    public void Setup()
    {
        _person = new Person("Rob", 1, new DateTime(year: 1973, month: 9, day: 10 ));
        _student = new Student(_person, "not mit", 1);
    }

    [Test]
    public void CreateStudentFrom_Student_ExpectNoExtensionData()
    {
        Student? newStudent = FxClass.Rebuild<Student>(_student.AsJson());
        int retainedProperties = newStudent?.ExtensionProperties.Count()??-1;

        Assert.IsNotNull(newStudent, "Materializing a new Student from a serialized Student should not return null.");
        Assert.That(retainedProperties, Is.EqualTo(0), "Materializing a Student from a Student should not use ExtensionData.");

    }

    [Test]
    public void CreatePersonFrom_Person_ExpectNoExtensionData()
    {
        Person? newPerson = FxClass.Rebuild<Person>(_person.AsJson());
        int retainedProperties = newPerson?.ExtensionProperties.Count()??-1;

        Assert.IsNotNull(newPerson, "Materializing a new Person from a serialized Person should not return null.");
        Assert.That(retainedProperties, Is.EqualTo(0), "Materializing a Person from a Person should not use ExtensionData.");
    }

    [Test]
    public void CreatePersonFromStudent_ExpectExtensionData()
    {
        int expectedExtensionProperties = 2;
        Person? newPerson = FxClass.Rebuild<Person>(_student.AsJson());
        int retainedProperties = newPerson?.ExtensionProperties.Count()??-1;

        Assert.IsNotNull(newPerson, "Materializing a new Person from a serialized Student should not return null.");
        Assert.That(retainedProperties, Is.EqualTo(expectedExtensionProperties), "Materializing a Person from a Student should have 2 extension properties.");
    }

    [Test]
    public void CreateStudentFromPerson_ExpectNoExtensionData()
    {
        int expectedExtensionProperties = 0;
        Student? newStudent = FxClass.Rebuild<Student>(_person.AsJson());
        int retainedProperties = newStudent?.ExtensionProperties.Count()??-1;

        Assert.IsNotNull(newStudent, "Materializing a new Student from a serialized Person should not return null.");
        Assert.That(retainedProperties, Is.EqualTo(expectedExtensionProperties), "Materializing a Student from a Person should not have any extension properties.");
    }

    [Test]
    public void TestStudentToPersonToStudent_ExpectRetainedValues()
    {
        // We'll use the Json of our base level Student instance to create a Person instance.
        // Then, to make sure the "student" properties are kept safe, we'll reserialize a new Student from
        // our local Person instance's json.
        Person? newPerson = FxClass.Rebuild<Person>(_student.AsJson());
        Student? newStudent2 = FxClass.Rebuild<Student>(newPerson?.AsJson()??string.Empty);

        // Let's make sure somethign REALLY stupid didn't happen.
        Assert.IsNotNull(newStudent2, "Bouncing instance types back and forth along the same inheritance branch should not result in a null reference.");
        
        // Now, we'll compare our new Student instance to our original Student instance.
        Assert.That(_student.School, Is.EqualTo(newStudent2?.School));
        Assert.That(_student.Grade, Is.EqualTo(newStudent2?.Grade));
        Assert.That(_student.Name, Is.EqualTo(newStudent2?.Name));
    }
}