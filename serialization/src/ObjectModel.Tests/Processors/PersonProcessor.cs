using MyFx.ObjectModel;
using ObjectModel.Tests.Classes;
using ObjectModel.Tests.Processors;

namespace ObjectModel.Tests;

public class PersonProcessor : IPersonProcessor
{
    public Type StrategySubject => typeof(Person);

    public void ExecuteActionStrategy(string actionName, FxClass obj)
    {
        GuardActionNameExists(actionName);
        GuardSubjectIsValid(obj);

        Person? subject = FxClass.Rebuild<Person>(obj.AsJson());
        if(subject == null)
        {
            throw new Exception("Failed to rehydrate");
        }

        var action = this.GetType().GetMethod(actionName);
        action?.Invoke(this, new[]{subject});
        
    }

    public T? ExecuteFunctionStrategy<T>(string functionName, FxClass obj) where T : FxClass
    {
        GuardFunctionExists(functionName);
        GuardSubjectIsValid(obj);

        Person? subject = FxClass.Rebuild<Person>(obj.AsJson());
        if(subject == null)
        {
            throw new Exception("Failed to rehydrate");
        }

        var function = this.GetType().GetMethod(functionName);
        var result = (function?.Invoke(this, new[]{subject})) as T;
        return result;
    }

    public void Display(Person obj)
    {
        Console.WriteLine($"Person: {obj.Name}");
    }

    public Person Normalize(Person subject)
    {
        subject.Name = subject.Name.ToUpper();
        return subject;
    }

    private void GuardActionNameExists(string actionName)
    {
        if(this.GetType().GetMethods().Any(m => m.Name == actionName) == false)
        {
            throw new Exception("Invalid Action Name");
        }
    }

    private void GuardFunctionExists(string functionName)
    {
        if(this.GetType().GetMethods().Any(m => m.Name == functionName) == false)
        {
            throw new Exception("Invalid Function Name");
        }
    }

    private void GuardSubjectIsValid(FxClass actionSubject)
    {
        //If the subject type is the same as the strategy subject, then we're definitely good.
        if (actionSubject.TypeName == StrategySubject.Name)
        {
            return;
        }

        //If the subject type is a subclass of the strategy subject, then we're also good.
        Type providedSubjectType = actionSubject.GetType();
        if(providedSubjectType.IsSubclassOf(StrategySubject))
        {
            return;
        }

        // Otherwise, we need to bail.
        throw new Exception("Invalid Subject Type");
    }

    
}
