﻿using System;
using CSF.Zpt.Tales;

namespace CSF.Zpt.Rendering
{
  /// <summary>
  /// Encapsulates the available options for rendering a <see cref="ZptDocument"/>.
  /// </summary>
  public class RenderingOptions
  {
    #region constants

    private static readonly IContextVisitor[] DefaultVisitors = new IContextVisitor[] {
      new CSF.Zpt.Metal.MetalVisitor(),
      new CSF.Zpt.Metal.SourceAnnotationVisitor(),
      new CSF.Zpt.Tal.TalVisitor(),
    };

    #endregion

    #region fields

    private static RenderingOptions _defaultOptions;

    #endregion

    #region properties

    /// <summary>
    /// Gets the factory implementation with which to create <see cref="RenderingContext"/> instances.
    /// </summary>
    /// <value>The rendering context factory.</value>
    protected IRenderingContextFactory ContextFactory
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether source file annotations should be added to the rendered output.
    /// </summary>
    /// <value><c>true</c> if source file annotations are to be added; otherwise, <c>false</c>.</value>
    public bool AddSourceFileAnnotation
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets the context visitors to be used when processing ZPT documents.
    /// </summary>
    /// <value>The context visitors.</value>
    public IContextVisitor[] ContextVisitors
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets the initial state of the model.
    /// </summary>
    /// <value>The initial state of the model.</value>
    public InitialModelState InitialModelState
    {
      get;
      private set;
    }

    #endregion

    #region methods

    /// <summary>
    /// Creates a new root <see cref="RenderingContext"/> instance.
    /// </summary>
    /// <returns>The root rendering context.</returns>
    public RenderingContext CreateRootContext(ZptElement element)
    {
      if(element == null)
      {
        throw new ArgumentNullException(nameof(element));
      }

      return this.ContextFactory.Create(element, this);
    }

    #endregion

    #region constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CSF.Zpt.Rendering.RenderingOptions"/> class.
    /// </summary>
    /// <param name="addSourceFileAnnotation">Indicates whether or not source file annotation is to be added.</param>
    /// <param name="elementVisitors">The element visitors to use.</param>
    /// <param name="contextFactory">The rendering context factory.</param>
    /// <param name="initialState">The initial state of the ZPT models.</param>
    public RenderingOptions(bool addSourceFileAnnotation = false,
                            IContextVisitor[] elementVisitors = null,
                            IRenderingContextFactory contextFactory = null,
                            InitialModelState initialState = null)
    {
      this.AddSourceFileAnnotation = addSourceFileAnnotation;
      this.ContextVisitors = elementVisitors?? DefaultVisitors;
      this.ContextFactory = contextFactory?? new TalesRenderingContextFactory();
      this.InitialModelState = initialState?? new InitialModelState();
    }

    /// <summary>
    /// Initializes the <see cref="CSF.Zpt.Rendering.RenderingOptions"/> class.
    /// </summary>
    static RenderingOptions()
    {
      _defaultOptions = new RenderingOptions();
    }

    #endregion

    #region static properties

    /// <summary>
    /// Gets a set of <see cref="RenderingOptions"/> representing the defaults.
    /// </summary>
    /// <value>The default rendering options.</value>
    public static RenderingOptions Default
    {
      get {
        return _defaultOptions;
      }
    }

    #endregion
  }
}

