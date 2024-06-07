using MyFx.ObjectModel;
using ObjectModel.Tests.Classes;
using ObjectModel.Tests.Processors;

namespace ObjectModel.Tests;

public class StudentProcessor : IStudentProcessor
{
    public Type StrategySubject => typeof(Student);

    public void ExecuteActionStrategy(string actionName, FxClass actionSubject)
    {
        //Guard ActionNameExists
        GuardActionNameExists(actionName);
        //Guard SubjectIsValid
        GuardSubjectIsValid(actionSubject);

        Student? subject = FxClass.Rebuild<Student>(actionSubject.AsJson());
        if(subject==null)
        {
            throw new Exception("Failed to rehydrate");
        }

        var action = this.GetType().GetMethod(actionName);
        action?.Invoke(this, new[]{subject});
    }

    public T? ExecuteFunctionStrategy<T>(string functioNName, FxClass obj) where T:FxClass
    {
        GuardFunctionExists(functioNName);
        //GuardSubjectIsValid(obj);

        Student? subject = FxClass.Rebuild<Student>(obj.AsJson());
        if(subject == null)
        {
            throw new Exception("Failed to rehydrate");
        }

        var function = this.GetType().GetMethod(functioNName);
        var result = (function?.Invoke(this, new[]{subject})) as T;
        return result;
    }

    public void Display(Student obj)
    {
        Console.WriteLine($"Student: {obj.Name}, Grade - {obj.Grade}, School - {obj.School}");
    }

    public Student Normalize(Student subject)
    {
        subject.School = subject.School.ToUpper();
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
        if (actionSubject.TypeName != StrategySubject.Name)
        {
            throw new Exception("Invalid Subject Type");
        }
    }
}
