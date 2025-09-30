using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageAnim : MonoBehaviour
{
    public Text label;            // Assign in prefab (the same Text component)
    public float riseSpeed = 40f; // pixels/sec
    public float lifetime = 0.8f; // seconds

    float t;
    RectTransform rt;

    public void Init(string text, Color color)
    {
        if (!label) label = GetComponent<Text>();
        rt = (RectTransform)transform;

        label.text = text;
        label.color = color;
    }

    void Update()
    {
        t += Time.deltaTime;

        // move up
        rt.anchoredPosition += new Vector2(0f, riseSpeed) * Time.deltaTime;

        // fade out
        var c = label.color;
        c.a = Mathf.Lerp(1f, 0f, t / lifetime);
        label.color = c;

        if (t >= lifetime) Destroy(gameObject);
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 8121d3332441f60867f27cb6fd953299c9939ef6
