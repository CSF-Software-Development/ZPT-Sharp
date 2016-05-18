﻿using System;

namespace CSF.Zpt.Rendering
{
  /// <summary>
  /// Null-object implementation of <see cref="SourceFileInfo"/>, representing an unknown source file.
  /// </summary>
  public class UnknownSourceFileInfo : SourceFileInfo
  {
    #region constants

    private const string NAME = "[Unknown source file]";

    #endregion

    #region fields

    private static UnknownSourceFileInfo _default;

    #endregion

    #region overrides

    /// <summary>
    /// Gets the filename of the current source file.
    /// </summary>
    /// <returns>The filename.</returns>
    public override string GetFullName()
    {
      return NAME;
    }

    /// <summary>
    /// Serves as a hash function for a <see cref="CSF.Zpt.Rendering.UnknownSourceFileInfo"/> object.
    /// </summary>
    /// <returns>
    /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
    /// hash table.
    /// </returns>
    public override int GetHashCode()
    {
      return 0;
    }

    /// <summary>
    /// Determines whether the specified <see cref="CSF.Zpt.Rendering.SourceFileInfo"/> is equal to the current
    /// <see cref="CSF.Zpt.Rendering.UnknownSourceFileInfo"/>.
    /// </summary>
    /// <param name="obj">
    /// The <see cref="CSF.Zpt.Rendering.SourceFileInfo"/> to compare with the current
    /// <see cref="CSF.Zpt.Rendering.UnknownSourceFileInfo"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="CSF.Zpt.Rendering.SourceFileInfo"/> is equal to the current
    /// <see cref="CSF.Zpt.Rendering.UnknownSourceFileInfo"/>; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(SourceFileInfo obj)
    {
      return Object.ReferenceEquals(this, obj);
    }

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CSF.Zpt.Rendering.UnknownSourceFileInfo"/> class.
    /// </summary>
    private UnknownSourceFileInfo() {}

    /// <summary>
    /// Initializes the <see cref="CSF.Zpt.Rendering.UnknownSourceFileInfo"/> class.
    /// </summary>
    static UnknownSourceFileInfo()
    {
      _default = new UnknownSourceFileInfo();
    }

    #endregion

    #region singleton

    /// <summary>
    /// Gets the default/singleton instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SourceFileInfo Instance { get { return _default; } }

    #endregion
  }
}

