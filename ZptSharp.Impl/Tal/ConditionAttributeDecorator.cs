﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ZptSharp.Dom;
using ZptSharp.Expressions;
using ZptSharp.Rendering;

namespace ZptSharp.Tal
{
    /// <summary>
    /// Decorator for <see cref="IProcessesExpressionContext"/> which handles TAL 'condition' attributes.
    /// </summary>
    public class ConditionAttributeDecorator : IProcessesExpressionContext
    {
        readonly IProcessesExpressionContext wrapped;
        readonly IGetsTalAttributeSpecs specProvider;
        readonly IEvaluatesExpression evaluator;
        readonly IInterpretsExpressionResult resultInterpreter;

        /// <summary>
        /// Processes the context using the rules defined within this object.
        /// </summary>
        /// <returns>A result object indicating the outcome of processing.</returns>
        /// <param name="context">The context to process.</param>
        /// <param name="token">An optional cancellation token.</param>
        public async Task<ExpressionContextProcessingResult> ProcessContextAsync(ExpressionContext context, CancellationToken token = default)
        {
            var wrappedResult = await wrapped.ProcessContextAsync(context, token);

            var conditionAttribute = context.CurrentElement.GetMatchingAttribute(specProvider.Condition);
            if (conditionAttribute == null) return wrappedResult;

            var expressionResult = await evaluator.EvaluateExpressionAsync(conditionAttribute.Value, context, token);
            if(ShouldRemoveAttribute(expressionResult))
            {
                context.CurrentElement.Remove();
                wrappedResult.AbortFurtherProcessing = true;
            }

            return wrappedResult;
        }

        bool ShouldRemoveAttribute(object expressionResult)
        {
            if (resultInterpreter.DoesResultCancelTheAction(expressionResult)) return false;
            return !resultInterpreter.CoerceResultToBoolean(expressionResult);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionAttributeDecorator"/> class.
        /// </summary>
        /// <param name="wrapped">Wrapped.</param>
        /// <param name="specProvider">Spec provider.</param>
        /// <param name="evaluator">Evaluator.</param>
        /// <param name="resultInterpreter">Result interpreter.</param>
        public ConditionAttributeDecorator(IProcessesExpressionContext wrapped,
                                           IGetsTalAttributeSpecs specProvider,
                                           IEvaluatesExpression evaluator,
                                           IInterpretsExpressionResult resultInterpreter)
        {
            this.wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            this.specProvider = specProvider ?? throw new ArgumentNullException(nameof(specProvider));
            this.evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            this.resultInterpreter = resultInterpreter ?? throw new ArgumentNullException(nameof(resultInterpreter));
        }
    }
}
