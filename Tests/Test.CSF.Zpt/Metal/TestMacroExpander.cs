using System;
using NUnit.Framework;
using Moq;
using CSF.Zpt.Rendering;
using CSF.Zpt.Metal;
using Ploeh.AutoFixture;
using CSF.Zpt.TestUtils.Autofixture;
using CSF.Zpt;
using CSF.Zpt.TestUtils;

namespace Test.CSF.Zpt.Metal
{
  [TestFixture]
  public class TestMacroExpander
  {
    #region fields

    private IFixture _fixture;

    private Mock<IMacroFinder> _finder;
    private Mock<ISourceAnnotator> _annotator;

    private MacroExpander _sut;

    #endregion

    #region setup

    [SetUp]
    public void Setup()
    {
      _fixture = new Fixture();
      new RenderingContextCustomisation().Customize(_fixture);

      _finder = new Mock<IMacroFinder>();
      _annotator = new Mock<ISourceAnnotator>();

      _sut = new MacroExpander(_finder.Object, _annotator.Object);
    }

    #endregion

    #region tests

    [Test]
    public void GetUsedMacro_uses_macro_finder()
    {
      // Arrange
      var context = _fixture.Create<IRenderingContext>();
      var element = Mock.Of<IZptElement>();
      _finder.Setup(x => x.GetUsedMacro(context)).Returns(element);

      // Act
      var result = _sut.GetUsedMacro(context);

      // Assert
      Assert.AreSame(element, result);
      _finder.Verify(x => x.GetUsedMacro(context), Times.Once());
    }

    [Test]
    public void HandleNoUsedMacro_processes_annotation()
    {
      // Arrange
      var context = _fixture.Create<IRenderingContext>();
      _annotator.Setup(x => x.ProcessAnnotation(context));

      // Act
      _sut.HandleNoUsedMacro(context);

      // Assert
      _annotator.Verify(x => x.ProcessAnnotation(context), Times.Once());
    }

    [Test]
    public void HandleNoUsedMacro_does_not_change_context()
    {
      // Arrange
      var context = _fixture.Create<IRenderingContext>();

      // Act
      var result = _sut.HandleNoUsedMacro(context);

      // Assert
      Assert.AreSame(context, result);
    }

    [Test]
    public void Expand_returns_context_containing_replacement_element()
    {
      // Arrange
      var context = _fixture.Create<RenderingContext>();
      var attribute = Mock.Of<global::CSF.Zpt.Rendering.ZptAttribute>();
      ZptElement
        macro = Mock.Of<ZptElement>(x => x.GetAttribute(ZptConstants.Metal.Namespace,
                                                        ZptConstants.Metal.DefineMacroAttribute) == attribute
                                         && x.SearchChildrenByAttribute(It.IsAny<ZptNamespace>(),
                                                                        It.IsAny<string>()) == new ZptElement[0]);
      _finder.Setup(x => x.GetUsedMacro(context)).Returns(macro);
      Mock.Get(context.Element)
        .Setup(x => x.SearchChildrenByAttribute(It.IsAny<ZptNamespace>(), It.IsAny<string>()))
        .Returns(new ZptElement[0]);
      Mock.Get(context.Element)
        .Setup(x => x.ReplaceWith(macro))
        .Returns(macro);

      // Act
      var result = _sut.ExpandMacros(context);

      // Assert
      Assert.NotNull(result, "Result nullability");
      Assert.AreSame(macro, result.Element, "Correct result");
    }

    [Test]
    public void Expand_replaces_original_element_with_replacement_in_DOM()
    {
      // Arrange
      var context = _fixture.Create<RenderingContext>();
      var attribute = Mock.Of<global::CSF.Zpt.Rendering.ZptAttribute>();
      ZptElement
      macro = Mock.Of<ZptElement>(x => x.GetAttribute(ZptConstants.Metal.Namespace,
                                                        ZptConstants.Metal.DefineMacroAttribute) == attribute
                                    && x.SearchChildrenByAttribute(It.IsAny<ZptNamespace>(),
                                                                        It.IsAny<string>()) == new ZptElement[0]);
      _finder.Setup(x => x.GetUsedMacro(context)).Returns(macro);
      Mock.Get(context.Element)
        .Setup(x => x.SearchChildrenByAttribute(It.IsAny<ZptNamespace>(), It.IsAny<string>()))
        .Returns(new ZptElement[0]);
      Mock.Get(context.Element)
        .Setup(x => x.ReplaceWith(macro))
        .Returns(macro);

      // Act
      _sut.ExpandMacros(context);

      // Assert
      Mock.Get(context.Element).Verify(x => x.ReplaceWith(macro), Times.Once());
    }

    [Test]
    public void Expand_makes_no_changes_when_attribute_not_present()
    {
      // Arrange
      var context = _fixture.Create<RenderingContext>();
      _finder.Setup(x => x.GetUsedMacro(context)).Returns((ZptElement) null);

      // Act
      var result = _sut.ExpandMacros(context);

      // Assert
      Assert.NotNull(result, "Result nullability");
      Assert.AreSame(context, result, "Correct result");
      Mock.Get(context.Element).Verify(x => x.ReplaceWith(It.IsAny<ZptElement>()), Times.Never());
    }

    #endregion
  }
}

