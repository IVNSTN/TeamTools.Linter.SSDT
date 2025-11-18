
# Docs on SSDT linter rules

## Rule groups

|||
|:-|:-|
| **[SSDT*](./Group_Project.md)** | Project
| **[RDL*](./Group_Report.md)** | Report
| **[RDS*](./Group_Datasource.md)** | Datasource

---

## How to contribute

You can improve rule docs by editing markdown files. Help in text translation is welcome.
All changes should be reviewed in PR.

For a rule documentation file use [this template](./_rule_template.md) and

- Keep the docs file markdown as simple as possible
- If have no Tips or Links or Exmaples - don't add those block to the docs file, they can be added later
- All text within curly braces should be replaced with the actual documentation text (with no braces)
- Don't put HTML or markdown elements into `<p id="descr">` element text - this text will be displayed
  in rule violation tooltip window in Visual Studio, unfortunately it supports plain text only
- A single blank like before the very first header is required for valid rendering in Bitbucket
- When updating main rule violation message, corresponding line in [ViolationMessages](../../../Resources/ViolationMessages.json) and in related docs Group or Category index file should be updated as well
- For better understanding of how rule docs file should look like - see existing doc files.
