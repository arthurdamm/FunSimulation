using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
// using Library.SpatialHash;

public class SpatialHashGrid3DTests
{
    [Test]
    public void Constructor_ClampsCellSizeToPositiveMinimum()
    {
        var grid = new SpatialHashGrid3D(0f);

        Assert.That(grid.CellSize, Is.GreaterThan(0f));
        Assert.That(grid.CellSize, Is.EqualTo(0.0001f).Within(0.0000001f));
    }

    [Test]
    public void Insert_AndQuery_SameCell_FindsInsertedIndex()
    {
        var grid = new SpatialHashGrid3D(10f);
        grid.Insert(7, new Vector3(1f, 2f, 3f));

        var found = new List<int>();
        grid.ForEachNeighbor(new Vector3(1f, 2f, 3f), found.Add);

        Assert.That(found, Has.Count.EqualTo(1));
        Assert.That(found, Contains.Item(7));
    }

    [Test]
    public void Insert_MultipleItemsInSameCell_FindsAll()
    {
        var grid = new SpatialHashGrid3D(10f);

        grid.Insert(1, new Vector3(1f, 1f, 1f));
        grid.Insert(2, new Vector3(2f, 2f, 2f));
        grid.Insert(3, new Vector3(9.999f, 0f, 0f)); // still same cell as 0..9.999

        var found = new List<int>();
        grid.ForEachNeighbor(Vector3.zero, found.Add);

        Assert.That(found, Has.Count.EqualTo(3));
        Assert.That(found, Does.Contain(1));
        Assert.That(found, Does.Contain(2));
        Assert.That(found, Does.Contain(3));
    }

    [Test]
    public void Query_AdjacentCell_FindsNeighborBecause27CellsAreSearched()
    {
        var grid = new SpatialHashGrid3D(10f);

        // Query position will be in cell (0,0,0)
        // Insert into adjacent x cell (1,0,0)
        grid.Insert(42, new Vector3(10.1f, 0f, 0f));

        var found = new List<int>();
        grid.ForEachNeighbor(Vector3.zero, found.Add);

        Assert.That(found, Contains.Item(42));
    }

    [Test]
    public void Query_DiagonalAdjacentCell_FindsNeighbor()
    {
        var grid = new SpatialHashGrid3D(10f);

        // Insert in (1,1,1), query from (0,0,0)
        grid.Insert(99, new Vector3(10.1f, 10.1f, 10.1f));

        var found = new List<int>();
        grid.ForEachNeighbor(Vector3.zero, found.Add);

        Assert.That(found, Contains.Item(99));
        Assert.That(found, Does.Contain(99));
    }

    [Test]
    public void Query_CellOutside27Neighborhood_DoesNotFindFarItem()
    {
        var grid = new SpatialHashGrid3D(10f);

        // Query near (0,0,0), insert in cell (2,0,0), which should be excluded
        grid.Insert(5, new Vector3(20.1f, 0f, 0f));

        var found = new List<int>();
        grid.ForEachNeighbor(Vector3.zero, found.Add);

        // Assert.That(found, Does.Not.Contain(5));
        Assert.That(found, Does.Not.Contains(5));
    }

    [Test]
    public void Clear_RemovesAllItems()
    {
        var grid = new SpatialHashGrid3D(10f);

        grid.Insert(1, Vector3.zero);
        grid.Insert(2, new Vector3(10.1f, 0f, 0f));

        grid.Clear();

        var found = new List<int>();
        grid.ForEachNeighbor(Vector3.zero, found.Add);

        Assert.That(found, Is.Empty);
    }

    [Test]
    public void NegativeCoordinates_AreBucketedCorrectly()
    {
        var grid = new SpatialHashGrid3D(10f);

        // -0.1 floors to cell -1, not 0
        grid.Insert(11, new Vector3(-0.1f, 0f, 0f));

        var foundNearNegative = new List<int>();
        grid.ForEachNeighbor(new Vector3(-0.1f, 0f, 0f), foundNearNegative.Add);

        Assert.That(foundNearNegative, Contains.Item(11));
    }

    [Test]
    public void NegativeAndPositiveSides_AcrossOrigin_AreStillNeighborsWhenCellsTouchQueryNeighborhood()
    {
        var grid = new SpatialHashGrid3D(10f);

        // -0.1 => cell -1
        grid.Insert(1, new Vector3(-0.1f, 0f, 0f));

        // query at +0.1 => cell 0, and ForEachNeighbor checks -1..1
        var found = new List<int>();
        grid.ForEachNeighbor(new Vector3(0.1f, 0f, 0f), found.Add);

        Assert.That(found, Contains.Item(1));
    }

    [Test]
    public void PositionExactlyOnCellBoundary_GoesToNextCell()
    {
        var grid = new SpatialHashGrid3D(10f);

        // x=10 goes to cell 1, so querying from cell 0 neighborhood should still find it
        grid.Insert(77, new Vector3(10f, 0f, 0f));

        var found = new List<int>();
        grid.ForEachNeighbor(Vector3.zero, found.Add);

        Assert.That(found, Contains.Item(77));
    }

    [Test]
    public void QueryOnBoundary_StillSearchesExpectedNeighborhood()
    {
        var grid = new SpatialHashGrid3D(10f);

        // Query at x=10 is cell 1. Item in cell 0 should still be found.
        grid.Insert(88, new Vector3(9.999f, 0f, 0f));

        var found = new List<int>();
        grid.ForEachNeighbor(new Vector3(10f, 0f, 0f), found.Add);

        Assert.That(found, Contains.Item(88));
    }

    [Test]
    public void ForEachNeighbor_CallsVisitorOncePerInsertedIndex()
    {
        var grid = new SpatialHashGrid3D(10f);

        grid.Insert(1, Vector3.zero);
        grid.Insert(2, Vector3.zero);
        grid.Insert(3, new Vector3(10.1f, 0f, 0f));

        int callCount = 0;
        var found = new HashSet<int>();

        grid.ForEachNeighbor(Vector3.zero, index =>
        {
            callCount++;
            found.Add(index);
        });

        Assert.That(callCount, Is.EqualTo(3));
        Assert.That(found.SetEquals(new[] { 1, 2, 3 }), Is.True);
    }

    [Test]
    public void Clear_ThenReuseGrid_WorksCorrectly()
    {
        var grid = new SpatialHashGrid3D(10f);

        grid.Insert(1, Vector3.zero);
        grid.Clear();
        grid.Insert(2, Vector3.zero);

        var found = new List<int>();
        grid.ForEachNeighbor(Vector3.zero, found.Add);

        Assert.That(found, Has.Count.EqualTo(1));
        Assert.That(found, Does.Contain(2));
        Assert.That(found, Does.Not.Contains(1));
    }
    
    [Test]
    public void SpatialHash_WithDistanceFilter_ReturnsOnlyActualCloseNeighbors()
    {
        var grid = new SpatialHashGrid3D(10f);

        Vector3 query = Vector3.zero;
        float radius = 3f;
        float radiusSq = radius * radius;

        grid.Insert(1, new Vector3(1f, 0f, 0f));   // close
        grid.Insert(2, new Vector3(2f, 2f, 0f));   // close
        grid.Insert(3, new Vector3(8f, 0f, 0f));   // same/adjacent cell maybe, but not actually close

        var positions = new Dictionary<int, Vector3>
        {
            [1] = new Vector3(1f, 0f, 0f),
            [2] = new Vector3(2f, 2f, 0f),
            [3] = new Vector3(8f, 0f, 0f),
        };

        var actualNeighbors = new List<int>();

        grid.ForEachNeighbor(query, index =>
        {
            if ((positions[index] - query).sqrMagnitude <= radiusSq)
                actualNeighbors.Add(index);
        });

        Assert.That(actualNeighbors, Has.Count.EqualTo(2));
        Assert.That(actualNeighbors, Does.Contain(1));
        Assert.That(actualNeighbors, Does.Contain(2));
        Assert.That(actualNeighbors, Does.Not.Contains(3));
    }
}