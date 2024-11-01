using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


public class PlayerAI : MonoBehaviour
{
    ItemScriptable itemSelectedForName;
    SwordScriptable swordSelectedForName;

    Camera mainCamera;
    NavMeshAgent agent;
    GameObject playerWeapon;
    Animation weaponAnim;
    Animator playerAnimator;
    TrailRenderer trail;
    GameCursor cursor;
    Renderer cursorRenderer;
    AudioSource[] sound;
    public AudioClip hitSound;
    public AudioClip lootSound;

    //переменные
    public float maxhealth = 100;
    public float health = 100;
    public float default_speed = 6;
    public float speed_addition = 0;
    public float attackDelayMultiplier = 1f;
    public float attackDelay;
    private float attackDelayUse;
    public float damageMultiplier;


    // МЕЧ
    public GameObject weapons;
    public WeaponsScript weaponScript;
    public SwordScriptable selectedSword;
    public int SwordID;

    public float damage = 25;
    private float attackDelayDefault = 0.75f;
    public float attackRange = 3.6f;
    public float attackSpawnOffset = 1f;
    public float attackSizeMultip;
    public AudioClip attackSound;
    public GameObject attackPrefab;

    public AnimationClip idleAnim;
    public AnimationClip walkAnim;
    public AnimationClip runAnim;
    public AnimationClip attackAnim;

    public string swordGameobjName;

    //приватные переменные
    private float attackTimer;
    private bool isAttacking;
    public GameObject selectedEnemy;
    private LayerMask enemyLayerMask;
    private LayerMask defaultLayerMask;
    private string messageNotiif;
    private bool isMoving;
    private string ItemSelName;
    private string SwrodSelName;
    private bool attackAnimPlayed;

    public bool shadowForm;
    public int shadowFormsCount;

    public bool useTorch;
    public GameObject torch;

    public bool isDead;

    [Header("Материалы!!")]
    public Material skinMaterial;
    public Material shadowSkinMaterial;
    public Material eyesMaterial;
    public Material shadowEyesMaterial;

    public float attackSizeMultipWidthX; //scale x
    public float attackSizeMultipForwardZ; //scale z
    public float attackSizeMultipHeightY; //scale y

    CameraScript cameraScript;

    void Start()
    {
        cameraScript = FindObjectOfType<CameraScript>();
        isDead = false;
        sound = GetComponents<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        cursor = FindObjectOfType<GameCursor>();
        cursorRenderer = cursor.GetComponent<Renderer>();
        mainCamera = Camera.main;
        Transform playerWeapon = transform.Find("Model/Body/R_HandUp_Bone/R_HandUp/R_Hand_Bone/R_Hand/Handle/Weapon");
        Debug.Log("Player Weapon Finding result: "+playerWeapon);
        //weaponAnim = playerWeapon.GetComponent<Animation>();
        trail = GetComponent<TrailRenderer>();

        useTorch = false;

        Transform playerModel = transform.Find("Model");
        playerAnimator = playerModel.GetComponent<Animator>();

        //maxhealth = 99999;
        //health = 99999;

        enemyLayerMask = LayerMask.GetMask("Enemy");
        defaultLayerMask = LayerMask.GetMask("Default");

        SwordID = 0;
        weaponScript = weapons.GetComponent<WeaponsScript>();
        SelectNewSword(SwordID);
    }

    public void SelectNewSword(int id)
    {
        SwordID = id;
        selectedSword = Resources.Load<SwordScriptable>(Globals.Sword[SwordID]);
        LoadSelectedSwordScriptable();
        weaponScript = weapons.GetComponent<WeaponsScript>();
        Debug.Log("Selected weapon name Player: " + swordGameobjName);
        weaponScript.SelectWeapon(swordGameobjName);
    }
    void LoadSelectedSwordScriptable()
    {
        damage = selectedSword.damage;
        attackDelay = selectedSword.attackDelay;
        attackRange = selectedSword.attackRange;
        attackSpawnOffset = selectedSword.attackSpawnDistance;
        attackSizeMultip = selectedSword.attackSizeMultip;
        attackSound = selectedSword.attackSound;
        attackPrefab = selectedSword.attackPrefab;
        swordGameobjName = selectedSword.swordName;
        attackSizeMultipWidthX = selectedSword.attackSizeMultipWidthX;
        attackSizeMultipHeightY = selectedSword.attackSizeMultipHeightY;
        attackSizeMultipForwardZ = selectedSword.attackSizeMultipForwardZ;

        //idleAnim = selectedSword.idleAnim;
        //walkAnim = selectedSword.walkAnim;
        //runAnim = selectedSword.runAnim;
        //attackAnim = selectedSword.attackAnim;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt) && Application.isEditor)
        {
            Debug.Log("Dev hacks activated!");
            maxhealth = 99999;
            health = 99999;
        }

        if(!useTorch) torch.SetActive(false);
        if(useTorch) torch.SetActive(true);

        agent.speed = default_speed + speed_addition;
        if(attackDelayMultiplier > 0 || attackDelayMultiplier < 30) attackDelayUse = attackDelay / attackDelayMultiplier;
        if(attackDelayUse <= 0.1f) attackDelayUse = 0.115f;
        //else attackDelayMultiplier = 1f;
        if(attackTimer > 99999999) attackTimer = attackDelayDefault;
        if(attackTimer < -1) attackTimer = 0f;

        if (health > maxhealth){
            health = maxhealth;
        }
        if(speed_addition >= 2) trail.emitting = true;
        if(speed_addition < 2) trail.emitting = false;

        if (Globals.menuOpened)
        {
            cursorRenderer.enabled = false;
        }
        if (selectedEnemy == null && !Globals.menuOpened)
        {
            isAttacking = false;
            cursorRenderer.enabled = true;
        }
        if(selectedEnemy != null && !isAttacking)
        {
            EnemyAI sc = selectedEnemy.GetComponent<EnemyAI>();
            if(sc != null){
                if(!sc.isdead)
                {
                    agent.SetDestination(selectedEnemy.transform.position);
                }
                if(sc.isdead)
                {
                    selectedEnemy = null;
                }
            }
        }
        if(selectedEnemy != null && !isDead)
        {
            cursorRenderer.enabled = false;
            Vector3 directionToPlayer = selectedEnemy.transform.position - transform.position;
            directionToPlayer.y = 0; // Убираем изменение по высоте, чтобы враг не наклонялся вверх/вниз
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // 10f — скорость поворота
        }

        if(attackTimer>0)
        {
            attackTimer -= Time.deltaTime;
        }
        if(attackTimer<=0&&isAttacking && !isDead)
        {
            Attack();
        }

        if(Globals.notAddedWarning)
        {
            Globals.notAddedWarning = false;
            UI_Elements uielem = FindObjectOfType<UI_Elements>();
            uielem.Notification("</size><color=red>Inventory is full!</color>",2f);
        }

        if(health<=0 && !isDead)
        {
            isDead = true;
            playerAnimator.SetTrigger("isDead");
            selectedEnemy = null;
            agent.ResetPath();
        }
        if(isDead)
        {
            agent.ResetPath();
            selectedEnemy = null;
            health = 0;
        }


        //Анимации

        // IDLE !weaponAnim.IsPlaying(idleAnim.name) && !weaponAnim.IsPlaying(attackAnim.name)
        if(!isAttacking && !isMoving)
        {
            playerAnimator.SetBool("IsMoving", false);
            playerAnimator.SetBool("IsRunning", false);
            playerAnimator.speed = default_speed / (1f*default_speed);
        }

        //WALK  && !weaponAnim.IsPlaying(walkAnim.name) && !weaponAnim.IsPlaying(attackAnim.name)
        if(!isAttacking && isMoving && speed_addition <= 0)
        {
            playerAnimator.SetBool("IsMoving", true);
            playerAnimator.SetBool("IsRunning", false);
            playerAnimator.speed = default_speed / (0.83f*default_speed);
        }

        //RUN  && !weaponAnim.IsPlaying(runAnim.name) && !weaponAnim.IsPlaying(attackAnim.name)
        if(!isAttacking && isMoving && speed_addition > 0)
        {
            playerAnimator.SetBool("IsMoving", true);
            playerAnimator.SetBool("IsRunning", true);
            playerAnimator.speed = 0.58f + (speed_addition / default_speed);
        }

        if(attackTimer <= 0.03) attackAnimPlayed = false;
        //ATTACK  && !weaponAnim.IsPlaying(attackAnim.name)
        if(attackTimer > 0.03)
        {
            if(!attackAnimPlayed)
            {
                playerAnimator.Play("Player_Attack", 0, 0f);
                attackAnimPlayed = true;
            }
            
            playerAnimator.speed = attackDelayDefault / attackDelayUse;
        }
        //weaponAnim[attackAnim.name].speed = attackDelayDefault / attackDelayUse;

        // Двигаемся, если кнопка мыши зажата, и останавливаемся, если она отжата
        if (Input.GetMouseButton(0) && !isDead && Globals.cursorMoving)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                cursor.isMoving = true;
                cursor.transform.position = hit.point;
                agent.SetDestination(cursor.transform.position);
                isMoving = true;
                Cursor.visible = false;
            }
        }
        
        if(!Globals.cursorMoving)
        {
            isMoving = true;
            Cursor.visible = true;
            cursorRenderer.enabled = false;
        }

        if(!Input.GetMouseButton(0) && !isDead)
        {
            agent.ResetPath();
            cursor.isMoving = false;
            cursorRenderer.enabled = false;
            isMoving = false;
            Cursor.visible = true;
        }

        if(isDead || Globals.menuOpened)
        {
            agent.ResetPath();
            cursor.isMoving = false;
            cursorRenderer.enabled = false;
            isMoving = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void FixedUpdate()
    {
        // Проверка, если выбран враг
        if (selectedEnemy != null && !isDead)
        {
            // Визуализируем сферу для отладки
            Debug.DrawRay(transform.position, Vector3.up * 0.1f, Color.green, 0.5f);  // Короткий луч вверх для визуализации позиции
            // Проверяем, находится ли враг в пределах сферы с радиусом attackRange
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, enemyLayerMask);

            foreach (Collider hit in hits)
            {
                //Debug.Log("Player Attack sphere collide with: " + hit.name);
                //Debug.Log("Player Attack sphere collider gameObject: " + hit.gameObject);
                // Если найденный объект — выбранный враг
                if (hit.gameObject == selectedEnemy && !selectedEnemy.name.Contains("Attack"))
                {
                    // Останавливаем движение и начинаем атаку
                    agent.ResetPath();
                    isAttacking = true;
                    break; // Прерываем цикл, если нашли врага
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        AttackPrefab attackComp = other.GetComponent<AttackPrefab>();
        BoxCollider attackBox = other.GetComponent<BoxCollider>();
        LootBox lootbox = other.GetComponent<LootBox>();
        if(attackComp!=null && attackBox != null && !isDead)
        {
            if(attackComp.playerAttack == false && shadowFormsCount <= 0)
            {
                PlaySound(hitSound, Random.Range(0.95f,1.05f), 1f, 1);
                health -= attackComp.damage/2;
                cameraScript.AddShake(0.1f, 0.32f);
                CreateDamageText(attackComp.damage, 40, 2f);
                SpawnParticles(Resources.Load<GameObject>("Particles/Damage"), false);
                Destroy(attackBox);
            }
        }
        if(lootbox != null && !isDead)
        {
            if(!lootbox.isLooted)
            {
                float messageDuration = 4f;
                messageNotiif = "";
                PlaySound(lootSound, Random.Range(0.95f,1.05f), 1f, 1);
                //if(lootbox.goldCount > 0){
                //    Globals.Money += lootbox.goldCount;
                    //messageNotiif = messageNotiif + "  +" + lootbox.goldCount.ToString() + " gold  ";
                //} 
                if(lootbox.lootItemID != 0){
                    messageDuration += 2f;
                    //проверка заполнен ли инвенарь перед добавлением предмета
                    if(!Globals.CheckInventoryIsFull())
                    {
                        Globals.AddItem(lootbox.lootItemID);

                        itemSelectedForName = Resources.Load<ItemScriptable>(Globals.Item[lootbox.lootItemID]);
                        ItemSelName = itemSelectedForName.itemName;

                        Debug.Log(lootbox.lootItemID);
                        //messageNotiif = " Item by ID: " + lootbox.lootItemID.ToString() + messageNotiif;
                        messageNotiif = " Новый предмет: \"" + ItemSelName + "\"!" + messageNotiif;
                        lootbox.isLooted = true;
                        lootbox.anim.Play();
                    }
                } 
                if(lootbox.lootSwordID != -1){
                    messageDuration += 2f;
                    SelectNewSword(lootbox.lootSwordID);

                    swordSelectedForName = Resources.Load<SwordScriptable>(Globals.Sword[lootbox.lootSwordID]);
                    SwrodSelName = swordSelectedForName.swordGameName;

                    messageNotiif = "Новый меч: <color=#ff8269><size=27>\"" + SwrodSelName + "\"</color></size>! " + messageNotiif;
                    lootbox.isLooted = true;
                    lootbox.anim.Play();
                } 
                
                
                UI_Elements uielem = FindObjectOfType<UI_Elements>(); 
                
                uielem.Notification("<color=#b0b0b0><size=22>Вы нашли: </color></size>" + messageNotiif,messageDuration);
                //тут анимка для лутбокса
                SpawnParticles(Resources.Load<GameObject>("Particles/Money"), false);
            }
        }
        if(other.CompareTag("LevelEnd") && !isDead)
        {
            LevelEnd levelendcomp = other.GetComponent<LevelEnd>();
            if(Globals.completedLevelID < levelendcomp.LevelID) Globals.completedLevelID = levelendcomp.LevelID;
            SceneManager.LoadScene("Menu");
        }
    }

    void Attack()
    {
        attackTimer = attackDelayUse;

        Vector3 directionToPlayer = selectedEnemy.transform.position - transform.position;
        directionToPlayer.y = 0; // Убираем изменение по высоте, чтобы враг не наклонялся вверх/вниз
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 1000f); // 10f — скорость поворота

        Vector3 attackDirection = transform.forward;
        Vector3 attackPosition = transform.position + transform.forward * attackSpawnOffset;
        GameObject attackObj = Instantiate(attackPrefab, attackPosition, Quaternion.LookRotation(attackDirection));
        attackObj.transform.localScale = new Vector3(attackObj.transform.localScale.x * attackSizeMultipWidthX, attackObj.transform.localScale.y * attackSizeMultipHeightY, attackObj.transform.localScale.z * attackSizeMultipForwardZ);
        AttackPrefab attackPrefabComp = attackObj.GetComponent<AttackPrefab>();
        attackPrefabComp.damage = damage * (1+damageMultiplier);
        attackPrefabComp.playerAttack = true;

        PlaySound(attackSound, Random.Range(0.95f,1.05f), 1f, 0);
    }

    void SpawnParticles(GameObject gameobj, bool spawnForward){
        GameObject itemInstance = Instantiate(gameobj, transform.position, Quaternion.identity);
        if(spawnForward) itemInstance.transform.forward = transform.forward;
        if(!spawnForward) itemInstance.transform.forward = transform.up;
    }

    public void PlaySound(AudioClip soundClip, float pitch, float volume, int sourceID)
    {
        sound[sourceID].Stop();
        sound[sourceID].clip = soundClip;
        sound[sourceID].volume = volume * Globals.soundVolume;
        sound[sourceID].pitch = pitch;
        sound[sourceID].Play();
        Debug.Log("Player sound play: " + soundClip + " in pitch " + pitch + " and vol " + volume);
    }

    public void CreateDamageText(float damage_count, float text_size, float destroy_time)
    {
        GameObject dmgTextPrefab = Resources.Load<GameObject>("Prefabs/Scene/Damage");
        GameObject dmgTextInst = Instantiate(dmgTextPrefab);
        DamageText dmgcomp = dmgTextInst.GetComponent<DamageText>();
        dmgTextInst.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        dmgcomp.destroy_time = destroy_time;
        dmgcomp.text_size = text_size;
        dmgcomp.damage_count = damage_count;

        // Заставляем Health Bar всегда быть повернутым к камере
        Vector3 directionToCamera = dmgTextInst.transform.position - mainCamera.gameObject.transform.position;
        Quaternion targetRotation2 = Quaternion.LookRotation(directionToCamera);
        dmgTextInst.transform.rotation = Quaternion.Euler(targetRotation2.eulerAngles.x, targetRotation2.eulerAngles.y, targetRotation2.eulerAngles.z);
    }

    // спасибо чату гпт :)
    public void UpdateMaterials(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Debug.Log("Checking child: " + child.name);

            // Проверяем, есть ли у объекта MeshRenderer
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                // Создаем копию массива материалов
                Material[] materials = meshRenderer.materials;

                // Проходим по каждому материалу объекта
                for (int i = 0; i < materials.Length; i++)
                {
                    // Получаем имя материала, убираем суффикс "(Instance)"
                    string materialName = materials[i].name.Replace(" (Instance)", "");

                    // Если shadowForm активен, заменяем материал Skin на Shadow Skin
                    if (shadowFormsCount > 0 && materialName == skinMaterial.name) materials[i] = shadowSkinMaterial;
                    // Если shadowForm не активен, заменяем материал Shadow Skin на Skin
                    else if (shadowFormsCount <= 0  && materialName == shadowSkinMaterial.name) materials[i] = skinMaterial;

                    // тут уже я дописываю менять глаза
                    if (shadowFormsCount > 0 && materialName == eyesMaterial.name) materials[i] = shadowEyesMaterial;
                    else if (shadowFormsCount <= 0  && materialName == shadowEyesMaterial.name) materials[i] = eyesMaterial;
                }
                // Присваиваем измененный массив материалов обратно в MeshRenderer
                meshRenderer.materials = materials;
            }
            // Проверяем дочерние объекты (рекурсия)
            if (child.childCount > 0) UpdateMaterials(child);
        }
    }
}
