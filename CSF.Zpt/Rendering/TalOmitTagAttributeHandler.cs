﻿using System;

namespace CSF.Zpt.Rendering
{
  /// <summary>
  /// Implementation of <see cref="ITalAttributeHandler"/> which handles a <c>tal:omit-tag</c> attribute.
  /// </summary>
  public class TalOmitTagAttributeHandler : ITalAttributeHandler
  {
    #region methods

    /// <summary>
    /// Handle the related attribute types which exist upon the element, if any.
    /// </summary>
    /// <returns>A collection of elements which are present in the DOM after this handler has completed its work.</returns>
    /// <param name="element">Element.</param>
    /// <param name="model">Model.</param>
    public ZptElement[] Handle(ZptElement element, Model model)
    {
      // TODO: Write this implementation
      throw new NotImplementedException();
    }

    #endregion
  }
}

