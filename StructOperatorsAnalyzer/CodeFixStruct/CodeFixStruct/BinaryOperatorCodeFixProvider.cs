using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Rename;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeFixStruct
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BinaryOperatorCodeFixProvider)), Shared]
    public class BinaryOperatorCodeFixProvider : CodeFixProvider
    {
        private const string title = "Replace operator with function";
        
        public sealed override ImmutableArray<string> FixableDiagnosticIds {
            get { return ImmutableArray.Create(BinaryOperatorsAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
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

        private static async Task<Document> Refactor(Document document, SyntaxNode nodeToFix, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var generator = editor.Generator;

            var operationA = semanticModel.GetOperation(nodeToFix, cancellationToken);
            SyntaxNode rightSyntaxNode;
            BinaryOperatorKind operatorKind;
            if (operationA.Kind == OperationKind.BinaryOperator)
            {
                var binOp = (IBinaryOperation)operationA;
                operatorKind = binOp.OperatorKind;
                rightSyntaxNode = binOp.RightOperand.Syntax;
            }
            else return document;

            if (!GetFunctionForOperator(operatorKind, out string op))
                return document;

            BinaryExpressionSyntax bes = nodeToFix as BinaryExpressionSyntax;
            var newExpression = generator.InvocationExpression(
                generator.MemberAccessExpression(bes.Left, op),
                rightSyntaxNode);

            editor.ReplaceNode(nodeToFix, newExpression.WithAdditionalAnnotations(Formatter.Annotation));
            return editor.GetChangedDocument();
        }


        static bool GetFunctionForOperator(BinaryOperatorKind binOp, out string functionName)
        {
            if (binOp == BinaryOperatorKind.Add) { functionName = "Add"; return true; }
            else if (binOp == BinaryOperatorKind.Subtract) { functionName = "Sub"; return true; }
            else if (binOp == BinaryOperatorKind.Multiply) { functionName = "Mul"; return true; }
            else if (binOp == BinaryOperatorKind.Divide) { functionName = "Div"; return true; }
            else if (binOp == BinaryOperatorKind.Remainder) { functionName = "Mod"; return true; }
            functionName = null;
            return false;
        }
    }
}
