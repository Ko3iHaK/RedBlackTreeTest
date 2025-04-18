using RedBlackTreeLib;
using Xunit;
using Moq;
using System;
using System.Linq;

namespace RedBlackTreeTests;

public class RedBlackTreeTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly Mock<ITreeChangeTracker<int>> _mockTracker;
    private readonly RedBlackTree<int> _tree;

    public RedBlackTreeTests()
    {
        _mockLogger = new Mock<ILogger>();
        _mockTracker = new Mock<ITreeChangeTracker<int>>();
        _tree = new RedBlackTree<int>(_mockLogger.Object, _mockTracker.Object);
    }

    // ------------------------- Основные операции -------------------------
    [Fact]
    public void Insert_ShouldMaintainRedBlackProperties()
    {
        // Arrange
        int[] values = { 10, 20, 30 };

        // Act
        foreach (var value in values)
            _tree.Insert(value);

        // Assert
        Assert.Equal(NodeColor.Black, _tree.Root.Color);
        Assert.Equal(20, _tree.Root.Key);
        Assert.Equal(10, _tree.Root.Left.Key);
        Assert.Equal(30, _tree.Root.Right.Key);
    }

    [Fact]
    public void Search_ExistingKey_ReturnsTrue()
    {
        // Arrange
        _tree.Insert(5);
        _tree.Insert(3);
        _tree.Insert(7);

        // Act & Assert
        Assert.True(_tree.Search(5));
        Assert.True(_tree.Search(3));
        Assert.True(_tree.Search(7));
    }

    // ------------------------- Логирование -------------------------
    [Fact]
    public void Logger_ShouldLogAllOperations()
    {
        // Act
        _tree.Insert(10);
        _tree.Insert(20);
        _tree.Search(15);

        // Assert
        _mockLogger.Verify(l => l.Log("Inserting 10"), Times.Once);
        _mockLogger.Verify(l => l.Log("Inserting 20"), Times.Once);
        _mockLogger.Verify(l => l.Log("Searching for 15"), Times.Once);
    }

    // ------------------------- Трекинг изменений -------------------------
    [Fact]
    public void Tracker_ShouldRecordAllEvents()
    {
        // Act
        _tree.Insert(10);
        _tree.Insert(20);
        _tree.Insert(30);

        // Assert
        _mockTracker.Verify(t => t.TrackInsert(10), Times.Once);
        _mockTracker.Verify(t => t.TrackInsert(20), Times.Once);
        _mockTracker.Verify(t => t.TrackRotation(It.IsAny<string>()), Times.AtLeastOnce);
        _mockTracker.Verify(t => t.TrackColorChange(It.IsAny<Node<int>>()), Times.AtLeast(2));
    }

    // ------------------------- Специальные случаи -------------------------
    [Fact]
    public void Insert_EmptyTree_ShouldCreateBlackRoot()
    {
        // Act
        _tree.Insert(42);

        // Assert
        Assert.Equal(42, _tree.Root.Key);
        Assert.Equal(NodeColor.Black, _tree.Root.Color);
        _mockTracker.Verify(t => t.TrackRotation(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Insert_ReverseSorted_ShouldTriggerRotations()
    {
        // Arrange
        int[] values = { 5, 4, 3, 2, 1 };

        // Act
        foreach (var value in values)
            _tree.Insert(value);

        // Assert
        // Проверка корня
        Assert.Equal(4, _tree.Root.Key);
        Assert.Equal(NodeColor.Black, _tree.Root.Color);

        // Проверка левого поддерева
        Assert.Equal(2, _tree.Root.Left.Key);
        Assert.Equal(NodeColor.Black, _tree.Root.Left.Color);

        // Проверка правого поддерева
        var right = _tree.Root.Right;
        Assert.Equal(5, right.Key);
        Assert.Equal(NodeColor.Black, right.Color);

        // Проверка вращений
        _mockTracker.Verify(t => t.TrackRotation("left"), Times.Never);
        _mockTracker.Verify(t => t.TrackRotation("right"), Times.Exactly(2));
    }

    [Fact]
    public void Insert_Duplicates_ShouldTrackAllInserts()
    {
        // Act
        _tree.Insert(3);
        _tree.Insert(3);
        _tree.Insert(3);

        // Assert
        _mockTracker.Verify(t => t.TrackInsert(3), Times.Exactly(3));
    }

    [Fact]
    public void Insert_NegativeNumbers_ShouldMaintainStructure()
    {
        // Arrange
        int[] values = { -5, -3, -1, 0, 2 };

        // Act
        foreach (var value in values)
            _tree.Insert(value);

        // Assert
        Assert.Equal(-3, _tree.Root.Key);
        Assert.Equal(NodeColor.Black, _tree.Root.Left.Color);
        Assert.Equal(NodeColor.Red, _tree.Root.Right.Left.Color);
    }

    // ------------------------- Проверка свойств -------------------------
    [Fact]
    public void AllPaths_ShouldHaveEqualBlackNodes()
    {
        // Arrange
        int[] values = { 10, 5, 15, 3, 7, 12, 20, 1 };

        // Act
        foreach (var value in values)
            _tree.Insert(value);

        // Assert
        int left = CountBlackNodes(_tree.Root.Left);
        int right = CountBlackNodes(_tree.Root.Right);
        Assert.Equal(left, right);
    }

    private int CountBlackNodes(Node<int> node)
    {
        if (node == null) return 1;
        int left = CountBlackNodes(node.Left);
        int right = CountBlackNodes(node.Right);
        Assert.Equal(left, right);
        return (node.Color == NodeColor.Black ? 1 : 0) + left;
    }

    // ------------------------- Стресс-тест -------------------------
    [Fact]
    public void LargeDataset_ShouldMaintainProperties()
    {
        // Arrange
        var rnd = new Random();
        int[] values = Enumerable.Range(1, 1000)
            .OrderBy(x => rnd.Next())
            .ToArray();

        // Act
        foreach (var value in values)
            _tree.Insert(value);

        // Assert
        Assert.Equal(NodeColor.Black, _tree.Root.Color);
        VerifyTreeProperties(_tree.Root);
    }

    private void VerifyTreeProperties(Node<int> node)
    {
        if (node == null) return;

        if (node.Color == NodeColor.Red)
        {
            Assert.Equal(NodeColor.Black, node.Left?.Color ?? NodeColor.Black);
            Assert.Equal(NodeColor.Black, node.Right?.Color ?? NodeColor.Black);
        }

        VerifyTreeProperties(node.Left);
        VerifyTreeProperties(node.Right);

        if (node.Left != null)
            Assert.True(node.Key.CompareTo(node.Left.Key) > 0);

        if (node.Right != null)
            Assert.True(node.Key.CompareTo(node.Right.Key) < 0);
    }

    // ------------------------- Граничные случаи -------------------------
    [Fact]
    public void Search_NonExistingKey_ReturnsFalse()
    {
        // Arrange
        _tree.Insert(5);
        _tree.Insert(3);

        // Act & Assert
        Assert.False(_tree.Search(7));
    }

    [Fact]
    public void Insert_SingleElement_ShouldNotRotate()
    {
        // Act
        _tree.Insert(100);

        // Assert
        Assert.Equal(100, _tree.Root.Key);
        _mockTracker.Verify(t => t.TrackRotation(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Insert_AlreadyBalanced_ShouldNotRotate()
    {
        // Arrange
        int[] values = { 4, 2, 6, 1, 3, 5, 7 };

        // Act
        foreach (var value in values)
            _tree.Insert(value);

        // Assert
        _mockTracker.Verify(t => t.TrackRotation(It.IsAny<string>()), Times.Never);
    }
}