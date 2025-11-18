using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Infrastructure;
using TeamTools.SSDT.ProjectValidator.Properties;

namespace TeamTools.SSDT.ProjectValidator.Rules.SqlProject
{
    [DataType("SQL")]
    [RuleIdentity("SSDT0006", "SQL_PROJECT_STRUCT_OPTIONS")]
    public sealed class SqlProjStructureOptionsRule : BaseDataToolsRule
    {
        private static readonly Dictionary<string, string> RequiredOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
           { "DefaultFileStructure", "BySchemaType" },
           { "IncludeSchemaNameInFileName", "True" },
           { "OutputType", "Database" },
           { "DeployToDatabase", "True" },
        };

        public SqlProjStructureOptionsRule() : base()
        {
        }

        private static string MissingOptionTemplate => Strings.ViolationDetails_SqlProjStructureOptionsRule_missing_option_template;

        private static string RedundantOptionInLinesTemplate => Strings.ViolationDetails_SqlProjStructureOptionsRule_redundant_option_in_lines_template;

        private static string RedundantOptionTemplate => Strings.ViolationDetails_SqlProjStructureOptionsRule_redundant_option_template;

        protected override void DoValidate(DataToolsFileInfo fileInfo)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace(SsdtNamespace, fileInfo.FileDom.Root.GetDefaultNamespace().NamespaceName);
            var nodes = new Dictionary<string, List<XElement>>();

            foreach (var element in RequiredOptions.Keys)
            {
                nodes.Add(element, new List<XElement>());
                nodes[element].AddRange(fileInfo.FileDom.XPathSelectElements($"/{SsdtNamespace}:Project/{SsdtNamespace}:PropertyGroup/{SsdtNamespace}:{element}", ns));
            }

            foreach (var element in RequiredOptions.Keys)
            {
                if (nodes[element].Count == 0)
                {
                    HandleFileError(string.Format(MissingOptionTemplate, element));
                    continue;
                }

                if (nodes[element].Count > 1)
                {
                    string lines = string.Join(",", nodes[element]
                        .Where(n => n is IXmlLineInfo lineInfo)
                        .Select(n => ((IXmlLineInfo)n).LineNumber.ToString()));

                    string msg = string.Format(string.IsNullOrEmpty(lines) ? RedundantOptionTemplate : RedundantOptionInLinesTemplate, element, lines);

                    HandleFileError(msg);
                    continue;
                }

                var wrongNode = nodes[element][0];

                if (!wrongNode.Value.Equals(RequiredOptions[element], StringComparison.OrdinalIgnoreCase))
                {
                    string msg = string.Format("{0} {1} != {2}", wrongNode.Name.LocalName, wrongNode.Value, RequiredOptions[element]);

                    HandleNodeError(wrongNode, msg);
                }
            }
        }
    }
}
