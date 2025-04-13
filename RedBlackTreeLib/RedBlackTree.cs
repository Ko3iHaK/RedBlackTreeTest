using System;

namespace RedBlackTreeLib;

public class RedBlackTree<T> where T : IComparable<T>
{
    public Node<T> Root { get; private set; }
    private readonly ILogger _logger;

    public RedBlackTree(ILogger logger = null)
    {
        _logger = logger;
    }

    public void Insert(T key)
    {
        _logger?.Log($"Inserting {key}");
        var newNode = new Node<T>(key);
        if (Root == null)
        {
            Root = newNode;
            Root.Color = NodeColor.Black;
            return;
        }

        Node<T> current = Root;
        Node<T> parent = null;
        while (current != null)
        {
            parent = current;
            current = key.CompareTo(current.Key) < 0 ? current.Left : current.Right;
        }

        newNode.Parent = parent;
        if (key.CompareTo(parent.Key) < 0)
            parent.Left = newNode;
        else
            parent.Right = newNode;

        FixInsertion(newNode);
    }

    private void FixInsertion(Node<T> node)
    {
        while (node != Root && node.Parent.Color == NodeColor.Red)
        {
            if (node.Parent == node.Parent.Parent.Left)
            {
                Node<T> uncle = node.Parent.Parent.Right;
                if (uncle?.Color == NodeColor.Red)
                {
                    // Случай 1: Дядя красный
                    node.Parent.Color = NodeColor.Black;
                    uncle.Color = NodeColor.Black;
                    node.Parent.Parent.Color = NodeColor.Red;
                    node = node.Parent.Parent; // Переходим к дедушке
                }
                else
                {
                    // Случаи 2 и 3: Дядя черный
                    if (node == node.Parent.Right)
                    {
                        node = node.Parent;
                        RotateLeft(node);
                    }
                    node.Parent.Color = NodeColor.Black;
                    node.Parent.Parent.Color = NodeColor.Red;
                    RotateRight(node.Parent.Parent);
                }
            }
            else
            {
                // Симметричный случай
                Node<T> uncle = node.Parent.Parent.Left;
                if (uncle?.Color == NodeColor.Red)
                {
                    node.Parent.Color = NodeColor.Black;
                    uncle.Color = NodeColor.Black;
                    node.Parent.Parent.Color = NodeColor.Red;
                    node = node.Parent.Parent;
                }
                else
                {
                    if (node == node.Parent.Left)
                    {
                        node = node.Parent;
                        RotateRight(node);
                    }
                    node.Parent.Color = NodeColor.Black;
                    node.Parent.Parent.Color = NodeColor.Red;
                    RotateLeft(node.Parent.Parent);
                }
            }
        }
        Root.Color = NodeColor.Black;
    }

    private void RotateLeft(Node<T> node)
    {
        Node<T> rightChild = node.Right;
        node.Right = rightChild.Left;
        if (rightChild.Left != null)
            rightChild.Left.Parent = node;
        rightChild.Parent = node.Parent;
        if (node.Parent == null)
            Root = rightChild;
        else if (node == node.Parent.Left)
            node.Parent.Left = rightChild;
        else
            node.Parent.Right = rightChild;
        rightChild.Left = node;
        node.Parent = rightChild;
    }

    private void RotateRight(Node<T> node)
    {
        Node<T> leftChild = node.Left;
        node.Left = leftChild.Right;
        if (leftChild.Right != null)
            leftChild.Right.Parent = node;
        leftChild.Parent = node.Parent;
        if (node.Parent == null)
            Root = leftChild;
        else if (node == node.Parent.Right)
            node.Parent.Right = leftChild;
        else
            node.Parent.Left = leftChild;
        leftChild.Right = node;
        node.Parent = leftChild;
    }

    public bool Search(T key)
    {
        _logger?.Log($"Searching for {key}");
        Node<T> current = Root;
        while (current != null)
        {
            int cmp = key.CompareTo(current.Key);
            if (cmp == 0) return true;
            current = cmp < 0 ? current.Left : current.Right;
        }
        return false;
    }
}