using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Rules.SqlProject
{
    [DataType("SQL")]
    [RuleIdentity("SSDT0004", "DACPAC_HINT_PATH")]
    public sealed class DacpacHintPathEqualsIncludePathRule : BaseDacpacRule
    {
        public DacpacHintPathEqualsIncludePathRule() : base()
        {
        }

        protected override bool IsValidDescription(XElement dacpac)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace(SsdtNamespace, dacpac.Document.Root.GetDefaultNamespace().NamespaceName);

            var hintPathElement = dacpac.XPathSelectElement($"./{SsdtNamespace}:HintPath", ns);
            if (hintPathElement is null)
            {
                // missing option is s
                return true;
            }

            var dacpacPath = dacpac.Attribute(XName.Get("Include", ns.DefaultNamespace)).Value;

            return string.Equals(hintPathElement.Value, dacpacPath);
        }
    }
}
