using Microsoft.CodeAnalysis;

namespace Sundew.DiscriminatedUnions.Analyzer
{
    public static class DiagnosticDescriptorHelper
    {
        public static DiagnosticDescriptor Create(
            string diagnosticId, 
            string titleResourceId, 
            string messageFormatResourceId, 
            string category, 
            DiagnosticSeverity diagnosticSeverity,
            bool isEnabledByDefault,
            string descriptionResourceId)
        {
            return new DiagnosticDescriptor(
                diagnosticId,
                new LocalizableResourceString(titleResourceId, Resources.ResourceManager, typeof(Resources)),
                new LocalizableResourceString(messageFormatResourceId, Resources.ResourceManager, typeof(Resources)),
                category,
                diagnosticSeverity,
                isEnabledByDefault,
                new LocalizableResourceString(descriptionResourceId, Resources.ResourceManager, typeof(Resources)));
        }
    }
}