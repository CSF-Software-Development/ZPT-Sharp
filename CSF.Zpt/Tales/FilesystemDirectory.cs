﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CSF.Zpt.Tales
{
  /// <summary>
  /// Object which represents a <c>System.IO.DirectoryInfo</c> - and implements <see cref="ITalesPathHandler"/> to
  /// wrap that directory in a TALES-friendly manner.
  /// </summary>
  public class FilesystemDirectory : ITalesPathHandler
  {
    #region fields

    private DirectoryInfo _directory;
    private bool _mandatoryExtensions;
    private ITemplateFileFactory _templateCreator;

    #endregion

    #region properties

    /// <summary>
    /// Gets a value indicating whether or not filename extensions are mandatory or not.
    /// </summary>
    /// <value><c>true</c> if extensions are mandatory; otherwise, <c>false</c>.</value>
    protected bool MandatoryExtensions
    {
      get {
        return _mandatoryExtensions;
      }
    }

    #endregion

    #region methods

    /// <summary>
    /// Gets an <c>System.Object</c> based upon a TALES path fragment.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method should return a <c>System.Object</c> which represents the traversal of a single-level TALES path
    /// fragment, from the current instance.  The value of that fragment is passed via the name
    /// <paramref name="pathFragment"/>.
    /// </para>
    /// <para>
    /// The precise meaning of 'traversal' is left to the implementation, but typical semantics will see an object
    /// return an associated object from an object graph.
    /// </para>
    /// <example>
    /// <para>
    /// In this simple example, the <c>Employee</c> class may return data from a related <c>Person</c> object, without
    /// exposing the Person object directly.  This might be because (as shown in this example), the API of that
    /// <c>Person</c> object is more complex than desired, and so TALES should see a simplified version.
    /// </para>
    /// <code>
    /// public class Employee : ITalesPathHandler
    /// {
    ///   private Person _person;
    ///   
    ///   public object HandleTalesPath(string pathFragment)
    ///   {
    ///     switch(pathFragment)
    ///     {
    ///     case: "name";
    ///       return _person.Name;
    ///     case: "address";
    ///       return _person.Address.FullAddress;
    ///     case: "gender":
    ///       return _person.Gender.ToString();
    ///     default:
    ///       return null;
    ///     }
    ///   }
    /// }
    /// </code>
    /// </example>
    /// <para>
    /// Note that the return value does not need to be a primitive type.  It may be a complex object, and the return
    /// value may also implement <see cref="ITalesPathHandler"/> if desired.
    /// </para>
    /// </remarks>
    /// <returns><c>true</c> if the path traversal was a success; <c>false</c> otherwise.</returns>
    /// <param name="pathFragment">The path fragment.</param>
    /// <param name="result">Exposes the result if the traversal was a success</param>
    public virtual bool HandleTalesPath(string pathFragment, out object result)
    {
      if(pathFragment == null)
      {
        throw new ArgumentNullException(nameof(pathFragment));
      }

      object output;
      var allInfos = _directory.GetFileSystemInfos().ToDictionary(k => k.Name, v => v);

      if(_mandatoryExtensions)
      {
        output = allInfos.ContainsKey(pathFragment)? allInfos[pathFragment] : null;
      }
      else
      {
        output = this.GetDirectoryOrFileWithoutExtension(pathFragment, allInfos);
      }

      if(output is DirectoryInfo)
      {
        var exposedDirectory = (DirectoryInfo) output;
        output = this.CreateChild(exposedDirectory);
      }
      else if(output is FileInfo)
      {
        var exposedTemplateFile = (FileInfo) output;
        if(_templateCreator.CanCreateFromFile(exposedTemplateFile))
        {
          output = _templateCreator.CreateTemplateFile(exposedTemplateFile);
        }
      }

      result = output;
      return output != null;
    }

    /// <summary>
    /// Creates a child instance of <see cref="FilesystemDirectory"/> for the given subdirectory.
    /// </summary>
    /// <returns>The child instance.</returns>
    /// <param name="directory">Directory.</param>
    protected virtual FilesystemDirectory CreateChild(DirectoryInfo directory)
    {
      return new FilesystemDirectory(directory, _mandatoryExtensions);
    }

    private FileSystemInfo GetDirectoryOrFileWithoutExtension(string pathFragment,
                                                              Dictionary<string,FileSystemInfo> allInfos)
    {
      FileSystemInfo output;

      if(allInfos.ContainsKey(pathFragment))
      {
        output = allInfos[pathFragment];
      }
      else
      {
        var nameWithExtension = String.Concat(pathFragment, ".");
        output = (from info in allInfos.Values
                  where info.Name.StartsWith(nameWithExtension)
                  select info)
          .FirstOrDefault();
      }

      return output;
    }

    #endregion

    #region constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CSF.Zpt.Tales.FilesystemDirectory"/> class.
    /// </summary>
    /// <param name="directory">The root directory for the current instance.</param>
    /// <param name="mandatoryExtensions">A value indicating whether filename extensions are mandatory or not.</param>
    /// <param name="templateCreator">An implementation of <see cref="ITemplateFileFactory"/>.</param>
    public FilesystemDirectory(DirectoryInfo directory,
                               bool mandatoryExtensions = false,
                               ITemplateFileFactory templateCreator = null)
    {
      if(directory == null)
      {
        throw new ArgumentNullException(nameof(directory));
      }

      _directory = directory;
      _mandatoryExtensions = mandatoryExtensions;
      _templateCreator = templateCreator?? new ZptDocumentFactory();
    }

    #endregion
  }
}

