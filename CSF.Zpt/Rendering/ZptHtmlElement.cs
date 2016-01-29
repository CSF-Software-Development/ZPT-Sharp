﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using CSF.Zpt.Resources;

namespace CSF.Zpt.Rendering
{
  /// <summary>
  /// Implementation of <see cref="ZptElement"/> based on documents parsed using the HTML Agility Pack.
  /// </summary>
  public class ZptHtmlElement : ZptElement
  {
    #region constants

    private const string
      INDENT_PATTERN        = @"([ \t]+)$",
      HTML_COMMENT_START    = "<!-- ",
      HTML_COMMENT_END      = " -->\n";
    private static readonly Regex Indent = new Regex(INDENT_PATTERN, RegexOptions.Compiled);

    #endregion

    #region fields

    private HtmlNode _node;

    #endregion

    #region properties

    /// <summary>
    /// Gets the HTML node object wrapped by the current instance.
    /// </summary>
    /// <value>The node.</value>
    public HtmlNode Node
    {
      get {
        return _node;
      }
    }

    /// <summary>
    /// Gets the element name.
    /// </summary>
    /// <value>The name.</value>
    public override string Name
    {
      get {
        return this.Node.Name;
      }
    }

    #endregion

    #region methods

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="ZptHtmlElement"/>.
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current <see cref="ZptHtmlElement"/>.</returns>
    public override string ToString()
    {
      return this.Node.OuterHtml;
    }

    /// <summary>
    /// Replaces the current element in its respective DOM with the given replacement.
    /// </summary>
    /// <returns>A reference to the replacement element, in its new DOM.</returns>
    /// <param name="replacement">Replacement.</param>
    public override ZptElement ReplaceWith(ZptElement replacement)
    {
      var repl = replacement as ZptHtmlElement;
      if(repl == null)
      {
        string message = String.Format(ExceptionMessages.ReplacementElementMustBeCorrectType,
                                       typeof(ZptHtmlElement).Name);
        throw new ArgumentException(message, "replacement");
      }

      var parent = this.GetParent();
      return new ZptHtmlElement(parent.ReplaceChild(repl.Node, this.Node),
                             repl.SourceFile,
                             isImported: true);
    }

    /// <summary>
    /// Replaces the current element instance with the given content.
    /// </summary>
    /// <returns>A collection of <see cref="ZptElement"/>, indicating the element(s) which replaced the current instance.</returns>
    /// <param name="content">The content with which to replace the current element.</param>
    /// <param name="interpretContentAsStructure">If set to <c>true</c> then the content is interpreted as structure.</param>
    public override ZptElement[] ReplaceWith(string content, bool interpretContentAsStructure)
    {
      // TODO: Write this implementation
      throw new NotImplementedException();
    }

    /// <summary>
    /// Removes all children of the current element instance and replaces them with the given content.
    /// </summary>
    /// <param name="content">The content with which to replace the children of the current element.</param>
    /// <param name="interpretContentAsStructure">If set to <c>true</c> then the content is interpreted as structure.</param>
    public override void ReplaceChildrenWith(string content, bool interpretContentAsStructure)
    {
      // TODO: Write this implementation
      throw new NotImplementedException();
    }

    /// <summary>
    /// Inserts a new child element into the current element's child elements.  The new child will be the previous
    /// sibling before a given existing child.
    /// </summary>
    /// <returns>The newly-added element.</returns>
    /// <param name="existing">An existing child element, before which the child will be inserted.</param>
    /// <param name="newChild">The new child element to insert.</param>
    public override ZptElement InsertBefore(ZptElement existing, ZptElement newChild)
    {
      // TODO: Write this implementation
      throw new NotImplementedException();
    }

    /// <summary>
    /// Inserts a new child element into the current element's child elements.  The new child will be the next
    /// sibling after a given existing child.
    /// </summary>
    /// <returns>The newly-added element.</returns>
    /// <param name="existing">An existing child element, after which the child will be inserted.</param>
    /// <param name="newChild">The new child element to insert.</param>
    public override ZptElement InsertAfter(ZptElement existing, ZptElement newChild)
    {
      // TODO: Write this implementation
      throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the element which is the parent of the current instance.
    /// </summary>
    /// <returns>The parent element.</returns>
    public override ZptElement GetParentElement()
    {
      return (this.Node.ParentNode != null)? new ZptHtmlElement(this.Node.ParentNode, this.SourceFile) : null;
    }

    /// <summary>
    /// Gets a collection of the child elements from the current source element.
    /// </summary>
    /// <returns>The children.</returns>
    public override ZptElement[] GetChildElements()
    {
      return this.Node.ChildNodes
        .Where(x => x.NodeType == HtmlNodeType.Element)
        .Select(x => new ZptHtmlElement(x, this.SourceFile))
        .ToArray();
    }

    /// <summary>
    /// Gets a collection of the attributes present upon the current element.
    /// </summary>
    /// <returns>The attributes.</returns>
    public override ZptAttribute[] GetAttributes()
    {
      return this.Node.Attributes
        .Select(x => new ZptHtmlAttribute(x))
        .ToArray();
    }

    /// <summary>
    /// Gets an attribute which matches the given criteria, or a <c>null</c> reference is no matching attribute is
    /// found.
    /// </summary>
    /// <returns>The attribute, or a <c>null</c> reference.</returns>
    /// <param name="attributeNamespace">The attribute namespace.</param>
    /// <param name="name">The attribute name.</param>
    public override ZptAttribute GetAttribute(ZptNamespace attributeNamespace, string name)
    {
      if(attributeNamespace == null)
      {
        throw new ArgumentNullException("attributeNamespace");
      }
      if(name == null)
      {
        throw new ArgumentNullException("name");
      }

      var attribName = this.GetName(attributeNamespace.Prefix, name);

      var htmlAttribute = this.Node.Attributes
        .FirstOrDefault(x => x.Name == attribName);

      return (htmlAttribute != null)? new ZptHtmlAttribute(htmlAttribute) : null;
    }

    /// <summary>
    /// Sets the value of an attribute.
    /// </summary>
    /// <param name="attributeNamespace">The attribute namespace.</param>
    /// <param name="name">The attribute name.</param>
    /// <param name="value">The attribute value.</param>
    public override void SetAttribute(ZptNamespace attributeNamespace, string name, string value)
    {
      // TODO: Write this implementation
      throw new NotImplementedException();
    }

    /// <summary>
    /// Removes a named attribute.
    /// </summary>
    /// <param name="attributeNamespace">The attribute namespace.</param>
    /// <param name="name">The attribute name.</param>
    public override void RemoveAttribute(ZptNamespace attributeNamespace, string name)
    {
      // TODO: Write this implementation
      throw new NotImplementedException();
    }

    /// <summary>
    /// Recursively searches the children of the current instance, returning a collection of elements which have an
    /// attribute matching the given criteria.
    /// </summary>
    /// <returns>The matching child elements.</returns>
    /// <param name="attributeNamespace">The attribute namespace.</param>
    /// <param name="name">The attribute name.</param>
    public override ZptElement[] SearchChildrenByAttribute(ZptNamespace attributeNamespace, string name)
    {
      string attribName = this.GetName(attributeNamespace.Prefix, name);

      return (from node in this.Node.Descendants()
              from attrib in node.Attributes
              where attrib.Name == attribName
              select new ZptHtmlElement(node, this.SourceFile))
        .ToArray();
    }

    /// <summary>
    /// Recursively searches for attributes with a given namespace or prefix and removes them from their parent
    /// element.
    /// </summary>
    /// <param name="attributeNamespace">The attribute namespace.</param>
    public override void PurgeAttributes(ZptNamespace attributeNamespace)
    {
      if(attributeNamespace == null)
      {
        throw new ArgumentNullException("attributeNamespace");
      }

      var nodes = this.Node
        .Descendants()
        .Union(new [] { this.Node })
        .ToArray();

      var toRemove = (from node in nodes
                      from attrib in node.Attributes
                      where attrib.Name.StartsWith(String.Concat(attributeNamespace.Prefix, ":"))
                      select new { Element = node, Attribute = attrib });

      foreach(var item in toRemove)
      {
        item.Element.Attributes.Remove(item.Attribute);
      }
    }

    /// <summary>
    /// Adds a new comment to the DOM immediately before the current element.
    /// </summary>
    /// <param name="comment">The comment text.</param>
    public override void AddCommentBefore(string comment)
    {
      if(comment == null)
      {
        throw new ArgumentNullException("comment");
      }

      var parent = this.GetParent();
      string indent = String.Empty;

      var previousNode = this.Node.PreviousSibling;
      if(previousNode != null
         && previousNode.NodeType == HtmlNodeType.Text)
      {
        HtmlTextNode previousText = (HtmlTextNode) previousNode;
        var indentMatch = Indent.Match(previousText.Text);
        if(indentMatch.Success)
        {
          indent = indentMatch.Value;
        }
      }

      string commentText = String.Concat(HTML_COMMENT_START, comment, HTML_COMMENT_END, indent);
      var commentNode = this.Node.OwnerDocument.CreateComment(commentText);

      parent.InsertBefore(commentNode, this.Node);
    }

    /// <summary>
    /// Gets an element or attribute name based upon its prefix and name.
    /// </summary>
    /// <returns>The assembled name.</returns>
    /// <param name="prefix">The name prefix.</param>
    /// <param name="name">The name.</param>
    private string GetName(string prefix, string name)
    {
      if(name == null)
      {
        throw new ArgumentNullException("name");
      }
      else if(name.Length == 0)
      {
        throw new ArgumentException(ExceptionMessages.NameMustNotBeEmptyString, "name");
      }

      string output;

      if(String.IsNullOrEmpty(prefix))
      {
        output = name;
      }
      else
      {
        output = String.Concat(prefix, ":", name);
      }

      return output;
    }

    /// <summary>
    /// Clone this instance into a new Element instance, which may be manipulated without affecting the original.
    /// </summary>
    public override ZptElement Clone()
    {
      var clone = _node.Clone();

      return new ZptHtmlElement(clone, this.SourceFile);
    }

    /// <summary>
    /// Gets the file location (typically a line number) for the current instance.
    /// </summary>
    /// <returns>The file location.</returns>
    public override string GetFileLocation()
    {
      return String.Format("Line {0}", _node.Line);
    }

    /// <summary>
    /// Omits the current element, replacing it with its children.
    /// </summary>
    /// <returns>
    /// A collection of the <see cref="ZptElement"/> instances which were children of the element traversed
    /// </returns>
    public override ZptElement[] Omit()
    {
      // TODO: Write this implementation
      throw new NotImplementedException();
    }

    /// <summary>
    /// Removes the current element from the DOM.
    /// </summary>
    public override void Remove()
    {
      this.GetParent().RemoveChild(this.Node);
    }

    /// <summary>
    /// Removes all child elements from the current element.
    /// </summary>
    public override void RemoveAllChildren()
    {
      // TODO: Write this implementation
      throw new NotImplementedException();
    }

    /// <summary>
    /// Determines whether or not the current instance is in the specified namespace.
    /// </summary>
    /// <returns>
    /// <c>true</c> if this instance is in the specified namespace; otherwise, <c>false</c>.
    /// </returns>
    /// <param name="nSpace">The namespace for which to test.</param>
    public override bool IsInNamespace(ZptNamespace nSpace)
    {
      // TODO: Write this implementation
      throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the parent of the current <see cref="Node"/>.
    /// </summary>
    /// <returns>The parent node.</returns>
    private HtmlNode GetParent()
    {
      var output = this.Node.ParentNode;

      if(output == null)
      {
        throw new InvalidOperationException(ExceptionMessages.CannotGetParentFromRootNode);
      }

      return output;
    }

    #endregion

    #region constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CSF.Zpt.Rendering.ZptHtmlElement"/> class.
    /// </summary>
    /// <param name="node">The source HTML node.</param>
    /// <param name="sourceFile">Information about the element's source file.</param>
    /// <param name="isRoot">Whether or not this is the root element.</param>
    /// <param name="isImported">Whether or not this element is imported.</param>
    public ZptHtmlElement(HtmlNode node,
                       SourceFileInfo sourceFile,
                       bool isRoot = false,
                       bool isImported = false) : base(sourceFile, isRoot, isImported)
    {
      if(node == null)
      {
        throw new ArgumentNullException("node");
      }

      HtmlNode actualNode;

      if(node.NodeType == HtmlNodeType.Document)
      {
        actualNode = node.FirstChild;
      }
      else if(node.NodeType != HtmlNodeType.Element)
      {
        string message = String.Format(ExceptionMessages.IncorrectWrappedNodeType, "HTML", "element");
        throw new ArgumentException(message, "node");
      }
      else
      {
        actualNode = node;
      }

      _node = actualNode;
    }

    #endregion
  }
}

