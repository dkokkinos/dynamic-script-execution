using DynamicScriptExecution.Tests.Properties;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEngineTests
{
    public abstract class DynamicScriptExecutionTestBase
    {
        protected string TestsDirectory => Directory.GetCurrentDirectory();
        protected string ReferencesDirectory => Path.Combine(Directory.GetCurrentDirectory(), "references");

        protected string NewtonsoftLocation => Path.Combine(ReferencesDirectory, "Newtonsoft.Json.dll");

        [SetUp]
        public void Initialize()
        {
            if (File.Exists(NewtonsoftLocation)) File.Delete(NewtonsoftLocation);
            if (!Directory.Exists(ReferencesDirectory)) Directory.CreateDirectory(ReferencesDirectory);
            File.WriteAllBytes(NewtonsoftLocation, Resources.Newtonsoft_Json);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(ReferencesDirectory, true);
        }
    }
}
