using RedBlackTreeLib;
using Xunit;
using Moq;

namespace RedBlackTreeTests;

public class RedBlackTreeTests
{
    [Fact]
    public void Insert_ShouldMaintainRedBlackProperties()
    {
        var tree = new RedBlackTree<int>();
        tree.Insert(10);
        tree.Insert(20);
        tree.Insert(30);

        Assert.Equal(NodeColor.Black, tree.Root.Color);
        Assert.Equal(20, tree.Root.Key);
        Assert.Equal(10, tree.Root.Left.Key);
        Assert.Equal(30, tree.Root.Right.Key);
        Assert.Equal(NodeColor.Red, tree.Root.Left.Color);
        Assert.Equal(NodeColor.Red, tree.Root.Right.Color);
    }

    [Fact]
    public void Search_ExistingKey_ReturnsTrue()
    {
        var tree = new RedBlackTree<int>();
        tree.Insert(5);
        tree.Insert(3);
        tree.Insert(7);

        Assert.True(tree.Search(5));
        Assert.True(tree.Search(3));
        Assert.True(tree.Search(7));
    }

    [Fact]
    public void Insert_ShouldCallLogger()
    {
        var mockLogger = new Mock<ILogger>();
        var tree = new RedBlackTree<int>(mockLogger.Object);
        tree.Insert(10);
        mockLogger.Verify(l => l.Log("Inserting 10"), Times.Once);
    }

    [Fact]
    public void Insert_DuplicateKey_ShouldAddToRightSubtree()
    {
        var tree = new RedBlackTree<int>();
        tree.Insert(10);
        tree.Insert(10);

        Assert.NotNull(tree.Root);
        Assert.Null(tree.Root.Left);
        Assert.Equal(10, tree.Root.Right.Key);
        Assert.Equal(NodeColor.Red, tree.Root.Right.Color);
    }

    [Fact]
    public void Insert_LeftLeftCase_ShouldRebalanceCorrectly()
    {
        var tree = new RedBlackTree<int>();
        tree.Insert(30);
        tree.Insert(20);
        tree.Insert(10); // ¬ызывает правое вращение

        Assert.Equal(20, tree.Root.Key);
        Assert.Equal(10, tree.Root.Left.Key);
        Assert.Equal(30, tree.Root.Right.Key);
        Assert.Equal(NodeColor.Black, tree.Root.Color);
    }

    [Fact]
    public void Insert_RightRightCase_ShouldRebalanceCorrectly()
    {
        var tree = new RedBlackTree<int>();
        tree.Insert(10);
        tree.Insert(20);
        tree.Insert(30);

        Assert.Equal(20, tree.Root.Key);
        Assert.Equal(10, tree.Root.Left.Key);
        Assert.Equal(30, tree.Root.Right.Key);
    }

    [Fact]
    public void Insert_LeftRightCase_ShouldRebalanceCorrectly()
    {
        var tree = new RedBlackTree<int>();
        tree.Insert(30);
        tree.Insert(10);
        tree.Insert(20);

        Assert.Equal(20, tree.Root.Key);
        Assert.Equal(10, tree.Root.Left.Key);
        Assert.Equal(30, tree.Root.Right.Key);
    }

    [Fact]
    public void Search_NonExistingKey_ReturnsFalse()
    {
        var tree = new RedBlackTree<int>();
        tree.Insert(5);
        tree.Insert(3);

        Assert.False(tree.Search(7));
    }

    [Fact]
    public void Root_AlwaysBlack_AfterOperations()
    {
        var tree = new RedBlackTree<int>();
        tree.Insert(10);
        tree.Insert(20);
        tree.Insert(30);

        Assert.Equal(NodeColor.Black, tree.Root.Color);
    }

    [Fact]
    public void Insert_AllElements_ShouldMaintainBlackDepth()
    {
        var tree = new RedBlackTree<int>();
        tree.Insert(10);
        tree.Insert(5);
        tree.Insert(15);
        tree.Insert(3);
        tree.Insert(7);

        // ѕроверка количества черных узлов на всех пут€х
        int leftPathBlackCount = CountBlackNodes(tree.Root.Left);
        int rightPathBlackCount = CountBlackNodes(tree.Root.Right);
        Assert.Equal(leftPathBlackCount, rightPathBlackCount);
    }

    private int CountBlackNodes(Node<int> node)
    {
        if (node == null) return 1; // NIL-узлы считаютс€ черными
        int left = CountBlackNodes(node.Left);
        int right = CountBlackNodes(node.Right);
        Assert.Equal(left, right);
        return (node.Color == NodeColor.Black ? 1 : 0) + left;
    }

    [Fact]
    public void Insert_ShouldCallLoggerForEachOperation()
    {
        var mockLogger = new Mock<ILogger>();
        var tree = new RedBlackTree<int>(mockLogger.Object);

        tree.Insert(10);
        tree.Insert(20);
        tree.Insert(30);

        mockLogger.Verify(l => l.Log("Inserting 10"), Times.Once);
        mockLogger.Verify(l => l.Log("Inserting 20"), Times.Once);
        mockLogger.Verify(l => l.Log("Inserting 30"), Times.Once);
    }

    [Fact]
    public void Search_ShouldCallLoggerIfProvided()
    {
        var mockLogger = new Mock<ILogger>();
        var tree = new RedBlackTree<int>(mockLogger.Object);
        tree.Insert(10);

        bool result = tree.Search(10);

        mockLogger.Verify(l => l.Log("Searching for 10"), Times.Once);
    }

    [Fact]
    public void FixInsertion_UncleIsRed_ShouldRecolor()
    {
        // слава богу отработал.
        var tree = new RedBlackTree<int>();

        // ѕостроение дерева:
        //       20(B)
        //      /   \
        //    10(R) 30(R)
        tree.Insert(20);
        tree.Insert(10);
        tree.Insert(30);

        // ¬ставл€ем 5: д€д€ (30) красный -> перекрашивание
        tree.Insert(5);

        // ќжидаема€ структура после перекрашивани€:
        //       20(B)
        //      /   \
        //    10(B) 30(B)
        //    /
        //   5(R)

        Assert.Equal(20, tree.Root.Key);
        Assert.Equal(NodeColor.Black, tree.Root.Color);
        Assert.Equal(NodeColor.Black, tree.Root.Left.Color);
        Assert.Equal(NodeColor.Black, tree.Root.Right.Color);
        Assert.Equal(NodeColor.Red, tree.Root.Left.Left.Color);
    }

    [Fact]
    public void RotateLeft_ShouldUpdateParentReferences()
    {
        var tree = new RedBlackTree<int>();
        tree.Insert(10);
        tree.Insert(20);
        tree.Insert(30);

        var rightChild = tree.Root.Right;
        Assert.Equal(30, rightChild.Key);
        Assert.Equal(tree.Root, rightChild.Parent);
    }
}