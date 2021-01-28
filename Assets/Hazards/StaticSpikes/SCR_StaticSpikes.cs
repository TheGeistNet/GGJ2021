using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_StaticSpikes : MonoBehaviour
{
    public GameObject m_SingleSpikePrefab;
    public int m_DamageAmount = 1;

    public int m_AmountInStrip = 1;
    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;
        if (other == null)
        {
            return;
        }
        SCR_IDamageable damagable = other.GetComponent<SCR_IDamageable>();
        if (damagable == null)
        {
            return;
        }
        damagable.Damage(m_DamageAmount);
    }

    public void LayoutSingleSpikes()
    {
        List<Transform> toDel = new List<Transform>();
        foreach (Transform child in transform)
        {
            toDel.Add(child);
        }
        toDel.ForEach((Transform t) => { DestroyImmediate(t.gameObject); });
        Vector3 selfPos = transform.position;
        float angle = transform.rotation.eulerAngles.z;
        Vector3 direction = Quaternion.Euler(0, 0, angle) * new Vector3(0.5f, 0, 0);
        for (int x = 0; x < m_AmountInStrip; ++x)
        {
            GameObject inst = Instantiate(m_SingleSpikePrefab, transform);
            inst.transform.position = transform.position + direction * x;
        }
    }
}
