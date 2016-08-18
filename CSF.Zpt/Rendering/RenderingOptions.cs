﻿using System;
using System.Text;
using CSF.Zpt.Tales;

namespace CSF.Zpt.Rendering
{
  /// <summary>
  /// Encapsulates the available options for rendering a <see cref="ZptDocument"/>.
  /// </summary>
  public class RenderingOptions : IRenderingOptions
  {
    #region constants

    private static readonly IContextVisitor[] DefaultVisitors = new IContextVisitor[] {
      new CSF.Zpt.Metal.MetalVisitor(),
      new CSF.Zpt.Tal.TalVisitor(),
      new CSF.Zpt.Metal.MetalTidyUpVisitor(),
      new CSF.Zpt.Tal.TalTidyUpVisitor(),
    };

    #endregion

    #region properties

    /// <summary>
    /// Gets the factory implementation with which to create <see cref="RenderingContext"/> instances.
    /// </summary>
    /// <value>The rendering context factory.</value>
    public virtual IRenderingContextFactory ContextFactory
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether source file annotations should be added to the rendered output.
    /// </summary>
    /// <value><c>true</c> if source file annotations are to be added; otherwise, <c>false</c>.</value>
    public virtual bool AddSourceFileAnnotation
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets the context visitors to be used when processing ZPT documents.
    /// </summary>
    /// <value>The context visitors.</value>
    public virtual IContextVisitor[] ContextVisitors
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets the encoding (eg: Unicode) for the rendered output.
    /// </summary>
    /// <value>The output encoding.</value>
    public virtual Encoding OutputEncoding
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets a value indicating whether the XML declaration should be omitted (where applicable).
    /// </summary>
    /// <value><c>true</c> if the XML declaration is to be omitted; otherwise, <c>false</c>.</value>
    public virtual bool OmitXmlDeclaration
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets a string used to indicate a single level of indentation to use when rendering an XML document.
    /// </summary>
    /// <value>The XML indentation characters.</value>
    public virtual string XmlIndentationCharacters
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets a value indicating whether XML documents should be rendered with indentated formatting or not.
    /// </summary>
    /// <value><c>true</c> if the rendering process is to output indented XML; otherwise, <c>false</c>.</value>
    public virtual bool OutputIndentedXml
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
    public virtual RenderingContext CreateRootContext(ZptElement element)
    {
      return this.CreateRootContext(element, null);
    }

    /// <summary>
    /// Creates a new root <see cref="RenderingContext"/> instance.
    /// </summary>
    /// <param name="element">The root ZPT element</param>
    /// <param name="model">The model to render</param>
    /// <returns>The root rendering context.</returns>
    public virtual RenderingContext CreateRootContext(ZptElement element, object model)
    {
      if(element == null)
      {
        throw new ArgumentNullException(nameof(element));
      }

      return this.ContextFactory.Create(element, this, model);
    }

    #endregion

    #region constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CSF.Zpt.Rendering.IRenderingOptions"/> class.
    /// </summary>
    /// <param name="addSourceFileAnnotation">Indicates whether or not source file annotation is to be added.</param>
    /// <param name="elementVisitors">The element visitors to use.</param>
    /// <param name="contextFactory">The rendering context factory.</param>
    /// <param name="outputEncoding">The desired output encoding.</param>
    /// <param name="omitXmlDeclaration">Whether or not to omit the XML declaration.</param>
    /// <param name="xmlIndentCharacters">The character(s) to use when indending XML.</param>
    /// <param name="outputIndentedXml">Whether or not to output indent-formatted XML.</param>
    public RenderingOptions(IContextVisitor[] elementVisitors = null,
                                   IRenderingContextFactory contextFactory = null,
                                   bool addSourceFileAnnotation = false,
                                   System.Text.Encoding outputEncoding = null,
                                   bool omitXmlDeclaration = false,
                                   string xmlIndentCharacters = "  ",
                                   bool outputIndentedXml = true)
    {
      this.AddSourceFileAnnotation = addSourceFileAnnotation;
      this.ContextVisitors = elementVisitors?? DefaultVisitors;
      this.ContextFactory = contextFactory?? new TalesRenderingContextFactory();
      this.OutputEncoding = outputEncoding?? System.Text.Encoding.UTF8;
      this.OmitXmlDeclaration = omitXmlDeclaration;
      this.XmlIndentationCharacters = xmlIndentCharacters?? "  ";
      this.OutputIndentedXml = outputIndentedXml;
    }

    #endregion
  }
}

