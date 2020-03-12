using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField, Range(1, 255)] protected int maxHealth = 3;
    protected int health;
    protected bool isDead;

    protected virtual void Start()
    {
        health = maxHealth;
    }

    protected virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0;
        }
    }

    protected virtual void TakeDamage(int damage, SpriteRenderer sprRend, Color flashColor)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
            health = 0;
        }
        sprRend.color = flashColor;
    }

    protected virtual void HealHealth(int heal)
    {
        health += heal;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }

    protected virtual void HealHealth(int heal, SpriteRenderer sprRend, Color flashColor)
    {
        health += heal;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        sprRend.color = flashColor;
    }

    protected virtual void Die()
    {
        isDead = true;
    }
}
