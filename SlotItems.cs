using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SlotItems : MonoBehaviour
{
    //настройки слота
    public int SlotID;
    public KeyCode SlotKeycode;

    //все компоненты
    private UnityEngine.UI.Image ItemImageComp;
    private UnityEngine.UI.Slider slider;
    private Sprite ItemSprite;
    private ItemScriptable selectedItem;
    private GameObject player;
    private PlayerAI playerai;
    private UnityEngine.UI.Image fillImage;
    private TextMeshProUGUI bindText;
    private GameObject newNotif;
    public bool NewItem;

    //переменные Item
    private int selectedItemID;
    public string itemName;
    private bool instantlyActivate;

    private float itemEnergy;
    public float itemEnergyMax;
    private float itemEnergyRegenDelay;
    private float itemEnergyRegenDelayMax;
    private float itemEnergyRegenMultiplier;
    private bool energyMultipToSeconds;
    private float energyRegenSeconds;

    public float itemSpeedIncrease;
    public float itemAttackSpeedMultiplier;

    private GameObject itemSpawnPrefab;
    private GameObject itemSpawnProjectile;
    private bool itemSpawnForward;
    public float addHP;
    private float regenerationTimer;
    private float regenerationSpeed;
    public bool regenerationEnabled;
    private AudioClip itemSoundActivate;
    private AudioClip itemSoundDeactivate;
    private float soundVolume;

    public float damageMultiplier;
    public bool setShadowForm;

    //активен ли предмет
    public bool isActived;
    public bool readyToUse;
    public bool oneUse;
    public bool useEnabledAfterCharge;
    

    public bool itemResetAlready = false;

    public float regenValue;
    public int itemRarity;

    public bool usingTorch;


    void Start()
    {
        Globals.Inventory[SlotID] = 0;
        Transform ItemSpriteFind = transform.Find("SlotItemSprite");
        ItemImageComp = ItemSpriteFind.GetComponent<UnityEngine.UI.Image>();
        Transform sliderFind = transform.Find("Slider");
        slider = sliderFind.GetComponent<UnityEngine.UI.Slider>();
        Transform newIco = transform.Find("New");
        newNotif = newIco.gameObject;
        fillImage = slider.fillRect.GetComponent<UnityEngine.UI.Image>();
        player = GameObject.Find("Player");
        playerai = player.GetComponent<PlayerAI>();
        fillImage.color = new Color(38f / 255f, 114f / 255f, 255f / 255f);
        Transform bindTextFind = transform.Find("Bind");
        bindText = bindTextFind.GetComponent<TextMeshProUGUI>();

        SetItemParameters();

        string bindkeyname = SlotKeycode.ToString();
        bindkeyname = bindkeyname.Replace("Alpha", "");
        bindText.text = bindkeyname;
    }

    void SetItemParameters(){

        //загрузка предметов по ID
        if(Globals.Inventory[SlotID] == 0 && !itemResetAlready) ResetItem();
        if(Globals.Inventory[SlotID] != 0){
            selectedItem = Resources.Load<ItemScriptable>(Globals.Item[Globals.Inventory[SlotID]]);
        } 

        //загрузить все параметры если ID не 0
        if(Globals.Inventory[SlotID] != 0){
            LoadScriptableItem();
        }
    }

    void Update()
    {
        if(Globals.UpdateInventorySignal)
        {
            //ResetItem();
            SetItemParameters();
            //Globals.UpdateInventorySignal = false;
        }
        if(Globals.SetMaxItemEnergyInCurSlot && Globals.selectedSlotIDinscript == SlotID)
        {
            itemEnergy = itemEnergyMax;
            itemEnergyRegenDelay = 0f;
            Globals.SetMaxItemEnergyInCurSlot = false;
        }

        //new item
        if(Globals.newItemInSlodID == SlotID)
        {
            Globals.newItemInSlodID = 0;
            NewItem = true;
        }
        if(!NewItem) newNotif.SetActive(false);
        if(NewItem) newNotif.SetActive(true);
        
        //Значения на Slider
        slider.maxValue = itemEnergyMax;
        slider.value = itemEnergy;

        // READY TO USE
        if(itemEnergy<=0 && !isActived)
        {
            readyToUse = false;
        }

        if(itemEnergy>0 && !isActived && !useEnabledAfterCharge)
        {
            readyToUse = true;
        }
        if(itemEnergy>itemEnergyMax-0.005f && useEnabledAfterCharge)
        {
            readyToUse = true;
        }


        if(!readyToUse && !isActived)
        {
            fillImage.color = new Color(100f / 255f, 100f / 255f, 100f / 255f);
        }
        if(readyToUse && !isActived)
        {
            fillImage.color = new Color(38f / 255f, 114f / 255f, 255f / 255f);
        }
        if(isActived)
        {
            fillImage.color = new Color(110f / 255f, 255f / 255f, 119f / 255f);
        }
        

        //Регенерация энергии у предметов
        if(itemEnergyRegenDelay > 0 && !isActived){
            itemEnergyRegenDelay -= Time.deltaTime;
        }
        if(itemEnergyRegenDelay <= 0 && itemEnergy < itemEnergyMax){
            if(!energyMultipToSeconds) itemEnergy += Time.deltaTime * itemEnergyRegenMultiplier;
            if(energyMultipToSeconds) itemEnergy += Time.deltaTime * (itemEnergyMax / energyRegenSeconds);
        }

        //ограничение энергии до максимальной
        if(itemEnergy > itemEnergyMax)
        {
            itemEnergy = itemEnergyMax;
        }
        //if(itemEnergy <= 0) fillImage.color = new Color(38f / 255f, 38f / 38f, 255f / 38f);

        //если активен отнимать энергию и ставить кд на реген на макс
        if(isActived){
            itemEnergy -= Time.deltaTime;
            itemEnergyRegenDelay = itemEnergyRegenDelayMax;
            regenerationTimer -= Time.deltaTime;
            if(regenerationTimer <= 0 && regenerationEnabled){
                playerai.health += 1;
                regenerationTimer = regenerationSpeed;
            } 
        }
        
        //активация предмета по бинду
        if(Input.GetKeyDown(SlotKeycode) && Globals.Inventory[SlotID] != 0 && itemEnergy > 0){
            if(useEnabledAfterCharge && itemEnergy >= itemEnergyMax - 0.005f) ActivateSlot();
            if(!useEnabledAfterCharge) ActivateSlot();
        }

        //деактивация предмета
        if(Input.GetKeyUp(SlotKeycode) && !instantlyActivate){
            DeactivateSlot();
        }
        if(itemEnergy <= 0){
            DeactivateSlot();
        }

        //other
        if(Globals.Inventory[SlotID] == 0 && !itemResetAlready){
            //ResetItem();
        }

        if(Input.GetKeyDown(SlotKeycode)) bindText.color =  Color.green;
        if(Input.GetKeyUp(SlotKeycode)) bindText.color =  Color.white;
    }

    // Активация и деактивация слота
    void ActivateSlot(){
        if(!isActived && !playerai.isDead){
            isActived = true;
            playerai.PlaySound(itemSoundActivate, Random.Range(0.97f,1.02f), soundVolume, 2);
            regenerationTimer = 0;
            if(itemSpeedIncrease != 0) playerai.speed_addition += itemSpeedIncrease;
            if(itemAttackSpeedMultiplier != 0) playerai.attackDelayMultiplier += itemAttackSpeedMultiplier-1;
            playerai.health += addHP-1;
            playerai.damageMultiplier +=damageMultiplier;
            if(setShadowForm == true) {
                playerai.shadowFormsCount += 1;
                playerai.UpdateMaterials(playerai.gameObject.transform);
            }
            if(usingTorch) playerai.useTorch = true;
            
            SpawnPrefabs();

            if(oneUse){
                Globals.Inventory[SlotID] = 0;
                ResetItem();
                Debug.Log("One use item reset!");
            }
        }
    }
    void DeactivateSlot(){
        if(isActived){
            isActived = false;
            if(usingTorch) playerai.useTorch = false;
            playerai.PlaySound(itemSoundDeactivate, Random.Range(0.97f,1.02f), soundVolume, 3);
            playerai.speed_addition -= itemSpeedIncrease;
            if(itemAttackSpeedMultiplier != 0) playerai.attackDelayMultiplier -= itemAttackSpeedMultiplier-1;
            playerai.damageMultiplier -=damageMultiplier;
            if(setShadowForm == true){
                playerai.shadowFormsCount -= 1;
                playerai.UpdateMaterials(playerai.gameObject.transform);
                SpawnPrefabs();
            }
            
        }
    }

    //загрузка предмета из ScriptableItem
    void LoadScriptableItem(){
        itemResetAlready = false;
        ItemSprite = selectedItem.itemSprite;
        instantlyActivate = selectedItem.instantlyActivate;
        itemEnergyMax = selectedItem.energyMax;
        itemEnergyRegenDelayMax = selectedItem.energyRegenDelayMax;
        itemEnergyRegenMultiplier = selectedItem.energyRegenMultiplier;
        itemSpeedIncrease = selectedItem.speedIncrease;
        itemAttackSpeedMultiplier = selectedItem.attackSpeedMultiplier;
        itemSpawnPrefab = selectedItem.spawnPrefab;
        itemSpawnProjectile = selectedItem.spawnProjectile;
        addHP = selectedItem.addHP;
        regenerationSpeed = selectedItem.energyMax / selectedItem.regenerateHP;
        regenValue = selectedItem.regenerateHP;
        oneUse = selectedItem.oneUse;
        itemName = selectedItem.itemName;
        itemSoundActivate = selectedItem.itemSoundActivate;
        itemSoundDeactivate = selectedItem.itemSoundDeactivate;
        soundVolume = selectedItem.soundVolume;
        damageMultiplier = selectedItem.damageMultiplier;
        setShadowForm = selectedItem.setShadowForm;
        useEnabledAfterCharge = selectedItem.useEnabledAfterCharge;
        energyMultipToSeconds = selectedItem.energyMultipToSeconds;
        energyRegenSeconds = selectedItem.energyRegenSeconds;
        itemRarity = selectedItem.itemRarity;
        usingTorch = selectedItem.usingTorch;
        itemSpawnForward = selectedItem.SpawnForward;

        ItemImageComp.sprite = ItemSprite;

        if(regenerationSpeed > 0) regenerationEnabled = true;
        if(itemAttackSpeedMultiplier == 0) itemAttackSpeedMultiplier = 1;
        if(oneUse) slider.gameObject.SetActive(false);
        if(!oneUse) slider.gameObject.SetActive(true);
    }

    void SpawnPrefabs(){
        if (itemSpawnPrefab != null){
            GameObject itemInstance = Instantiate(itemSpawnPrefab, player.transform.position, Quaternion.identity);
            if(itemSpawnForward) itemInstance.transform.forward = player.transform.forward;
            if(!itemSpawnForward) itemInstance.transform.forward = player.transform.up;
        }
        if (itemSpawnProjectile != null){
            GameObject itemInstance = Instantiate(itemSpawnProjectile, player.transform.position, Quaternion.identity);
            if(itemSpawnForward) itemInstance.transform.forward = player.transform.forward;
            if(!itemSpawnForward) itemInstance.transform.forward = player.transform.up;
        }
    }

    public void ResetItem()
    {
        if(!itemResetAlready)
        {
            DeactivateSlot();
            NewItem = false;
            itemResetAlready = true;
            //selectedItem = null;
            ItemSprite = null;
            ItemImageComp.sprite = Resources.Load<Sprite>("Images/emptySlot");
            slider.gameObject.SetActive(false);
            itemEnergy = 1;
            itemEnergyRegenDelay = 1;
            itemSpawnPrefab = null;
            itemSpawnProjectile = null;
            SetItemParameters();
            Debug.Log("Reset item: Inventory id in slot " + SlotID + " = " + Globals.Inventory[SlotID]);
        }
    }
}
