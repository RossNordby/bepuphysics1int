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
	public class BinaryOperatorsAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "CodeFixStruct";
		public const string Title = "Operators not supported.";
		public const string MessageFormat = "Replace operator with methods.";
		public const string Category = "Errors";
		public const DiagnosticSeverity Severity = DiagnosticSeverity.Error;

		internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, Severity, true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.EnableConcurrentExecution();
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

			context.RegisterOperationAction(AnalyzeInvocationBinary, OperationKind.BinaryOperator); // x + y
		}

        const string StructStartName = "Fix";
        
		private void AnalyzeInvocationBinary(OperationAnalysisContext context)
		{
			var operation = (IBinaryOperation)context.Operation;
			if (
				operation.OperatorKind == BinaryOperatorKind.Add ||
				operation.OperatorKind == BinaryOperatorKind.Subtract ||
				operation.OperatorKind == BinaryOperatorKind.Multiply ||
                operation.OperatorKind == BinaryOperatorKind.Divide ||
                operation.OperatorKind == BinaryOperatorKind.Remainder
                )
			{
				bool leftOk = 
                    operation.LeftOperand != null && 
                    operation.LeftOperand.Type != null &&
                    operation.LeftOperand.Type.Name.StartsWith(StructStartName);
				bool rightOk =
                    operation.RightOperand != null &&
                    operation.RightOperand.Type != null &&
                    operation.RightOperand.Type.Name.StartsWith(StructStartName);
				if (leftOk && rightOk)
				{
					context.ReportDiagnostic(Diagnostic.Create(Rule, operation.Syntax.GetLocation(), $"{operation.OperatorKind} operator"));
				}
			}
		}
        
		private static bool IsNull(IOperation operation)
		{
			return operation.ConstantValue.HasValue && operation.ConstantValue.Value == null;
		}
	}
}
