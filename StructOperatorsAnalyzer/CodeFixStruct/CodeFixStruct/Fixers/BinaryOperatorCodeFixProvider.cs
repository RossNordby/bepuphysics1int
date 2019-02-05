using AsyncUsageAnalyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CodeFixStruct {
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BinaryOperatorCodeFixProvider)), Shared]
	public class BinaryOperatorCodeFixProvider : CodeFixProvider {
		private const string title = "Replace operator with function";

		public sealed override ImmutableArray<string> FixableDiagnosticIds {
			get { return ImmutableArray.Create(BinaryOperatorsAnalyzer.DiagnosticId); }
		}

		public sealed override FixAllProvider GetFixAllProvider() {
			// See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
			//return WellKnownFixAllProviders.BatchFixer; // Breaks parenthesis
			return CustomBatchFixAllProvider.Instance;
		}

		public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			var nodeToFix = root.FindNode(context.Span, getInnermostNodeForTie: true);
			if (nodeToFix == null)
				return;

			var title = "Use function";
			var codeAction = CodeAction.Create(
				title,
				ct => Refactor(context.Document, nodeToFix, ct),
				equivalenceKey: title);

			context.RegisterCodeFix(codeAction, context.Diagnostics);
		}

		private static async Task<Document> Refactor(Document document, SyntaxNode nodeToFix, CancellationToken cancellationToken) {
			var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

			var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
			var generator = editor.Generator;

			var operationA = semanticModel.GetOperation(nodeToFix, cancellationToken);
			if (operationA.Kind == OperationKind.BinaryOperator) {
				var binOp = (IBinaryOperation)operationA;

				string op;
				if (binOp.OperatorKind == BinaryOperatorKind.Add) { op = "Add"; }
				else if (binOp.OperatorKind == BinaryOperatorKind.Subtract) { op = "Sub"; }
				else if (binOp.OperatorKind == BinaryOperatorKind.Multiply) { op = "Mul"; }
				else if (binOp.OperatorKind == BinaryOperatorKind.Divide) { op = "Div"; }
				else if (binOp.OperatorKind == BinaryOperatorKind.Remainder) { op = "Mod"; }
				else return document;

				BinaryExpressionSyntax bes = nodeToFix as BinaryExpressionSyntax;
				var replacement = generator.InvocationExpression(
					generator.MemberAccessExpression(bes.Left.WithoutTrivia(), op),
					bes.Right.WithoutTrivia());

				editor.ReplaceNode(nodeToFix, replacement);
				Debug.WriteLine(nodeToFix.ToFullString() + " ===> " + replacement.ToFullString());
				Console.WriteLine(nodeToFix.ToFullString() + " ===> " + replacement.ToFullString());
				return editor.GetChangedDocument();
			}

			return document;
		}
	}
}
