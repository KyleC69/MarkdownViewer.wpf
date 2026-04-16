# MarkdownViewer.Wpf Sample

This document is bundled with the sample app and is intended to exercise the full visible renderer surface.

## Inline Formatting

Paragraph text can include *italic*, **bold**, ~~strikethrough~~, ~subscript~, ^superscript^, ++inserted++, ==marked==, `inline code`, a [regular link](https://github.com/KyleC69/MarkdownViewer.Wpf "Project repository"), an auto-link <https://example.com>, an email auto-link <hello@example.com>, an HTML entity &amp;, and inline HTML like <span>inline span text</span>.

This line ends with a hard break.  
The renderer should place the next sentence on a new line.

## Headings

### Heading 3

#### Heading 4

##### Heading 5

###### Heading 6

---

## Lists

- Unordered item one
- Unordered item two with **bold text**

3. Ordered item starting at three
4. Ordered item after the explicit start value

- [x] Completed task item
- [ ] Incomplete task item

## Block Quote

> A block quote is rendered inside a bordered container.
>
> It can also span multiple paragraphs.

## Code

Indented code block:

    public static int Add(int left, int right) => left + right;

Fenced code block with a language tag:

```csharp
public sealed class Sample
{
    public string Name { get; } = "MarkdownViewer.Wpf";
}
```

## Table

| Feature | Purpose |
| ------- | ------- |
| Heading renderers | Styled heading output |
| Task lists | Disabled check boxes |
| Tables | Grid-based layout |

## HTML Block

<div class="sample-html-block">
  <strong>HTML block content</strong>
  <em>is rendered through the HTML block renderer instead of being shown as raw tags.</em>
</div>

## Image

![Bundled sample image]({{SampleImageUri}} "Bundled sample image")