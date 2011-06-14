// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Chiro.Cdf.Data
{
	/// <summary>
	/// Represents a node in a tree structure and helps
	/// constructing and navigating trees based on paths.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TreeNode<T>
	{
		/// <summary>
		/// Constructs the root node of a tree.
		/// </summary>
		/// <param name="item"></param>
		public TreeNode(T item)
			: this(null, item)
		{ }

		/// <summary>
		/// Constructs a node in a tree.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="item"></param>
		public TreeNode(TreeNode<T> parent, T item)
		{
			Parent = parent;
			Item = item;
			Children = new HashSet<TreeNode<T>>();
		}

		/// <summary>
		/// The item held by the node.
		/// </summary>
		internal T Item { get; set; }

		/// <summary>
		/// The parent node. Null for the root node.
		/// </summary>
		internal TreeNode<T> Parent { get; set; }

		/// <summary>
		/// The children nodes of the current node.
		/// </summary>
		internal HashSet<TreeNode<T>> Children { get; set; }

		/// <summary>
		/// Adds a path of items to the current node, extending
		/// the tree if needed.
		/// </summary>
		/// <param name="path"></param>
		public void AddPath(IEnumerable<T> path)
		{
			// Start at the current node:
			TreeNode<T> node = this;
			bool extendingTree = false;

			// Walk path:
			foreach (T item in path)
			{
				TreeNode<T> itemNode = null;

				// Look for a matching node in the children of the parent node:
				if (!extendingTree)
				{
					T item1 = item;
					itemNode = node.Children.Where(p => p.Item.Equals(item1)).FirstOrDefault();
				}

				// Extend the tree with a new node if none found:
				if (itemNode == null)
				{
					itemNode = new TreeNode<T>(node, item);
					node.Children.Add(itemNode);
					extendingTree = true;
				}

				// Hop to child node for next iteration:
				node = itemNode;
			}
		}
	}
}
