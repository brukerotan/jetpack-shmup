using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector2 lastPosition;
    [SerializeField] LayerMask layer;
    [SerializeField] GameObject onDestroyParticlePrefab;

    public float speed;
    public int damage = 1;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(transform.InverseTransformDirection(transform.right) * speed * Time.deltaTime);
        RaycastHit2D hit = Physics2D.Linecast(transform.position, lastPosition, layer.value);
        Debug.DrawLine(transform.position, lastPosition, Color.cyan);

        if (hit.collider)
        {
            Destroy(gameObject);
        }

        lastPosition = transform.position;
    }

    void OnDestroy()
    {
        Instantiate(onDestroyParticlePrefab, transform.position, Quaternion.identity);
    }
}
