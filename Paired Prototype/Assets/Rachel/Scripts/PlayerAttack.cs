using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //temporary for testing
    public int damage = 5;
    private Health playerHealth;
    private GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }

    private IEnumerator AttackLunge(Transform transform)
    {
        Vector3 start = player.transform.localPosition;
        Vector3 forward = start + new Vector3(40.0f, 0, 0);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 5;
            player.transform.localPosition = Vector3.Lerp(start, forward, t);
            yield return null;
        }

        DamageInit.I.Show(damage, transform.position + Vector3.up * 90.0f + Vector3.right * 30.0f); ;

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 5;
            player.transform.localPosition = Vector3.Lerp(forward, start, t);
            yield return null;
        }
    }    

    public void Attack()
    {
        if (playerHealth.currentHealth > 0)
        {

            if (SelectManager.Instance != null && SelectManager.Instance.Current != null)
            {
                StartCoroutine(AttackLunge(SelectManager.Instance.Current.transform));

                Health enemyHealth = SelectManager.Instance.Current.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
            }
        }
    }
}
