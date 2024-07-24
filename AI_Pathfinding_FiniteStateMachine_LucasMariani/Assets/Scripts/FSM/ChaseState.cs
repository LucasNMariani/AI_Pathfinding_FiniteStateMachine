using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    Agent _agent;
    FiniteStateMachine _fsm;
    Vector3 _playerPos;
    Vector3 _velocity;
    private float _maxForce;
    private float _arriveRadius;

    public ChaseState(Agent a, FiniteStateMachine fsm, float maxForce, float arriveRadius)
    {
        _agent = a;
        _fsm = fsm;
        _maxForce = maxForce;
        _arriveRadius = arriveRadius;
    }

    public void OnStart()
    {
        Debug.Log(_agent.name + " " + _fsm.CurrentState);
    }

    public void OnUpdate()
    {
        Player player = GameManager.instance.player;
        _playerPos = player.GetPlayerPos();
        if (_agent.InFOV(player.gameObject))
        {
            AddForce(Arrive(_playerPos));

            _agent.transform.position += _velocity * Time.deltaTime;
            _agent.transform.forward = _velocity;
        }
        else
            _fsm.ChangeState(AgentStates.Patrol);
    }

    Vector3 Arrive(Vector3 target)
    {
        Vector3 desired = (target - _agent.transform.position);

        Vector3 distance = desired;

        if (distance.magnitude < _arriveRadius)
        {
            desired.Normalize();
            desired *= (_agent.speed / 2) * (distance.magnitude / _arriveRadius);
        }
        else
        {
            desired.Normalize();
            desired *= _agent.speed / 2;
        }

        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);

        return steering;
    }

    void AddForce(Vector3 force)
    {
        _velocity += force;
        _velocity = Vector3.ClampMagnitude(_velocity, _agent.speed);
    }

    public void OnExit()
    {

    }
}
