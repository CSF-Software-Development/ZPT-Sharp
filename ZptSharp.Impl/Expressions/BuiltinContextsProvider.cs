﻿using System;
using System.Collections.Generic;
using System.Linq;
using ZptSharp.Config;
using ZptSharp.Metal;

namespace ZptSharp.Expressions
{
    public class BuiltinContextsProvider : IGetsNamedTalesValue
    {
        readonly ExpressionContext context;
        readonly RenderingConfig config;

        /// <summary>
        /// An identifier for the keyword-options presented to the rendering process.
        /// </summary>
        public static readonly string Options = "options";

        /// <summary>
        /// An identifier for the collection of named repetition-information objects available in the
        /// expression context.
        /// </summary>
        public static readonly string Repeat = "repeat";

        /// <summary>
        /// An identifier/alias for the model object contained within the expression context.
        /// </summary>
        public static readonly string Here = "here";

        /// <summary>
        /// An identifier/alias for a non-object.  This translates to <c>null</c> in C# applications.
        /// </summary>
        public static readonly string Nothing = "nothing";

        /// <summary>
        /// An identifier/alias for an object which indicates that the current action should be cancelled.
        /// </summary>
        public static readonly string Default = "default";

        /// <summary>
        /// An identifier/alias for getting the attributes from the current <see cref="Dom.IElement"/>.
        /// </summary>
        public static readonly string Attributes = "attrs";

        /// <summary>
        /// An identifier/alias for getting the <see cref="Dom.IDocument"/> 
        /// </summary>
        public static readonly string Template = "template";

        /// <summary>
        /// An identifier/alias for getting the container of the current <see cref="Dom.IDocument"/>.
        /// </summary>
        public static readonly string Container = "container";

        /// <summary>
        /// Attempts to get a value for a named reference, relative to the current instance.
        /// </summary>
        /// <returns>A boolean indicating whether a value was successfully retrieved or not.</returns>
        /// <param name="name">The name of the value to retrieve.</param>
        /// <param name="value">Exposes the retrieved value if this method returns success.</param>
        public bool TryGetValue(string name, out object value)
        {
            if(BuiltinContextsAndValues.TryGetValue(name, out var valueFunc))
            {
                value = valueFunc();
                return true;
            }

            value = null;
            return false;
        }

        Dictionary<string,Func<object>> BuiltinContextsAndValues
        {
            get
            {
                return new Dictionary<string, Func<object>>
                {
                    { Here, () => context.Model },
                    { Repeat, () => context.Repetitions },
                    { Options, () => config.KeywordOptions},
                    { Nothing, () => null },
                    { Default, () => AbortZptActionToken.Instance },
                    { Attributes, GetAttributesDictionary },
                    { Template, GetMetalDocumentAdapter },
                    { Container, GetTemplateContainer },
                };
            }
        }

        IDictionary<string,Dom.IAttribute> GetAttributesDictionary()
        {
            return context.CurrentElement.Attributes.ToDictionary(k => k.Name, v => v);
        }

        MetalDocumentAdapter GetMetalDocumentAdapter()
        {
            return new MetalDocumentAdapter(context.TemplateDocument);
        }

        object GetTemplateContainer()
        {
            return (context.TemplateDocument.SourceInfo is Rendering.IHasContainer containerProvider) ? containerProvider.GetContainer() : null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuiltinContextsProvider"/> class.
        /// </summary>
        /// <param name="context">The rendering context.</param>
        /// <param name="config">The configuration.</param>
        public BuiltinContextsProvider(ExpressionContext context, RenderingConfig config)
        {
            this.context = context ?? throw new System.ArgumentNullException(nameof(context));
            this.config = config ?? throw new System.ArgumentNullException(nameof(config));
        }
    }
}
