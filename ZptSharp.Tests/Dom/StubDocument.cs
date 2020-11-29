﻿using System;
using System.Collections.Generic;

namespace ZptSharp.Dom
{
    /// <summary>
    /// A stub document with no backing implementation, for testing purposes.
    /// Members are all intentionally virtual so that this class may be mocked.
    /// </summary>
    public class StubDocument : DocumentBase
    {
        public virtual INode Root { get; set; }

        public override INode RootElement => Root;

        public override INode CreateComment(string commentText) => null;

        public override INode CreateTextNode(string content) => throw new NotImplementedException();

        public override IList<INode> ParseAsNodes(string markup) => throw new NotImplementedException();

        public override IAttribute CreateAttribute(AttributeSpec spec) => throw new NotImplementedException();

        public override void AddCommentToBeginningOfDocument(string commentText) { }

        public StubDocument(Rendering.IDocumentSourceInfo source) : base(source) { }
    }
}
