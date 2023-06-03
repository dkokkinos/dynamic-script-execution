﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SoftwareParticles.DynamicScriptExecution.DotNet;
using System;
using System.IO;

namespace SoftwareParticles.DynamicScriptExecution.CSharp
{
    public class CSharpDynamicScriptController : DotNetDynamicScriptController
    {
        public CSharpDynamicScriptController(CodeTemplate codeTemplate) : base(codeTemplate)
        {
        }

        protected override Compilation GetCompilationForAssembly(string assemblyName)
            => CSharpCompilation.Create(assemblyName);

        protected override CompilationOptions GetOptions()
            => new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        protected override SyntaxTree GetSyntaxTree(string code)
        {
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp5);
            return CSharpSyntaxTree.ParseText(code, options);
        }

        protected override Compilation AddReferences(string rootPath, Compilation compilation)
        {
            return compilation.AddReferences(MetadataReference.CreateFromFile(Path.Combine(rootPath, "Microsoft.CSharp.dll")));
        }
    }
}
