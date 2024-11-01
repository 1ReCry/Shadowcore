using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopSlot : MonoBehaviour, IPointerClickHandler
{
    public int itemID;
    
    private ItemScriptable item_scriptable;
    private Sprite itemSprite;
    private UnityEngine.UI.Image itemImageComp;
    public int item_cost;
    private TextMeshProUGUI cost_text;

    //Стена переменных характеристик предмета (извинитесь)
    private int item_scriptableID;
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
    public float addHP;
    private float regenerationTimer;
    private float regenerationSpeed;
    public bool regenerationEnabled;
    public float damageMultiplier;
    public bool setShadowForm;
    public bool isActived;
    public bool oneUse;
    public float regenValue;
    public int itemRarity;
    public bool usingTorch;

    // Start is called before the first frame update
    void Start()
    {
        Transform imageFind = transform.Find("ItemSprite");
        itemImageComp = imageFind.GetComponent<UnityEngine.UI.Image>();

        Transform cost_text_find = transform.Find("CostText");
        cost_text = cost_text_find.GetComponent<TextMeshProUGUI>();

        item_scriptable = Resources.Load<ItemScriptable>(Globals.Item[itemID]);

        LoadItem();
        LoadItemParameters();
    }

    void LoadItem()
    {
        itemSprite = item_scriptable.itemSprite;
        item_cost = item_scriptable.survivalCost;

        itemImageComp.sprite = itemSprite;
        cost_text.text = item_cost.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(Globals.Money >= item_cost) BuyItem();
        Debug.Log("Buy Item!");
    }

    void BuyItem()
    {
        Globals.Money -= item_cost;
        Globals.AddItem(itemID);
    }

    void LoadItemParameters()
    {
        instantlyActivate = item_scriptable.instantlyActivate;
        itemEnergyMax = item_scriptable.energyMax;
        itemEnergyRegenDelayMax = item_scriptable.energyRegenDelayMax;
        itemEnergyRegenMultiplier = item_scriptable.energyRegenMultiplier;
        itemSpeedIncrease = item_scriptable.speedIncrease;
        itemAttackSpeedMultiplier = item_scriptable.attackSpeedMultiplier;
        addHP = item_scriptable.addHP;
        regenerationSpeed = item_scriptable.energyMax / item_scriptable.regenerateHP;
        regenValue = item_scriptable.regenerateHP;
        oneUse = item_scriptable.oneUse;
        itemName = item_scriptable.itemName;
        damageMultiplier = item_scriptable.damageMultiplier;
        setShadowForm = item_scriptable.setShadowForm;
        energyMultipToSeconds = item_scriptable.energyMultipToSeconds;
        energyRegenSeconds = item_scriptable.energyRegenSeconds;
        itemRarity = item_scriptable.itemRarity;
        usingTorch = item_scriptable.usingTorch;
    }
}
