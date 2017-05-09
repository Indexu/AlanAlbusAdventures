using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;
    public GameObject canvas;
    public RectTransform canvasRect;
    public Camera mainCamera;
    public GameObject damageText;

    private const float damageTextDuration = 0.6f;
    private const float damageTextSpeed = 3f;

    public Vector3 PositionToUI(Vector3 pos)
    {
        Vector2 viewportPos = UIManager.instance.mainCamera.WorldToViewportPoint(pos);
        Vector2 screenPos = new Vector2
        (
            viewportPos.x * instance.canvasRect.sizeDelta.x,
            viewportPos.y * instance.canvasRect.sizeDelta.y
        );

        screenPos -= instance.canvasRect.sizeDelta * 0.5f;

        return screenPos;
    }

    public void DisplayDamageText(Vector3 pos, float amount, float offset)
    {
        var damageTextInstance = Instantiate(damageText, Vector3.zero, Quaternion.identity, UIManager.instance.canvas.transform);

        Vector2 screenPos = UIManager.instance.PositionToUI(pos);
        screenPos.y += offset;

        var rt = damageTextInstance.GetComponent<RectTransform>();
        rt.GetComponent<RectTransform>().anchoredPosition = screenPos;
        damageTextInstance.GetComponent<Text>().text = amount.ToString();

        StartCoroutine(AnimateDamageText(rt));
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    private IEnumerator AnimateDamageText(RectTransform rt)
    {
        var seconds = 0f;
        var vector = new Vector2(0f, damageTextSpeed);
        var text = rt.gameObject.GetComponent<Text>();
        var color = text.color;

        while (seconds < damageTextDuration)
        {
            seconds += Time.deltaTime;
            rt.anchoredPosition += vector;
            color.a -= Time.deltaTime / damageTextDuration;
            text.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        GameObject.Destroy(rt.gameObject);
    }
}
