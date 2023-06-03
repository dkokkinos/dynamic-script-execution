using NUnit.Framework;
using SoftwareParticles.DynamicScriptExecution.Core;
using SoftwareParticles.DynamicScriptExecution.DotNet;
using SoftwareParticles.DynamicScriptExecution.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotNetEngineTests
{
    public class VisualBasicWithMethodTemplateTests : DynamicScriptExecutionTestBase
    {
        [Test]
        public void WithInputParamValue()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
            File.Delete(path);

            var controller = new VisualBasicDynamicScriptController(new VisualBasicMethodBodyCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter($@"
System.IO.File.WriteAllText(path, ""contents"")
", parameters: new List<ParameterDefinition>()
            {
                new ParameterDefinition("path", ParameterDefinitionType.String, ParameterDirection.Input)
            }));

            controller.Execute(methodArgs: new List<ParameterArgument>() {
                new ParameterArgument("path", path)
            });

            var contents = File.ReadAllText(path);
            File.Delete(path);
            Assert.AreEqual("contents", contents);
        }

        [Test]
        public void WithInputParamAndOutParamValue()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
            File.WriteAllText(path, "contents");

            var controller = new VisualBasicDynamicScriptController(new VisualBasicMethodBodyCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter($@"
Dim content = System.IO.File.ReadAllText(path)
length = content.length
", parameters: new List<ParameterDefinition>()
            {
                new ParameterDefinition("path", ParameterDefinitionType.String, ParameterDirection.Input),
                new ParameterDefinition("length", ParameterDefinitionType.Int, ParameterDirection.Output)
            }));

            var executionResult = controller.Execute(methodArgs: new List<ParameterArgument>() {
                new ParameterArgument("path", path)
            });

            File.Delete(path);

            Assert.AreEqual(executionResult["length"], "contents".Length);
        }

        [Test]
        public void WithInputParamAndOutParamAndExternalReferencesValue()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
            File.WriteAllText(path, "contents");

            var controller = new VisualBasicDynamicScriptController(new VisualBasicMethodBodyCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter($@"
Dim content = System.IO.File.ReadAllText(path)
length = content.length
result = JsonConvert.SerializeObject(New With {{ content, length }})
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
        }

        [Test]
        public void WithInputOutParam()
        {
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
        }


        [Test]
        public void WithInputParamAndOutParamAndExternalReferencesWrongArgumentOrder()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
            File.WriteAllText(path, "contents");

            var controller = new VisualBasicDynamicScriptController(new VisualBasicMethodBodyCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter($@"
Dim content = System.IO.File.ReadAllText(path)
length = content.Length + CInt(extraLength)
result = JsonConvert.SerializeObject(New With {{ content, length }})
", parameters: new List<ParameterDefinition>()
            {
                new ParameterDefinition("path", ParameterDefinitionType.String, ParameterDirection.Input),
                new ParameterDefinition("length", ParameterDefinitionType.Int, ParameterDirection.Output),
                new ParameterDefinition("result", ParameterDefinitionType.String, ParameterDirection.Output),
                new ParameterDefinition("extraLength", ParameterDefinitionType.Decimal, ParameterDirection.InputOutput),
            }, imports: new List<string>() { "Newtonsoft.Json" }, references: new List<string>() { NewtonsoftLocation }));

            var executionResult = controller.Execute(methodArgs: new List<ParameterArgument>() {
                new ParameterArgument("extraLength", 10m),
                // new ParameterArgument("length"), // Ignore this
                new ParameterArgument("path", path),
                new ParameterArgument("result")
            });

            File.Delete(path);

            Assert.AreEqual(executionResult["length"], "contents".Length + 10);
            Assert.AreEqual(executionResult["result"], @"{""content"":""contents"",""length"":18}");
        }

        [Test]
        public void WithInputOutAndExecutionErrorParam()
        {
            var controller = new VisualBasicDynamicScriptController(new VisualBasicMethodBodyCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter($@"
ddate = ddate.AddDays(10)
", parameters: new List<ParameterDefinition>()
            {
                new ParameterDefinition("ddate", ParameterDefinitionType.Datetime, ParameterDirection.InputOutput)
            }));

            var executionResult = controller.Execute(methodArgs: new List<ParameterArgument>() {
                new ParameterArgument("ddate", "wrong input type")
            });

            Assert.IsFalse(executionResult.Success);
            Assert.AreEqual(1, executionResult.Errors.Count);
        }

        [Test]
        public void WithInputOutAndCompilationErrorParam()
        {
            var controller = new VisualBasicDynamicScriptController(new VisualBasicMethodBodyCodeTemplate());
            var result = controller.Evaluate(new DotNetDynamicScriptParameter($@"
ddate  ddate.AddDays(10) // missing =
", parameters: new List<ParameterDefinition>()
            {
                new ParameterDefinition("ddate", ParameterDefinitionType.Datetime, ParameterDirection.InputOutput)
            }));

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.All(x=>x.FromLine == 2 && x.ToLine == 2));
        }
    }
}
