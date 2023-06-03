using Newtonsoft.Json;
using NUnit.Framework;
using SoftwareParticles.DynamicScriptExecution.Core;
using SoftwareParticles.DynamicScriptExecution.CSharp;
using SoftwareParticles.DynamicScriptExecution.DotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEngineTests
{
    public class CSharpWithClassTemplateTests : DynamicScriptExecutionTestBase
    {
        [Test]
        public void WithMethodReturnValue()
        {
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

            Assert.AreEqual(1, executionResult.ReturnValue);
        }

        [Test]
        public void WithMethodReturnWithoutNamespace()
        {
            var controller = new CSharpDynamicScriptController(new ClassCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter(@"using System;
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
        public void WithNamespanceAndParameters()
        {
            var controller = new CSharpDynamicScriptController(new ClassCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter(@"using System;
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
        public void WithNamespaceAndMainMethod()
        {
            var controller = new CSharpDynamicScriptController(new ClassCodeTemplate());

            var path = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
            File.Delete(path);

            var result = controller.Evaluate(new DotNetDynamicScriptParameter($@"using System;
namespace Test
{{
    public class TestClass
    {{
        public static void Main() {{
            System.IO.File.WriteAllText(@""{path}"", ""contents"");
        }} 
    }}
}}"));

            controller.Execute();

            var contents = File.ReadAllText(path);
            File.Delete(path);
            Assert.AreEqual("contents", contents);
        }

        [Test]
        public void WithNamespaceAndMainMethodAndParametersAndReturnValue()
        {
            var controller = new CSharpDynamicScriptController(new ClassCodeTemplate());

            controller.Evaluate(new DotNetDynamicScriptParameter($@"using System;
namespace Test
{{
    public class TestClass
    {{
        public static int Main(string [] args) {{
            return args.Length;
        }} 
    }}
}}"));

            var executionResult = controller.Execute(methodArgs: new List<ParameterArgument>()
            {
                new ParameterArgument("args", new string[] { "arg1", "arg2" })
            });

            Assert.AreEqual(2, executionResult.ReturnValue);
        }

        [Test]
        public void WithNamespaceAndAnyMethodAndParametersAndReturnValue()
        {
            var controller = new CSharpDynamicScriptController(new ClassCodeTemplate());

            controller.Evaluate(new DotNetDynamicScriptParameter($@"using System;
namespace Test
{{
    public class TestClass
    {{
        public static int Main(string [] args) {{
            return args.Length;
        }} 
    }}
}}"));

            var executionResult = controller.Execute(callArgs:new DotNetCallArguments("Test", "TestClass", "Main"), methodArgs: new List<ParameterArgument>()
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
            var controller = new CSharpDynamicScriptController(new ClassCodeTemplate());
            controller.Evaluate( new DotNetDynamicScriptParameter($@"using System;
using Newtonsoft.Json;

namespace Test
{{
    public class CustomObject
    {{
        public string Name {{ get; set; }}
    }}

    public class TestClass
    {{
        public static CustomObject Main(string [] args) {{
            return JsonConvert.DeserializeObject<CustomObject>(args[0]);
        }} 
    }}
}}", references: new List<string>() { NewtonsoftLocation }));

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
