using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Domain.Foundation.SourceGenerator
{
    internal static class Extensions
    {

        private static string _iApply = "IApply";

        public static bool ImplementInterface(this INamedTypeSymbol type, INamedTypeSymbol interfaceSymbol)
        {
            if (!interfaceSymbol.IsGenericType)
                return type.AllInterfaces.Any(x => SymbolEqualityComparer.Default.Equals(x, interfaceSymbol));
            
            return type.AllInterfaces
                .Where(x => x.IsGenericType)
                .Any(x => SymbolEqualityComparer.Default.Equals(x.ConstructUnboundGenericType(), interfaceSymbol));
        }
        
        public static bool InheritType(this INamedTypeSymbol type, INamedTypeSymbol interfaceSymbol)
        {
            if (!interfaceSymbol.IsGenericType)
                return type.GetBaseTypes().Any(x => SymbolEqualityComparer.Default.Equals(x, interfaceSymbol));
            
            return type.GetBaseTypes()
                .Where(x => x.IsGenericType)
                .Any(x => SymbolEqualityComparer.Default.Equals(x.ConstructUnboundGenericType(), interfaceSymbol));
        }
        
        static IEnumerable<INamedTypeSymbol> GetBaseTypes(this INamedTypeSymbol type)
        {
            var baseType = type.BaseType;
            while (baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
        }

        public static string GetIApplyInterfaceName(this INamedTypeSymbol symbol) 
            => $"{_iApply}{symbol.Name.TrimStart('I')}";

        public static string GetIApplyInterfacesDeclaration(this IEnumerable<INamedTypeSymbol> symbols)
        {
            var declarations =  symbols.Select(x => x.GetIApplyInterfaceDeclaration());
            return string.Join($", {Environment.NewLine}\t\t", declarations);
        }

        public static IEnumerable<string> GetIApplyInterfaceDeclaration(this IEnumerable<INamedTypeSymbol> symbols)
            => symbols.Select(x => x.GetIApplyInterfaceDeclaration());

        public static string GetIApplyInterfaceDeclaration(this INamedTypeSymbol symbol)
            => $"{_iApply}<{symbol.OriginalDefinition}>";
        
        public static IEnumerable<SemanticTree> SemanticTrees(this Compilation compilation) 
            => compilation
                .SyntaxTrees
                .Select(x => new SemanticTree
                {
                    SemanticModel = compilation.GetSemanticModel(x), 
                    SyntaxTree = x
                });
    }
}