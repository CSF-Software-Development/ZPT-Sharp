﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using ZptSharp.Expressions;

namespace ZptSharp.Rendering
{
    [TestFixture,Parallelizable]
    public class ExpressionContextIterativeProcessorTests
    {
        [Test, AutoMoqData]
        public async Task IterateContextAndChildrenAsync_should_process_the_specified_context([Frozen] IProcessesExpressionContext contextProcessor,
                                                                                              ExpressionContext context,
                                                                                              ExpressionContextIterativeProcessor sut)
        {
            await sut.IterateContextAndChildrenAsync(context);
            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(context, CancellationToken.None), Times.Once);
        }

        [Test, AutoMoqData]
        public async Task IterateContextAndChildrenAsync_should_process_child_contexts([Frozen] IProcessesExpressionContext contextProcessor,
                                                                                       [Frozen] IGetsChildExpressionContexts childContextProvider,
                                                                                       ExpressionContext context,
                                                                                       ExpressionContext child1,
                                                                                       ExpressionContext child2,
                                                                                       ExpressionContextIterativeProcessor sut)
        {
            Mock.Get(childContextProvider)
                .Setup(x => x.GetChildContexts(context))
                .Returns(new[] { child1, child2 });

            await sut.IterateContextAndChildrenAsync(context);

            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(child1, CancellationToken.None), Times.Once, $"Processed {nameof(child1)}");
            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(child2, CancellationToken.None), Times.Once, $"Processed {nameof(child2)}");
        }

        [Test, AutoMoqData]
        public async Task IterateContextAndChildrenAsync_should_not_process_child_contexts_if_result_indicates_not_to([Frozen] IProcessesExpressionContext contextProcessor,
                                                                                                                      [Frozen] IGetsChildExpressionContexts childContextProvider,
                                                                                                                      ExpressionContext context,
                                                                                                                      ExpressionContext child1,
                                                                                                                      ExpressionContext child2,
                                                                                                                      ExpressionContextIterativeProcessor sut)
        {
            Mock.Get(childContextProvider)
                .Setup(x => x.GetChildContexts(context))
                .Returns(new[] { child1, child2 });
            Mock.Get(contextProcessor)
                .Setup(x => x.ProcessContextAsync(context, CancellationToken.None))
                .Returns(Task.FromResult(ExpressionContextProcessingResult.WithoutChildren));

            await sut.IterateContextAndChildrenAsync(context);

            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(child1, CancellationToken.None), Times.Never, $"Does not process {nameof(child1)}");
            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(child2, CancellationToken.None), Times.Never, $"Does not process {nameof(child2)}");
        }

        [Test, AutoMoqData]
        public async Task IterateContextAndChildrenAsync_should_process_additional_contexts([Frozen] IProcessesExpressionContext contextProcessor,
                                                                                            ExpressionContext context,
                                                                                            ExpressionContext additional1,
                                                                                            ExpressionContext additional2,
                                                                                            ExpressionContextIterativeProcessor sut)
        {
            Mock.Get(contextProcessor)
                .Setup(x => x.ProcessContextAsync(context, CancellationToken.None))
                .Returns(() => Task.FromResult(new ExpressionContextProcessingResult { AdditionalContexts = new[] { additional1, additional2 } }));

            await sut.IterateContextAndChildrenAsync(context);

            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(additional1, CancellationToken.None), Times.Once, $"Processed {nameof(additional1)}");
            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(additional2, CancellationToken.None), Times.Once, $"Processed {nameof(additional2)}");
        }

        [Test, AutoMoqData]
        public async Task IterateContextAndChildrenAsync_should_process_grandchild_contexts([Frozen] IProcessesExpressionContext contextProcessor,
                                                                                            [Frozen] IGetsChildExpressionContexts childContextProvider,
                                                                                            ExpressionContext context,
                                                                                            ExpressionContext child,
                                                                                            ExpressionContext grandchild1,
                                                                                            ExpressionContext grandchild2,
                                                                                            ExpressionContextIterativeProcessor sut)
        {
            Mock.Get(childContextProvider)
                .Setup(x => x.GetChildContexts(context))
                .Returns(new[] { child });
            Mock.Get(childContextProvider)
                .Setup(x => x.GetChildContexts(child))
                .Returns(new[] { grandchild1, grandchild2 });

            await sut.IterateContextAndChildrenAsync(context);

            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(grandchild1, CancellationToken.None), Times.Once, $"Processed {nameof(grandchild1)}");
            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(grandchild2, CancellationToken.None), Times.Once, $"Processed {nameof(grandchild2)}");
        }

        [Test, AutoMoqData]
        public async Task IterateContextAndChildrenAsync_should_process_additional_contexts_from_child_context([Frozen] IProcessesExpressionContext contextProcessor,
                                                                                                               [Frozen] IGetsChildExpressionContexts childContextProvider,
                                                                                                               ExpressionContext context,
                                                                                                               ExpressionContext child,
                                                                                                               ExpressionContext additional1,
                                                                                                               ExpressionContext additional2,
                                                                                                               ExpressionContextIterativeProcessor sut)
        {
            Mock.Get(childContextProvider)
                .Setup(x => x.GetChildContexts(context))
                .Returns(new[] { child });
            Mock.Get(contextProcessor)
                .Setup(x => x.ProcessContextAsync(child, CancellationToken.None))
                .Returns(() => Task.FromResult(new ExpressionContextProcessingResult { AdditionalContexts = new[] { additional1, additional2 } }));

            await sut.IterateContextAndChildrenAsync(context);

            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(additional1, CancellationToken.None), Times.Once, $"Processed {nameof(additional1)}");
            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(additional2, CancellationToken.None), Times.Once, $"Processed {nameof(additional2)}");
        }

        [Test, AutoMoqData]
        public async Task IterateContextAndChildrenAsync_should_process_sibling_additional_contexts([Frozen] IProcessesExpressionContext contextProcessor,
                                                                                                    ExpressionContext context,
                                                                                                    ExpressionContext additional,
                                                                                                    ExpressionContext sibling1,
                                                                                                    ExpressionContext sibling2,
                                                                                                    ExpressionContextIterativeProcessor sut)
        {
            Mock.Get(contextProcessor)
                .Setup(x => x.ProcessContextAsync(context, CancellationToken.None))
                .Returns(() => Task.FromResult(new ExpressionContextProcessingResult { AdditionalContexts = new[] { additional } }));
            Mock.Get(contextProcessor)
                .Setup(x => x.ProcessContextAsync(additional, CancellationToken.None))
                .Returns(() => Task.FromResult(new ExpressionContextProcessingResult { AdditionalContexts = new[] { sibling1, sibling2 } }));

            await sut.IterateContextAndChildrenAsync(context);

            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(sibling1, CancellationToken.None), Times.Once, $"Processed {nameof(sibling1)}");
            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(sibling2, CancellationToken.None), Times.Once, $"Processed {nameof(sibling2)}");
        }

        [Test, AutoMoqData]
        public async Task IterateContextAndChildrenAsync_should_process_children_of_additional_context([Frozen] IProcessesExpressionContext contextProcessor,
                                                                                                       [Frozen] IGetsChildExpressionContexts childContextProvider,
                                                                                                       ExpressionContext context,
                                                                                                       ExpressionContext additional,
                                                                                                       ExpressionContext child1,
                                                                                                       ExpressionContext child2,
                                                                                                       ExpressionContextIterativeProcessor sut)
        {
            Mock.Get(contextProcessor)
                .Setup(x => x.ProcessContextAsync(context, CancellationToken.None))
                .Returns(() => Task.FromResult(new ExpressionContextProcessingResult { AdditionalContexts = new[] { additional } }));
            Mock.Get(childContextProvider)
                .Setup(x => x.GetChildContexts(additional))
                .Returns(new[] { child1, child2 });

            await sut.IterateContextAndChildrenAsync(context);

            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(child1, CancellationToken.None), Times.Once, $"Processed {nameof(child1)}");
            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(child2, CancellationToken.None), Times.Once, $"Processed {nameof(child1)}");
        }


        [Test, AutoMoqData]
        public async Task IterateContextAndChildrenAsync_should_process_children_before_siblings(ExpressionContext context,
                                                                                                 ExpressionContext sibling1,
                                                                                                 ExpressionContext sibling2,
                                                                                                 ExpressionContext child1,
                                                                                                 ExpressionContext child2)
        {
            var contextProcessor = new Mock<IProcessesExpressionContext>(MockBehavior.Strict).Object;
            var childContextProvider = Mock.Of<IGetsChildExpressionContexts>();
            var sut = new ExpressionContextIterativeProcessor(contextProcessor, childContextProvider);

            Mock.Get(childContextProvider)
                .Setup(x => x.GetChildContexts(context))
                .Returns(new[] { sibling1, sibling2 });
            Mock.Get(childContextProvider)
                .Setup(x => x.GetChildContexts(sibling1))
                .Returns(new[] { child1, child2 });

            var sequence = new MockSequence();
            Mock.Get(contextProcessor)
                .InSequence(sequence)
                .Setup(x => x.ProcessContextAsync(context, CancellationToken.None))
                .Returns(Task.FromResult(ExpressionContextProcessingResult.Noop));
            Mock.Get(contextProcessor)
                .InSequence(sequence)
                .Setup(x => x.ProcessContextAsync(sibling1, CancellationToken.None))
                .Returns(Task.FromResult(ExpressionContextProcessingResult.Noop));
            Mock.Get(contextProcessor)
                .InSequence(sequence)
                .Setup(x => x.ProcessContextAsync(child1, CancellationToken.None))
                .Returns(Task.FromResult(ExpressionContextProcessingResult.Noop));
            Mock.Get(contextProcessor)
                .InSequence(sequence)
                .Setup(x => x.ProcessContextAsync(child2, CancellationToken.None))
                .Returns(Task.FromResult(ExpressionContextProcessingResult.Noop));
            Mock.Get(contextProcessor)
                .InSequence(sequence)
                .Setup(x => x.ProcessContextAsync(sibling2, CancellationToken.None))
                .Returns(Task.FromResult(ExpressionContextProcessingResult.Noop));

            await sut.IterateContextAndChildrenAsync(context);

            Mock.Get(contextProcessor)
                .Verify(x => x.ProcessContextAsync(sibling2, CancellationToken.None), Times.Once);
        }

    }
}
