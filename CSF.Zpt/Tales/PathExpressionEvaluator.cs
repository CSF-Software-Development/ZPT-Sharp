﻿using System;
using CSF.Zpt.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace CSF.Zpt.Tales
{
  /// <summary>
  /// Implementation of <see cref="ExpressionEvaluatorBase"/> which handles TALES path expressions.
  /// </summary>
  public class PathExpressionEvaluator : ExpressionEvaluatorBase
  {
    #region constants

    private static readonly string Prefix = "path";

    #endregion

    #region properties

    /// <summary>
    /// Gets the expression prefix handled by the current evaluator instance.
    /// </summary>
    /// <value>The prefix.</value>
    public override string ExpressionPrefix
    {
      get {
        return Prefix;
      }
    }

    #endregion

    #region methods

    /// <summary>
    /// Evaluate the specified expression, for the given element and model.
    /// </summary>
    /// <param name="expression">The expression to evaluate.</param>
    /// <param name="element">The <see cref="ZptElement"/> for which the expression is being evaluated.</param>
    /// <param name="model">The ZPT model, providing the context for evaluation.</param>
    public override ExpressionResult Evaluate(Expression expression, ZptElement element, TalesModel model)
    {
      if(expression == null)
      {
        throw new ArgumentNullException(nameof(expression));
      }
      if(element == null)
      {
        throw new ArgumentNullException(nameof(element));
      }
      if(model == null)
      {
        throw new ArgumentNullException(nameof(model));
      }

      var expressionText = expression.GetContent();
      var path = Path.Create(expressionText);
      var walker = new PathWalker(path);
      object output;

      try
      {
        output = this.WalkPath(walker, element, model);
      }
      catch(TraversalException ex)
      {
        string message = String.Format(Resources.ExceptionMessages.CouldNotWalkAnyPathsWithExpression,
                                       expressionText);
        throw new ModelEvaluationException(message, ex) {
          ExpressionText = expressionText
        };
      }

      return new ExpressionResult(output);
    }

    /// <summary>
    /// Walks and evaluates a TALES path.
    /// </summary>
    /// <returns>The evaluated value of the path.</returns>
    /// <param name="walker">A TALES path walker, containing a path.</param>
    /// <param name="element">The <see cref="ZptElement"/> for which the path is being evaluated.</param>
    /// <param name="model">The ZPT model, providing the context for evaluation.</param>
    private object WalkPath(PathWalker walker, ZptElement element, TalesModel model)
    {
      object output = null;
      bool success = false;

      while(walker.NextComponent() && !success)
      {
        success = this.WalkComponent(walker, element, model, out output);
      }

      if(!success)
      {
        throw new TraversalException(Resources.ExceptionMessages.CouldNotWalkAnyPaths);
      }

      return output;
    }

    /// <summary>
    /// Walks and evaluates a single TALES path component.
    /// </summary>
    /// <returns>
    /// <c>true</c>, if the path component was successfully evaluated, <c>false</c> otherwise.
    /// </returns>
    /// <param name="walker">A TALES path walker, containing a path.</param>
    /// <param name="element">The <see cref="ZptElement"/> for which the path is being evaluated.</param>
    /// <param name="model">The ZPT model, providing the context for evaluation.</param>
    /// <param name="result">Exposes the result of the evaluation.</param>
    private bool WalkComponent(PathWalker walker, ZptElement element, TalesModel model, out object result)
    {
      bool output;

      if(this.TryGetTraversalRoot(walker, element, model, out result))
      {
        object traversalChild;
        output = true;

        while(walker.NextPart())
        {
          string partName;

          if(this.TryGetPartName(walker.CurrentPart, element, model, out partName)
             && ObjectTraverser.Default.Traverse(result, partName, out traversalChild))
          {
            result = traversalChild;
          }
          else
          {
            output = false;
            result = null;
            break;
          }
        }
      }
      else
      {
        output = false;
        result = null;
      }

      return output;
    }

    /// <summary>
    /// Attempts to get a root object, from which to begin traversal of the path.
    /// </summary>
    /// <returns><c>true</c>, if the root object was retrieved, <c>false</c> otherwise.</returns>
    /// <param name="walker">A TALES path walker.</param>
    /// <param name="element">A ZPT element.</param>
    /// <param name="model">The TALES model.</param>
    /// <param name="result">Exposes the result of this operation.</param>
    protected virtual bool TryGetTraversalRoot(PathWalker walker,
                                               ZptElement element,
                                               TalesModel model,
                                               out object result)
    {
      bool output;

      if(walker.NextPart()
         && model.TryGetRootObject(walker.CurrentPart.Value, element, out result))
      {
        output = true;
      }
      else
      {
        output = false;
        result = null;
      }

      return output;
    }

    /// <summary>
    /// Attempts to get a part-name for traversal, expanding interpolated part-names if appropriate.
    /// </summary>
    /// <returns><c>true</c>, if a part name was resolved, <c>false</c> otherwise.</returns>
    /// <param name="part">A TALES path part.</param>
    /// <param name="element">A ZPT element.</param>
    /// <param name="model">The TALES model.</param>
    /// <param name="result">Exposes the result of this operation.</param>
    private bool TryGetPartName(PathPart part, ZptElement element, TalesModel model, out string result)
    {
      result = part.Value;
      bool output = true;

      if(part.IsInterpolated)
      {
        object interpolatedValue;

        if(model.TryGetRootObject(result, element, out interpolatedValue)
           && interpolatedValue != null)
        {
          output = true;
          result = interpolatedValue.ToString();
        }
        else
        {
          output = false;
          result = null;
        }
      }

      return output;
    }

    #endregion

    #region constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CSF.Zpt.Tales.PathExpressionEvaluator"/> class.
    /// </summary>
    /// <param name="registry">Registry.</param>
    public PathExpressionEvaluator(IEvaluatorRegistry registry) : base(registry) {}

    #endregion
  }
}

