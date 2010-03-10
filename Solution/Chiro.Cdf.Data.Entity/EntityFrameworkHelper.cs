// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Linq;

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
				MemberExpression mexp = (MemberExpression)exp;
				CollectRelationalMembers(mexp.Expression, members);
				members.Add(new ExtendedPropertyInfo((PropertyInfo)mexp.Member));
			}
			else if (exp.NodeType == ExpressionType.Call)
			{
				MethodCallExpression cexp = (MethodCallExpression)exp;

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
			this.PropertyInfo = propertyInfo;
		}

		public PropertyInfo PropertyInfo { get; private set; }

		public bool NoUpdate { get; set; }

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ExtendedPropertyInfo);
		}

		public bool Equals(ExtendedPropertyInfo other)
		{
			if (Object.ReferenceEquals(other, null))
			{
				return false;
			}
			else
			{
				return (Object.Equals(this.PropertyInfo, other.PropertyInfo));
			}
		}

		public override int GetHashCode()
		{
			return this.GetType().GetHashCode() ^ this.PropertyInfo.GetHashCode();
		}
	}
}
