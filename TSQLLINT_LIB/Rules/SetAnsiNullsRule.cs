using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLLINT_LIB.Rules
{
    public class SetAnsiNullsRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "set-ansi"; } }
        public string RULE_TEXT { get { return "SET ANSI_NULLS ON at top of file"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        private bool SetNoCountFound;
        private bool ErrorLogged;

        public SetAnsiNullsRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(SetOnOffStatement node)
        {
            var typedNode = node as PredicateSetStatement;
            if (typedNode.Options == SetOptions.AnsiNulls)
            {
                SetNoCountFound = true;
            }
        }

        public override void Visit(TSqlStatement node)
        {
            var nodeType = node.GetType();

            if (nodeType == typeof(SetTransactionIsolationLevelStatement))
            {
                return;
            }

            if (nodeType == typeof(PredicateSetStatement))
            {
                var typedNode = node as PredicateSetStatement;
                if (typedNode.Options == SetOptions.AnsiNulls || 
                    typedNode.Options == SetOptions.QuotedIdentifier)
                {
                    return;
                }
            }

            if (!SetNoCountFound && !ErrorLogged)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
                ErrorLogged = true;
            }
        }
    }
}