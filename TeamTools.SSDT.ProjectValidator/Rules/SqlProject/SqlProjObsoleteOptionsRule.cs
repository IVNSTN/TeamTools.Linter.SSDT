using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Infrastructure;

namespace TeamTools.SSDT.ProjectValidator.Rules.SqlProject
{
    [DataType("SQL")]
    [RuleIdentity("SSDT0007", "SQL_PROJECT_OBSOLETE_OPTIONS")]
    public sealed class SqlProjObsoleteOptionsRule : BaseDataToolsRule
    {
        private static readonly List<string> ObsoleteElements = new List<string>
        {
            $"//{SsdtNamespace}:SccProjectName",
            $"//{SsdtNamespace}:SccProvider",
            $"//{SsdtNamespace}:SccAuxPath",
            $"//{SsdtNamespace}:SccLocalPath",
        };

        public SqlProjObsoleteOptionsRule() : base()
        {
        }

        protected override void DoValidate(DataToolsFileInfo fileInfo)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace(SsdtNamespace, fileInfo.FileDom.Root.GetDefaultNamespace().NamespaceName);

            int n = ObsoleteElements.Count;
            for (int i = 0; i < n; i++)
            {
                // TODO : try to scan once
                foreach (var badElement in fileInfo.FileDom.XPathSelectElements(ObsoleteElements[i], ns))
                {
                    HandleNodeError(badElement, badElement.Name.LocalName);
                }
            }
        }
    }
}
