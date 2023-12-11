using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PathCalculator
{
    /// <summary>
    /// Calculate the path
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="start"></param>
    /// <param name="finish"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static List<NavGridPathNode> Calculate(NavGrid grid, NavGridPathNode start, NavGridPathNode finish, Vector2 min, Vector2 max)
    {
        List<NavGridPathNode> result = new List<NavGridPathNode>();
        List<NavGridPathNode> activeNodes = new List<NavGridPathNode>();
        List<NavGridPathNode> visitedNodes = new List<NavGridPathNode>();

        start.SetDistance(finish.X, finish.Y);
        activeNodes.Add(start);

        while (activeNodes.Any())
        {
            NavGridPathNode checkNode = activeNodes.OrderBy(x => x.CostDistance).First();

            // We found the destination and we can be sure (Because the the OrderBy above)
            if (checkNode.X == finish.X && checkNode.Y == finish.Y)
            {
                while (true)
                {
                    if (!checkNode.Occupied)
                    {
                        checkNode.Position = new Vector3(checkNode.X, 0, checkNode.Y);
                        result.Add(checkNode);
                    }

                    checkNode = checkNode.Parent;

                    if (checkNode == null)
                    {
                        return result;
                    }
                }
            }

            // Continue evaluating nodes
            visitedNodes.Add(checkNode);
            activeNodes.Remove(checkNode);
            List<NavGridPathNode> traversableNodes = GetTraversableNodes(grid, min, max, checkNode, finish);

            foreach (NavGridPathNode node in traversableNodes)
            {
                // We have already visited this NavGridPathNode so we don't need to do so again.
                if (visitedNodes.Any(x => x.X == node.X && x.Y == node.Y))
                {
                    continue;
                }

                // It's already in the active list, but that's ok, maybe this new NavGridPathNode has a better value (e.g. We might zigzag earlier but this is now straighter). 
                if (activeNodes.Any(x => x.X == node.X && x.Y == node.Y))
                {
                    NavGridPathNode existingNavGridPathNode = activeNodes.First(x => x.X == node.X && x.Y == node.Y);
                    
                    if (existingNavGridPathNode.CostDistance > checkNode.CostDistance)
                    {
                        activeNodes.Remove(existingNavGridPathNode);
                        activeNodes.Add(node);
                    }
                }
                else
                {
                    // We've never seen this NavGridPathNode before so add it to the list. 
                    activeNodes.Add(node);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Evaluate nodes to determine which are traversable
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="currentNode"></param>
    /// <param name="targetNode"></param>
    /// <returns></returns>
    private static List<NavGridPathNode> GetTraversableNodes(NavGrid grid, Vector2 min, Vector2 max, NavGridPathNode currentNode, NavGridPathNode targetNode)
    {
        // Add potential nodes for each node surrounding the current node
        List<NavGridPathNode> neighborNodes = new List<NavGridPathNode>()
        {
            new NavGridPathNode { X = currentNode.X, Y = currentNode.Y - 1, Parent = currentNode, Cost = currentNode.Cost + 1 },
            new NavGridPathNode { X = currentNode.X, Y = currentNode.Y + 1, Parent = currentNode, Cost = currentNode.Cost + 1 },
            new NavGridPathNode { X = currentNode.X + 1, Y = currentNode.Y - 1, Parent = currentNode, Cost = currentNode.Cost + 1 },
            new NavGridPathNode { X = currentNode.X + 1, Y = currentNode.Y + 1, Parent = currentNode, Cost = currentNode.Cost + 1 },
            new NavGridPathNode { X = currentNode.X + 1, Y = currentNode.Y, Parent = currentNode, Cost = currentNode.Cost + 1 },
            new NavGridPathNode { X = currentNode.X - 1, Y = currentNode.Y - 1, Parent = currentNode, Cost = currentNode.Cost + 1 },
            new NavGridPathNode { X = currentNode.X - 1, Y = currentNode.Y + 1, Parent = currentNode, Cost = currentNode.Cost + 1 },
            new NavGridPathNode { X = currentNode.X - 1, Y = currentNode.Y, Parent = currentNode, Cost = currentNode.Cost + 1 },
        };

        // Elimate anything outside the grid
        List<NavGridPathNode> legalNodes = neighborNodes.Where(possible => possible.X >= min.x && possible.X <= max.x)
                .Where(possible => possible.Y >= min.y && possible.Y <= max.y).ToList();

        // Eliminate occupied nodes
        List<NavGridPathNode> possibleNodes = legalNodes.Where(possible => grid.GetNodeByLocation(new Vector2(possible.X, possible.Y)) != null && grid.GetNodeByLocation(new Vector2(possible.X, possible.Y)).Occupied == false).ToList();
        
        possibleNodes.ForEach(possible => possible.SetDistance(targetNode.X, targetNode.Y));

        return possibleNodes;
    }
}