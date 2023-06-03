# How To Dynamically Execute Code In .NET.

Executing code dynamically without precompiling is a powerful feature that can enhance the capabilities of an application. In this article we explore how we can execute C# and Visual Basic code dynamically at runtime, without the need for precompilation. We will dive into the steps involved in the dynamic compilation process, including referencing external DLLs, adding imports, and handling potential errors. By the end of this article, you will have a full understanding of dynamic code execution with a fully functional library that takes code as input, executes it dynamically, and returns the desired result.

# Usage examples
These examples demonstrate the usage of the dynamic script library in different scenarios.

## Example1: Execute a C# code containing a whole class and get the result

```csharp
var controller = new CSharpDynamicScriptController(new ClassCodeTemplate());
controller.Evaluate(new DotNetDynamicScriptParameter(@"using System;
namespace Test
{
    public class TestClass
    {
        public int Run() {
            return 1;
        } 
    }
}"));


var executionResult = controller.Execute(
   new DotNetCallArguments(namespaceName: "Test", className: "TestClass", methodName: "Run"),
   new List<ParameterArgument>() { });
Console.WriteLine(executionResult.ReturnValue);
```

This example compiles and executes a C# code containing a class TestClass with a method Run that returns an integer. It uses the CSharpDynamicScriptController with the ClassCodeTemplate to evaluate and execute the code. The result of the execution is obtained and printed.

## Example 2: Execute C# code by providing only the method body, parameters, and an external reference

```csharp
var path = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
File.WriteAllText(path, "contents");

var controller = new CSharpDynamicScriptController(new CSharpMethodBodyCodeTemplate());
controller.Evaluate(new DotNetDynamicScriptParameter($@"
var content = System.IO.File.ReadAllText(path);
length = content.Length;
result = JsonConvert.SerializeObject(new {{ content, length }});
", parameters: new List<ParameterDefinition>()
{
   new("path", ParameterDefinitionType.String, ParameterDirection.Input),
   new("length", ParameterDefinitionType.Int, ParameterDirection.Output),
   new("result", ParameterDefinitionType.String, ParameterDirection.Output)
}, imports: new List<string>() { "Newtonsoft.Json" }, references: new List<string>() { NewtonsoftLocation }));

var executionResult = controller.Execute(methodArgs: new List<ParameterArgument>() {
   new("path", path),
   new("length"),
   new("result")
});

File.Delete(path);

Assert.AreEqual(executionResult["length"], "contents".Length);
Assert.AreEqual(executionResult["result"], @"{""content"":""contents"",""length"":8}");
```

In this example, C# code is executed by providing the method body, parameters, and an external reference to Newtonsoft.Json. The code reads the contents of a file, calculates its length, and serializes the result using Newtonsoft.Json. The CSharpDynamicScriptController with the CSharpMethodBodyCodeTemplate is used.

## Example 3: Execute Visual Basic code by providing the method body and a Datetime InputOutput argument

```csharp
var controller = new VisualBasicDynamicScriptController(new VisualBasicMethodBodyCodeTemplate());
controller.Evaluate(new DotNetDynamicScriptParameter($@"
ddate = ddate.AddDays(10)
", parameters: new List<ParameterDefinition>()
{
   new ParameterDefinition("ddate", ParameterDefinitionType.Datetime, ParameterDirection.InputOutput)
}));

var executionResult = controller.Execute(methodArgs: new List<ParameterArgument>() {
   new ParameterArgument("ddate", new DateTime(2020, 2, 2))
});

Assert.AreEqual(executionResult.GetValue<DateTime>("ddate").Day, 12);
```

In this example, Visual Basic code is executed by providing the method body and a DateTime InputOutput argument. The code adds 10 days to the provided DateTime value. The VisualBasicDynamicScriptController with the VisualBasicMethodBodyCodeTemplate is used.
