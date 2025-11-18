using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Rules.Reports
{
    [DataType("ReportDesign")]
    [RuleIdentity("RDL0001", "EMBEDDED_IMAGES")]
    public sealed class EmbeddedImagesRule : BaseDataToolsXmlNodesRule
    {
        private const string ReportsNamespace = "rdef";
        private static readonly string NodeSelector = $"//{ReportsNamespace}:EmbeddedImages";

        public EmbeddedImagesRule() : base()
        {
            DatasourceNamespace = ReportsNamespace;
        }

        protected override void DoWhenFound(IEnumerable<XElement> nodes)
        {
            HandleNodeError(nodes.First(), "EmbeddedImages");
        }

        protected override string GetNodeSelectExpression()
        {
            return NodeSelector;
        }
    }
}
