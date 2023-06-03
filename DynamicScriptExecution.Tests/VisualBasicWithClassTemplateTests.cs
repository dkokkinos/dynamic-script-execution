using Newtonsoft.Json;
using NUnit.Framework;
using SoftwareParticles.DynamicScriptExecution.Core;
using SoftwareParticles.DynamicScriptExecution.DotNet;
using SoftwareParticles.DynamicScriptExecution.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEngineTests
{
    public class VisualBasicWithClassTemplateTests : DynamicScriptExecutionTestBase
    {
        [Test]
        public void WithMethodReturnValue()
        {
            var controller = new VisualBasicDynamicScriptController(new ClassCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter(@"Imports System
Namespace Test
    Public Class TestClass
        Public Function Run() As Integer
            Return 1
        End Function
    End Class
End Namespace"));

            var executionResult = controller.Execute(
                new DotNetCallArguments(namespaceName: "Test", className: "TestClass", methodName: "Run"));

            Assert.AreEqual(1, executionResult.ReturnValue);
        }

        [Test]
        public void WithMethodReturnWithoutNamespace()
        {
            var controller = new VisualBasicDynamicScriptController(new ClassCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter(@"Imports System

Public Class TestClass
    Public Function Run() As Integer
        Return 1
    End Function
End Class
"));

            var executionResult = controller.Execute(
                new DotNetCallArguments(className: "TestClass", methodName: "Run"),
                new List<ParameterArgument>() { });

            Assert.AreEqual(1, executionResult.ReturnValue);
        }

        [Test]
        public void WithNamespanceAndParameters()
        {
            var controller = new VisualBasicDynamicScriptController(new ClassCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter(@"Imports System
Namespace Test
    Public Class TestClass
        Public Function Run(ByVal i As Integer) As Integer
            Return i
        End Function
    End Class
End Namespace
"));

            var executionResult = controller.Execute(
                new DotNetCallArguments(namespaceName: "Test", className: "TestClass", methodName: "Run"),
                new List<ParameterArgument>() { new ParameterArgument("i", 5) });

            Assert.AreEqual(5, executionResult.ReturnValue);
        }

        [Test]
        public void WithNamespaceAndMainMethod()
        {
            var controller = new VisualBasicDynamicScriptController(new ClassCodeTemplate());

            var path = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
            File.Delete(path);

            controller.Evaluate(new DotNetDynamicScriptParameter($@"Imports System
Namespace Test
    Public Class TestClass
        Public Shared Sub Main()
            System.IO.File.WriteAllText(""{ path }"", ""contents"")
        End Sub
    End Class
End Namespace"));

            controller.Execute();

            var contents = File.ReadAllText(path);
            File.Delete(path);
            Assert.AreEqual("contents", contents);
        }

        [Test]
        public void WithNamespaceAndMainMethodAndParametersAndReturnValue()
        {
            var controller = new VisualBasicDynamicScriptController(new ClassCodeTemplate());

            controller.Evaluate(new DotNetDynamicScriptParameter($@"Imports System
Namespace Test
    Public Class TestClass
        Public Shared Function Main(ByVal args As String()) As Integer
            Return args.Length
        End Function
    End Class
End Namespace"));

            var executionResult = controller.Execute(methodArgs: new List<ParameterArgument>()
            {
                new ParameterArgument("args", new string[] { "arg1", "arg2" })
            });

            Assert.AreEqual(2, executionResult.ReturnValue);
        }

        [Test]
        public void WithNamespaceAndAnyMethodAndParametersAndReturnValue()
        {
            var controller = new VisualBasicDynamicScriptController(new ClassCodeTemplate());

            controller.Evaluate(new DotNetDynamicScriptParameter($@"Imports System
Namespace Test
    Public Class TestClass
        Public Shared Function Main(ByVal args As String()) As Integer
            Return args.Length
        End Function
    End Class
End Namespace"));

            var executionResult = controller.Execute(callArgs: new DotNetCallArguments("Test", "TestClass", "Main"), methodArgs: new List<ParameterArgument>()
            {
                new ParameterArgument("args", new string[] { "arg1", "arg2" })
            });

            Assert.AreEqual(2, executionResult.ReturnValue);
        }


        public class CustomObject
        {
            public string Name { get; set; }

        }

        [Test]
        public void WithNamespaceAndMainMethodAndParametersAndReturnValueAndExternalLibraries()
        {
            var controller = new VisualBasicDynamicScriptController(new ClassCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter($@"Imports System
Imports Newtonsoft.Json

Namespace Test
    Public Class CustomObject
        Public Property Name As String
    End Class

    Public Class TestClass
        Public Shared Function Main(ByVal args As String()) As CustomObject
            Return JsonConvert.DeserializeObject(Of CustomObject)(args(0))
        End Function
    End Class
End Namespace", references: new List<string>() { NewtonsoftLocation }));

            var executionResult = controller.Execute(methodArgs: new List<ParameterArgument>()
            {
                new ParameterArgument("args", new string[] { JsonConvert.SerializeObject(new CustomObject() { Name = "a name"}) })
            });

            var obj = executionResult.ReturnValue;
            var deserializedObj = JsonConvert.DeserializeObject<CustomObject>(JsonConvert.SerializeObject(obj));
            Assert.AreEqual("a name", deserializedObj.Name);
        }
    }

}
