using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float _inputX;
    float _inputY;
    [SerializeField]
    float _speed = 5;

    void Update()
    {
        _inputX = Input.GetAxis("Horizontal");
        _inputY = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(_inputX, 0, _inputY);
        transform.position += dir * _speed * Time.deltaTime;
    }

    public Vector3 GetPlayerPos()
    {
        return transform.position;
    }
}
