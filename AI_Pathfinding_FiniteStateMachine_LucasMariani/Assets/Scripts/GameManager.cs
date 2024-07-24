using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathfindingType
{
    AStar,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player;

    private Dictionary<Agent,FiniteStateMachine> _agentsFSM = new Dictionary<Agent, FiniteStateMachine>();

    public List<Node> nodes = new List<Node>();

    public LayerMask wallMask;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void FindPlayer(Agent agentCalling)
    {
        foreach (var agent in _agentsFSM)
        {
            if (agent.Key != agentCalling)
            {
                agent.Key.GoToAlert(player.transform.position);
                agent.Value.ChangeState(AgentStates.Alert);
            }
        }
    }

    public void GetFSMAgents(Agent agent, FiniteStateMachine fsm)
    {
        if (!_agentsFSM.ContainsKey(agent)) _agentsFSM.Add(agent, fsm);
    }

    public bool InLineOfSight(Vector3 startPos, Vector3 endPos)
    {
        //origen, direccion, distancia, layerMask
        Vector3 dir = endPos - startPos;
        return !Physics.Raycast(startPos, dir, dir.magnitude, wallMask);
    }

}
