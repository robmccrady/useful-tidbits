using MyFx.ObjectModel;
using ObjectModel.Tests.Classes;

namespace ObjectModel.Tests.Processors;

public class TypedProcessor : IProcessor
{

    private readonly Dictionary<string, IStrategyProvider> _processors;

    public TypedProcessor()
    {
        _processors = new Dictionary<string, IStrategyProvider>();
        // We'd never directly register our Typed Processor Strategies in the constructor of the root.
        // Instead, we'd likely create some kind of factory that would create them on demand, or
        // instantiate them outside of our root processor class and pass them in.

        _processors.Add(typeof(Person).Name, new PersonProcessor());
        _processors.Add(typeof(Student).Name, new StudentProcessor());
    }

    public void Display(FxClass obj)
    {

        var processor = GetProcessor(obj.TypeName, "");
        if(processor == null){
            Console.WriteLine("No processor found for type: " + obj.TypeName);
            return;
        }

        processor.ExecuteActionStrategy(nameof(Display), obj);
    }

    public void DisplayAs(FxClass obj, string overrideProcessorType)
    {

        var processor = GetProcessor(obj.TypeName, overrideProcessorType);
        if(processor == null)
        {
            Console.WriteLine("No processor found for type: " + overrideProcessorType);
            return;  
        }

        processor.ExecuteActionStrategy(nameof(Display), obj);
    }

    public FxClass Normalize(FxClass obj)
    {
        Student? student = Student.Rebuild<Student>(obj.AsJson());
        if(student == null)
            throw new ArgumentException("Cannot Normalize an object of type: " + obj.TypeName);

        // We want to "pipeline" our obj through a series of processors.
        var personProc = GetProcessor(typeof(Person).Name);
        var studentProc = GetProcessor(typeof(Student).Name);

        var result = personProc?.ExecuteFunctionStrategy<Person>(nameof(Normalize), obj)??obj;
        result = studentProc?.ExecuteFunctionStrategy<Student>(nameof(Normalize), result)??obj;
        
        return result;
    }

    private IStrategyProvider? GetProcessor(string subjectType, string overrideProcessorType = "")
    {
        string processorType = string.IsNullOrWhiteSpace(overrideProcessorType) ? subjectType : overrideProcessorType;
        if (_processors.TryGetValue(processorType, out IStrategyProvider? processor))
        {
            return processor;
        }

        return null;
    }
}
