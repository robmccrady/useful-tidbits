using MyFx.ObjectModel;
using ObjectModel.Tests.Classes;

namespace ObjectModel.Tests.Processors;

/// <summary>
/// Provides a rudimentary way of registering non-generic IObjectProcessors so that they can be dynamically added to a root
/// object processor.
/// </summary>
public interface IStrategyProvider
{
    Type StrategySubject { get; }
    void ExecuteActionStrategy(string actionName, FxClass actionSubject);

    T? ExecuteFunctionStrategy<T>(string functionName, FxClass obj) where T: FxClass;
}


public interface IObjectProcessor<T> where T: FxClass
{
    void Display(T obj);

    T Normalize(T obj);
}

public interface IProcessor: IObjectProcessor<FxClass>
{
    void DisplayAs(FxClass subject, string overrideTypeame);
}

public interface IPersonProcessor : IStrategyProvider, IObjectProcessor<Person> 
{
    
}

public interface IStudentProcessor : IStrategyProvider, IObjectProcessor<Student> 
{
    
}
