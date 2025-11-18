using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Infrastructure;
using TeamTools.SSDT.ProjectValidator.Interfaces;

namespace TeamTools.SSDT.ProjectValidator.Rules
{
    // TODO : very similar to StructuredFiles.BaseFileValidator
    public abstract class BaseDataToolsRule : IDataToolsRule, ILinterRule
    {
        protected const string SsdtNamespace = "ssdt"; // TODO : Extract to shared SsdtDomainInfo static class?

        protected BaseDataToolsRule()
        {
        }

        public event EventHandler<RuleViolationEventDto> ViolationCallback;

        public void Validate(DataToolsFileInfo fileInfo)
        {
            Debug.Assert(fileInfo != null, "missing fileInfo");
            Debug.Assert(fileInfo.FileDom != null, "FileDom not initialized");
            Debug.Assert(!string.IsNullOrEmpty(fileInfo.FilePath), "FilePath not initialized");

            try
            {
                DoValidate(fileInfo);
            }
            catch (AggregateException e)
            {
                foreach (var err in e.InnerExceptions)
                {
                    HandleRuleFailure(err.Message);
                }
            }
            catch (Exception e)
            {
                HandleRuleFailure(e.Message);
            }
        }

        public string GetSupportedDataType()
        {
            return this.GetType().GetAttributeValue((DataTypeAttribute attr) => attr.DataTypeName);
        }

        public void HandleLineError(int line, int col, string details = default)
        {
            ViolationCallback?.Invoke(this, new RuleViolationEventDto
            {
                ErrorDetails = details,
                Line = line,
                Column = col,
            });
        }

        public void HandleFileError(string details = default)
        {
            HandleLineError(0, 1, details);
        }

        public void HandleNodeError(XElement node, string details = default)
        {
            if (node is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
            {
                HandleLineError(lineInfo.LineNumber, lineInfo.LinePosition, details);
            }
            else
            {
                HandleFileError(details);
            }
        }

        public void Subscribe(ViolationCallbackEvent callback)
        {
            if (callback != null)
            {
                ViolationCallback += (obj, dto) => callback.Invoke(obj, dto);
            }
        }

        protected abstract void DoValidate(DataToolsFileInfo fileInfo);

        private void HandleRuleFailure(string failure) => HandleFileError("Failed: " + failure);
    }
}
