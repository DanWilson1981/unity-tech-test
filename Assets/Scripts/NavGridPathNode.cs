using UnityEngine;
using System;

[Serializable]
public class NavGridPathNode
{
    /// <summary>
    /// World position of the node
    /// </summary>
    public Vector3 Position;

    public bool Occupied;
    public int X { get; set; }
    public int Y { get; set; }
    public int Cost { get; set; }
    public int Distance { get; set; }
    public int CostDistance => Cost + Distance;
    public NavGridPathNode Parent { get; set; }

    public void SetDistance(int targetX, int targetY)
    {
        Distance = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
    }
}