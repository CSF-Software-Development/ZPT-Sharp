using System;
using NUnit.Framework;
using CraigFowler.Web.ZPT.Metal;
using CraigFowler.Web.ZPT;
using System.IO;
using System.Xml;
using CraigFowler.Web.ZPT.Tales;
using CraigFowler.Web.ZPT.Tales.Exceptions;
using CraigFowler.Web.ZPT.Metal.Exceptions;
using System.Collections.Generic;

namespace Test.CraigFowler.Web.ZPT.Metal
{
  [TestFixture]
  public class TestMetalDocument
  {
    #region properties
    
    public TestConfiguration Config
    {
      get;
      private set;
    }
    
    #endregion
    
    #region tests
    
    [Test]
    public void TestLoad()
    {
      MetalDocument testDoc = GetMasterDocument();
      
      Assert.IsNotNull(testDoc, "Document is not null");
    }
    
    [Test]
    [Category("Integration")]
    public void TestGetUseMacro()
    {
      TalesContext context = new TalesContext();
      MetalDocument testDoc = GetMasterDocument();
      Assert.IsNotNull(testDoc, "Document is not null");
      
      XmlNode targetNode = testDoc.GetElementsByTagName("p", "http://www.w3.org/1999/xhtml")[1];
      Assert.IsInstanceOfType(typeof(MetalElement), targetNode, "Target node is a METAL element node.");
      
      context.AddDefinition("documents", this.Config.GetTestDocuments());
      
      MetalElement element = targetNode as MetalElement;
      MetalMacro macro = null;
      
      try
      {
        macro = element.GetUseMacro(context);
      }
      catch(TraversalException ex)
      {
        foreach(TalesPath path in ex.Attempts.Keys)
        {
          Console.WriteLine (@"Traversal exception information:

Path
----
{0}

Exception
---------
{1}",
                             path.ToString(),
                             ex.Attempts[path].ToString());
        }
        
        throw;
      }
      catch(MetalException ex)
      {
        Console.WriteLine ("Exception information.  Path = {0}", ex.Data["Path"]);
        throw;
      }
      
      
      Assert.AreEqual("test", macro.MacroName, "Correct macro name");
    }
    
    [Test]
    [Category("Integration")]
    public void TestRetrieveMacroUsingTalesExpression()
    {
      MetalDocument innerDoc = new MetalDocument();
      ZptDocumentCollection documentCollection = new ZptDocumentCollection();
      ZptDocument document;
      TalesContext context = new TalesContext();
      
      innerDoc.LoadXml("<p xmlns:metal=\"http://xml.zope.org/namespaces/metal\" metal:define-macro=\"test\">Foo</p>");
      
      document = new ZptDocument(new ZptMetadata(), innerDoc);
      document.Metadata.DocumentType = ZptDocumentType.Metal;
      
      documentCollection.StoreItem(new TalesPath("foo/bar"), document);
      
      context.AddDefinition("documents", documentCollection);
      
      Assert.IsInstanceOfType(typeof(MetalMacroCollection),
                              context.CreateExpression("documents/foo/bar/macros").GetValue(),
                              "Macro collection is at correct location");
      
      Assert.IsInstanceOfType(typeof(MetalMacro),
                              context.CreateExpression("documents/foo/bar/macros/test").GetValue(),
                              "Test macro is at correct location");
      
      MetalMacro macro = context.CreateExpression("documents/foo/bar/macros/test").GetValue() as MetalMacro;
      
      Assert.AreEqual("test", macro.MacroName, "Correct macro name");
    }
    
    [Test]
    public void TestImportNode()
    {
      MetalDocument
        doc1 = new MetalDocument(),
        doc2 = new MetalDocument();
      XmlNode imported;
      
      doc1.LoadXml(@"
<html xmlns=""http://www.w3.org/1999/xhtml""
      xmlns:tal=""http://xml.zope.org/namespaces/tal""
      xmlns:metal=""http://xml.zope.org/namespaces/metal"">
<head>
<title>Macro test document</title>
</head>
<body>
  <h1>What this file is</h1>
  <p>This document is a test of the METAL macro system.  Parts of this document are delegated out to METAL macros.</p>
  <p metal:use-macro=""documents/three/macro-test-macro1/macros/test"" id=""useMacroNode"">
    This is default text provided by the master document.
  </p>
</body>
</html>");
      
      doc2.LoadXml(@"
<html xmlns=""http://www.w3.org/1999/xhtml""
      xmlns:tal=""http://xml.zope.org/namespaces/tal""
      xmlns:metal=""http://xml.zope.org/namespaces/metal"">
<head>
<title>Macro test document</title>
</head>
<body>
  <h1>What this file is</h1>
  <p>This document is a test of the METAL macro system.  Parts of this document are delegated out to METAL macros.</p>
  <p metal:use-macro=""documents/three/macro-test-macro1/macros/test"" id=""useMacroNode"">
    This is alternative text provided by the other document.
  </p>
</body>
</html>");
      
      imported = doc1.ImportNode(doc2.GetElementsByTagName("p", "http://www.w3.org/1999/xhtml")[1], true);
      
      Assert.IsNotNull(imported, "Imported node is not null");
      Assert.IsNotNull(imported.FirstChild.Value, "Imported node value is not null");
      
      Assert.AreEqual("This is alternative text provided by the other document.",
                      imported.FirstChild.Value.Trim(),
                      "Correct node value");
      
      doc1.GetElementsByTagName("body", "http://www.w3.org/1999/xhtml")[0].AppendChild(imported);
    }
    
    [Test]
    [Category("Integration")]
    public void TestRenderUseMacro()
    {
      TestConfiguration config = new TestConfiguration();
      DirectoryInfo testDataPath = config.GetTestDataDirectoryInfo().GetDirectories("Document Root")[0];
      ZptDocumentCollection testData = ZptDocumentCollection.CreateFromFilesystem(testDataPath);
      ZptDocument document = (ZptDocument) testData["macro-test-master"];
      XmlDocument renderedDocument = new XmlDocument();
      
      document.GetTemplateDocument().TalesContext.AddDefinition("documents", testData);
      
      renderedDocument.LoadXml(document.GetTemplateDocument().Render());
      
      Assert.AreEqual("This is text that is generated by the <strong xmlns=\"http://www.w3.org/1999/xhtml\">macro one document</strong> within the macro named <code xmlns=\"http://www.w3.org/1999/xhtml\">test</code>.",
                      renderedDocument.GetElementsByTagName("p", "http://www.w3.org/1999/xhtml")[1].InnerXml.Trim(),
                      "Correct inner XML");
    }
    
    [Test]
    [Category("Integration")]
    public void TestRenderUseMacroWithSlots()
    {
      TestConfiguration config = new TestConfiguration();
      DirectoryInfo testDataPath = config.GetTestDataDirectoryInfo().GetDirectories("Document Root")[0];
      ZptDocumentCollection testData = ZptDocumentCollection.CreateFromFilesystem(testDataPath);
      ZptDocument document = ((ZptDocument) ((TalesStructureProvider) testData["three"])["macro-test-macro3"]);
      
      document.GetTemplateDocument().TalesContext.AddDefinition("documents", testData);
      
      try
      {
        Console.WriteLine (document.GetTemplateDocument().Render());
      }
      catch(Exception ex)
      {
        foreach(object key in ex.Data.Keys)
        {
          Console.WriteLine ("{0,-20} : {1}", key, ex.Data[key]);
        }
        
        throw;
      }
    }
    
    #endregion
    
    #region helper methods
    
    private MetalDocument GetMasterDocument()
    {
      MetalDocument output = new MetalDocument();
      string filePath = Path.Combine(this.Config.GetTestDataDirectoryInfo().FullName,
                                     "Document Root/macro-test-master.pt");
      
      output.Load(filePath);
      
      return output;
    }
    
    #endregion
    
    #region constructor
    
    public TestMetalDocument ()
    {
      this.Config = new TestConfiguration();
    }
    
    #endregion
  }
}

