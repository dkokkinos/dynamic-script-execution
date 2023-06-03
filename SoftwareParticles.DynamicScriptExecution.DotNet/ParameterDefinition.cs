using System;

namespace SoftwareParticles.DynamicScriptExecution.Core
{
    public class DotNetCallArguments : CallArguments
    {
        public string NamespaceName { get; }
        public string ClassName { get; }

        public DotNetCallArguments(string namespaceName = null, string className = null, string methodName = null)
            : base(methodName ?? "Main")
        {
            NamespaceName = namespaceName;
            ClassName = className;
        }

        public string InstanceSignature
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(NamespaceName) && !string.IsNullOrWhiteSpace(ClassName))
                    return $"{NamespaceName}.{ClassName}";
                if (string.IsNullOrWhiteSpace(NamespaceName) && !string.IsNullOrWhiteSpace(ClassName))
                    return ClassName;

                return string.Empty;
            }
        }
    }

    public class ParameterDefinition
    {
        public string Key { get; }
        public ParameterDefinitionType Type { get; }
        public ParameterDirection Direction { get; }

        public ParameterDefinition(string key, ParameterDefinitionType type, ParameterDirection direction)
        {
            Key = key;
            Type = type;
            Direction = direction;
        }
    }

    public enum ParameterDirection
    {
        Input,
        Output,
        InputOutput
    }

    public enum ParameterDefinitionType
    {
        Bool,
        Int,
        Datetime,
        Datatable,
        Double,
        Decimal ,
        Dynamic,
        Long,
        String ,
        List
    }

}