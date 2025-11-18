using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TeamTools.SSDT.ProjectValidator.Infrastructure;

namespace TeamTools.SSDT.ProjectValidator.Rules.SqlProject
{
    public abstract class BaseDacpacRule : BaseDataToolsRule
    {
        protected const string DacpacExtension = ".dacpac";
        private static readonly string DacpacSelectExpression = $"/{SsdtNamespace}:Project/{SsdtNamespace}:ItemGroup/{SsdtNamespace}:ArtifactReference[substring(@Include, string-length(@Include)-{DacpacExtension.Length}+1) = '{DacpacExtension}']";

        protected BaseDacpacRule() : base()
        {
        }

        protected string NodeSelectExpression { get => GetNodeSelectExpression(); }

        protected virtual string GetNodeSelectExpression()
        {
            return DacpacSelectExpression;
        }

        protected override void DoValidate(DataToolsFileInfo fileInfo)
        {
            var dacpacNodes = ExtractDacpacReferences(fileInfo.FileDom);
            foreach (var dacpac in dacpacNodes)
            {
                ProcessDacpacReference(dacpac);
            }
        }

        protected virtual bool IsValidDescription(XElement dacpac)
        {
            // default behavior is to throw error on each item
            return false;
        }

        protected void ProcessDacpacReference(XElement dacpac)
        {
            if (IsValidDescription(dacpac))
            {
                return;
            }

            string dacpacName = Path.GetFileName(dacpac.Attribute("Include").Value);

            HandleNodeError(dacpac, dacpacName);
        }

        protected IEnumerable<XElement> ExtractDacpacReferences(XDocument doc)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace(SsdtNamespace, doc.Root.GetDefaultNamespace().NamespaceName);
            var nodes = doc.XPathSelectElements(NodeSelectExpression, ns);

            return nodes;
        }
    }
}
