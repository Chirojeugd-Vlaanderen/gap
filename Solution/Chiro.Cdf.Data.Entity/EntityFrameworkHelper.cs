// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Chiro.Gap.Dummies")]

namespace Chiro.Cdf.Data.Entity
{
	internal static class EntityFrameworkHelper
	{
		/// <summary>
		/// Deze method bepaalt alle (sub)property's waar de lambda-expressie <paramref name="exp"/>
		/// betrekking op heeft, en voegt die toe aan de lijst <paramref name="members"/>.
		/// </summary>
		/// <param name="exp">1 'path' bepaald door een lambda-expressie</param>
		/// <param name="members">Lijst waar de gevonden properties aan toegevoegd moeten worden</param>
		internal static void CollectRelationalMembers(Expression exp, IList<ExtendedPropertyInfo> members)
		{
			if (exp.NodeType == ExpressionType.Lambda)
			{
				// At root, handle body:
				CollectRelationalMembers(((LambdaExpression)exp).Body, members);
			}
			else if (exp.NodeType == ExpressionType.MemberAccess)
			{
				// Add expression property to collected members and handle remainder of expression:
				var mexp = (MemberExpression)exp;
				CollectRelationalMembers(mexp.Expression, members);
				members.Add(new ExtendedPropertyInfo((PropertyInfo)mexp.Member));
			}
			else if (exp.NodeType == ExpressionType.Call)
			{
				var cexp = (MethodCallExpression)exp;

				// Only static (extension) methods with 1 argument are supported:
				if (cexp.Method.IsStatic == false || cexp.Arguments.Count != 1)
				{
					throw new InvalidOperationException("Invalid type of expression.");
				}

				// Recursively handle arguments:
				foreach (var arg in cexp.Arguments)
				{
					CollectRelationalMembers(arg, members);
				}

				// Handle special marker method 'WithoutUpdate':
				if (cexp.Method.Name == "WithoutUpdate")
				{
					members.Last().NoUpdate = true;
				}
			}
			else if (exp.NodeType == ExpressionType.Parameter)
			{
				// Reached the toplevel:
				return;
			}
			else
			{
				throw new InvalidOperationException("Invalid type of expression.");
			}
		}
	}

	/// <summary>
	/// Deze klasse wordt gebruikt om 'propertybomen' op te stellen op basis van lambda-expressies 
	/// </summary>
	internal class ExtendedPropertyInfo : IEquatable<ExtendedPropertyInfo>
	{
		public ExtendedPropertyInfo(PropertyInfo propertyInfo)
		{
			PropertyInfo = propertyInfo;
		}

		public PropertyInfo PropertyInfo { get; private set; }

		public bool NoUpdate { get; set; }

		/// <summary>
		/// Vergelijkt het huidige object met een ander om te zien of het over
		/// twee instanties van hetzelfde object gaat
		/// </summary>
		/// <param name="obj">Het object waarmee we het huidige willen vergelijken</param>
		/// <returns><c>True</c> als het schijnbaar om twee instanties van hetzelfde object gaat</returns>
		public override bool Equals(object obj)
		{
			return Equals(obj as ExtendedPropertyInfo);
		}

		public bool Equals(ExtendedPropertyInfo other)
		{
		    if (ReferenceEquals(other, null))
			{
				return false;
			}
		    return (Equals(PropertyInfo, other.PropertyInfo));
		}

	    /// <summary>
		/// Een waarde waarmee we het object kunnen identificeren
		/// </summary>
		/// <returns>Een int waarmee we het object kunnen identificeren</returns>
		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ PropertyInfo.GetHashCode();
		}
	}
}
