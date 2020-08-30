﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZptSharp.Config;
using System.Xml.Linq;
using System.Xml;

namespace ZptSharp.Dom
{
    /// <summary>
    /// An implementation of <see cref="IReadsAndWritesDocument"/> which uses the XML/Linq API built-into .NET
    /// Framework and dotnet core.
    /// See also: <seealso cref="XDocument"/>
    /// </summary>
    public class XmlDocumentProvider : IReadsAndWritesDocument
    {
        /// <summary>This matches the default buffer size for a built-in <see cref="StreamWriter"/>.</summary>
        const int BufferSize = 1024;

        /// <summary>
        /// Gets a document instance from the specified input stream.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This implementation is not truly asynchronous, as the APIs involved do not provide async functionality.
        /// </para>
        /// </remarks>
        /// <returns>A task which provides the document which has been read.</returns>
        /// <param name="stream">A stream containing the source of the document.</param>
        /// <param name="config">Rendering configuration.</param>
        /// <param name="token">An object used to cancel the operation if required.</param>
        public Task<IDocument> GetDocumentAsync(Stream stream, RenderingConfig config, CancellationToken token)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            token.ThrowIfCancellationRequested();

            using (var reader = new StreamReader(stream, config.DocumentEncoding))
            {
                var loadOptions = LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo;
                token.ThrowIfCancellationRequested();
                var doc = XDocument.Load(reader, loadOptions);
                IDocument xmlDoc = new XmlDocument(doc);
                return Task.FromResult(xmlDoc);
            }
        }

        /// <summary>
        /// Writes the specified document to a specified output stream.
        /// </summary>
        /// <returns>A task which provides the output stream.</returns>
        /// <param name="document">The document to write.</param>
        /// <param name="config">Rendering configuration.</param>
        /// <param name="token">An object used to cancel the operation if required.</param>
        public async Task<Stream> WriteDocumentAsync(IDocument document, RenderingConfig config, CancellationToken token)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            if (!(document is XmlDocument xmlDocument))
                throw new ArgumentException($"The document must be an instance of {nameof(XmlDocument)}.", nameof(document));
            token.ThrowIfCancellationRequested();

            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, config.DocumentEncoding, BufferSize, true))
            using (var xmlWriter = GetXmlWriter(config, writer))
            {
                token.ThrowIfCancellationRequested();
                xmlDocument.Document.Save(xmlWriter);
                token.ThrowIfCancellationRequested();
                await writer.FlushAsync().ConfigureAwait(false);
            }

            stream.Position = 0;
            return stream;
        }

        XmlWriter GetXmlWriter(RenderingConfig config, TextWriter writer)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = config.DocumentEncoding;
            settings.OmitXmlDeclaration = config.OmitXmlDeclaration;

            return XmlWriter.Create(writer, settings);
        }
    }
}
