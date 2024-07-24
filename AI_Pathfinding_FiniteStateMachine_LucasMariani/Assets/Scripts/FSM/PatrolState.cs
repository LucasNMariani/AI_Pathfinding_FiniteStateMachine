using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    Agent _agent;
    FiniteStateMachine _fsm;
    int _dir = 1;

    public PatrolState(Agent a, FiniteStateMachine fsm)
    {
        _agent = a;
        _fsm = fsm;
    }

    public void OnStart()
    {
        Debug.Log(_agent.name + " " + _fsm.CurrentState + "| LineOfSight result: " + _agent.InLineOfSight(_agent.transform.position, _agent.allWaypoints[_agent._currentWaypoint].transform.position));
        if (!_agent.InLineOfSight(_agent.transform.position, _agent.allWaypoints[_agent._currentWaypoint].transform.position))
        {
            //Node _nearNode = _agent.allWaypoints[_agent._currentWaypoint];
            //foreach (var node in GameManager.instance.nodes)
            //{
            //    if (!_agent.InLineOfSight(_agent.transform.position, node.transform.position))
            //        continue;
            //    Vector3 dist = node.transform.position - _agent.transform.position;
            //    dist.y = 0;
            //    Vector3 nearNodeDist = _nearNode.transform.position - _agent.transform.position;
            //    nearNodeDist.y = 0;
            //    if (dist.magnitude < nearNodeDist.magnitude)
            //    {
            //        _nearNode = node;
            //    }
            //    else continue;
            //}
            _agent.GetPath(_agent.NearNodeToAgent(), _agent.allWaypoints[_agent._currentWaypoint]);//Antes se calculaba acá y se pasaba el _nearNode
        }
    }

    public void OnUpdate()
    {
        if (_agent.InLineOfSight(_agent.transform.position, _agent.allWaypoints[_agent._currentWaypoint].transform.position) && _agent.PathToFollowCount() == 0)
            Patrol();
        else if(_agent.PathToFollowCount() > 0)
            _agent.FollowPath();

        if (_agent.InFOV(GameManager.instance.player.gameObject))
        {
            GameManager.instance.FindPlayer(_agent);
            _fsm.ChangeState(AgentStates.Chase);
        }
    }

    void Patrol()
    {
        Transform waypoint = _agent.allWaypoints[_agent._currentWaypoint].transform; 
        Vector3 dir = waypoint.position - _agent.transform.position;
        dir.y = 0;

        _agent.transform.forward = dir;
        _agent.transform.position += _agent.transform.forward * _agent.speed * Time.deltaTime;

        if (dir.magnitude <= 0.3f)
        {
            _agent._currentWaypoint += _dir;
            if (_agent._currentWaypoint > _agent.allWaypoints.Count - 1 || _agent._currentWaypoint < 0)
            {
                _agent._currentWaypoint = 0;
            }
        }
    }

    public void OnExit()
    {
        _agent.ClearPathToFollow();
    }
}
