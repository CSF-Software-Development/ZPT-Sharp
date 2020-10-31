﻿using System;
namespace ZptSharp.Dom
{
    /// <summary>
    /// Abstract base class for an <see cref="IAttribute"/>, containing functionality
    /// which is neutral to the specific implementation.
    /// </summary>
    public abstract class AttributeBase : IAttribute
    {
        readonly IElement element;

        /// <summary>
        /// Gets the element upon which this attribute appears.
        /// </summary>
        /// <value>The element.</value>
        public virtual IElement Element => element;

        /// <summary>
        /// Gets the attribute name, including any relevant prefix.
        /// </summary>
        /// <value>The name.</value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <value>The value.</value>
        public abstract string Value { get; }

        /// <summary>
        /// Gets a value indicating whether or not the current instance matches a specified attribute.
        /// </summary>
        /// <returns><see langword="true"/> if the current instance matches the specified attribute; <see langword="false"/> if it does not.</returns>
        /// <param name="spec">The spec to match against.</param>
        public abstract bool Matches(AttributeSpec spec);

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeBase"/> class.
        /// </summary>
        /// <param name="element">Element.</param>
        public AttributeBase(IElement element)
        {
            this.element = element ?? throw new ArgumentNullException(nameof(element));
        }
    }
}
