using SoftwareParticles.DynamicScriptExecution.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftwareParticles.DynamicScriptExecution.DotNet
{
    public class CodeTemplateResult 
    { 
        public object Instance { get; }
        public MethodInfo Method { get; }

        public CodeTemplateResult(object instance, MethodInfo method)
        {
            Instance = instance;
            Method = method;
        }
    }

    public abstract class CodeTemplate
    {
        protected string _generatedCode;
        public virtual string GeneratedCodeNamespaceName => "MyNamespace";
        public virtual string GeneratedCodeClassName => "MyClass";
        public virtual string GeneratedCodeMethodName => "Run";

        public abstract CodeTemplateResult CreateInstance(DotNetCallArguments instanceArgs, Assembly assembly);

        public string Build(DotNetDynamicScriptParameter scriptParameters)
        {
            var _generatedCode = GetCodeTemplate();
            scriptParameters.Imports.AddRange(DefaultImports);
            BuildImports(scriptParameters.Imports, ref _generatedCode);
            BuildMethodParameters(scriptParameters, ref _generatedCode);
            BuildBody(scriptParameters.Script, ref _generatedCode);
            return _generatedCode;
        }

        public virtual string GetInstanceSignature() => $"{GeneratedCodeNamespaceName}.{GeneratedCodeClassName}";
        public virtual string GetMethodName() => GeneratedCodeMethodName;

        protected abstract List<string> DefaultImports { get; }

        public abstract int GetCodeLineOffset(string code);

        protected abstract string GetCodeTemplate();

        protected abstract void BuildMethodParameters(DotNetDynamicScriptParameter p, ref string code);

        protected abstract void BuildImports(List<string> imports, ref string code);

        private void BuildBody(string methodBody, ref string code)
        {
            code = code.Replace("{code}", methodBody);
        }
    }

    public abstract class MethodBodyCodeTemplate : CodeTemplate
    {
        protected override void BuildMethodParameters(DotNetDynamicScriptParameter operationParams, ref string code)
        {
            var methodParameters = string.Empty;
            if (operationParams != null && operationParams.Parameters != null && operationParams.Parameters.Any())
                methodParameters = DoBuildMethodParameters(operationParams.Parameters);
            code = code.Replace("{methodParameters}", methodParameters);
        }

        protected abstract string DoBuildMethodParameters(List<ParameterDefinition> parameterDefinitions);

        public override CodeTemplateResult CreateInstance(DotNetCallArguments instanceArgs, Assembly assembly)
        {
            var instance = assembly.CreateInstance(GetInstanceSignature());
            var method = instance.GetType().GetMethod(GetMethodName());

            return new CodeTemplateResult(instance, method);
        }
    }

    public class ClassCodeTemplate : CodeTemplate
    {
        public override int GetCodeLineOffset(string code) => 0;

        protected override string GetCodeTemplate() => "{code}";

        protected override List<string> DefaultImports => new List<string>();

        protected override void BuildImports(List<string> imports, ref string code) { }

        protected override void BuildMethodParameters(DotNetDynamicScriptParameter p, ref string code) { }

        public override CodeTemplateResult CreateInstance(DotNetCallArguments args, Assembly assembly)
        {
            // foreach class find method? i to methodtocall na einai klasi me ola ta parameters
            object instance = null;
            if(string.IsNullOrEmpty(args.InstanceSignature))
            {
                var type = assembly.GetExportedTypes().FirstOrDefault(x => x.GetMethod(args.MethodName) != null);
                instance = Activator.CreateInstance(type);
            }else
                instance = assembly.CreateInstance(args.InstanceSignature);
            
            var method = instance.GetType().GetMethod(args.MethodName ?? GetMethodName());

            return new CodeTemplateResult(instance, method);
        }
    }
}
