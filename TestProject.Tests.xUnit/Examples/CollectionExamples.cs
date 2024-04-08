namespace TestProject.Tests.xUnit.Examples;

class CollectionEquivalenceComparer<T> : IEqualityComparer<IEnumerable<T>>
    where T : IEquatable<T>
{
    public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
    {
        List<T> leftList = new (x);
        List<T> rightList = new (y);
        leftList.Sort();
        rightList.Sort();

        IEnumerator<T> enumeratorX = leftList.GetEnumerator();
        IEnumerator<T> enumeratorY = rightList.GetEnumerator();

        bool res;
        while (true)
        {
            bool hasNextX = enumeratorX.MoveNext();
            bool hasNextY = enumeratorY.MoveNext();

            if (!hasNextX || !hasNextY)
            {
                res = hasNextX == hasNextY;
                break;
            }

            if (!enumeratorX.Current.Equals(enumeratorY.Current))
            {
                res = false;
                break;
            }
        }

        enumeratorX.Dispose();
        enumeratorY.Dispose();
        
        return res;
    }

    public int GetHashCode(IEnumerable<T> obj) => throw new NotImplementedException();
}

public class CollectionExamples
{
    [Fact]
    public void CollectionEquality()
    {
        var left = new List<int>(new [] { 4, 12, 16, 27 });
        var right = new List<int>(new [] { 4, 12, 16, 27 });

        Assert.Equal(left, right, new CollectionEquivalenceComparer<int>());
    }

    [Fact]
    public void LeftCollectionSmallerThanRight()
    {
        var left = new List<int>(new int[] { 4, 12, 16 });
        var right = new List<int>(new int[] { 4, 12, 16, 27 });

        Assert.NotEqual(left, right, new CollectionEquivalenceComparer<int>());
    }

    [Fact]
    public void LeftCollectionLargerThanRight()
    {
        List<int> left = new List<int>(new int[] { 4, 12, 16, 27, 42 });
        List<int> right = new List<int>(new int[] { 4, 12, 16, 27 });

        Assert.NotEqual(left, right, new CollectionEquivalenceComparer<int>());
    }

    [Fact]
    public void SameValuesOutOfOrder()
    {
        List<int> left = new List<int>(new int[] { 4, 16, 12, 27 });
        List<int> right = new List<int>(new int[] { 4, 12, 16, 27 });

        Assert.Equal(left, right, new CollectionEquivalenceComparer<int>());
    }

    [Fact]
    public void DuplicatedItemInOneListOnly()
    {
        List<int> left = new List<int>(new int[] { 4, 16, 12, 27, 4 });
        List<int> right = new List<int>(new int[] { 4, 12, 16, 27 });

        Assert.NotEqual(left, right, new CollectionEquivalenceComparer<int>());
    }

    [Fact]
    public void DuplicatedItemInBothLists()
    {
        List<int> left = new List<int>(new int[] { 4, 16, 12, 27, 4 });
        List<int> right = new List<int>(new int[] { 4, 12, 16, 4, 27 });

        Assert.Equal(left, right, new CollectionEquivalenceComparer<int>());
    }

    [Fact]
    public void CollectionIsEmpty()
    {
        var empty = new List<int>();
        
        Assert.Empty(empty);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void CollectionContains(int expected)
    {
        var includes = new[] { 1, 2, 3, 4, 5 };
        Assert.Contains(expected, includes);
    }
}