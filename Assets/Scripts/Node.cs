using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField]
    private List<Node> _neighbors = new List<Node>();
    public int cost = 1;

    public List<Node> GetNeighbors()
    {
        if (_neighbors.Count == 0)
            return null;
        return _neighbors;
    }


    private void OnMouseOver()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            SetCost(cost + 1);
        if (Input.GetKey(KeyCode.DownArrow))
            SetCost(cost - 1);
        if (Input.GetKeyDown(KeyCode.R))
            SetCost(1);
    }

    void SetCost(int c)
    {
        cost = Mathf.Clamp(c, 1, 99);
    }

}
