﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{

    public sealed class AttributeInfo
    {
        public class Argument
        {
            public ParameterInfo Parameter { get; }
            public object        Value     { get; }

            public Argument(ParameterInfo parameter, object value)
            {
                this.Parameter = parameter;
                this.Value     = value;
            }
        }

        public TypeInfo                            Type            { get; }
        public MethodMemberInfo                    Constructor     { get; }
        public IList<object>                       PassedArguments { get; }
        public IReadOnlyDictionary<string, object> NamedArguments  { get; }

        public AttributeInfo(TypeInfo type, MethodMemberInfo constructor, IList<object> passedArguments, IReadOnlyDictionary<string, object> namedArguments)
        {
            this.Type            = type            ?? throw new ArgumentNullException(nameof(type));
            this.Constructor     = constructor     ?? throw new ArgumentNullException(nameof(constructor));
            this.PassedArguments = passedArguments ?? throw new ArgumentNullException(nameof(passedArguments));
            this.NamedArguments  = namedArguments  ?? throw new ArgumentNullException(nameof(namedArguments));

            if (this.Constructor.Parameters.Count() != passedArguments.Count)
            {
                throw new ArgumentException(
                   "Number of passed arguments and constructor parameters in an attribute do not match. " +
                   "Attribute {" + this.Type.Namespace + "." + this.Type.Name + "}.");
            }
        }

        public static AttributeInfo Create(AttributeData attributeData)
        {
            return new AttributeInfo(
                TypeInfo.FromSymbol(attributeData.AttributeClass),
                MethodMemberInfo.FromSymbol(attributeData.AttributeConstructor),
                attributeData.ConstructorArguments.Select(getPassedArg).ToArray(),
                attributeData.NamedArguments.ToDictionary(x => x.Key, x => getPassedArg(x.Value)));

            object getPassedArg(TypedConstant arg)
            {
                if (arg.Kind == TypedConstantKind.Array)
                    return arg.Values.Select(getPassedArg).Select(transformArg).ToArray();

                else
                    return transformArg(arg.Value);
            }

            object transformArg(object arg)
            {
                if (arg.GetType().IsPrimitive)
                    return arg;

                if (arg is INamedTypeSymbol s)
                    return TypeInfo.FromSymbol(s);

                return arg;
            }
        }
    }
}
