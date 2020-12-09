using System.Collections.Generic;
using ZptSharp.Rendering;

namespace ZptSharp.Dom
{
    /// <summary>
    /// Abstraction for a DOM node.
    /// </summary>
    public interface INode : IHasDocumentSourceInfo, IHasElements
    {
        /// <summary>
        /// Gets the parent document for the current element.
        /// </summary>
        /// <value>The document.</value>
        IDocument Document { get; }

        /// <summary>
        /// Gets information which indicates the original source of the element (for example, a file path and line number).
        /// </summary>
        /// <value>The source info.</value>
        new ElementSourceInfo SourceInfo { get; }

        /// <summary>
        /// Gets or sets information about the source of the element  (for example, a file path and line number) before it was replaced.
        /// For most elements this will be <see langword="null"/>, but when the current element is a replacement (such as METAL macro usage),
        /// this property will contain source information for the replaced element.
        /// </summary>
        /// <value>The pre-replacement source info.</value>
        ElementSourceInfo PreReplacementSourceInfo { get; set; }

        /// <summary>
        /// Gets a collection of the element's attributes.
        /// </summary>
        /// <value>The attributes.</value>
        IList<IAttribute> Attributes { get; }

        /// <summary>
        /// Gets the children contained within the current node.
        /// </summary>
        /// <value>The child elements.</value>
        IList<INode> ChildNodes { get; }

        /// <summary>
        /// Gets or sets the parent for the current element.  This will be a <see langword="null"/>
        /// reference if the current instance is the root element of the document or if the element
        /// is not attached to a DOM.
        /// </summary>
        /// <value>The parent element.</value>
        INode ParentElement { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="INode"/> is an element node.
        /// </summary>
        /// <value><c>true</c> if the current instance is an element; otherwise, <c>false</c>.</value>
        bool IsElement { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="INode"/> is imported.  A node is imported
        /// if its associated <see cref="Document"/> differs from the document upon its <see cref="ParentElement"/>.
        /// </summary>
        /// <value><c>true</c> if this node is imported; otherwise, <c>false</c>.</value>
        bool IsImported { get; }

        /// <summary>
        /// Gets a value which indicates whether or not the current element is in the specified namespace.
        /// </summary>
        /// <returns><c>true</c>, if the element is in the specified namespace, <c>false</c> otherwise.</returns>
        /// <param name="namespace">A namespace.</param>
        bool IsInNamespace(Namespace @namespace);

        /// <summary>
        /// Gets a copy of the current element and all of its children.
        /// </summary>
        /// <returns>The copied element.</returns>
        INode GetCopy();

        /// <summary>
        /// Creates and returns a new comment node.
        /// </summary>
        /// <returns>The comment node.</returns>
        /// <param name="commentText">The text for the comment.</param>
        INode CreateComment(string commentText);

        /// <summary>
        /// Creates and returns a new text node from the specified content.
        /// Even if the content contains valid markup, it is strictly to be treated as text.
        /// </summary>
        /// <returns>A text node.</returns>
        /// <param name="content">The text content for the node.</param>
        INode CreateTextNode(string content);

        /// <summary>
        /// Creates and returns a new attribute from the specified specification.
        /// </summary>
        /// <returns>An attribute.</returns>
        /// <param name="spec">The attribute specification which will be used to name the attribute.</param>
        IAttribute CreateAttribute(AttributeSpec spec);

        /// <summary>
        /// Parses the specified text <paramref name="markup"/> and returns the resulting nodes.
        /// </summary>
        /// <returns>The parsed nodes.</returns>
        /// <param name="markup">Markup text.</param>
        IList<INode> ParseAsNodes(string markup);
    }
}
