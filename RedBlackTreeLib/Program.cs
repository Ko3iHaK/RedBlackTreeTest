using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RedBlackTreeLib;

var tree = new RedBlackTree<int>();
tree.Insert(10);
tree.Insert(20);
tree.Insert(30);
Console.WriteLine("Корень дерева: " + tree.Root.Key); // Выведет 20
