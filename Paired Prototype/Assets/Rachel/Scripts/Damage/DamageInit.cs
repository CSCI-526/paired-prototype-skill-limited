using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInit : MonoBehaviour
{
    public static DamageInit I;
    private RectTransform uiCanvas;
    public GameObject prefab = null;
    private DamageAnim popupPrefab;
    Camera cam;

    void Awake()
    {
        uiCanvas = GameObject.FindGameObjectWithTag("UI").GetComponent<RectTransform>();
        popupPrefab = prefab.GetComponent<DamageAnim>();

        I = this;
        cam = Camera.main;
    }

    public void Show(int amount, Vector3 worldPos, Color? color = null)
    {
        if (!popupPrefab || !uiCanvas) { Debug.LogError("Assign popupPrefab & uiCanvas"); return; }

        Vector2 screen = cam.WorldToScreenPoint(worldPos);
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiCanvas, screen, null, out local); // null camera is correct for Overlay

        var pop = Instantiate(popupPrefab, uiCanvas);
        var rt = (RectTransform)pop.transform;
        rt.anchoredPosition = local;

        string text = "-" + amount;
        pop.Init(text, color ?? Color.red);
    }
}