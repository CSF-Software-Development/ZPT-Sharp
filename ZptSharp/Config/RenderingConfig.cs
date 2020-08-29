﻿using System;
using System.Text;
using ZptSharp.Dom;

namespace ZptSharp.Config
{
    /// <summary>
    /// Configuration for a single instance of <see cref="IRendersZptDocuments"/>.  One configuration
    /// object is used for all operations of a single renderer object.
    /// </summary>
    public partial class RenderingConfig
    {
        /// <summary>
        /// An object which provides service-resolution/dependency injection for ZPT Sharp types.
        /// If this is unset or <see langword="null"/> then a default resolution service will be used.
        /// </summary>
        /// <value>The service provider.</value>
        public virtual IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Gets or sets the encoding which will be used to read &amp; write all documents.
        /// If this is unset then documents will be read &amp; written as UTF-8.
        /// </summary>
        /// <value>The document encoding.</value>
        public virtual Encoding DocumentEncoding { get; private set; }

        /// <summary>
        /// <para>
        /// Gets or sets the document provider to be used for reading/writing documents.
        /// </para>
        /// <para>
        /// This property is not used if the <see cref="ServiceProvider"/> has been set to anything
        /// other than the default (<see langword="null"/>) value.  If a custom service provider is
        /// used then the document provider must be resolvable from that service provider.
        /// </para>
        /// </summary>
        /// <value>The document provider.</value>
        public virtual IReadsAndWritesDocument DocumentProvider { get; private set; }

        /// <summary>
        /// <para>
        /// The constructor for <see cref="RenderingConfig"/> is intentionally <see langword="protected"/>.
        /// Instances of this class must be created via an instance of <see cref="Builder"/>.
        /// </para>
        /// <example>
        /// <para>
        /// Here is an example showing how to create a new config object.
        /// </para>
        /// <code>
        /// var builder = new RenderingConfig.Builder();
        /// /* Use the builder to set config values */
        /// var config = builder.GetConfig();
        /// </code>
        /// </example>
        /// </summary>
        protected RenderingConfig() {}
    }
}
