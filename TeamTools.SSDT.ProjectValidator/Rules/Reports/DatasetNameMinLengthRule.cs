using System.Collections.Generic;
using System.Xml.Linq;
using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Rules.Reports
{
    [DataType("ReportDesign")]
    [RuleIdentity("RDL0004", "DATASET_NAME_LENGTH")]
    public sealed class DatasetNameMinLengthRule : BaseDataToolsXmlNodesRule
    {
        private const string ReportsNamespace = "rdef";
        private const int MinLength = 2;
        private static readonly string NodeSelector = $"//{ReportsNamespace}:DataSets/{ReportsNamespace}:DataSet";

        public DatasetNameMinLengthRule() : base()
        {
            DatasourceNamespace = ReportsNamespace;
        }

        protected override void DoWhenFound(IEnumerable<XElement> nodes)
        {
            foreach (var ds in nodes)
            {
                string datasetName = ds.Attribute("Name")?.Value.Trim();
                if (string.IsNullOrEmpty(datasetName) || datasetName.Length < MinLength)
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
