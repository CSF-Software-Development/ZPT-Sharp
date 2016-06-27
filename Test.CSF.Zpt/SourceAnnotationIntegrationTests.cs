using System;
using NUnit.Framework;
using CSF.Zpt;
using CSF.Zpt.Tales;
using System.IO;
using CSF.Configuration;
using CSF.Zpt.Rendering;
using System.Collections.Generic;
using System.Linq;
using CSF.IO;

namespace Test.CSF.Zpt
{
  [TestFixture]
  [Category("Integration")]
  public class SourceAnnotationIntegrationTests
  {
    #region fields

    private IZptDocumentFactory _documentFactory;
    private IIntegrationTestConfiguration _config;
    private DirectoryInfo _sourcePath, _expectedPath;
    private log4net.ILog _logger;

    #endregion

    #region setup & teardown

    [TestFixtureSetUp]
    public void FixtureSetup()
    {
      _logger = log4net.LogManager.GetLogger(this.GetType());

      IIntegrationTestConfiguration configuredConfig = ConfigurationHelper.GetSection<IntegrationTestConfiguration>();

      _config = configuredConfig?? new FallbackIntegrationTestConfiguration();

      _sourcePath = _config.GetSourceDocumentPath(IntegrationTestType.SourceAnnotation);
      _expectedPath = _config.GetExpectedOutputPath(IntegrationTestType.SourceAnnotation);
    }

    [SetUp]
    public void Setup()
    {
      var fac = new ZptDocumentFactory();
      _documentFactory = fac;
    }

    #endregion

    #region tests

    [Test]
    public void RunIntegrationTests()
    {
      // Arrange
      var filePairsToTest = this.GetFilePairsToTest();
      var failedTests = new List<FileInfo>();

      // Act
      foreach(var pair in filePairsToTest)
      {
        if(!PerformTestRun(pair.Item1, pair.Item2))
        {
          failedTests.Add(pair.Item2);
        }
      }

      _logger.InfoFormat("{0} integration test cases processed", filePairsToTest.Count());

      // Assert
      Assert.That(!failedTests.Any(),
                  "Out of {0} integration tests, {1} failed. See the log file for more info.",
                  filePairsToTest.Count(),
                  failedTests.Count());
    }

    [TestCase("test_sa1.xml")]
    [Explicit("This test is covered by RunIntegrationTests - this method is for running them one at a time though.")]
    public void TestSingleIntegrationTest(string inputFileName)
    {
      // Arrange
      var allFilePairs = this.GetFilePairsToTest();
      var filePairToTest = allFilePairs.SingleOrDefault(x => x.Item2.Name == inputFileName);

      if(filePairToTest == null)
      {
        Assert.Fail("The input parameter must identify a valid expected output document by filename.");
      }

      // Act
      var result = this.PerformTestRun(filePairToTest.Item1, filePairToTest.Item2);

      // Assert
      Assert.IsTrue(result, "Test must result in a successful rendering");
    }

    #endregion

    #region methods

    private bool PerformTestRun(FileInfo sourceDocument,
                                FileInfo expectedResultDocument)
    {
      bool output = false;
      IZptDocument document = null;
      string expectedRendering, actualRendering = null;
      bool exceptionCaught = false;

      try
      {
        document = _documentFactory.CreateDocument(sourceDocument);
      }
      catch(Exception ex)
      {
        _logger.ErrorFormat("Exception caught whilst loading the source document:{0}{1}{2}",
                            expectedResultDocument.Name,
                            Environment.NewLine,
                            ex);
        exceptionCaught = true;
      }

      if(!exceptionCaught)
      {
        using(var stream = expectedResultDocument.OpenRead())
        using(var reader = new StreamReader(stream))
        {
          expectedRendering = reader.ReadToEnd();
        }

        try
        {
          var root = sourceDocument.GetParent().GetParent();
          var options = new DefaultRenderingOptions(contextFactory: this.CreateTestEnvironment(root),
                                                    outputIndentedXml: true,
                                                    xmlIndentCharacters: "\t",
                                                    addSourceFileAnnotation: true);

          actualRendering = document.Render(options).Replace(Environment.NewLine, "\n");
          output = (actualRendering == expectedRendering);
        }
        catch(Exception ex)
        {
          _logger.ErrorFormat("Exception caught whilst processing output file:{0}{1}{2}",
                              expectedResultDocument.Name,
                              Environment.NewLine,
                              ex);
          output = false;
          exceptionCaught = true;
        }
      }

      if(!output && !exceptionCaught)
      {
        _logger.ErrorFormat("Unexpected rendering whilst processing expected output:{0}{1}{2}",
                            expectedResultDocument.Name,
                            Environment.NewLine,
                            actualRendering);
      }

      return output;
    }

    private IRenderingContextFactory CreateTestEnvironment(DirectoryInfo rootPath)
    {
      var output = new TalesRenderingContextFactory();
      output.RootDocumentPath = rootPath.FullName;

      // The location of the other ZPT documents
      output.MetalLocalDefinitions.Add("documents", new TemplateDirectory(_sourcePath));
      var tests = new NamedObjectWrapper();
      tests["input"] = new TemplateDirectory(_sourcePath, true);
      output.MetalLocalDefinitions.Add("tests", tests);

      // The 'content' keyword option
      var content = new NamedObjectWrapper();
      content["args"] = "yes";
      output.TalKeywordOptions.Add("content", content);

      // The 'batch' keyword option
      var batch = new EnumerableObjectWrapperWithNamedItems();
      batch["previous_sequence"] = false;
      batch["previous_sequence_start_item"] = "yes";
      batch["next_sequence"] = true;
      batch["next_sequence_start_item"] = "six";
      batch["next_sequence_end_item"] = "ten";
      var items = Enumerable
        .Range(1, 5)
        .Select(x => {
        var item = new NamedObjectWrapper();
        item["num"] = x.ToString();
        return item;
      })
        .ToArray();
      items[0].SetStringRepresentation("one");
      items[1].SetStringRepresentation("two");
      items[2].SetStringRepresentation("three");
      items[3].SetStringRepresentation("four");
      items[4].SetStringRepresentation("five");
      foreach(var item in items)
      {
        batch.Items.Add(item);
      }
      output.TalKeywordOptions.Add("batch", batch);

      // The 'laf' keyword option

      // The 'getProducts' option
      var getProducts = new [] {
        new NamedObjectWrapper(),
        new NamedObjectWrapper(),
      };
      getProducts[0]["description"] = "This is the tee for those who LOVE Zope. Show your heart on your tee.";
      getProducts[0]["image"] = "smlatee.jpg";
      getProducts[0]["price"] = 12.99m;
      getProducts[1]["description"] = "This is the tee for Jim Fulton. He's the Zope Pope!";
      getProducts[1]["image"] = "smpztee.jpg";
      getProducts[1]["price"] = 11.99m;
      output.TalKeywordOptions.Add("getProducts", getProducts);

      return output;
    }

    private IEnumerable<Tuple<FileInfo,FileInfo>> GetFilePairsToTest()
    {
      return (from expectedFile in _expectedPath.GetFiles()
              let filename = expectedFile.Name
              join sourceFile in _sourcePath.GetFiles() on
              filename equals sourceFile.Name
              select new Tuple<FileInfo,FileInfo>(sourceFile, expectedFile));
    }

    #endregion
  }
}

