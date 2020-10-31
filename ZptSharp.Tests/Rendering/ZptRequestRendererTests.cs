﻿using System;
using System.IO;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using ZptSharp.Autofixture;
using ZptSharp.Config;
using ZptSharp.Dom;

namespace ZptSharp.Rendering
{
    [TestFixture,Parallelizable]
    public class ZptRequestRendererTests
    {
        [Test, AutoMoqData]
        public void RenderAsync_returns_stream_using_correct_process(IReadsAndWritesDocument documentReaderWriter,
                                                                     [Frozen] IModifiesDocument renderer,
                                                                     ZptRequestRenderer sut,
                                                                     Stream input,
                                                                     Stream output,
                                                                     object model,
                                                                     IDocumentSourceInfo sourceInfo,
                                                                     IDocument document,
                                                                     [MockedConfig] RenderingConfig config)
        {
            var request = new RenderZptDocumentRequest(input, model, config, sourceInfo: sourceInfo,  readerWriter: documentReaderWriter);
            Mock.Get(documentReaderWriter)
                .Setup(x => x.GetDocumentAsync(input, config, sourceInfo, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(() => Task.FromResult(document));
            Mock.Get(documentReaderWriter)
                .Setup(x => x.WriteDocumentAsync(document, config, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(() => Task.FromResult(output));

            var result = sut.RenderAsync(request).Result;

            Assert.That(result, Is.SameAs(output), "Output stream is as returned from writer");
            Mock.Get(renderer)
                .Verify(x => x.ModifyDocumentAsync(document, request, It.IsAny<System.Threading.CancellationToken>()),
                        Times.Once,
                        "The rendering service should have been used with the document.");
        }

        [Test, AutoMoqData]
        public void RenderAsync_uses_reader_writer_from_service_provider_if_not_included_in_request(IReadsAndWritesDocument documentReaderWriter,
                                                                                                    [Frozen] IModifiesDocument renderer,
                                                                                                    [Frozen] IServiceProvider serviceProvider,
                                                                                                    ZptRequestRenderer sut,
                                                                                                    Stream input,
                                                                                                    Stream output,
                                                                                                    object model,
                                                                                                    IDocumentSourceInfo sourceInfo,
                                                                                                    IDocument document,
                                                                                                    [MockedConfig] RenderingConfig config)
        {
            var request = new RenderZptDocumentRequest(input, model, config, sourceInfo: sourceInfo);
            Mock.Get(serviceProvider).Setup(x => x.GetService(typeof(IReadsAndWritesDocument))).Returns(documentReaderWriter);
            Mock.Get(documentReaderWriter)
                .Setup(x => x.GetDocumentAsync(input, config, sourceInfo, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(() => Task.FromResult(document));
            Mock.Get(documentReaderWriter)
                .Setup(x => x.WriteDocumentAsync(document, config, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(() => Task.FromResult(output));

            var result = sut.RenderAsync(request).Result;

            Assert.That(result, Is.SameAs(output), "Output stream is as returned from writer");
        }
    }
}
