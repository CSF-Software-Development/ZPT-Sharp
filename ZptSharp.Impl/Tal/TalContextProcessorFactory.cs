﻿using ZptSharp.Expressions;
using ZptSharp.Rendering;

namespace ZptSharp.Tal
{
    /// <summary>
    /// Implementation of <see cref="IGetsTalContextProcessor"/> which returns
    /// a context processor suitable for processing the TAL model-binding syntax.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This factory assembles a service which uses the decorator pattern.  Each individual attribute
    /// is handled by a separate decorator class (which handles just that attribute).  At the centre
    /// of the decorator stack is a no-op service.
    /// </para>
    /// <para>
    /// The <see cref="OnErrorAttributeDecorator"/> is special, in that it uses a <c>try/catch</c>
    /// construct to intercept any errors which occur in other decorators which are 'deeper' in the stack.
    /// </para>
    /// </remarks>
    public class TalContextProcessorFactory : IGetsTalContextProcessor
    {
        readonly IGetsTalAttributeSpecs specProvider;
        readonly IEvaluatesExpression evaluator;
        readonly IInterpretsExpressionResult resultInterpreter;
        readonly IGetsVariableDefinitionsFromAttributeValue definitionProvider;
        readonly IEvaluatesDomValueExpression domEvaluator;
        private readonly Microsoft.Extensions.Logging.ILogger<OnErrorAttributeDecorator> onErrorlogger;

        /// <summary>
        /// Gets the TAL context processor.
        /// </summary>
        /// <returns>The TAL context processor.</returns>
        public IProcessesExpressionContext GetTalContextProcessor()
        {
            var service = GetNoOpService();
            service = GetOmitTagDecorator(service);
            service = GetAttributesDecorator(service);
            service = GetContentOrReplaceDecorator(service);
            service = GetRepeatDecorator(service);
            service = GetConditionDecorator(service);
            service = GetDefineDecorator(service);
            service = GetOnErrorDecorator(service);

            return service;
        }

        IProcessesExpressionContext GetNoOpService()
            => new NoOpTalContextProcessor();

        IProcessesExpressionContext GetOnErrorDecorator(IProcessesExpressionContext service)
            => new OnErrorAttributeDecorator(service, specProvider, domEvaluator, onErrorlogger);

        IProcessesExpressionContext GetOmitTagDecorator(IProcessesExpressionContext service)
            => new OmitTagAttributeDecorator(service, specProvider);

        IProcessesExpressionContext GetAttributesDecorator(IProcessesExpressionContext service)
            => new AttributesAttributeDecorator(service, specProvider);

        IProcessesExpressionContext GetContentOrReplaceDecorator(IProcessesExpressionContext service)
            => new ContentOrReplaceAttributeDecorator(service, specProvider);

        IProcessesExpressionContext GetRepeatDecorator(IProcessesExpressionContext service)
            => new RepeatAttributeDecorator(service, specProvider);

        IProcessesExpressionContext GetConditionDecorator(IProcessesExpressionContext service)
            => new ConditionAttributeDecorator(service, specProvider, evaluator, resultInterpreter);

        IProcessesExpressionContext GetDefineDecorator(IProcessesExpressionContext service)
            => new DefineAttributeDecorator(service, specProvider, evaluator, resultInterpreter, definitionProvider);

        public TalContextProcessorFactory(IGetsTalAttributeSpecs specProvider,
                                          IEvaluatesExpression evaluator,
                                          IInterpretsExpressionResult resultInterpreter,
                                          IGetsVariableDefinitionsFromAttributeValue definitionProvider,
                                          IEvaluatesDomValueExpression domEvaluator,
                                          Microsoft.Extensions.Logging.ILogger<OnErrorAttributeDecorator> onErrorlogger)
        {
            this.specProvider = specProvider ?? throw new System.ArgumentNullException(nameof(specProvider));
            this.evaluator = evaluator ?? throw new System.ArgumentNullException(nameof(evaluator));
            this.resultInterpreter = resultInterpreter ?? throw new System.ArgumentNullException(nameof(resultInterpreter));
            this.definitionProvider = definitionProvider ?? throw new System.ArgumentNullException(nameof(definitionProvider));
            this.domEvaluator = domEvaluator ?? throw new System.ArgumentNullException(nameof(domEvaluator));
            this.onErrorlogger = onErrorlogger ?? throw new System.ArgumentNullException(nameof(onErrorlogger));
        }
    }
}
