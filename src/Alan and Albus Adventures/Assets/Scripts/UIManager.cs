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
    public GameObject reviveUI;
    public GameObject button;
    public Texture PS4Confirm;
    public Texture XboxConfirm;
    public Texture PS4Stats;
    public Texture XboxStats;
    public Texture PS4UseItem;
    public Texture XboxUseItem;
    public Texture PS4LeftStick;
    public Texture XboxLeftStick;
    public Texture PS4RightStick;
    public Texture XboxRightStick;
    public Texture PS4Attack;
    public Texture XboxAttack;
    public Texture PS4ItemMode;
    public Texture XboxItemMode;
    public Texture PS4Pause;
    public Texture XboxPause;
    public Texture PS4PassiveButtonLeft;
    public Texture XboxPassiveButtonLeft;
    public Texture PS4PassiveButtonRight;
    public Texture XboxPassiveButtonRight;

    private const float damageTextDuration = 0.6f;
    private const float damageTextSpeed = 3f;

    private const int numberOfSlots = 5;
    private RawImage[] AlanButtons;
    private RawImage[] AlbusButtons;
    private RawImage[] AlanItemIcons;
    private RawImage[] AlbusItemIcons;

    private GameObject reviveUIInstance;
    private Slider reviveUISlider;
    private Transform reviveUITransformPos;
    private RectTransform reviveUIRectTransform;
    private const float reviveUIOffset = 150f;

    private RawImage doorButton1;
    private Direction doorButton1Direction;
    private RawImage doorButton2;
    private Direction doorButton2Direction;


    private GameObject experienceBar;
    private Slider experienceBarSlider;
    private Text experienceBarText;

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

    public void ApplySpritesToButtons(int playerID, bool ps4)
    {
        var arr = (playerID == 0 ? AlanButtons : AlbusButtons);

        if (ps4)
        {
            arr[0].texture = PS4UseItem;
            arr[1].texture = PS4PassiveButtonLeft;
            arr[2].texture = PS4Confirm;
            arr[3].texture = PS4PassiveButtonRight;
            arr[4].texture = PS4Stats;
        }
        else
        {
            arr[0].texture = XboxUseItem;
            arr[1].texture = XboxPassiveButtonLeft;
            arr[2].texture = XboxConfirm;
            arr[3].texture = XboxPassiveButtonRight;
            arr[4].texture = XboxStats;
        }
    }

    public void SetPassiveItemLeft(int playerID, Texture icon)
    {
        var arr = (playerID == 0 ? AlanItemIcons : AlbusItemIcons);

        arr[1].gameObject.SetActive(true);
        arr[1].texture = icon;
    }

    public void SetPassiveItemDown(int playerID, Texture icon)
    {
        var arr = (playerID == 0 ? AlanItemIcons : AlbusItemIcons);

        arr[2].gameObject.SetActive(true);
        arr[2].texture = icon;
    }

    public void SetPassiveItemRight(int playerID, Texture icon)
    {
        var arr = (playerID == 0 ? AlanItemIcons : AlbusItemIcons);

        arr[3].gameObject.SetActive(true);
        arr[3].texture = icon;
    }

    public void SetPassiveItemUp(int playerID, Texture icon)
    {
        var arr = (playerID == 0 ? AlanItemIcons : AlbusItemIcons);

        arr[4].gameObject.SetActive(true);
        arr[4].texture = icon;
    }

    public void DestroyPassiveItemLeft(int playerID)
    {
        var arr = (playerID == 0 ? AlanItemIcons : AlbusItemIcons);

        arr[1].gameObject.SetActive(false);
    }

    public void DestroyPassiveItemDown(int playerID)
    {
        var arr = (playerID == 0 ? AlanItemIcons : AlbusItemIcons);

        arr[2].gameObject.SetActive(false);
    }

    public void DestroyPassiveItemRight(int playerID)
    {
        var arr = (playerID == 0 ? AlanItemIcons : AlbusItemIcons);

        arr[3].gameObject.SetActive(false);
    }

    public void DestroyPassiveItemUp(int playerID)
    {
        var arr = (playerID == 0 ? AlanItemIcons : AlbusItemIcons);

        arr[4].gameObject.SetActive(false);
    }

    public void SetPassiveItemSlots(int playerID, bool show)
    {
        var arr = (playerID == 0 ? AlanButtons : AlbusButtons);

        for (int i = 1; i < arr.Length; i++)
        {
            arr[i].gameObject.SetActive(show);
        }
    }

    public void ShowReviveUI(Transform pos, bool ps4)
    {
        reviveUITransformPos = pos;
        reviveUIInstance = Instantiate(reviveUI, Vector3.zero, Quaternion.identity, UIManager.instance.canvas.transform);
        reviveUIRectTransform = reviveUIInstance.GetComponent<RectTransform>();

        reviveUISlider = reviveUIInstance.transform.Find("Slider").GetComponent<Slider>();
        reviveUISlider.value = 0f;

        if (!ps4)
        {
            reviveUIInstance.transform.Find("Button").GetComponent<RawImage>().texture = XboxConfirm;
        }
    }

    public void HideReviveUI()
    {
        if (reviveUIInstance != null)
        {
            GameObject.Destroy(reviveUIInstance);
        }
    }

    public void UpdateReviveSlider(float value)
    {
        reviveUISlider.value = value;
    }

    public void ShowDoorButton(Vector3 pos, Direction dir, bool ps4)
    {
        var UIpos = (Vector2)UIManager.instance.PositionToUI(pos);
        var createButton = true;

        if (doorButton1 != null && dir == doorButton1Direction)
        {
            createButton = false;
            var color = doorButton1.color;
            color.a = 1f;

            doorButton1.color = color;
        }
        else if (doorButton2 != null && dir == doorButton2Direction)
        {
            createButton = false;
            var color = doorButton2.color;
            color.a = 1f;

            doorButton2.color = color;
        }

        if (createButton)
        {
            var doorButtonInstance = Instantiate(button, Vector3.zero, Quaternion.identity, UIManager.instance.canvas.transform);

            var rt = doorButtonInstance.GetComponent<RectTransform>();
            rt.anchoredPosition = UIpos;

            RawImage doorButton;
            if (doorButton1 == null)
            {
                doorButton1 = doorButtonInstance.GetComponent<RawImage>();
                doorButton1Direction = dir;
                doorButton = doorButton1;
            }
            else
            {
                doorButton2 = doorButtonInstance.GetComponent<RawImage>();
                doorButton2Direction = dir;
                doorButton = doorButton2;
            }

            if (!ps4)
            {
                doorButton.texture = XboxConfirm;
            }

            var color = doorButton.color;
            color.a = 0.5f;

            doorButton.color = color;
        }

    }

    public void HideDoorButton(Direction dir)
    {
        if (doorButton1 != null && doorButton1Direction == dir)
        {
            if (doorButton1.color.a == 1f)
            {
                var color = doorButton1.color;
                color.a = 0.5f;

                doorButton1.color = color;
            }
            else
            {
                GameObject.Destroy(doorButton1.gameObject);
            }
        }
        else if (doorButton2 != null && doorButton2Direction == dir)
        {
            if (doorButton2.color.a == 1f)
            {
                var color = doorButton2.color;
                color.a = 0.5f;

                doorButton2.color = color;
            }
            else
            {
                GameObject.Destroy(doorButton2.gameObject);
            }
        }
    }

    public void ClearDoorButtons()
    {
        if (doorButton1 != null)
        {
            GameObject.Destroy(doorButton1.gameObject);
        }
        if (doorButton2 != null)
        {
            GameObject.Destroy(doorButton2.gameObject);
        }
    }

    public void ShowExperienceBar()
    {
        experienceBar.SetActive(true);
    }

    public void HideExperienceBar()
    {
        experienceBar.SetActive(false);
    }

    public void SetExperienceBar(float current, float max)
    {
        experienceBarText.text = (int)current + "/" + (int)max;
        experienceBarSlider.value = current / max;
    }

    public GameObject CreateAndShowButton(Vector3 pos, float yOffset, Direction dir, bool ps4)
    {
        var buttonInstance = Instantiate(button, Vector3.zero, Quaternion.identity, UIManager.instance.canvas.transform);

        var rt = buttonInstance.GetComponent<RectTransform>();

        rt.sizeDelta = new Vector2(100f, 100f);

        Vector2 screenPos = (Vector2)UIManager.instance.PositionToUI(pos);
        screenPos.y += yOffset;
        rt.anchoredPosition = screenPos;

        if (ps4)
        {
            if (dir == Direction.up)
            {
                buttonInstance.GetComponent<RawImage>().texture = PS4Stats;
            }
            else if (dir == Direction.right)
            {
                buttonInstance.GetComponent<RawImage>().texture = PS4PassiveButtonRight;
            }
            else if (dir == Direction.down)
            {
                buttonInstance.GetComponent<RawImage>().texture = PS4Confirm;
            }
            else
            {
                buttonInstance.GetComponent<RawImage>().texture = PS4PassiveButtonLeft;
            }
        }
        else
        {
            if (dir == Direction.up)
            {
                buttonInstance.GetComponent<RawImage>().texture = XboxStats;
            }
            else if (dir == Direction.right)
            {
                buttonInstance.GetComponent<RawImage>().texture = XboxPassiveButtonRight;
            }
            else if (dir == Direction.down)
            {
                buttonInstance.GetComponent<RawImage>().texture = XboxConfirm;
            }
            else
            {
                buttonInstance.GetComponent<RawImage>().texture = XboxPassiveButtonLeft;
            }
        }

        return buttonInstance;
    }

    public void MoveUIElement(GameObject elem, Vector3 pos, float yOffset)
    {
        var rt = elem.GetComponent<RectTransform>();

        Vector2 screenPos = (Vector2)UIManager.instance.PositionToUI(pos);
        screenPos.y += yOffset;
        rt.anchoredPosition = screenPos;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Init();
        }
        else if (instance != this)
        {
            Init();
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnGUI()
    {
        if (reviveUIInstance != null)
        {
            Vector2 screenPos = UIManager.instance.PositionToUI(reviveUITransformPos.position);
            screenPos.y += reviveUIOffset;
            reviveUIRectTransform.anchoredPosition = screenPos;
        }
    }

    private void Init()
    {
        UIManager.instance.mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        UIManager.instance.canvas = GameObject.FindGameObjectWithTag("Canvas");
        UIManager.instance.canvasRect = UIManager.instance.canvas.GetComponent<RectTransform>();

        UIManager.instance.experienceBar = UIManager.instance.canvas.transform.Find("ExperienceBar").gameObject;
        UIManager.instance.experienceBarSlider = experienceBar.transform.Find("ExperienceSlider").GetComponent<Slider>();
        UIManager.instance.experienceBarText = experienceBarSlider.transform.Find("BarText").GetComponent<Text>();

        UIManager.instance.AlanButtons = new RawImage[numberOfSlots];
        UIManager.instance.AlbusButtons = new RawImage[numberOfSlots];
        UIManager.instance.AlanItemIcons = new RawImage[numberOfSlots];
        UIManager.instance.AlbusItemIcons = new RawImage[numberOfSlots];

        UIManager.instance.doorButton1 = null;
        UIManager.instance.doorButton2 = null;

        UIManager.instance.GetItemSlotButtons();
        UIManager.instance.SetPassiveItemSlots(0, false);
        UIManager.instance.SetPassiveItemSlots(1, false);
        UIManager.instance.HideExperienceBar();
    }

    private void GetItemSlotButtons()
    {
        var slots = instance.canvas.transform.Find("AlanItemSlots");
        var counter = 0;

        foreach (Transform slot in slots)
        {
            var button = slot.Find("Button").GetComponent<RawImage>();
            var icon = slot.Find("Item").GetComponent<RawImage>();

            AlanButtons[counter] = button;
            AlanItemIcons[counter] = icon;
            icon.gameObject.SetActive(false);

            counter++;
        }

        slots = instance.canvas.transform.Find("AlbusItemSlots");
        counter = 0;

        foreach (Transform slot in slots)
        {
            var button = slot.Find("Button").GetComponent<RawImage>();
            var icon = slot.Find("Item").GetComponent<RawImage>();

            AlbusButtons[counter] = button;
            AlbusItemIcons[counter] = icon;
            icon.gameObject.SetActive(false);

            counter++;
        }
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
