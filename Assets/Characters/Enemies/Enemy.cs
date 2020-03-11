using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField, Range(-3, 3)] float knockbackMultiplier = 1f;
    SpriteRenderer sprRenderer;
    
    protected override void Start()
    {
        base.Start();
        sprRenderer = GetComponentInChildren<SpriteRenderer>();      
    }

    void Update()
    {
        if (!isDead)
        {
            sprRenderer.color = Vector4.MoveTowards(sprRenderer.color, Color.white, 15.0f * Time.deltaTime);
        }
        else
        {
            sprRenderer.color = Vector4.MoveTowards(sprRenderer.color, new Color(0.25f, 0.25f, 0.25f), 15.0f * Time.deltaTime);
        }
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
        if (TryGetComponent(out Rigidbody2D rb))
        {
            rb.gravityScale = 2f;
            Vector2 rand = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            rb.AddForceAtPosition(rand * 5, rb.position + rand);
        }
        
        if (GetComponentsInChildren<ParticleSystem>() != null)
        {
            foreach (ParticleSystem particle in GetComponentsInChildren<ParticleSystem>())
            {
                particle.Stop();
            }
        }        
    }
}
