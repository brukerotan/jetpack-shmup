using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    Rigidbody2D rb;
    [SerializeField, Range(-3, 3)] float knockbackMultiplier = 1f;
    SpriteRenderer sprRenderer;
    float speed = 0.25f;
    public Vector2 direction = Vector2.zero;
    
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        sprRenderer = GetComponentInChildren<SpriteRenderer>();
        direction.x = Random.Range(-1f, 1f);
        direction.y = Mathf.Sign(Random.Range(-1f, 1f));
    }

    void Update()
    {
        if (!isDead)
        {
            sprRenderer.color = Vector4.MoveTowards(sprRenderer.color, Color.white, 15.0f * Time.deltaTime);
            transform.Translate((10 * Time.deltaTime) * direction);
        }
        else
        {
            sprRenderer.color = Vector4.MoveTowards(sprRenderer.color, new Color(0.25f, 0.25f, 0.25f), 15.0f * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            rb.MovePosition(rb.position + (direction * speed) * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        direction = Vector2.Reflect(direction, collision.GetContact(0).normal);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack") && !isDead)
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            TakeDamage(bullet.damage, sprRenderer, new Color(1, 0.5f, 0));
            if (TryGetComponent(out Rigidbody2D rb))
            {
                Debug.Log("Has rigidbody");
                Vector2 direction = new Vector2(bullet.transform.position.x - rb.position.x, bullet.transform.position.y - rb.position.y);
                direction.Normalize();
                Debug.Log(direction * knockbackMultiplier);
                //rb.position = rb.position + (direction * knockbackMultiplier);
                rb.AddForce(direction * knockbackMultiplier);
            }
            Destroy(collision.gameObject);
        }
    }

    protected override void Die()
    {
        base.Die();
        //if(TryGetComponent(out AIPath AI))
        //{
        //    AI.canMove = false;
        //    AI.canSearch = false;
        //}

        rb.gravityScale = 2f;
        Vector2 rand = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        rb.AddForceAtPosition(rand * 5, rb.position + rand);

        if (GetComponentsInChildren<ParticleSystem>() != null)
        {
            foreach (ParticleSystem particle in GetComponentsInChildren<ParticleSystem>())
            {
                particle.Stop();
            }
        }        
    }
}
