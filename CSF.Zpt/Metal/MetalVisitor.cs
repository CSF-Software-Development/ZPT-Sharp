﻿using System;
using System.Linq;
using System.Collections.Generic;
using CSF.Zpt.Rendering;

namespace CSF.Zpt.Metal
{
  /// <summary>
  /// Implementation of <see cref="IContextVisitor"/> which performs METAL-related functionality.
  /// </summary>
  public class MetalVisitor : ContextVisitorBase
  {
    #region fields

    private MacroExpander _macroExpander;

    #endregion

    #region methods

    /// <summary>
    /// Visit the given context and return a collection of the resultant contexts.
    /// </summary>
    /// <returns>Zero or more <see cref="IRenderingContext"/> instances, determined by the outcome of this visit.</returns>
    /// <param name="context">The rendering context to visit.</param>
    public override IRenderingContext[] Visit(IRenderingContext context)
    {
      if(context == null)
      {
        throw new ArgumentNullException(nameof(context));
      }

      IZptAttribute attrib;
      if((attrib = context.GetMetalAttribute(ZptConstants.Metal.DefineMacroAttribute)) != null)
      {
        context.MetalModel.AddGlobal(attrib.Value, context.Element);
      }

      return new [] { _macroExpander.Expand(context) };
    }

    #endregion

    #region constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CSF.Zpt.Metal.MetalVisitor"/> class.
    /// </summary>
    public MetalVisitor() : this(null) {}

    /// <summary>
    /// Initializes a new instance of the <see cref="CSF.Zpt.Metal.MetalVisitor"/> class.
    /// </summary>
    /// <param name="expander">The macro expander to use.</param>
    public MetalVisitor(MacroExpander expander = null)
    {
      _macroExpander = expander?? new MacroExpander();
    }

    #endregion
  }
}

