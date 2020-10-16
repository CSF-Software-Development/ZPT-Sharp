﻿using System;
using System.IO;
using ZptSharp.Config;

namespace ZptSharp.Rendering
{
    /// <summary>
    /// Represents a request to render a single ZPT document.
    /// </summary>
    public class RenderZptDocumentRequest
    {
        /// <summary>
        /// Gets the model - arbitrary data passed by the caller
        /// representing the object which is being rendered to the
        /// ZPT document.
        /// </summary>
        /// <value>The model.</value>
        public object Model { get; }

        /// <summary>
        /// Gets a stream containing the document to use for rendering.
        /// </summary>
        /// <value>The document stream.</value>
        public Stream DocumentStream { get; }

        /// <summary>
        /// Gets a configuration object to be used for the rendering process.
        /// </summary>
        /// <value>The configuration.</value>
        public RenderingConfig Config { get; }

        /// <summary>
        /// An action which is used to build &amp; add values to the root ZPT context.
        /// </summary>
        /// <value>The context builder.</value>
        public Action<IConfiguresRootContext> ContextBuilder { get; }

        /// <summary>
        /// Gets information about the source of the <see cref="DocumentStream"/>, for example its original file path.
        /// </summary>
        /// <value>The source info.</value>
        public IDocumentSourceInfo SourceInfo { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderZptDocumentRequest"/> class.
        /// </summary>
        /// <param name="document">The document stream.</param>
        /// <param name="model">The model.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="contextBuilder">The context builder action.</param>
        /// <param name="sourceInfo">Information which identifies the source of the <paramref name="document"/>.</param>
        public RenderZptDocumentRequest(Stream document,
                                        object model,
                                        RenderingConfig config,
                                        Action<IConfiguresRootContext> contextBuilder = null,
                                        IDocumentSourceInfo sourceInfo = null)
        {
            DocumentStream = document ?? throw new System.ArgumentNullException(nameof(document));
            Config = config ?? throw new System.ArgumentNullException(nameof(config));
            Model = model;
            ContextBuilder = contextBuilder ?? (c => { });
            SourceInfo = sourceInfo ?? new UnknownSourceInfo();
        }
    }
}
