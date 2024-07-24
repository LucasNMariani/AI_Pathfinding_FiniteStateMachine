using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    public List<Node> AStar(Node startingNode, Node goalNode)
    {
        PriorityQueue<Node> frontier = new PriorityQueue<Node>();
        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(startingNode, 0);

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();
            if (current == goalNode)
            {
                List<Node> path = new List<Node>();
                while (current != startingNode)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Add(startingNode);
                path.Reverse();
                return path;
            }

            foreach (var next in current.GetNeighbors())
            {
                int newCost = costSoFar[current] + next.cost;

                if (!costSoFar.ContainsKey(next))
                {
                    float priority = newCost + Vector3.Distance(next.transform.position, goalNode.transform.position);
                    costSoFar.Add(next, newCost);
                    frontier.Enqueue(next, priority);
                    cameFrom.Add(next, current);
                }
                else if (newCost < costSoFar[next])
                {
                    float priority = newCost + Vector3.Distance(next.transform.position, goalNode.transform.position);
                    costSoFar[next] = newCost;
                    cameFrom[next] = current;
                    frontier.Enqueue(next, priority);
                }

            }
        }
        return new List<Node>();
    }
}