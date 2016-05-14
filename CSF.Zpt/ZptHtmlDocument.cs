﻿using System;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using CSF.Zpt.Rendering;
using CSF.Zpt.Resources;

namespace CSF.Zpt
{
  /// <summary>
  /// Implementation of <see cref="ZptDocument"/> based on an <c>HtmlAgilityPack.HtmlDocument</c>.
  /// </summary>
  public class ZptHtmlDocument : ZptDocument
  {
    #region fields

    private HtmlDocument _document;
    private SourceFileInfo _sourceFile;

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

    /// <summary>
    /// Gets information about the document's source file.
    /// </summary>
    /// <value>The source file.</value>
    public virtual SourceFileInfo SourceFile
    {
      get {
        return _sourceFile;
      }
    }

    #endregion

    #region methods

    /// <summary>
    /// Renders the document to an <c>HtmlAgilityPack.HtmlDocument</c> instance.
    /// </summary>
    /// <returns>The rendered HTML document.</returns>
    /// <param name="options">The rendering options to use.  If <c>null</c> then default options are used.</param>
    public HtmlDocument RenderHtml(RenderingOptions options = null)
    {
      var opts = this.GetOptions(options);
      var element = this.RenderElement(opts);

      var output = new HtmlDocument();
      output.LoadHtml(element.ToString());

      return output;
    }

    /// <summary>
    /// Gets a collection of elements in the document which are defined as METAL macros.
    /// </summary>
    /// <returns>Elements representing the METAL macros.</returns>
    internal override CSF.Zpt.Metal.MetalMacro[] GetMacros()
    {
      return this.Document
          .DocumentNode
          .DescendantsAndSelf()
          .Where(ele => ele.Attributes.Any(attr => attr.Name == String.Format("{0}:{1}",
                                                                              ZptConstants.Metal.Namespace.Prefix,
                                                                              ZptConstants.Metal.DefineMacroAttribute)))
          .Select(x => {
          var element = new ZptHtmlElement(x, this.SourceFile);
          return new Metal.MetalMacro(element.GetMetalAttribute(ZptConstants.Metal.DefineMacroAttribute).Value, element);
        })
        .ToArray();
    }

    /// <summary>
    /// Renders an element to the given <c>System.IO.TextWriter</c>.
    /// </summary>
    /// <param name="writer">The text writer to render to.</param>
    /// <param name="element">The element to render.</param>
    /// <param name="options">The rendering options to use.  If <c>null</c> then default options are used.</param>
    protected override void Render(TextWriter writer,
                                   ZptElement element,
                                   RenderingOptions options)
    {
      if(writer == null)
      {
        throw new ArgumentNullException(nameof(writer));
      }
      if(element == null)
      {
        throw new ArgumentNullException(nameof(element));
      }

      var htmlElement = element as ZptHtmlElement;
      if(htmlElement == null)
      {
        string message = String.Format(ExceptionMessages.RenderedElementIncorrectType,
                                       typeof(ZptHtmlElement).Name);
        throw new ArgumentException(message, "element");
      }

      htmlElement.Node.WriteTo(writer);
    }

    /// <summary>
    /// Creates a rendering model from the current instance.
    /// </summary>
    /// <returns>The rendering model.</returns>
    protected override ZptElement GetRootElement()
    {
      return new ZptHtmlElement(this.Document.DocumentNode, this.SourceFile, isRoot: true);
    }

    #endregion

    #region constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CSF.Zpt.ZptHtmlDocument"/> class.
    /// </summary>
    /// <param name="document">An HTML document from which to create the current instance.</param>
    /// <param name="sourceFile">Information about the document's source file.</param>
    public ZptHtmlDocument(HtmlDocument document,
                           SourceFileInfo sourceFile)
    {
      if(document == null)
      {
        throw new ArgumentNullException(nameof(document));
      }
      if(sourceFile == null)
      {
        throw new ArgumentNullException(nameof(sourceFile));
      }

      _document = document;
      _sourceFile = sourceFile;
    }

    #endregion
  }
}

