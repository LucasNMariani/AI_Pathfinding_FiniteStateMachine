using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private FiniteStateMachine _fsm;
    Pathfinding _pf = new Pathfinding();
    private List<Node> _pathToFollow = new List<Node>();

    public List<Node> allWaypoints = new List<Node>();
    public int _currentWaypoint = 0;

    public float viewRadius;
    public float viewAngle;

    private Vector3 _playerPosAlert;
    public Vector3 GetPlayerPosAlert => _playerPosAlert;

    public float speed = 5;
    [Range(0, 0.1f)]
    public float maxForce;
    public float arriveRadius;

    void Start()
    {
        _fsm = new FiniteStateMachine();
        _fsm.AddState(AgentStates.Patrol, new PatrolState(this, _fsm));
        _fsm.AddState(AgentStates.Chase, new ChaseState(this, _fsm, maxForce, arriveRadius)); 
        _fsm.AddState(AgentStates.Alert, new AlertState(this, _fsm)); 
        _fsm.ChangeState(AgentStates.Patrol);

        GameManager.instance.GetFSMAgents(this, _fsm);
    }

    void Update()
    {
        _fsm.Update();
    }

    public int PathToFollowCount()
    {
        return _pathToFollow.Count;
    }
    
    public void ClearPathToFollow()
    {
        _pathToFollow.Clear();
    }

    public void GoToAlert(Vector3 pos)
    {
        _playerPosAlert = pos;
    }

    public List<Node> GetPath(Node startingNode, Node goalNode)
    {
        List<Node> path = new List<Node>();

        path = _pf.AStar(startingNode, goalNode);

        return _pathToFollow = path;
    }

    public void FollowPath()
    {
        Vector3 pos = _pathToFollow[0].transform.position;
        Vector3 dir = pos - transform.position;
        dir += Vector3.up * 1.5f;
        dir.y = 0; //Sino se van a pos y 1.5 no sé por qué
        transform.forward = dir;

        transform.position += transform.forward * speed * Time.deltaTime;

        if (dir.magnitude < 0.1f)
        {
            _pathToFollow.RemoveAt(0);
        }
    }

    public Node NearNodeToAgent()
    {
        Node _nearNode = allWaypoints[_currentWaypoint];
        foreach (var node in GameManager.instance.nodes)
        {
            if (!InLineOfSight(transform.position, node.transform.position))
                continue;
            Vector3 dist = node.transform.position - transform.position;
            dist.y = 0;
            Vector3 nearNodeDist = _nearNode.transform.position - transform.position;
            nearNodeDist.y = 0;
            if (dist.magnitude < nearNodeDist.magnitude)
            {
                _nearNode = node;
            }
            else continue;
        }
        return _nearNode;
    }

    #region FOV & LineOfSight
    
    public bool InLineOfSight(Vector3 startPos, Vector3 endPos)
    {
        //origen, direccion, distancia, layerMask
        Vector3 dir = endPos - startPos;
        return !Physics.Raycast(startPos, dir, dir.magnitude, GameManager.instance.wallMask);
    }

    public bool InFOV(GameObject target)
    {
        if (Vector3.Distance(transform.position, target.transform.position) > viewRadius) return false;
        Vector3 dir = target.transform.position - transform.position;
        if (Physics.Raycast(transform.position, dir, dir.magnitude, GameManager.instance.wallMask)) return false;
        return Vector3.Angle(transform.forward, dir) <= viewAngle / 2;
    }

    Vector3 GetVectorFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 lineA = GetVectorFromAngle(viewAngle / 2 + transform.eulerAngles.y);
        Vector3 lineB = GetVectorFromAngle(-viewAngle / 2 + transform.eulerAngles.y);

        Gizmos.DrawLine(transform.position, transform.position + lineA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + lineB * viewRadius);
    }
}
