﻿using System;
using System.IO;
using HtmlAgilityPack;
using CSF.Zpt.Rendering;

namespace CSF.Zpt
{
  /// <summary>
  /// Implementation of <see cref="ZptDocument"/> based on an <c>HtmlAgilityPack.HtmlDocument</c>.
  /// </summary>
  public class ZptHtmlDocument : ZptDocument
  {
    #region fields

    private HtmlDocument _document;

    #endregion

    #region properties

    /// <summary>
    /// Gets the original HTML document.
    /// </summary>
    /// <value>The original HTML document.</value>
    public virtual HtmlDocument Document
    {
      get {
        return _document;
      }
    }

    #endregion

    #region methods

    /// <summary>
    /// Renders the document to an <c>HtmlAgilityPack.HtmlDocument</c> instance.
    /// </summary>
    /// <returns>The rendered HTML document.</returns>
    /// <param name="context">The rendering context, containing the object model of data available to the document.</param>
    public HtmlDocument RenderHtml(RenderingContext context)
    {
      if(context == null)
      {
        throw new ArgumentNullException("context");
      }

      var element = this.RenderElement(context);

      var output = new HtmlDocument();
      output.LoadHtml(element.ToString());

      return output;
    }

    /// <summary>
    /// Renders the document to the given <c>System.IO.TextWriter</c>.
    /// </summary>
    /// <param name="writer">The text writer to render to.</param>
    /// <param name="context">The rendering context, containing the object model of data available to the document.</param>
    public override void Render(TextWriter writer, RenderingContext context)
    {
      var doc = this.RenderHtml(context);
      doc.Save(writer);
    }

    /// <summary>
    /// Renders an element to the given <c>System.IO.TextWriter</c>.
    /// </summary>
    /// <param name="writer">The text writer to render to.</param>
    /// <param name="element">The element to render.</param>
    protected override void Render(TextWriter writer, Element element)
    {
      if(writer == null)
      {
        throw new ArgumentNullException("writer");
      }
      if(element == null)
      {
        throw new ArgumentNullException("element");
      }

      var htmlElement = element as HtmlElement;
      if(htmlElement == null)
      {
        throw new ArgumentException("Element must be an instance of HtmlElement.", "element");
      }

      htmlElement.Node.WriteTo(writer);
    }

    /// <summary>
    /// Creates a rendering model from the current instance.
    /// </summary>
    /// <returns>The rendering model.</returns>
    protected override Element GetRootElement()
    {
      return new HtmlElement(this.Document.DocumentNode);
    }

    #endregion

    #region constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CSF.Zpt.ZptHtmlDocument"/> class.
    /// </summary>
    /// <param name="document">An HTML document from which to create the current instance.</param>
    public ZptHtmlDocument(HtmlDocument document)
    {
      if(document == null)
      {
        throw new ArgumentNullException("document");
      }

      _document = document;
    }

    #endregion
  }
}

