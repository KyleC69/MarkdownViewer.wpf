// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         EmphasisInlineRenderer.cs
// Author: Kyle L. Crowder
// Build Num: 184534



using System;

using Markdig.Syntax.Inlines;




namespace MarkdownViewer.Wpf.Rendering.Xaml.Defaults.Inlines
{


    /// <summary>
    ///     A XAML renderer for an <see cref="EmphasisInline" />.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class DefaultEmphasisInlineRenderer : DefaultXamlInlineRenderer<EmphasisInline>
    {
        /// <summary>
        ///     Delegates to get the tag associated to an <see cref="EmphasisInline" /> object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The XAML tag associated to this <see cref="EmphasisInline" /> object</returns>
        public delegate string GetTagDelegate(EmphasisInline obj);








        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultEmphasisInlineRenderer" /> class.
        /// </summary>
        public DefaultEmphasisInlineRenderer()
        {
            GetTag = GetDefaultTag;
        }








        protected override void WriteObject(Abstractions.IXamlRenderContext renderer, EmphasisInline obj)
        {
            var tag = GetTag(obj);
            renderer.Write("<").Write(tag);
            switch (obj.DelimiterChar)
            {
                case '*':
                case '_':
                    break;
                case '~':
                    renderer.Write(obj.DelimiterCount == 2 ? " Style=\"{DynamicResource {x:Static markdig:Styles.StrikeThroughStyleKey}}\"" : " Style=\"{DynamicResource {x:Static markdig:Styles.SubscriptStyleKey}}\"");
                    break;
                case '^':
                    renderer.Write(" Style=\"{DynamicResource {x:Static markdig:Styles.SuperscriptStyleKey}}\"");
                    break;
                case '+':
                    renderer.Write(" Style=\"{DynamicResource {x:Static markdig:Styles.InsertedStyleKey}}\"");
                    break;
                case '=':
                    renderer.Write(" Style=\"{DynamicResource {x:Static markdig:Styles.MarkedStyleKey}}\"");
                    break;
            }

            renderer.Write(">");
            renderer.WriteChildren(obj);
            renderer.Write("</").Write(tag).Write(">");
        }








        /// <summary>
        ///     Gets or sets the GetTag delegate.
        /// </summary>
        public GetTagDelegate GetTag { get; set; }








        /// <summary>
        ///     Gets the default XAML tag for ** and __ emphasis.
        /// </summary>
        /// <param name="emphasis">The emphasis inline object.</param>
        /// <returns></returns>
        public static string GetDefaultTag(EmphasisInline emphasis)
        {
            if (emphasis == null) throw new ArgumentNullException(nameof(emphasis));

            if (emphasis.DelimiterChar == '*' || emphasis.DelimiterChar == '_')
            {
                return emphasis.DelimiterCount == 2 ? "Bold" : "Italic";
            }

            return "Span";
        }
    }


}