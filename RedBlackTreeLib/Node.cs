using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace RedBlackTreeLib;

public enum NodeColor { Red, Black }

public class Node<T> where T : IComparable<T>
{
    public T Key { get; set; }
    public NodeColor Color { get; set; }
    public Node<T> Left { get; set; }
    public Node<T> Right { get; set; }
    public Node<T> Parent { get; set; }

    public Node(T key)
    {
        Key = key;
        Color = NodeColor.Red;
    }
}
