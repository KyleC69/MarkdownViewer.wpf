// Build Date: 2026/04/15
// Solution: Markdig.Wpf
// Project:   MarkdownViewer.Wpf
// File:         XamlRendererRegistry.cs
// Author: Kyle L. Crowder
// Build Num: 184538



using System;
using System.Collections.Generic;
using System.Linq;

using Markdig.Renderers;
using Markdig.Syntax;

using MarkdownViewer.Wpf.Abstractions;




namespace MarkdownViewer.Wpf.Rendering.Xaml
{


    /// <summary>
    ///     A composable XAML renderer registry that supports additive and replacement customization.
    /// </summary>
    public sealed class XamlRendererRegistry : IXamlRendererRegistry
    {
        private readonly List<Registration> registrations = new List<Registration>();








        void IXamlRendererRegistry.Configure(XamlRendererRegistry registry)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));

            foreach (Registration registration in registrations) registry.registrations.Add(registration);
        }








        public XamlRendererRegistry Add<TRenderer>() where TRenderer : class, IMarkdownObjectRenderer, new()
        {
            registrations.Add(new Registration(typeof(TRenderer), GetHandledNodeType(typeof(TRenderer)), static () => new TRenderer()));
            return this;
        }








        public XamlRendererRegistry Add<TRenderer>(Func<TRenderer> factory) where TRenderer : class, IMarkdownObjectRenderer
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            registrations.Add(new Registration(typeof(TRenderer), GetHandledNodeType(typeof(TRenderer)), () => factory()));
            return this;
        }








        private XamlRendererRegistry AddChecked<TNode, TRenderer>(Func<TRenderer> factory) where TNode : MarkdownObject where TRenderer : class, IMarkdownObjectRenderer
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            Type? handledNodeType = GetHandledNodeType(typeof(TRenderer));
            if (handledNodeType != typeof(TNode))
            {
                throw new InvalidOperationException($"Renderer type {typeof(TRenderer).FullName} must target markdown node type {typeof(TNode).FullName}.");
            }

            registrations.Add(new Registration(typeof(TRenderer), handledNodeType, () => factory()));
            return this;
        }








        internal void ApplyTo(XamlRenderer renderer)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            foreach (Registration registration in registrations) renderer.ObjectRenderers.Add(registration.Factory());
        }








        /// <summary>
        ///     Creates a mutable registry preloaded with the package default renderer set.
        /// </summary>
        public static XamlRendererRegistry CreateDefault()
        {
            return DefaultXamlRendererRegistry.CreateMutable();
        }








        private static Type? GetHandledNodeType(Type rendererType)
        {
            Type? handledByInterface = rendererType.GetInterfaces().FirstOrDefault(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IXamlNodeRenderer<>))?.GetGenericArguments()[0];

            if (handledByInterface != null)
            {
                return handledByInterface;
            }

            for (Type? current = rendererType; current != null; current = current.BaseType)
            {
                if (!current.IsGenericType)
                {
                    continue;
                }

                Type genericType = current.GetGenericTypeDefinition();
                if (genericType == typeof(XamlObjectRenderer<>))
                {
                    return current.GetGenericArguments()[0];
                }
            }

            return null;
        }








        public XamlRendererRegistry Remove<TRenderer>() where TRenderer : class, IMarkdownObjectRenderer
        {
            registrations.RemoveAll(static registration => typeof(TRenderer).IsAssignableFrom(registration.RendererType));
            return this;
        }








        public XamlRendererRegistry RemoveFor<TNode>() where TNode : MarkdownObject
        {
            registrations.RemoveAll(static registration => registration.HandledNodeType == typeof(TNode));
            return this;
        }








        public XamlRendererRegistry Replace<TRenderer, TReplacement>() where TRenderer : class, IMarkdownObjectRenderer where TReplacement : class, IMarkdownObjectRenderer, new()
        {
            return Remove<TRenderer>().Add<TReplacement>();
        }








        public XamlRendererRegistry Replace<TRenderer, TReplacement>(Func<TReplacement> factory) where TRenderer : class, IMarkdownObjectRenderer where TReplacement : class, IMarkdownObjectRenderer
        {
            return Remove<TRenderer>().Add(factory);
        }








        public XamlRendererRegistry ReplaceFor<TNode, TReplacement>() where TNode : MarkdownObject where TReplacement : class, IMarkdownObjectRenderer, new()
        {
            return RemoveFor<TNode>().AddChecked<TNode, TReplacement>(static () => new TReplacement());
        }








        public XamlRendererRegistry ReplaceFor<TNode, TReplacement>(Func<TReplacement> factory) where TNode : MarkdownObject where TReplacement : class, IMarkdownObjectRenderer
        {
            return RemoveFor<TNode>().AddChecked<TNode, TReplacement>(factory);
        }








        private readonly struct Registration
        {
            public Registration(Type rendererType, Type? handledNodeType, Func<IMarkdownObjectRenderer> factory)
            {
                RendererType = rendererType ?? throw new ArgumentNullException(nameof(rendererType));
                HandledNodeType = handledNodeType;
                Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            }








            public Type RendererType { get; }

            public Type? HandledNodeType { get; }

            public Func<IMarkdownObjectRenderer> Factory { get; }
        }
    }


}