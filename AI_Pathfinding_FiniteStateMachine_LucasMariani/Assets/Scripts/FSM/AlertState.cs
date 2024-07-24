using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : IState
{
    Agent _agent;
    FiniteStateMachine _fsm;
    Vector3 _playerPosAlert;

    public AlertState(Agent a, FiniteStateMachine fsm)
    {
        _agent = a;
        _fsm = fsm;
    }

    public void OnStart()
    {
        Debug.Log(_agent.name + " " + _fsm.CurrentState);
        _playerPosAlert = _agent.GetPlayerPosAlert;

        if (!_agent.InLineOfSight(_agent.transform.position, _playerPosAlert) && _agent.PathToFollowCount() == 0)
        {
            Node _nearNodeToPlayerPos = _agent.allWaypoints[_agent._currentWaypoint];
            foreach (var node in GameManager.instance.nodes)
            {
                if (!_agent.InLineOfSight(node.transform.position, _playerPosAlert))
                    continue;
                Vector3 dist = _playerPosAlert - node.transform.position;
                dist.y = 0;
                Vector3 nearNodeDist = _playerPosAlert - _nearNodeToPlayerPos.transform.position;
                nearNodeDist.y = 0;
                if (dist.magnitude < nearNodeDist.magnitude)
                {
                    _nearNodeToPlayerPos = node;
                }
                else continue;
            }
            if(!_agent.InLineOfSight(_agent.transform.position, _agent.allWaypoints[_agent._currentWaypoint].transform.position))
                _agent.GetPath(_agent.NearNodeToAgent(), _nearNodeToPlayerPos); 
            else
                _agent.GetPath(_agent.allWaypoints[_agent._currentWaypoint], _nearNodeToPlayerPos); //Antes se hacía solo desde el _currentWaypoint por eso traspasaba
        }
    }

    public void OnUpdate()
    {
        if (_agent.PathToFollowCount() == 0)
        {
            if (_agent.InLineOfSight(_agent.transform.position, _playerPosAlert))
            {
                Vector3 dir = _playerPosAlert - _agent.transform.position;
                dir.y = 0;
                _agent.transform.forward = dir;
                _agent.transform.position += _agent.transform.forward * _agent.speed * Time.deltaTime;
                if (dir.magnitude <= 0.3f)
                {
                    _fsm.ChangeState(AgentStates.Patrol);
                }
            }
        }
        else if (_agent.PathToFollowCount() > 0)
            _agent.FollowPath();
        
        if (_agent.InFOV(GameManager.instance.player.gameObject))
        {
            _fsm.ChangeState(AgentStates.Chase);
        }
    }

    public void OnExit()
    {
        _agent.ClearPathToFollow();
    }
}
