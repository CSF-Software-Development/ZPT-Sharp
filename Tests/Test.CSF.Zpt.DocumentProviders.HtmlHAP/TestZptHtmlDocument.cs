﻿using System;
using NUnit.Framework;
using CSF.Zpt.DocumentProviders;
using CSF.Zpt.Rendering;
using CSF.Zpt;
using CSF.Zpt.Metal;

namespace Test.CSF.Zpt.DocumentProviders.HtmlHAP
{
  [TestFixture]
  public class TestZptHtmlDocument
  {
    #region sample document 1

    // This is a real document, taken from CSF.Screenplay
    const string SampleDocument1 = @"<html>
<head>
<title>Screenplay report</title>
<style tal:content=""here/RenderedStyles | nothing""></style>
<style tal:condition=""nothing"">
@import url(reset.css);
@import url(page.css);
@import url(reports.css);
</style>
<script tal:content=""here/RenderedScripts | nothing""></script>
<script tal:condition=""nothing"" src=""""></script>
</head>
<body tal:define=""report here/Report"">
<header>
  <h1>Screenplay report</h1>
</header>
<section class=""report""
         tal:define=""features report/Features;
                     template here/Template;
                     myMacros csharp:template.GetMacros();"">
  <ul class=""features"">
    <li tal:condition=""nothing"" class=""feature"">
      <header class=""success"">
        <div class=""identifier"">Another.Feature.Identifier</div>
        <h2 class=""name"">Another feature name</h2>
      </header>
      <ul class=""scenarios"">
        <li class=""scenario"">
          <header class=""success"">
            <div class=""identifier"">Feature2.Scenario1.Identifier.Here</div>
            <h3 class=""name"">A scenario in a different feature</h3>
          </header>
          <ol class=""reportables"">
            <li class=""reportable"">
              <!-- The base macro for a reportable, we never use this directly, only extend it -->
              <div metal:define-macro=""reportable""
                   tal:define=""outcomeClass csharp:here.GetOutcomeClass(reportable);
                               reportableClass csharp:here.GetReportableClass(reportable);
                               containerClass string:$outcomeClass $reportableClass""
                   tal:attributes=""class containerClass""
                   class=""performance success"">
                <p class=""description""
                   metal:define-slot=""description""
                   tal:define=""actor reportable/Actor"">
                  <strong tal:condition=""rootReportable""
                          tal:content=""reportable/PerformanceType""
                          class=""performance_type"">Given</strong>
                  <span metal:define-slot=""body"" class=""body"">the body of this reportable</span>
                </p>
                <div class=""additional_content""
                     metal:define-slot=""additional_content""
                     tal:condition=""nothing""
                     style=""display: none;"">
                  Additional content
                </div>
              </div>
            </li>
            <li class=""reportable"">
              <!-- The macro for an ability -->
              <div metal:extend-macro=""reportable""
                   metal:define-macro=""ability""
                   class=""ability success"">
                <p class=""description"">
                  <strong class=""performance_type"">Given</strong>
                  <span metal:fill-slot=""body"" class=""body""
                        tal:define=""ability reportable/Ability""
                        tal:content=""csharp:ability.GetReport(actor)"">Joe is able to browse the web</span>
                </p>
              </div>
            </li>
            <li class=""reportable"">
              <!-- The base macro for a performance, it can be used directly or extended -->
              <div metal:extend-macro=""reportable""
                   metal:define-macro=""performance""
                   class=""performance success"">
                <p class=""description"">
                  <strong class=""performance_type"">When</strong>
                  <span metal:fill-slot=""body"" class=""body""
                        tal:define=""performable reportable/Performable""
                        tal:content=""csharp:performable.GetReport(actor)"">Joe attempts to do a thing</span>
                </p>
                <div class=""additional_content""
                     metal:fill-slot=""additional_content""
                     tal:define=""performable reportable/Performable"">
                  <div class=""extended_content""
                       metal:define-slot=""extended_content""
                       tal:condition=""nothing""
                       style=""display: none;"">
                    Extended content
                  </div>
                  <ol tal:define=""reportables performable/Reportables;
                                  rootReportable csharp:false""
                      class=""reportables""
                      tal:condition=""reportables""
                      metal:define-slot=""child_reportables"">
                    <li tal:repeat=""reportable reportables"" class=""reportable"">
                      <tal:block define=""macroName csharp:here.GetMacroName(reportable);
                                         macro load:myMacros/?macroName""
                                 replace=""structure macro | nothing"">
                        <p>Child reportable content</p>
                      </tal:block>
                    </li>
                  </ol>
                </div>
              </div>
            </li>
            <li class=""reportable"">
              <!-- The macro for a performance which has a result -->
              <div metal:extend-macro=""performance""
                   metal:define-macro=""performance_success_result""
                   class=""performance success"">
                <p class=""description"">
                  <strong class=""performance_type"">Then</strong>
                  <span class=""body"">Joe reads some kind of value</span>
                </p>
                <div class=""additional_content"">
                  <div class=""extended_content performance_result""
                       metal:fill-slot=""extended_content"">
                    <p>
                      <strong>Result</strong>
                      <span tal:replace=""csharp:here.Format(performable.Result)"">The performance result</span>
                    </p>
                  </div>
                </div>
              </div>
            </li>
            <li class=""reportable"">
              <!-- The macro for a performance which has failed with an exception -->
              <div metal:extend-macro=""performance""
                   metal:define-macro=""performance_failure_exception""
                   class=""performance failure"">
                <p class=""description"">
                  <strong class=""performance_type"">Then</strong>
                  <span class=""body"">Joe does something that fails</span>
                </p>
                <div class=""additional_content"">
                  <div class=""extended_content performance_exception""
                       metal:fill-slot=""extended_content"">
                    <p>Failed with an <strong>exception</strong></p>
                    <pre tal:content=""performable/Exception"">Exception
  at My.Funky.Stack.Trace.Line1 &gt; Something
  at My.Funky.Stack.Trace.Line2 &gt; Something else
  at My.Funky.Stack.Trace.Line3 &gt; Something different</pre>
                  </div>
                </div>
              </div>
            </li>
          </ol>
        </li>
      </ul>
    </li>
    <li tal:repeat=""feature features"" class=""feature"">
      <header tal:define=""outcome csharp:here.GetOutcomeClass(feature)""
              tal:attributes=""class outcome""
              class=""success"">
        <div class=""identifier""
             tal:condition=""feature/Identifier | nothing""
             tal:content=""feature/Identifier"">Feature.Identifier.Here</div>
        <h2 class=""name"" tal:content=""feature/FriendlyName"">This is where the feature name goes</h2>
      </header>
      <ul class=""scenarios"" tal:define=""scenarios feature/Scenarios"">
        <li tal:repeat=""scenario scenarios"" class=""scenario"">
          <header tal:define=""outcome csharp:here.GetOutcomeClass(scenario)""
                  tal:attributes=""class outcome""
                  class=""success"">
            <div class=""identifier""
                 tal:condition=""scenario/Id | nothing""
                 tal:content=""scenario/Id"">Scenario.Identifier.Here</div>
            <h3 class=""name"" tal:content=""scenario/FriendlyName"">My very interesting scenario</h3>
          </header>
          <ol class=""reportables""
              tal:define=""reportables scenario/Reportables;
                          rootReportable csharp:true"">
            <li tal:repeat=""reportable reportables"" class=""reportable"">
              <div tal:define=""macroName csharp:here.GetMacroName(reportable);
                               macro load:myMacros/?macroName""
                   tal:replace=""structure macro | nothing"">
                <p class=""description"">
                  <strong class=""performance_type"">Given</strong>
                  <span class=""body"">Reportable content</span>
                </p>
              </div>
            </li>
          </ol>
        </li>
        <li class=""scenario"" tal:condition=""nothing"">
          <header class=""success"">
            <div class=""identifier"">Feature1.Scenario2.Identifier.Here</div>
            <h3 class=""name"">A second scenario in the feature</h3>
          </header>
          <ol class=""reportables"">
            <li class=""reportable"">
              <div>
                <p class=""description"">
                  <strong class=""performance_type"">Given</strong>
                  <span class=""body"">Reportable content</span>
                </p>
              </div>
            </li>
          </ol>
        </li>
        <li class=""scenario"" tal:condition=""nothing"">
          <header class=""success"">
            <div class=""identifier"">Feature1.Scenario3.Identifier.Here</div>
            <h3 class=""name"">A third scenario in the feature</h3>
          </header>
          <ol class=""reportables"">
            <li class=""reportable"">
              <div>
                <p class=""description"">
                  <strong class=""performance_type"">Given</strong>
                  <span class=""body"">Reportable content</span>
                </p>
              </div>
            </li>
          </ol>
        </li>
      </ul>
    </li>
  </ul>
</section>
<footer tal:define=""timestamp csharp:report.Timestamp.ToString(&quot;T&quot;);
                    datestamp csharp:report.Timestamp.ToString(&quot;D&quot;)"">
  <p>
    This report was created at
    <span tal:content=""timestamp"" class=""timestamp"">12:40:33</span>
    on
    <span class=""datestamp"" tal:content=""datestamp"">2011-03-01</span>
    using <a href=""https://github.com/csf-dev/CSF.Screenplay"">CSF.Screenplay</a>.
  </p>
</footer>
</body>
</html>
";
    
    #endregion

    #region sample document 2

    // This sample document has a macro with an empty name
    const string SampleDocument2 = @"<html>
<body>
<div metal:define-macro="""">Foo</div>
</body>
</html>";

    #endregion

    [Test]
    public void GetMacros_should_not_raise_an_exception_with_a_valid_document()
    {
      // Arrange
      var doc = GetSampleDocument(SampleDocument1);
      IMetalMacroContainer result = null;

      // Act
      Assert.DoesNotThrow(() => result = doc.GetMacros(), "Getting macros does not raise an exception");

      // Assert
      Assert.NotNull(result, "Result is not null");
    }

    [Test]
    public void GetMacros_should_not_raise_an_exception_with_an_empty_macro_name()
    {
      // Arrange
      var doc = GetSampleDocument(SampleDocument2);
      IMetalMacroContainer result = null;

      // Act
      Assert.DoesNotThrow(() => result = doc.GetMacros(), "Getting macros does not raise an exception");

      // Assert
      Assert.NotNull(result, "Result is not null");
    }

    IZptDocument GetSampleDocument(string document)
    {
      var htmlDoc = new HtmlAgilityPack.HtmlDocument();
      htmlDoc.LoadHtml(document);
      return new ZptHtmlDocument(htmlDoc, UnknownSourceFileInfo.Instance);
    }
  }
}
