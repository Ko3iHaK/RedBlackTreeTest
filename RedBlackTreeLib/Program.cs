using System;
using System.IO;

namespace RedBlackTreeLib
{
    public static class Program
    {
        public static void Main()
        {
            var logger = new ConsoleLogger();
            var tracker = new FileChangeTracker<int>("operations.log");
            var tree = new RedBlackTree<int>(logger, tracker);

            int[] values = { 10, 20, 30, 15, 25 };
            foreach (int value in values)
            {
                tree.Insert(value);
                Console.WriteLine($"Вставлено: {value}");
            }

            Console.WriteLine("\nСтруктура дерева:");
            PrintTree(tree.Root, 0);
            Console.WriteLine("\nЛог операций сохранён в operations.log");
        }

        private static void PrintTree(Node<int> node, int indent)
        {
            if (node == null) return;
            Console.WriteLine($"{new string(' ', indent * 4)}[{node.Color}] {node.Key}");
            PrintTree(node.Left, indent + 1);
            PrintTree(node.Right, indent + 1);
        }
    }

    internal class ConsoleLogger : ILogger
    {
        public void Log(string message) => Console.WriteLine($"[LOG] {DateTime.Now:HH:mm:ss} {message}");
    }

    internal class FileChangeTracker<T> : ITreeChangeTracker<T> where T : IComparable<T>
    {
        private readonly string _filePath;

        public FileChangeTracker(string filePath)
        {
            _filePath = filePath;
            File.Delete(filePath);
        }

        public void TrackInsert(T key) => File.AppendAllText(_filePath, $"[INSERT] {key}\n");

        public void TrackRotation(string direction) => File.AppendAllText(_filePath, $"[ROTATE] {direction.ToUpper()}\n");

        public void TrackColorChange(Node<T> node) => File.AppendAllText(_filePath, $"[COLOR] {node.Key} -> {node.Color}\n");
    }
}