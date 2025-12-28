
# {Rule violation message}

|||
|:-|:-|
| Id | **{Rule Id}**
| Mnemo | {Rule mnemo}
| Severity | {⛔, ⚠ or ℹ}  {Rule severity: Failure, Warning or Hint}
| Category | [{Rule group name}](./Group_{Rule group file name}.md) [,.. additional group and category links]
| Source code | [{Rule source file name}.cs](../../../Rules/{Rule subfolder}/{Rule source file name}.cs)

## Cause

{Description of the cause for the rule to be triggered, for example: This rule is triggered if cross-database reference not via synonyms detected.}

<p id="descr">{A short recommendations on what to do to fix the violation found. This text will be displayed as a tooltip in the VS interface. Please note that this element cannot contain nested HTML or Markdown markup elements. Example: Use synonyms for cross-database reference.}</p>

## Examples

Bad

```xml
{RDL markup example with violation}
```

Good

```xml
{RDL markup example fixed}
```

## Tips

⚠️ {One or more important points to note.}

💡 { One or more additional helpful comments }

## Links

{ Links to official SSDR, SSRT docs or related rule doc files }
