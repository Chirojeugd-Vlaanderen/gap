/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace CodeProject.Data.Entity
{
	internal static class EntityFrameworkHelper
	{
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
					throw new InvalidOperationException("Invalid type of expression.");

				// Recursively handle arguments:
				foreach (var arg in cexp.Arguments)
					CollectRelationalMembers(arg, members);

				// Handle special marker method 'WithoutUpdate':
				if (cexp.Method.Name == "WithoutUpdate")
					members.Last().NoUpdate = true;

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

	internal class ExtendedPropertyInfo
	{
		public ExtendedPropertyInfo(PropertyInfo propertyInfo)
		{
			this.PropertyInfo = propertyInfo;
		}

		public PropertyInfo PropertyInfo { get; private set; }

		public bool NoUpdate { get; set; }

		public override bool Equals(object obj)
		{
			ExtendedPropertyInfo other = obj as ExtendedPropertyInfo;
			if (obj == null)
				return false;
			else
				return (Object.Equals(this.PropertyInfo, other.PropertyInfo));
		}

		public override int GetHashCode()
		{
			return this.GetType().GetHashCode() ^ this.PropertyInfo.GetHashCode();
		}
	}
}
