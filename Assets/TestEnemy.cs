using System;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    public int hp;
    public int maxHp = 10;

    public Action<Vector3> onDie;

    private bool isDead = false; // 🔥 추가 (중복 방지)

    private void Start()
    {
        hp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        hp -= damage;

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        onDie?.Invoke(transform.position); // 👉 "죽었다" 신호만 보냄

        Destroy(gameObject);
    }
}