using System;
using System.Collections.Generic;
using UnityEngine;


    public sealed class SpatialHashGrid3D
    {
        public readonly float CellSize;
        private readonly float invCellSize;

        // Key: hashed cell coord -> list of indices in that cell
        private readonly Dictionary<long, List<int>> buckets = new(1024);
        private readonly Stack<List<int>> pool = new(); // reuse lists, avoid GC

        public SpatialHashGrid3D(float cellSize)
        {
            CellSize = Mathf.Max(0.0001f, cellSize);
            invCellSize = 1f / CellSize;
        }

        public void Clear()
        {
            // Return lists to pool, keep dictionary capacity
            foreach (var kv in buckets)
            {
                kv.Value.Clear();
                pool.Push(kv.Value);
            }
            buckets.Clear();
        }

        public void Insert(int index, Vector3 position)
        {
            var cell = WorldToCell(position);
            long key = Hash(cell.x, cell.y, cell.z);

            if (!buckets.TryGetValue(key, out var list))
            {
                list = pool.Count > 0 ? pool.Pop() : new List<int>(8);
                buckets[key] = list;
            }
            list.Add(index);
        }

        // Enumerate indices in the 27 neighboring cells (including the cell itself)
        public void ForEachNeighbor(Vector3 position, Action<int> visitor)
        {
            var c = WorldToCell(position);

            for (int dz = -1; dz <= 1; dz++)
            for (int dy = -1; dy <= 1; dy++)
            for (int dx = -1; dx <= 1; dx++)
            {
                long key = Hash(c.x + dx, c.y + dy, c.z + dz);
                if (!buckets.TryGetValue(key, out var list)) continue;

                for (int i = 0; i < list.Count; i++)
                    visitor(list[i]);
            }
        }

        private Vector3Int WorldToCell(Vector3 p)
        {
            // floor gives stable cells for negative coordinates too
            return new Vector3Int(
                Mathf.FloorToInt(p.x * invCellSize),
                Mathf.FloorToInt(p.y * invCellSize),
                Mathf.FloorToInt(p.z * invCellSize)
            );
        }

        // 3D integer hash to a single long. Deterministic and fast.
        private static long Hash(int x, int y, int z)
        {
            // Mix bits using large primes, then pack into long-ish hash space
            // (Collisions are possible in any hash; dictionary handles them.)
            unchecked
            {
                long h = 1469598103934665603L; // FNV offset basis
                h = (h ^ x) * 1099511628211L;
                h = (h ^ y) * 1099511628211L;
                h = (h ^ z) * 1099511628211L;
                return h;
            }
        }
    }
