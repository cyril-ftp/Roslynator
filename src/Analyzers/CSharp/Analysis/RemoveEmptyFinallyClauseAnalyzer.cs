﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveEmptyFinallyClauseAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveEmptyFinallyClause); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeFinallyClause, SyntaxKind.FinallyClause);
        }

        private static void AnalyzeFinallyClause(SyntaxNodeAnalysisContext context)
        {
            var finallyClause = (FinallyClauseSyntax)context.Node;

            if (!(finallyClause.Parent is TryStatementSyntax tryStatement))
                return;

            if (!tryStatement.Catches.Any())
                return;

            BlockSyntax block = finallyClause.Block;

            if (block?.Statements.Any() != false)
                return;

            if (!finallyClause.FinallyKeyword.TrailingTrivia.IsEmptyOrWhitespace())
                return;

            if (!block.OpenBraceToken.LeadingTrivia.IsEmptyOrWhitespace())
                return;

            if (!block.OpenBraceToken.TrailingTrivia.IsEmptyOrWhitespace())
                return;

            if (!block.CloseBraceToken.LeadingTrivia.IsEmptyOrWhitespace())
                return;

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.RemoveEmptyFinallyClause, finallyClause);
        }
    }
}
