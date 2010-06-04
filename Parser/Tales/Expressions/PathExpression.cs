//  
//  PathExpression.cs
//  
//  Author:
//       Craig Fowler <craig@craigfowler.me.uk>
// 
//  Copyright (c) 2010 Craig Fowler
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace CraigFowler.Web.ZPT.Tales.Expressions
{
  /// <summary>
  /// <para>Represents a <see cref="TalesExpression"/> that represents a 'path' type expression.</para>
  /// </summary>
  public class PathExpression : TalesExpression
  {
    #region constants
    
    private const char PATH_SEPARATOR   = '|';
    public const string Prefix          = "path:";
    
    #endregion
    
    #region fields
    
    private List<TalesPath> paths;
    
    #endregion
    
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets a queue of the paths that are present in the expression body of this expression.</para>
    /// <para>Independant paths are searated by the pipe character '|'.</para>
    /// </summary>
    public List<TalesPath> Paths
    {
      get {
        return paths;
      }
      private set {
        paths = value;
      }
    }
    
    #endregion
    
    #region methods
    
    public override object GetValue()
    {
      bool success = false;
      object output = null;
      
      for(int i = 0; !success && i < this.Paths.Count; i++)
      {
        try
        {
          output = EvaluatePath(this.Paths[i]);
          success = true;
        }
        catch(Exception)
        {
          success = false;
        }
      }
      
      if(!success)
      {
        throw new FormatException("Could not evaluate any of the given paths.");
      }
      
      return output;
    }
    
    #endregion
    
    #region private methods
    
    /// <summary>
    /// <para>Extracts a <see cref="List"/> of <see cref="TalesPath"/> from the given string.</para>
    /// </summary>
    /// <param name="expression">
    /// A <see cref="System.String"/>
    /// </param>
    /// <returns>
    /// A <see cref="List<TalesPath>"/>
    /// </returns>
    private List<TalesPath> ExtractPaths(string expression)
    {
      List<TalesPath> output = new List<TalesPath>();
      
      foreach(string path in expression.Split(new char[] {PATH_SEPARATOR}))
      {
        output.Add(new TalesPath(path));
      }
      
      return output;
    }
    
    /// <summary>
    /// <para>Overloaded. Evaluates a single <see cref="TalesPath"/> to an object reference.</para>
    /// </summary>
    /// <param name="path">
    /// A <see cref="TalesPath"/>
    /// </param>
    /// <returns>
    /// A <see cref="System.Object"/>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// If the <paramref name="path"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If the <see cref="TalesPath.Parts.Count"/> of the parts in the <paramref name="path"/> is zero.
    /// </exception>
    /// <exception cref="TalesException">
    /// If there i a problem evaluating the <paramref name="path"/>.  This could be caused by a problem in fetching
    /// the root object reference from the <see cref="TalesExpression.Context"/> or if an unknown (or null) reference
    /// is followed whilst traversing the path.
    /// </exception>
    private object EvaluatePath(TalesPath path)
    {
      object rootReference, output;
      
      if(path == null)
      {
        throw new ArgumentNullException("path");
      }
      else if(path.Parts.Count == 0)
      {
        throw new ArgumentOutOfRangeException("path", "The path has no parts.");
      }
      
      try
      {
        rootReference = this.Context.GetRootObject(path.Parts[0]);
      }
      catch(ArgumentException ex)
      {
        TalesException talesException;
        talesException = new TalesException("Could not fetch the root object of the path from the current context",
                                            ex);
        talesException.Data.Add("path", path);
        talesException.Data.Add("context", this.Context);
        throw talesException;
      }
      
      if(path.Parts.Count == 1)
      {
        output = rootReference;
      }
      else
      {
        output = EvaluatePath(path, 1, rootReference);
      }
      
      return output;
    }
    
    /// <summary>
    /// <para>Overloaded.  Evaluates a portion of a <see cref="TalesPath"/> to an object reference.</para>
    /// </summary>
    /// <param name="path">
    /// A <see cref="TalesPath"/>
    /// </param>
    /// <param name="position">
    /// A <see cref="System.Int32"/>
    /// </param>
    /// <param name="parentObject">
    /// A <see cref="System.Object"/>
    /// </param>
    /// <returns>
    /// A <see cref="System.Object"/>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// If the <paramref name="parentObject"/> is null and this is not the last step in traversing the
    /// <paramref name="path"/>.
    /// </exception>
    private object EvaluatePath(TalesPath path, int position, object parentObject)
    {
      object output = null, thisObject = null;
      MemberInfo currentMember;
      
      if(position < path.Parts.Count)
      {
        currentMember = SelectMember(parentObject, path.Parts[position]);
        
        switch(currentMember.MemberType)
        {
        case MemberTypes.Property:
          PropertyInfo property = currentMember as PropertyInfo;
          if(property.CanRead)
          {
            thisObject = InvokeMethod(property.GetGetMethod(), parentObject, path, ref position);
          }
          else
          {
            TalesException ex = new TalesException("Cannot traverse a non-readable property.");
            ex.Data.Add("member", currentMember);
            ex.Data.Add("target type", parentObject.GetType());
            throw ex;
          }
          break;
        case MemberTypes.Method:
          MethodInfo method = currentMember as MethodInfo;
          thisObject = InvokeMethod(method, parentObject, path, ref position);
          break;
        case MemberTypes.Field:
          FieldInfo field = currentMember as FieldInfo;
          if(field.IsPublic)
          {
            thisObject = field.GetValue(parentObject);
          }
          else
          {
            TalesException ex = new TalesException("Cannot traverse a non-readable field.");
            ex.Data.Add("member", currentMember);
            ex.Data.Add("target type", parentObject.GetType());
            throw ex;
          }
          break;
        default:
          TalesException ex = new TalesException("The member type encountered whilst traversing the path expression " +
                                                 "is not supported.");
          ex.Data.Add("member", currentMember);
          ex.Data.Add("target type", parentObject.GetType());
          throw ex;
        }
        
        // And now we recurse into ourself, traversing another piece of the path each time we go.
        position ++;
        output = EvaluatePath(path, position, thisObject);
      }
      else
      {
        output = parentObject;
      }
      
      return output;
    }
    
    /// <summary>
    /// <para>
    /// Selects a member from a <paramref name="containingObject"/> based on its <see cref="MemberInfo.Name"/>.
    /// </para>
    /// </summary>
    /// <param name="containingObject">
    /// A <see cref="System.Object"/>
    /// </param>
    /// <param name="memberIdentifier">
    /// A <see cref="System.String"/>
    /// </param>
    /// <returns>
    /// A <see cref="MemberInfo"/>
    /// </returns>
    private MemberInfo SelectMember(object containingObject, string memberIdentifier)
    {
      MemberInfo[] members;
      Type containingType;
      
      // Quick sanity-check for some impossible situations
      if(containingObject == null)
      {
        throw new ArgumentNullException("containingObject");
      }
      else if(String.IsNullOrEmpty(memberIdentifier))
      {
        throw new ArgumentException("Member identifier may not be null or empty.", "memberIdentifier");
      }
      
      containingType = containingObject.GetType();
      members = containingType.GetMember(memberIdentifier);
      
      if(members.Length == 0)
      {
        TalesException ex = new TalesException("No members of the target type match the given identifier.");
        ex.Data.Add("identifier", memberIdentifier);
        ex.Data.Add("target type", containingType);
        throw ex;
      }
      else if(members.Length != 1)
      {
        TalesException ex = new TalesException("Ambiguous reference to more than one member of the target type.");
        ex.Data.Add("identifier", memberIdentifier);
        ex.Data.Add("target type", containingType);
        throw ex;
      }
      
      return members[0];
    }
    
    /// <summary>
    /// <para>Invokes the given method with parameters (if applicable) and returns the value.</para>
    /// </summary>
    /// <param name="method">
    /// A <see cref="MethodInfo"/>
    /// </param>
    /// <param name="targetObject">
    /// A <see cref="System.Object"/>
    /// </param>
    /// <param name="path">
    /// A <see cref="TalesPath"/>
    /// </param>
    /// <param name="basePosition">
    /// A <see cref="System.Int32"/>
    /// </param>
    /// <returns>
    /// A <see cref="System.Object"/>
    /// </returns>
    private object InvokeMethod(MethodInfo method, object targetObject, TalesPath path, ref int basePosition)
    {
      object[] parameterValues = new object[method.GetParameters().Length];
      
      if(method.ReturnType == typeof(void))
      {
        ArgumentOutOfRangeException ex;
        ex = new ArgumentOutOfRangeException("Cannot invoke and traverse a method with a void return type.");
        ex.Data.Add("method name", method.Name);
        ex.Data.Add("target type", targetObject.GetType());
        throw ex;
      }
      
      // Extract all of the parameter information from the path (where applicable)
      for(int i = 0; i < parameterValues.Length; i++)
      {
        if(basePosition +1 >= path.Parts.Count)
        {
          IndexOutOfRangeException ex;
          ex = new IndexOutOfRangeException("Parameters to the given method require more path pieces than are " +
                                            "available.");
          ex.Data.Add("parameter count", parameterValues.Length);
          ex.Data.Add("method name", method.Name);
          ex.Data.Add("target type", targetObject.GetType());
          throw ex;
        }
        else
        {
          basePosition ++;
          parameterValues[i] = path.Parts[basePosition];
        }
      }
      
      return method.Invoke(targetObject, parameterValues);
    }
    
    #endregion
    
    #region constructor
    
    internal PathExpression(string expression, TalesContext context) : base(expression, context)
    {
      this.Paths = ExtractPaths(ExpressionBody);
    }
    
    #endregion
  }
}
