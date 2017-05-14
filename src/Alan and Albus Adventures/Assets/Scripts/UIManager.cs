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
    public GameObject tooltip;
    public Color rareItemColor;
    public Color epicItemColor;
    public Color legendaryItemColor;
    public Color minorItemColor;
    public Color lesserItemColor;
    public Color superiorItemColor;
    public Color majorItemColor;
    public Color statPointUpColor;
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
    private RawImage holeButton;


    private GameObject experienceBar;
    private Slider experienceBarSlider;
    private Text experienceBarText;

    private GameObject[] playerTooltips;
    private Text[] playerTooltipsQualityText;
    private Text[] playerTooltipsItemNameText;
    private Text[] playerTooltipsPostfixText;
    private Text[] playerTooltipsStatsText;
    private RawImage[] playerTooltipsDestroyButton;
    private Slider[] playerTooltipsDestroySlider;

    private GameObject transitionScreen;
    private RawImage transitionBackground;
    private Text transitionText;

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

    public void DisplayStatUpText(Vector3 pos, float offset)
    {
        var damageTextInstance = Instantiate(damageText, Vector3.zero, Quaternion.identity, UIManager.instance.canvas.transform);

        Vector2 screenPos = UIManager.instance.PositionToUI(pos);
        screenPos.y += offset;

        var rt = damageTextInstance.GetComponent<RectTransform>();
        rt.GetComponent<RectTransform>().anchoredPosition = screenPos;

        var txt = damageTextInstance.GetComponent<Text>();
        txt.text = "Stat Point!";
        txt.fontSize = 55;
        txt.color = statPointUpColor;

        StartCoroutine(AnimateStatUpText(rt));
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

    public void ShowHoleButton(Vector3 pos, bool ps4)
    {
        if (holeButton != null)
        {
            var color = holeButton.color;
            color.a = 1f;

            holeButton.color = color;
        }
        else
        {
            var buttonInstance = Instantiate(button, Vector3.zero, Quaternion.identity, UIManager.instance.canvas.transform);

            var rt = buttonInstance.GetComponent<RectTransform>();
            rt.anchoredPosition = (Vector2)UIManager.instance.PositionToUI(pos);

            holeButton = buttonInstance.GetComponent<RawImage>();

            if (!ps4)
            {
                holeButton.texture = XboxConfirm;
            }

            var color = holeButton.color;
            color.a = 0.5f;

            holeButton.color = color;
        }
    }

    public void HideHoleButton()
    {
        if (holeButton != null)
        {
            if (holeButton.color.a == 1f)
            {
                var color = holeButton.color;
                color.a = 0.5f;

                holeButton.color = color;
            }
            else
            {
                GameObject.Destroy(holeButton.gameObject);
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
        if (holeButton != null)
        {
            GameObject.Destroy(holeButton.gameObject);
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

    public GameObject CreateAndShowTooltip(Vector3 pos, float yOffset, Quality quality, Postfix postfix, Property postFixProperty, bool hasPostfix, string itemName, string statsText)
    {
        var tooltipInstance = Instantiate(tooltip, Vector3.zero, Quaternion.identity, UIManager.instance.canvas.transform);

        var rt = tooltipInstance.GetComponent<RectTransform>();

        Vector2 screenPos = (Vector2)UIManager.instance.PositionToUI(pos);
        screenPos.y += yOffset;
        rt.anchoredPosition = screenPos;

        var nameContainer = tooltipInstance.transform.Find("Name").transform;
        var statsTextObject = tooltipInstance.transform.Find("Stats").Find("Text").GetComponent<Text>();
        var qualityText = nameContainer.Find("QualityText").GetComponent<Text>();
        var nameText = nameContainer.Find("ItemText").GetComponent<Text>();
        var postfixText = nameContainer.Find("PostfixText").GetComponent<Text>();

        nameText.text = " " + itemName.Replace("(Clone)", string.Empty) + " ";
        statsTextObject.text = statsText;

        switch (quality)
        {
            case Quality.RARE:
                {
                    qualityText.color = rareItemColor;
                    qualityText.text = "Rare";
                    break;
                }
            case Quality.EPIC:
                {
                    qualityText.color = epicItemColor;
                    qualityText.text = "Epic";
                    break;
                }
            case Quality.LEGENDARY:
                {
                    qualityText.color = legendaryItemColor;
                    qualityText.text = "Legendary";
                    break;
                }
            default:
                {
                    GameObject.Destroy(qualityText.gameObject);
                    break;
                }
        }

        if (hasPostfix)
        {
            postfixText.text = "of " + Item.PostfixToString(postfix) + " " + Item.PropertyToString(postFixProperty);

            switch (postfix)
            {
                case Postfix.MINOR:
                    {
                        postfixText.color = minorItemColor;
                        break;
                    }
                case Postfix.LESSER:
                    {
                        postfixText.color = lesserItemColor;
                        break;
                    }
                case Postfix.SUPERIOR:
                    {
                        postfixText.color = superiorItemColor;
                        break;
                    }
                case Postfix.MAJOR:
                    {
                        postfixText.color = majorItemColor;
                        break;
                    }
                default:
                    {
                        postfixText.text = string.Empty;
                        break;
                    }
            }
        }
        else
        {
            GameObject.Destroy(postfixText.gameObject);
        }

        return tooltipInstance;
    }

    public void ShowInventoryTooltip(int playerID, bool ps4, Direction dir, Quality quality, Postfix postfix, Property postFixProperty, bool hasPostfix, string itemName, string statsText)
    {
        playerTooltips[playerID].SetActive(true);

        playerTooltipsItemNameText[playerID].text = itemName.Replace("(Clone)", string.Empty);


        UIManager.instance.SetInventoryTooltipDestroySlider(playerID, 0f);

        switch (quality)
        {
            case Quality.RARE:
                {
                    playerTooltipsQualityText[playerID].gameObject.SetActive(true);
                    playerTooltipsQualityText[playerID].color = rareItemColor;
                    playerTooltipsQualityText[playerID].text = "Rare";
                    break;
                }
            case Quality.EPIC:
                {
                    playerTooltipsQualityText[playerID].gameObject.SetActive(true);
                    playerTooltipsQualityText[playerID].color = epicItemColor;
                    playerTooltipsQualityText[playerID].text = "Epic";
                    break;
                }
            case Quality.LEGENDARY:
                {
                    playerTooltipsQualityText[playerID].gameObject.SetActive(true);
                    playerTooltipsQualityText[playerID].color = legendaryItemColor;
                    playerTooltipsQualityText[playerID].text = "Legendary";
                    break;
                }
            default:
                {
                    playerTooltipsQualityText[playerID].gameObject.SetActive(false);
                    break;
                }
        }

        if (hasPostfix)
        {
            playerTooltipsPostfixText[playerID].gameObject.SetActive(true);
            playerTooltipsPostfixText[playerID].text = "of " + Item.PropertyToString(postFixProperty);

            switch (postfix)
            {
                case Postfix.MINOR:
                    {
                        playerTooltipsPostfixText[playerID].color = minorItemColor;
                        break;
                    }
                case Postfix.LESSER:
                    {
                        playerTooltipsPostfixText[playerID].color = lesserItemColor;
                        break;
                    }
                case Postfix.SUPERIOR:
                    {
                        playerTooltipsPostfixText[playerID].color = superiorItemColor;
                        break;
                    }
                case Postfix.MAJOR:
                    {
                        playerTooltipsPostfixText[playerID].color = majorItemColor;
                        break;
                    }
                default:
                    {
                        playerTooltipsPostfixText[playerID].text = string.Empty;
                        break;
                    }
            }
        }
        else
        {
            playerTooltipsPostfixText[playerID].gameObject.SetActive(false);
        }

        if (ps4)
        {
            if (dir == Direction.up)
            {
                playerTooltipsDestroyButton[playerID].texture = PS4Stats;
            }
            else if (dir == Direction.right)
            {
                playerTooltipsDestroyButton[playerID].texture = PS4PassiveButtonRight;
            }
            else if (dir == Direction.down)
            {
                playerTooltipsDestroyButton[playerID].texture = PS4Confirm;
            }
            else
            {
                playerTooltipsDestroyButton[playerID].texture = PS4PassiveButtonLeft;
            }
        }
        else
        {
            if (dir == Direction.up)
            {
                playerTooltipsDestroyButton[playerID].texture = XboxStats;
            }
            else if (dir == Direction.right)
            {
                playerTooltipsDestroyButton[playerID].texture = XboxPassiveButtonRight;
            }
            else if (dir == Direction.down)
            {
                playerTooltipsDestroyButton[playerID].texture = XboxConfirm;
            }
            else
            {
                playerTooltipsDestroyButton[playerID].texture = XboxPassiveButtonLeft;
            }
        }

        StartCoroutine(StatsAndSpacing(playerID, statsText));
    }

    public void HideInventoryTooltip(int playerID)
    {
        playerTooltips[playerID].SetActive(false);
    }

    public void SetInventoryTooltipDestroySlider(int playerID, float value)
    {
        playerTooltipsDestroySlider[playerID].value = value;
    }

    public void SetTransitionAlpha(float value)
    {
        if (value <= 0f)
        {
            var backgroundColor = transitionBackground.color;
            backgroundColor.a = 0f;
            transitionBackground.color = backgroundColor;

            var textColor = transitionText.color;
            textColor.a = 0f;
            transitionText.color = textColor;

            transitionScreen.SetActive(false);
        }
        else if (1f <= value)
        {
            if (!transitionScreen.activeSelf)
            {
                transitionScreen.SetActive(true);
            }

            var backgroundColor = transitionBackground.color;
            backgroundColor.a = 1f;
            transitionBackground.color = backgroundColor;

            var textColor = transitionText.color;
            textColor.a = 1f;
            transitionText.color = textColor;
        }
        else
        {
            if (!transitionScreen.activeSelf)
            {
                transitionScreen.SetActive(true);
            }

            var backgroundColor = transitionBackground.color;
            backgroundColor.a = value;
            transitionBackground.color = backgroundColor;

            var textColor = transitionText.color;
            textColor.a = value;
            transitionText.color = textColor;
        }
    }

    public void SetTransitionText(string text)
    {
        transitionText.text = text;
    }

    public void HideAllTooltips()
    {
        foreach (Transform child in UIManager.instance.canvas.transform)
        {
            if (child.name == "Tooltip(Clone)")
            {
                child.gameObject.SetActive(false);
            }
        }
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

        UIManager.instance.transitionScreen = UIManager.instance.canvas.transform.Find("TransitionScreen").gameObject;
        UIManager.instance.transitionBackground = UIManager.instance.transitionScreen.transform.Find("Background").GetComponent<RawImage>();
        UIManager.instance.transitionText = UIManager.instance.transitionScreen.transform.Find("Text").GetComponent<Text>();

        UIManager.instance.AlanButtons = new RawImage[numberOfSlots];
        UIManager.instance.AlbusButtons = new RawImage[numberOfSlots];
        UIManager.instance.AlanItemIcons = new RawImage[numberOfSlots];
        UIManager.instance.AlbusItemIcons = new RawImage[numberOfSlots];

        UIManager.instance.doorButton1 = null;
        UIManager.instance.doorButton2 = null;

        UIManager.instance.playerTooltips = new GameObject[2];
        UIManager.instance.playerTooltipsItemNameText = new Text[2];
        UIManager.instance.playerTooltipsPostfixText = new Text[2];
        UIManager.instance.playerTooltipsQualityText = new Text[2];
        UIManager.instance.playerTooltipsStatsText = new Text[2];
        UIManager.instance.playerTooltipsDestroyButton = new RawImage[2];
        UIManager.instance.playerTooltipsDestroySlider = new Slider[2];

        UIManager.instance.transitionScreen.SetActive(false);
        UIManager.instance.FindTooltips();
        UIManager.instance.playerTooltips[0].SetActive(false);
        UIManager.instance.playerTooltips[1].SetActive(false);
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

            if (slot.name != "UseItemSlot")
            {
                icon.gameObject.SetActive(false);
            }

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

            if (slot.name != "UseItemSlot")
            {
                icon.gameObject.SetActive(false);
            }

            counter++;
        }
    }

    private void FindTooltips()
    {
        playerTooltips[0] = UIManager.instance.canvas.transform.Find("AlanTooltip").gameObject;
        playerTooltips[1] = UIManager.instance.canvas.transform.Find("AlbusTooltip").gameObject;

        for (int i = 0; i < playerTooltips.Length; i++)
        {
            UIManager.instance.playerTooltipsStatsText[i] = playerTooltips[i].transform.Find("Stats").Find("Text").GetComponent<Text>();
            UIManager.instance.playerTooltipsItemNameText[i] = playerTooltips[i].transform.Find("Name").Find("ItemText").GetComponent<Text>();
            UIManager.instance.playerTooltipsPostfixText[i] = playerTooltips[i].transform.Find("Name").Find("PostfixText").GetComponent<Text>();
            UIManager.instance.playerTooltipsQualityText[i] = playerTooltips[i].transform.Find("Name").Find("QualityText").GetComponent<Text>();
            UIManager.instance.playerTooltipsDestroyButton[i] = playerTooltips[i].transform.Find("DestructionText").Find("DestroyButton").GetComponent<RawImage>();
            UIManager.instance.playerTooltipsDestroySlider[i] = playerTooltips[i].transform.Find("DestroySlider").GetComponent<Slider>();
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

    private IEnumerator AnimateStatUpText(RectTransform rt)
    {
        var seconds = 0f;
        var vector = new Vector2(0f, damageTextSpeed);
        var text = rt.gameObject.GetComponent<Text>();
        var color = text.color;

        while (seconds < (damageTextDuration * 2))
        {
            seconds += Time.deltaTime;
            rt.anchoredPosition += vector;
            color.a -= Time.deltaTime / (damageTextDuration * 2);
            text.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        GameObject.Destroy(rt.gameObject);
    }

    private IEnumerator StatsAndSpacing(int playerID, string statsText)
    {
        var lines = statsText.Split('\n');
        yield return null;
        playerTooltipsItemNameText[playerID].text = " " + playerTooltipsItemNameText[playerID].text;
        yield return null;
        playerTooltipsItemNameText[playerID].text += " ";

        playerTooltipsStatsText[playerID].text = string.Empty;

        foreach (var line in lines)
        {
            yield return null;
            playerTooltipsStatsText[playerID].text += line + "\n";
        }
    }
}
