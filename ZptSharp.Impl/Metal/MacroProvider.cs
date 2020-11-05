﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ZptSharp.Dom;
using ZptSharp.Expressions;

namespace ZptSharp.Metal
{
    /// <summary>
    /// Implementation of <see cref="IGetsMacro"/> which uses an expression evaluator
    /// to get macros from attributes.
    /// </summary>
    public class MacroProvider : IGetsMacro
    {
        readonly IEvaluatesExpression expressionEvaluator;

        /// <summary>
        /// Gets the METAL macro referenced by the specified element's attribute, if such an attribute is present.
        /// </summary>
        /// <returns>The METAL macro, or a null reference if the <paramref name="element"/>
        /// has no attribute matching the <paramref name="attributeSpec"/>.</returns>
        /// <param name="element">The element from which to get the macro.</param>
        /// <param name="context">The current expression context.</param>
        /// <param name="attributeSpec">An attribute spec.</param>
        /// <param name="token">An optional cancellation token.</param>
        /// <exception cref="MacroNotFoundException">If the element does have an attribute matching
        /// the <paramref name="attributeSpec"/> but no macro could be resolved from the attribute's expression.</exception>
        public Task<MetalMacro> GetMacroAsync(IElement element,
                                              ExpressionContext context,
                                              AttributeSpec attributeSpec,
                                              CancellationToken token = default)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (attributeSpec == null)
                throw new ArgumentNullException(nameof(attributeSpec));

            return GetMacroPrivateAsync(element, context, attributeSpec, token);
        }

        async Task<MetalMacro> GetMacroPrivateAsync(IElement element,
                                                    ExpressionContext context,
                                                    AttributeSpec attributeSpec,
                                                    CancellationToken token)
        {
            var attribute = element.GetMatchingAttribute(attributeSpec);
            if (attribute == null) return null;

            MetalMacro macro = null;
            try
            {
                macro = await expressionEvaluator.EvaluateExpressionAsync<MetalMacro>(attribute.Value, context, token);
            }
            catch(Exception ex)
            {
                AssertMacroIsNotNull(macro, element, attribute.Value, attributeSpec, ex);
            }

            AssertMacroIsNotNull(macro, element, attribute.Value, attributeSpec);
            return macro;
        }

        void AssertMacroIsNotNull(MetalMacro macro,
                                  IElement element,
                                  string macroExpression,
                                  AttributeSpec attributeSpec,
                                  Exception inner = null)
        {
            if (macro != null) return;

            var message = String.Format(Resources.ExceptionMessage.MacroNotFound,
                                        attributeSpec.Name,
                                        element,
                                        macroExpression);

            if(inner != null)
                throw new MacroNotFoundException(message, inner);

            throw new MacroNotFoundException(message);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroProvider"/> class.
        /// </summary>
        /// <param name="expressionEvaluator">Expression evaluator.</param>
        public MacroProvider(IEvaluatesExpression expressionEvaluator)
        {
            this.expressionEvaluator = expressionEvaluator ?? throw new ArgumentNullException(nameof(expressionEvaluator));
        }

    }
}
