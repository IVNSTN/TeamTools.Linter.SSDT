using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TeamTools.SSDT.ProjectValidator.Infrastructure;

namespace TeamTools.SSDT.ProjectValidator.Rules
{
    public abstract class BaseDataToolsXmlNodesRule : BaseDataToolsRule
    {
        protected BaseDataToolsXmlNodesRule() : base()
        {
        }

        protected string DatasourceNamespace { get; set; } = SsdtNamespace;

        protected override sealed void DoValidate(DataToolsFileInfo src)
        {
            var nodes = SelectNodes(src.FileDom, DatasourceNamespace, GetNodeSelectExpression());

            if (!nodes.Any())
            {
                DoWhenNotFound();
                return;
            }

            DoWhenFound(nodes);
        }

        protected abstract string GetNodeSelectExpression();

        protected virtual void DoWhenNotFound()
        {
            return;
        }

        protected virtual void DoWhenFound(IEnumerable<XElement> nodes)
        {
            return;
        }

        private static IEnumerable<XElement> SelectNodes(XDocument src, string namespaceName, string xpathSelector)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace(namespaceName, src.Root.GetDefaultNamespace().NamespaceName);
            return src.XPathSelectElements(xpathSelector, ns);
        }
    }
}
