using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Rules.Reports
{
    [DataType("ReportDesign")]
    [RuleIdentity("RDL0005", "DATASET_NAME_ALLOWED_SYMBOLS")]
    public sealed class DatasetNameAlphanumericRule : BaseDataToolsXmlNodesRule
    {
        private const string ReportsNamespace = "rdef";
        private static readonly string NodeSelector = $"//{ReportsNamespace}:DataSets/{ReportsNamespace}:DataSet";
        private static readonly Regex DisallowedSymbols = new Regex(@"[^\w.-]", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        public DatasetNameAlphanumericRule() : base()
        {
            DatasourceNamespace = ReportsNamespace;
        }

        protected override void DoWhenFound(IEnumerable<XElement> nodes)
        {
            foreach (var ds in nodes)
            {
                string datasetName = ds.Attribute("Name")?.Value;
                if (!string.IsNullOrEmpty(datasetName) && DisallowedSymbols.IsMatch(datasetName))
                {
                    HandleNodeError(ds, datasetName);
                }
            }
        }

        protected override string GetNodeSelectExpression()
        {
            return NodeSelector;
        }
    }
}
