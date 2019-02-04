using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace CodeFixStruct
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class CompoundAssignmentAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "CodeFixStruct";
		public const string Title = "Operators with asignment not supported.";
		public const string MessageFormat = "Replace operator with asignment with operators only.";
		public const string Category = "Errors";
		public const DiagnosticSeverity Severity = DiagnosticSeverity.Error;

		internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, Severity, true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.EnableConcurrentExecution();
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(AnalyzeSyntax, 
                SyntaxKind.AddAssignmentExpression, 
                SyntaxKind.SubtractAssignmentExpression,
                SyntaxKind.MultiplyAssignmentExpression,
                SyntaxKind.DivideAssignmentExpression,
                SyntaxKind.ModuloAssignmentExpression);
		}

        const string StructStartName = "Fix";

        private void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
        {
            var assignmentNode = context.Node as AssignmentExpressionSyntax;
            string assignmentType = "";

            if (assignmentNode.IsKind(SyntaxKind.AddAssignmentExpression))
                assignmentType = "+=";
            else if (assignmentNode.IsKind(SyntaxKind.SubtractAssignmentExpression))
                assignmentType = "-=";
            else if (assignmentNode.IsKind(SyntaxKind.MultiplyAssignmentExpression))
                assignmentType = "*=";
            else if (assignmentNode.IsKind(SyntaxKind.DivideAssignmentExpression))
                assignmentType = "/=";
            else if (assignmentNode.IsKind(SyntaxKind.ModuloAssignmentExpression))
                assignmentType = "%=";
            else
                return;


            
            var semanticModel = context.SemanticModel;
            var arrayAccess = assignmentNode.Left as ElementAccessExpressionSyntax;
            ISymbol symbolForAssignment = arrayAccess != null
                ? semanticModel.GetSymbolInfo(arrayAccess.Expression).Symbol
                : semanticModel.GetSymbolInfo(assignmentNode.Left).Symbol;
            
            ITypeSymbol type;
            if (symbolForAssignment is IPropertySymbol) type = ((IPropertySymbol)symbolForAssignment).Type;
            else if (symbolForAssignment is ILocalSymbol) type = ((ILocalSymbol)symbolForAssignment).Type;
            else if (symbolForAssignment is IFieldSymbol) type = ((IFieldSymbol)symbolForAssignment).Type;
            else return;

            if (type.Name.StartsWith(StructStartName))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Rule,
                    assignmentNode.GetLocation(),
                    (LocalizableString) $"Replace the operator \"x {assignmentType} y\" with \"x = x {assignmentType.Substring(0, 1)} y\" operator"));
            }
        }
        
		private static bool IsNull(IOperation operation)
		{
			return operation.ConstantValue.HasValue && operation.ConstantValue.Value == null;
		}
	}
}
