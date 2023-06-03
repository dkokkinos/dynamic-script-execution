using Newtonsoft.Json;
using NUnit.Framework;
using SoftwareParticles.DynamicScriptExecution.Core;
using SoftwareParticles.DynamicScriptExecution.CSharp;
using SoftwareParticles.DynamicScriptExecution.CSharp.CodeTemplates;
using SoftwareParticles.DynamicScriptExecution.DotNet;
using SoftwareParticles.DynamicScriptExecution.Python;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DynamicScriptExecutionTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        // execute class with main method,
        // execute class without main method, prepei na kanoume emeis provide ena method name gia call
        // execute method me to method template

        [Test]
        public void ExecuteCSharpWithClassTemplateWithMethodReturnValue()
        {
            var controller = new CSharpDynamicScriptController(new ClassCodeTemplate());
            var result = controller.Evaluate(new DotNetDynamicScriptParameter(@"using System;
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

            Assert.AreEqual(1, executionResult.ReturnValue);
        }

        [Test]
        public void CSharpWithClassTemplateWithMethodReturnWithoutProvidingNamespaceCallArgumentValue()
        {
            var controller = new CSharpDynamicScriptController(new ClassCodeTemplate());
            var result = controller.Evaluate(new DotNetDynamicScriptParameter(@"using System;
public class TestClass
{
    public int Run() {
        return 1;
    } 
}"));

            var executionResult = controller.Execute(
                new DotNetCallArguments(className: "TestClass", methodName: "Run"),
                new List<ParameterArgument>() { });

            Assert.AreEqual(1, executionResult.ReturnValue);
        }

        [Test]
        public void ExecuteCSharpWithClassTemplateWithParametersTest()
        {
            var controller = new CSharpDynamicScriptController(new ClassCodeTemplate());
            var result = controller.Evaluate(new DotNetDynamicScriptParameter(@"using System;
namespace Test
{
    public class TestClass
    {
        public int Run(int i) {
            return i;
        } 
    }
}"));

            var executionResult = controller.Execute(
                new DotNetCallArguments(namespaceName: "Test", className: "TestClass", methodName: "Run"),
                new List<ParameterArgument>() { new ParameterArgument("i", 5) });

            Assert.AreEqual(5, executionResult.ReturnValue);
        }

        [Test]
        public void ExecuteCSharpWithMethodTemplateWithRefParametersTest()
        {
            var controller = new CSharpDynamicScriptController(new CSharpMethodBodyCodeTemplate());
            var result = controller.Evaluate(new DotNetDynamicScriptParameter(@"x = new DateTime(2020,1,1);",
                parameters: new List<ParameterDefinition>()
                {
                    new ParameterDefinition("x", ParameterDefinitionType.Datetime, ParameterDirection.Output)
                }));

            var executionResult = controller.Execute(
                new DotNetCallArguments(namespaceName: "Test", className: "TestClass", methodName: "Run"),
                new List<ParameterArgument>() { new ParameterArgument("x") });

            Assert.AreEqual(2020, executionResult.GetValue<DateTime>("x").Year);
        }

        [Test]
        public void ExecuteCSharpWithMethodTemplateWithRefParametersTest2()
        {
            var r = JsonConvert.SerializeObject(new DateTime(2020, 1, 1));
            var controller = new CSharpDynamicScriptController(new CSharpMethodBodyCodeTemplate());
            var result = controller.Evaluate(new DotNetDynamicScriptParameter(@"x = new DateTime(2020,1,1);
y = JsonConvert.SerializeObject(x);
z = y + z;",
                imports: new List<string>() { "Newtonsoft.Json" },
                references: new List<string>()
                {
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Newtonsoft.Json.dll")
                },
                parameters: new List<ParameterDefinition>()
                {
                    new ParameterDefinition("x", ParameterDefinitionType.Datetime, ParameterDirection.Output),
                    new ParameterDefinition("y", ParameterDefinitionType.String, ParameterDirection.Output),
                    new ParameterDefinition("z", ParameterDefinitionType.String, ParameterDirection.InputOutput),
                }));

            var executionResult = controller.Execute(
                new DotNetCallArguments(namespaceName: "Test", className: "TestClass", methodName: "Run"),
                new List<ParameterArgument>() { new ParameterArgument("x"), new ParameterArgument("y"), new ParameterArgument("z", "123") });

            Assert.AreEqual("\"2020-01-01T00:00:00\"123", executionResult["z"]);
        }

        [Test]
        public void CSharpMethodBodyTest()
        {
            var controller = new CSharpDynamicScriptController(new CSharpMethodBodyCodeTemplate());
            var result = controller.Evaluate(new DotNetDynamicScriptParameter(@""));

        }

        [Test]
        public void PythonTest()
        {
            var controller = new PythonDynamicScriptController();
            var res = controller.Evaluate(new PythonEvaluationParameters(@"
def greetings():
    return 'Hello '
"));

            var r = controller.Execute(new CallArguments(methodName: "greetings"), new List<ParameterArgument>());
        }

        [Test]
        public void PythonMethodTemplateExecutionTest()
        {
            var controller = new PythonDynamicScriptController();

        }
    }
}