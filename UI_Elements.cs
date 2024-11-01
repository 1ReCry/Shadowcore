using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UI_Elements : MonoBehaviour
{
    public bool start_items;
    private float start_items_timer = 0.2f;
    private bool start_items_enabled;
    public int start_Slot1_ID;
    public int start_Slot2_ID;
    public int start_Slot3_ID;
    public int start_Slot4_ID;
    public int start_Slot5_ID;
    public int start_Slot6_ID;
    public int start_SwordID;

    private GameObject player;
    private PlayerAI playerai;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI healthText;
    public Slider healthBar;
    public TextMeshProUGUI soulsText;
    public Slider soulsBar;
    public GameObject soulsBarObj;
    public TextMeshProUGUI notificationText;
    public TextMeshProUGUI storyNotificationText;
    public GameObject notifGameobj;
    public GameObject stroyNotifGameobj;
    private Animation notifiAnim;
    private Animator storyNotifAnimator;
    public Slider bossBar;
    public TextMeshProUGUI bossBarHealthText;

    public RectTransform Slot1;
    public RectTransform Slot2;
    public RectTransform Slot3;
    public RectTransform Slot4;
    public RectTransform Slot5;
    public RectTransform Slot6;
    public RectTransform ItemInfoPanel;
    public TextMeshProUGUI ItemInfoPanelText;
    public KeyCode destroyItemKey;

    public GameObject deathPanel;
    public GameObject pauseMenu;
    public GameObject shopMenu;
    public bool isPaused;
    public GameObject blinkOfCore;
    public Animator bocAnim;

    private float notifTimer;
    private float animDur = 2.5f;
    private float currentDuration = 4f;

    private float storyNotifTimer;

    private float blinkTimer;

    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    bool isHoveringOverShopSlot;

    bool shop_opened;
    
    void Start()
    {
        shopMenu.SetActive(false);
        ResumeGame();
        raycaster = GetComponent<GraphicRaycaster>();
        Time.timeScale = 1f;
        player = GameObject.Find("Player");
        notifiAnim = notifGameobj.GetComponent<Animation>();
        storyNotifAnimator = stroyNotifGameobj.GetComponent<Animator>();
        if(player != null) playerai = player.GetComponent<PlayerAI>();
        notifiAnim.Stop();
        Notification("<size=37>"+Globals.levelName[Globals.CurrentLevelID], 4f);
        bocAnim = blinkOfCore.GetComponent<Animator>();
        blinkOfCore.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B) && Globals.survivalMode)
        {
            if(shop_opened)
            {
                shopMenu.SetActive(false);
                shop_opened = false;
            }
            else
            {
                shopMenu.SetActive(true);
                shop_opened = true;
            }
        }
        if(!Globals.survivalMode)
        {
            soulsBarObj.SetActive(false);
        }
        if(Globals.survivalMode)
        {
            soulsBarObj.SetActive(true);
        }

        if(playerai.isDead)
        {
            shopMenu.SetActive(false);
            shop_opened = false;
        }

        Globals.menuOpened = isPaused;
        if(blinkTimer > -1) blinkTimer -= Time.deltaTime;
        moneyText.text = Globals.Money.ToString() + " gold";
        soulsText.text = Globals.Money.ToString();
        soulsBar.value = Globals.Money;
        if(playerai.maxhealth <= 99998){
            healthText.text = Mathf.RoundToInt(playerai.health).ToString()+ "+";
            healthBar.value = playerai.health;
            healthBar.maxValue = playerai.maxhealth;
        } 
        if(notifTimer > 0){
            notifTimer -= Time.deltaTime;
            notifGameobj.SetActive(true);
        } 
        if(notifTimer <= 0){
            notifGameobj.SetActive(false);
            notifiAnim.Stop();
        } 

        if(start_items_timer > 0) start_items_timer -= Time.deltaTime;
        if(start_items_timer <= 0 && !start_items_enabled)
        {
            start_items_enabled = true;
            if(start_items)
            {
                PlayerAI playerai = FindObjectOfType<PlayerAI>();
                playerai.SelectNewSword(start_SwordID);

                Globals.Inventory[1] = start_Slot1_ID;
                Globals.Inventory[2] = start_Slot2_ID;
                Globals.Inventory[3] = start_Slot3_ID;
                Globals.Inventory[4] = start_Slot4_ID;
                Globals.Inventory[5] = start_Slot5_ID;
                Globals.Inventory[6] = start_Slot6_ID;
            }
        }
        
        //blink
        if(Globals.activateBlink)
        {
            Globals.activateBlink = false;
            blinkOfCore.SetActive(true);
            bocAnim.SetBool("blinkB", true);
            blinkTimer = 2.6f;
        }

        if(blinkTimer <= 0) {
            if(bocAnim.gameObject.activeInHierarchy)
            {
                if(bocAnim != null && bocAnim.runtimeAnimatorController != null) bocAnim.SetBool("blinkB", false);
            }
            blinkOfCore.SetActive(false);
        }
            

        //bossbar
        if(Globals.BossbarActiveTimer > 0)
        {
            Globals.BossbarActiveTimer -= Time.deltaTime;
            bossBar.gameObject.SetActive(true);
            bossBar.value = Globals.BossHealth;
            bossBar.maxValue = Globals.BossHealthMax;
            if(Globals.BossPhase <= 1 && !Globals.BossMultiplePhases) bossBarHealthText.text = Globals.BossHealth+"/"+Globals.BossHealthMax;
            if(Globals.BossPhase <= 1 && Globals.BossMultiplePhases) bossBarHealthText.text = "<size=20>Фаза "+Globals.BossPhase+"\n</size>"+Globals.BossHealth+"/"+Globals.BossHealthMax;
            if(Globals.BossPhase > 1) bossBarHealthText.text = "<size=20>Фаза "+Globals.BossPhase+"\n</size>"+Globals.BossHealth+"/"+Globals.BossHealthMax;
        }
        if(Globals.BossbarActiveTimer <= 0)
        {
            bossBar.gameObject.SetActive(false);  
        }
        if(Globals.BossHealth < 0) Globals.BossHealth = 0;

        //уведы стори
        if(storyNotifTimer > 0)
        {
            storyNotifAnimator.SetBool("IsActivated", true);
            storyNotifTimer -= Time.deltaTime;
        }
        if(storyNotifTimer <= 0)
        {
            storyNotifAnimator.SetBool("IsActivated", false);
        }

        if(playerai.maxhealth > 99998)
        {
            healthText.text = "<size=18><color=#969696>Dev hacks</size>";
            healthBar.maxValue = playerai.maxhealth;
            healthBar.value = playerai.health;
        } 
        if(Cursor.visible) CheckSlotByCursor();

        //dead panel
        if(!playerai.isDead)
        {
            deathPanel.SetActive(false);
        }
        if(playerai.isDead)
        {
            deathPanel.SetActive(true);
            ResumeGame();
        }


        //pause menu
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        pauseMenu.SetActive(true);
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenu.SetActive(false);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void MenuScene()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Notification(string text, float timer)
    {
        notifiAnim.Stop();
        notifiAnim.Play();
        notifTimer = timer;
        currentDuration = timer;
        notificationText.text = text;
        notifiAnim["notification_ui"].speed = animDur / currentDuration;
    }

    public void StoryNotification(string text, float timer)
    {
        storyNotifTimer = timer;
        storyNotificationText.text = text;
    }

    void MoveToCursor(RectTransform uiElement)
    {
        // Получаем позицию курсора в экранных координатах
        Vector2 cursorPos = Input.mousePosition;

        // Преобразуем экранные координаты в координаты канваса
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiElement.parent.GetComponent<RectTransform>(),
            cursorPos,
            null, // Камера, если используете Canvas с World Space
            out Vector2 localPoint);

        // Устанавливаем позицию UI элемента
        uiElement.anchoredPosition = localPoint;
    }


    void CheckSlotByCursor()
    {
        // Создаем объект PointerEventData с текущей позицией курсора
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);
        
        bool foundShopSlot = false; // Флаг для отслеживания нахождения ShopSlot
        Globals.cursorMoving = true;

        // Проверяем, были ли попадания
        foreach (RaycastResult result in results)
        {
            // Проверяем, является ли элемент слотом с предметом
            ShopSlot shopSlot = result.gameObject.GetComponent<ShopSlot>();
            if (shopSlot != null)
            {
                // Действия, если курсор над ShopSlot
                ItemInfoPanel.gameObject.SetActive(true);
                ItemInfoPanel.position = new Vector2(Input.mousePosition.x + 500f, Input.mousePosition.y);
                SetInfoPanelTextShop(shopSlot);
                foundShopSlot = true; // Устанавливаем флаг
                Globals.cursorMoving = false;
                break; // Выходим из цикла, если нашли ShopSlot
            }
        }

        // Если не нашли ShopSlot, проверяем другие слоты
        if (!foundShopSlot)
        {
            // Сначала отключаем панель, если курсор не над ни одним из слотов
            ItemInfoPanel.gameObject.SetActive(false);

            // Проверяем другие слоты
            if (RectTransformUtility.RectangleContainsScreenPoint(Slot1, Input.mousePosition))
            {
                CheckSlot(Slot1);
            }
            else if (RectTransformUtility.RectangleContainsScreenPoint(Slot2, Input.mousePosition))
            {
                CheckSlot(Slot2);
            }
            else if (RectTransformUtility.RectangleContainsScreenPoint(Slot3, Input.mousePosition))
            {
                CheckSlot(Slot3);
            }
            else if (RectTransformUtility.RectangleContainsScreenPoint(Slot4, Input.mousePosition))
            {
                CheckSlot(Slot4);
            }
            else if (RectTransformUtility.RectangleContainsScreenPoint(Slot5, Input.mousePosition))
            {
                CheckSlot(Slot5);
            }
            else if (RectTransformUtility.RectangleContainsScreenPoint(Slot6, Input.mousePosition))
            {
                CheckSlot(Slot6);
            }
        }
    }

    void CheckSlot(RectTransform slot)
    {
        SlotItems slot_script = slot.GetComponent<SlotItems>();
        if (Globals.Inventory[slot_script.SlotID] != 0)
        {
            SetInfoPanelText(slot_script);
            ItemInfoPanel.gameObject.SetActive(true);
            ItemInfoPanel.position = Input.mousePosition;

            if (Input.GetKeyUp(destroyItemKey))
            {
                Globals.Inventory[slot_script.SlotID] = 0;
                slot_script.ResetItem();
                ItemInfoPanel.gameObject.SetActive(false); // Отключаем панель
                Globals.cursorMoving = false;
            }
        }
    }

    void SetInfoPanelText(SlotItems slot_script)
    {
        string item_name_text = "";
        if(slot_script.itemRarity == 0) item_name_text += "<size=14>Редкость: <color=#d9fffe>Обычный\n<size=20><color=#d9fffe>"+slot_script.itemName;
        if(slot_script.itemRarity == 1) item_name_text += "<size=14>Редкость: <color=#57ff5a>Редкий\n<size=20><color=#57ff5a>"+slot_script.itemName;
        if(slot_script.itemRarity == 2) item_name_text += "<size=15>Редкость: <color=#6554ff>Эпический\n<size=20><color=#6554ff>"+slot_script.itemName;
        if(slot_script.itemRarity == 3) item_name_text += "<size=15>Редкость: <color=#ff5454>Мифический\n<size=20><color=#ff5454>"+slot_script.itemName;
        if(slot_script.itemRarity == 4) item_name_text += "<size=17>Редкость: <color=#ffcb3b>Легендарный\n<size=20><color=#ffcb3b>"+slot_script.itemName;
        
        string text_output = ""+item_name_text+"</color>\n\n<size=14><color=#b0b0b0>Когда предмет активирован:</color></size>\n";
        if(slot_script.setShadowForm) text_output += "</color>Применяет к игроку <color=#b273ff>теневую форму</color>\n";
        if(slot_script.usingTorch) text_output += "</color>Светящийся факел в левой руке</color>\n";
        if(slot_script.damageMultiplier != 0) text_output += "</color>+"+(slot_script.damageMultiplier+1)+" к множителю урона\n";
        if(slot_script.itemSpeedIncrease != 0 && slot_script.itemSpeedIncrease > 0) text_output += "</color>+"+slot_script.itemSpeedIncrease+" к скорости\n";
        if(slot_script.itemSpeedIncrease != 0 && slot_script.itemSpeedIncrease < 0) text_output += "</color>"+slot_script.itemSpeedIncrease+" от скорости\n";
        if(slot_script.itemAttackSpeedMultiplier != 1) text_output += "</color>+"+slot_script.itemAttackSpeedMultiplier+" к множителю скорости удара\n";
        if(slot_script.addHP != 0) text_output += "</color>+"+slot_script.addHP+" к здоровью моментально\n";
        if(slot_script.regenValue > 0)text_output += "</color>Регенерирует "+slot_script.regenValue+" здоровья\n\n";

        if(slot_script.itemEnergyMax > 0f) text_output += "Время использования: "+ slot_script.itemEnergyMax +" сек.\n";
        if(slot_script.oneUse) text_output += "Одноразовый предмет\n";

        text_output += "<size=14><color=#b0b0b0>Нажмите [" + destroyItemKey.ToString() + "] что бы уничтожить предмет</color></size>";
        ItemInfoPanelText.text = text_output;
        slot_script.NewItem = false;

        ItemInfoPanel.sizeDelta = new Vector2(ItemInfoPanel.sizeDelta.x, ItemInfoPanelText.preferredHeight+35f);
    }
    void SetInfoPanelTextShop(ShopSlot shop_script)
    {
        string item_name_text = "";
        if(shop_script.itemRarity == 0) item_name_text += "<size=14>Редкость: <color=#d9fffe>Обычный\n<size=20><color=#d9fffe>"+shop_script.itemName;
        if(shop_script.itemRarity == 1) item_name_text += "<size=14>Редкость: <color=#57ff5a>Редкий\n<size=20><color=#57ff5a>"+shop_script.itemName;
        if(shop_script.itemRarity == 2) item_name_text += "<size=15>Редкость: <color=#6554ff>Эпический\n<size=20><color=#6554ff>"+shop_script.itemName;
        if(shop_script.itemRarity == 3) item_name_text += "<size=15>Редкость: <color=#ff5454>Мифический\n<size=20><color=#ff5454>"+shop_script.itemName;
        if(shop_script.itemRarity == 4) item_name_text += "<size=17>Редкость: <color=#ffcb3b>Легендарный\n<size=20><color=#ffcb3b>"+shop_script.itemName;
        
        string text_output = ""+item_name_text+"</color>\n\n<size=14><color=#b0b0b0>Когда предмет активирован:</color></size>\n";
        if(shop_script.setShadowForm) text_output += "</color>Применяет к игроку <color=#b273ff>теневую форму</color>\n";
        if(shop_script.usingTorch) text_output += "</color>Светящийся факел в левой руке</color>\n";
        if(shop_script.damageMultiplier != 0) text_output += "</color>+"+(shop_script.damageMultiplier+1)+" к множителю урона\n";
        if(shop_script.itemSpeedIncrease != 0 && shop_script.itemSpeedIncrease > 0) text_output += "</color>+"+shop_script.itemSpeedIncrease+" к скорости\n";
        if(shop_script.itemSpeedIncrease != 0 && shop_script.itemSpeedIncrease < 0) text_output += "</color>"+shop_script.itemSpeedIncrease+" от скорости\n";
        if(shop_script.itemAttackSpeedMultiplier != 0) text_output += "</color>+"+shop_script.itemAttackSpeedMultiplier+" к множителю скорости удара\n";
        if(shop_script.addHP != 0) text_output += "</color>+"+shop_script.addHP+" к здоровью моментально\n";
        if(shop_script.regenValue > 0)text_output += "</color>Регенерирует "+shop_script.regenValue+" здоровья\n\n";

        if(shop_script.itemEnergyMax > 0f) text_output += "Время использования: "+ shop_script.itemEnergyMax +" сек.\n";
        if(shop_script.oneUse) text_output += "Одноразовый предмет\n";
        text_output += "\n</color><color=#858585><size=17>Стоимость: \n<size=23><color=#b917ff>"+shop_script.item_cost+" эссенций душ";
        ItemInfoPanelText.text = text_output;

        ItemInfoPanel.sizeDelta = new Vector2(ItemInfoPanel.sizeDelta.x, ItemInfoPanelText.preferredHeight+35f);
    }
}
