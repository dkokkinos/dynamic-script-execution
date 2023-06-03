using NUnit.Framework;
using SoftwareParticles.DynamicScriptExecution.Core;
using SoftwareParticles.DynamicScriptExecution.CSharp;
using SoftwareParticles.DynamicScriptExecution.CSharp.CodeTemplates;
using SoftwareParticles.DynamicScriptExecution.DotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotNetEngineTests
{
    public class CSharpWithMethodTemplateTests : DynamicScriptExecutionTestBase
    {
        [Test]
        public void WithInputParamValue()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
            File.Delete(path);

            var controller = new CSharpDynamicScriptController(new CSharpMethodBodyCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter($@"
System.IO.File.WriteAllText(path, ""contents"");
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
            
            var controller = new CSharpDynamicScriptController(new CSharpMethodBodyCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter($@"
var content = System.IO.File.ReadAllText(path);
length = content.Length;
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
        }

        [Test]
        public void WithInputOutParam()
        {
            var controller = new CSharpDynamicScriptController(new CSharpMethodBodyCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter($@"
date = date.AddDays(10);
", parameters: new List<ParameterDefinition>()
            {
                new ParameterDefinition("date", ParameterDefinitionType.Datetime, ParameterDirection.InputOutput)
            }));

            var executionResult = controller.Execute(methodArgs: new List<ParameterArgument>() {
                new ParameterArgument("date", new DateTime(2020, 2, 2))
            });

            Assert.AreEqual( executionResult.GetValue<DateTime>("date").Day, 12);
        }


        [Test]
        public void WithInputParamAndOutParamAndExternalReferencesWrongArgumentOrder()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
            File.WriteAllText(path, "contents");

            var controller = new CSharpDynamicScriptController(new CSharpMethodBodyCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter($@"
var content = System.IO.File.ReadAllText(path);
length = content.Length + (int)extraLength;
result = JsonConvert.SerializeObject(new {{ content, length }});
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
            var controller = new CSharpDynamicScriptController(new CSharpMethodBodyCodeTemplate());
            controller.Evaluate(new DotNetDynamicScriptParameter($@"
date = date.AddDays(10);
", parameters: new List<ParameterDefinition>()
            {
                new ParameterDefinition("date", ParameterDefinitionType.Datetime, ParameterDirection.InputOutput)
            }));

            var executionResult = controller.Execute(methodArgs: new List<ParameterArgument>() {
                new ParameterArgument("date", "wrong input type")
            });

            Assert.IsFalse(executionResult.Success);
            Assert.AreEqual(1, executionResult.Errors.Count);
        }

        [Test]
        public void WithInputOutAndCompilationErrorParam()
        {
            var controller = new CSharpDynamicScriptController(new CSharpMethodBodyCodeTemplate());
            var result = controller.Evaluate(new DotNetDynamicScriptParameter($@"
date = date.AddDays(10) // missing ;
", parameters: new List<ParameterDefinition>()
            {
                new ParameterDefinition("date", ParameterDefinitionType.Datetime, ParameterDirection.InputOutput)
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Errors.Count());
            Assert.AreEqual(2, result.Errors.FirstOrDefault().FromLine);
            Assert.AreEqual(2, result.Errors.FirstOrDefault().ToLine);
        }
    }
}
