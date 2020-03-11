using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public List<Transform> targets;
    Vector3 mousePos;
    public Vector2 offset = new Vector2(0, 0);
    public float smoothTime = .2f;
    Vector3 velocity;
    Vector3 resultVector;
    Vector3 centerPoint;

    private void Start()
    {
        targets.Add(GameObject.FindGameObjectWithTag("Player").transform);

        resultVector.z = transform.position.z;
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = (targets[0].position + mousePos) / 2;

        centerPoint = GetCenterPoint();
        resultVector.x = centerPoint.x;
        resultVector.y = centerPoint.y;

        transform.position = Vector3.SmoothDamp(transform.position, resultVector + (Vector3)offset, ref velocity, smoothTime);
        
    }

    Vector3 GetCenterPoint()
    {
        //if (targets.Count == 1)
        //{
        //    return targets[0].position;
        //}

        Bounds bounds = new Bounds(targets[0].position, Vector2.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        bounds.Encapsulate(mousePos);

        return bounds.center;
    }
}
