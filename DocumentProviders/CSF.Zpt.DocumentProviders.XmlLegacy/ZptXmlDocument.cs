﻿using System;
using System.Xml;
using System.IO;
using System.Linq;
using CSF.Zpt.Rendering;
using CSF.Zpt.Resources;
using CSF.Zpt.Tales;

namespace CSF.Zpt.DocumentProviders
{
  /// <summary>
  /// Implementation of <see cref="ZptDocument"/> based on a <c>System.Xml.XmlDocument</c>.
  /// </summary>
  public class ZptXmlDocument : ZptDocument
  {
    #region constants

    private const bool DefaultIndentMode = true;
    private const string DefaultIndentationCharacters = "  ";

    #endregion

    #region fields

    private XmlDocument _document;
    private ISourceInfo _sourceFile;
    private string _indentationCharacters;

    #endregion

    #region properties

    /// <summary>
    /// Gets the original XML document.
    /// </summary>
    /// <value>The original XML document.</value>
    public virtual XmlDocument Document
    {
      get {
        return _document;
      }
    }

    /// <summary>
    /// Gets information about the document's source file.
    /// </summary>
    /// <value>The source file.</value>
    public virtual ISourceInfo SourceFile
    {
      get {
        return _sourceFile;
      }
    }

    /// <summary>
    /// Gets the <see cref="RenderingMode"/> for which the current document type caters.
    /// </summary>
    /// <value>The rendering mode.</value>
    public override RenderingMode Mode
    {
      get {
        return RenderingMode.Xml;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to output indented XML or not.
    /// </summary>
    /// <value><c>true</c> if the rendered output should be indented; otherwise, <c>false</c>.</value>
    public bool IndentOutput
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets a character string representing the indentation.
    /// </summary>
    /// <value>The indentation characters.</value>
    public string IndentationCharacters
    {
      get {
        return _indentationCharacters;
      }
      set {
        if(value == null)
        {
          throw new ArgumentNullException(nameof(value));
        }

        _indentationCharacters = value;
      }
    }

    #endregion

    #region methods

    /// <summary>
    /// Renders the document to an <c>System.Xml.XmlDocument</c> instance.
    /// </summary>
    /// <returns>The rendered XML document.</returns>
    /// <param name="options">The rendering options to use.  If <c>null</c> then default options are used.</param>
    /// <param name="contextConfigurator">An optional action to perform upon the root <see cref="IModelValueContainer"/>, to configure it.</param>
    public XmlDocument RenderXml(IRenderingSettings options = null,
                                 Action<IModelValueContainer> contextConfigurator = null)
    {
      return RenderXml(null, options, contextConfigurator);
    }

    /// <summary>
    /// Renders the document to an <c>System.Xml.XmlDocument</c> instance.
    /// </summary>
    /// <returns>The rendered XML document.</returns>
    /// <param name="model">An object for which the ZPT document is to be applied.</param>
    /// <param name="options">The rendering options to use.  If <c>null</c> then default options are used.</param>
    /// <param name="contextConfigurator">An optional action to perform upon the root <see cref="IModelValueContainer"/>, to configure it.</param>
    public XmlDocument RenderXml(object model,
                                 IRenderingSettings options = null,
                                 Action<IModelValueContainer> contextConfigurator = null)
    {
      var opts = this.GetOptions(options);
      var element = this.RenderElement(model, opts, contextConfigurator);

      var output = new XmlDocument();
      output.LoadXml(element.ToString());

      return output;
    }

    /// <summary>
    /// Gets a collection of elements in the document which are defined as METAL macros.
    /// </summary>
    /// <returns>Elements representing the METAL macros.</returns>
    public override ITalesPathHandler GetMacros()
    {
      var xpath = String.Format("//*[@{0}:{1}]",
                                ZptConstants.Metal.Namespace.Prefix,
                                ZptConstants.Metal.DefineMacroAttribute);

      var nsManager = new XmlNamespaceManager(this.Document.CreateNavigator().NameTable);
      nsManager.AddNamespace(ZptConstants.Metal.Namespace.Prefix, ZptConstants.Metal.Namespace.Uri);

      var output = this.Document.DocumentElement
        .SelectNodes(xpath, nsManager)
        .Cast<XmlNode>()
        .Select(x => {
          var element = new ZptXmlElement(x, this.SourceFile, this, isImported: true);
          var context = new RenderingContext(Model.Empty, Model.Empty, element, GetDefaultOptions());
          return new Metal.MetalMacro(context.GetMetalAttribute(ZptConstants.Metal.DefineMacroAttribute).Value, element);
        })
        .ToArray();

      return new CSF.Zpt.Metal.MetalMacroCollection(output);
    }

    /// <summary>
    /// Gets information about the source medium for the current instance
    /// </summary>
    /// <returns>The source info.</returns>
    public override ISourceInfo GetSourceInfo()
    {
      return this.SourceFile;
    }

    /// <summary>
    /// Renders an element to the given <c>System.IO.TextWriter</c>.
    /// </summary>
    /// <param name="writer">The text writer to render to.</param>
    /// <param name="element">The element to render.</param>
    /// <param name="options">The rendering options to use.  If <c>null</c> then default options are used.</param>
    protected override void Render(TextWriter writer,
                                   IZptElement element,
                                   IRenderingSettings options)
    {
      if(writer == null)
      {
        throw new ArgumentNullException(nameof(writer));
      }
      if(element == null)
      {
        throw new ArgumentNullException(nameof(element));
      }

      var xmlElement = ConvertElement<ZptXmlElement>(element);

      var settings = new XmlWriterSettings();
      settings.Indent = this.IndentOutput;
      settings.IndentChars = this.IndentationCharacters;
      settings.Encoding = options.OutputEncoding;
      settings.OmitXmlDeclaration = options.OmitXmlDeclaration;

      using(var xmlWriter = XmlTextWriter.Create(writer, settings))
      {
        xmlElement.Node.OwnerDocument.WriteTo(xmlWriter);  
      }
    }

    /// <summary>
    /// Creates a rendering model from the current instance.
    /// </summary>
    /// <returns>The rendering model.</returns>
    protected override IZptElement GetRootElement()
    {
      return new ZptXmlElement(this.Document.DocumentElement, this.SourceFile, this, isRoot: true);
    }

    /// <summary>
    /// Gets an instance of <see cref="IRenderingSettings"/> which represents the default options.
    /// </summary>
    /// <returns>The default options.</returns>
    protected override IRenderingSettings GetDefaultOptions()
    {
      return new DefaultRenderingSettings();
    }

    #endregion

    #region constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CSF.Zpt.DocumentProviders.ZptXmlDocument"/> class.
    /// </summary>
    /// <param name="document">An XML document from which to create the current instance.</param>
    /// <param name="sourceFile">Information about the document's source file.</param>
    public ZptXmlDocument(XmlDocument document,
                          ISourceInfo sourceFile)
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

      this.IndentOutput = DefaultIndentMode;
      this.IndentationCharacters = DefaultIndentationCharacters;
    }

    #endregion
  }
}

