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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeFixStruct
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CompoundAssignmentCodeFixProvider)), Shared]
    public class CompoundAssignmentCodeFixProvider : CodeFixProvider
    {
        private const string title = "Replace compount assignment with simple assignment";
        
        public sealed override ImmutableArray<string> FixableDiagnosticIds {
            get { return ImmutableArray.Create(CompoundAssignmentAnalyzer.DiagnosticId); }
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

            var title = "Use simple operator";
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
            if (operationA.Kind == OperationKind.CompoundAssignment)
            {
                var binOp = (ICompoundAssignmentOperation)operationA;
                var syntaxBase = binOp.Syntax;
                var pepe = (IAssignmentOperation)operationA;
                var target = binOp.Target;
                var value = binOp.Value;
                var str = "temp";
                //var asd = syntaxBase.
                AssignmentExpressionSyntax operatorSyntax = binOp.Syntax as AssignmentExpressionSyntax;

                if (!GetFunctionForOperator(operatorSyntax, out OperatorKind op))
                    return document;

                SyntaxNode rightSide;

                if (operatorSyntax.IsKind(SyntaxKind.AddAssignmentExpression)) { rightSide = generator.AddExpression(operatorSyntax.Left, operatorSyntax.Right); }
                else if (operatorSyntax.IsKind(SyntaxKind.SubtractAssignmentExpression)) { rightSide = generator.SubtractExpression(operatorSyntax.Left, operatorSyntax.Right); }
                else if (operatorSyntax.IsKind(SyntaxKind.MultiplyAssignmentExpression)) { rightSide = generator.MultiplyExpression(operatorSyntax.Left, operatorSyntax.Right); }
                else if (operatorSyntax.IsKind(SyntaxKind.DivideAssignmentExpression)) { rightSide = generator.DivideExpression(operatorSyntax.Left, operatorSyntax.Right); }
                else if (operatorSyntax.IsKind(SyntaxKind.ModuloAssignmentExpression)) { rightSide = generator.ModuloExpression(operatorSyntax.Left, operatorSyntax.Right); }
                else return document;

                var newExpression = 
                    generator.AssignmentStatement(
                    operatorSyntax.Left,
                    rightSide);

                editor.ReplaceNode(nodeToFix, newExpression.WithAdditionalAnnotations(Formatter.Annotation));
                return editor.GetChangedDocument();
            }
            else return document;
        }


        static bool GetFunctionForOperator(AssignmentExpressionSyntax assignmentNode, out OperatorKind operatorKind)
        {
            if (assignmentNode.IsKind(SyntaxKind.AddAssignmentExpression)) { operatorKind = OperatorKind.Addition; return true; }
            else if (assignmentNode.IsKind(SyntaxKind.SubtractAssignmentExpression)) { operatorKind = OperatorKind.Subtraction; return true; }
            else if (assignmentNode.IsKind(SyntaxKind.MultiplyAssignmentExpression)) { operatorKind = OperatorKind.Multiply; return true; }
            else if (assignmentNode.IsKind(SyntaxKind.DivideAssignmentExpression)) { operatorKind = OperatorKind.Division; return true; }
            else if (assignmentNode.IsKind(SyntaxKind.ModuloAssignmentExpression)) { operatorKind = OperatorKind.Modulus; return true; }
            operatorKind = default(OperatorKind);
            return false;
        }
    }
}
