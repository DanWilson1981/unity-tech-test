using UnityEngine;
using System.Collections.Generic;

public class NavGrid : MonoBehaviour
{
    private int _gridWidth = 50;
    private int _gridDepth = 50;
    private Dictionary<Vector2, NavGridPathNode> _nodes;
    private List<NavGridPathNode> _currentPath;
    private List<GameObject> _pathVisuals;
    private Transform _objectHolder;

    private void Awake()
    {
        _pathVisuals = new List<GameObject>();
        _objectHolder = new GameObject("Holder").transform;
        SetupGrid();
        AddBlockingObjects();
    }

    /// <summary>
    /// Given the current and desired location, return a path to the destination
    /// </summary>
    public NavGridPathNode[] GetPath(Vector3 origin, Vector3 destination)
    {
        NavGridPathNode start = new NavGridPathNode();
        start.X = Mathf.CeilToInt(origin.x);
        start.Y = Mathf.CeilToInt(origin.z);
        start.Position = new Vector3(start.X, 0, start.Y);
        start.Occupied = false;

        NavGridPathNode finish = new NavGridPathNode();
        finish.X = Mathf.CeilToInt(destination.x);
        finish.Y = Mathf.CeilToInt(destination.z);
        finish.Position = new Vector3(finish.X, 0, finish.Y);
        finish.Occupied = false;

        _currentPath = PathCalculator.Calculate(this, start, finish, new Vector2(_gridWidth / -2.0F, _gridDepth / -2.0F), new Vector2(_gridWidth, _gridDepth));
        _currentPath.Reverse();
        DrawPath();

        return _currentPath.ToArray();
    }

    public NavGridPathNode GetNodeByLocation(Vector2 location)
    {
        NavGridPathNode result = null;

        if(_nodes.ContainsKey(location))
        {
            result = _nodes[location];
        }

        return result;
    }

    private void SetupGrid()
    {
        _nodes = new Dictionary<Vector2, NavGridPathNode>();

        for(int x = 0; x < _gridWidth; ++x)
        {
            for(int d = 0; d < _gridDepth; ++d)
            {
                Vector2 vec = new Vector2(x - (_gridWidth / 2.0F), d - (_gridDepth / 2.0F));
                NavGridPathNode node = new NavGridPathNode();
                node.Position = new Vector3(vec.x, 0, vec.y);
                node.Occupied = false;
                node.X = (int)vec.x;
                node.Y = (int)vec.y;
                _nodes.Add(vec, node);
            }
        }
    }

    private void AddBlockingObjects()
    {
        float total = _nodes.Count * Random.Range(0.1F, 0.15F); // 10% - 15% blocking nodes
        List<Vector2> locations = new List<Vector2>();

        foreach(Vector2 vec in _nodes.Keys)
        {
            locations.Add(vec);
        }

        Utilities.RandomizeList<Vector2>(locations);

        for(int i = 0; i < total; ++i)
        {
            Transform t = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            t.position = new Vector3(locations[i].x, 0, locations[i].y);
            t.localScale = Vector3.one * 0.5F;
            t.SetParent(_objectHolder);
            _nodes[locations[i]].Occupied = true;
        }
    }

    private void DrawPath()
    {
        ClearVisuals();

        foreach (NavGridPathNode a in _currentPath)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = a.Position;
            obj.transform.localScale = Vector3.one * 0.25F;
            obj.transform.SetParent(_objectHolder);
            obj.GetComponent<MeshRenderer>().material.color = Color.green;
            _pathVisuals.Add(obj);
        }
    }

    private void ClearVisuals()
    {
        foreach (GameObject obj in _pathVisuals)
        {
            Destroy(obj);
        }

        _pathVisuals.Clear();
    }
}