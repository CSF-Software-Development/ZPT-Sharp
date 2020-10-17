﻿using System;
using System.Collections.Generic;
using ZptSharp.Dom;

namespace ZptSharp.Expressions
{
    /// <summary>
    /// An object which provides contextual values for the rendering of TALES expressions.
    /// </summary>
    public class ExpressionContext
    {
        /// <summary>
        /// Gets or sets an object representing an error which was encountered whilst rendering or
        /// evaluating an expression.
        /// </summary>
        /// <value>The error.</value>
        public object Error { get; set; }

        /// <summary>
        /// Gets or sets the primary model object from which this context is created.
        /// </summary>
        /// <value>The model.</value>
        public object Model { get; set; }

        /// <summary>
        /// Gets or sets the current DOM element being rendered by this context.
        /// </summary>
        /// <value>The DOM element.</value>
        public IElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the DOM document being used as a template to render the current rendering request.
        /// </summary>
        /// <value>The template document.</value>
        public IDocument TemplateDocument { get; set; }

        /// <summary>
        /// Gets the local variable definitions for the current context.
        /// </summary>
        /// <value>The local definitions.</value>
        public IDictionary<string, object> LocalDefinitions { get; }

        /// <summary>
        /// Gets the global variable definitions for the current context.
        /// </summary>
        /// <value>The global definitions.</value>
        public IDictionary<string, object> GlobalDefinitions { get; }

        /// <summary>
        /// Gets the repetition variable definitions for the current context.
        /// </summary>
        /// <value>The repetition definitions.</value>
        public IDictionary<string, RepetitionInfo> Repetitions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionContext"/> class.
        /// </summary>
        /// <param name="localDefinitions">Local definitions.</param>
        /// <param name="globalDefinitions">Global definitions.</param>
        /// <param name="repetitions">Repetitions.</param>
        public ExpressionContext(IDictionary<string, object> localDefinitions = null,
                                 IDictionary<string, object> globalDefinitions = null,
                                 IDictionary<string, RepetitionInfo> repetitions = null)
        {
            LocalDefinitions = localDefinitions ?? new Dictionary<string, object>();
            GlobalDefinitions = globalDefinitions ?? new Dictionary<string, object>();
            Repetitions = repetitions ?? new Dictionary<string, RepetitionInfo>();
        }
    }
}
