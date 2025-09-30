using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    //public Animator animator;

    public int damage = 5;
    private GameObject player;
    private Health playerHealth;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }

    private IEnumerator AttackLunge()
    {
        Vector3 start = this.transform.localPosition;
        Vector3 forward = start - new Vector3(1.0f, 0, 0);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 5;
            this.transform.localPosition = Vector3.Lerp(start, forward, t);
            yield return null;
        }

        Vector3 baseOffset = Vector3.up * 90f + Vector3.right * 150f;
        Vector3 randomOffset = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), 0f);

        DamageInit.I.Show(
            damage,
            player.transform.position + baseOffset + randomOffset
        );

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 5;
            this.transform.localPosition = Vector3.Lerp(forward, start, t);
            yield return null;
        }
    }


    public void Attack()
    {
        StartCoroutine(AttackLunge());

        playerHealth.TakeDamage(damage);
    }
}
