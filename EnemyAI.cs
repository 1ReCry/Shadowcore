using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO.Compression;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemyAI : MonoBehaviour
{
    private GameObject player;
    private PlayerAI playerai;
    private GameObject selectedObj;
    private ParticleSystem deathParticles;
    private GameObject particlesObj;
    public EnemyScriptable selectedScriptable;
    AudioSource[] sound;

    [Header("Настройки Enemy AI")]
    public bool NoAI;
    public bool instantDeathDestroy;
    public bool randomStartRotate = true; 
    [Header("Дальность обнаружения")]
    public float viewRange = 10f; // Дальность обнаружения

    [Header("Настройки свойств")]
    public int maxHealth;
    private float health;
    public float speed;
    public int damage;
    public int moneyReward;
    public Material skinMaterial;
    public Material eyeMaterial;
    public Vector3 modelSize;

    public int phase;
    
    [Header("Настройки атаки")]
    public float attackDelay;
    public GameObject attackPrefab;
    public float attackSpawnOffset;
    public float attackRange = 1.5f;

    public float attackSpawnDelay;
    public float attackSpawnDelayTimer;
    public float attackSize;

    [Header("Настройки Health Bar")]
    public GameObject healthBar;
    public GameObject healthBarFill;

    public AudioClip hitSound;
    public AudioClip attackSound;
    public AudioClip killSound;

    private NavMeshAgent agent;

    // приватные переменные
    private bool isAttacking = false;
    private float attackTimer;
    private Transform mainCamera;
    private float healthPercentage;
    private bool nowIsAttack;

    private Animation anim;
    private Animator animator;

    private bool goToPlayer;

    //анимации
    public AnimationClip animIdle;
    public AnimationClip animWalk;
    public AnimationClip animStartAttack;
    public AnimationClip animAttack;

    private BoxCollider selfCollider;


    public bool isdead;
    public GameObject weapons;
    public WeaponsScript weaponScript;
    public string selectedWeaponName = "null";

    public bool openingDoorsAfterDeath;
    public bool isBoss;

    private bool attackPlayed;
    public bool forceDetectPlayer;

    CameraScript cameraScript;

    void Start()
    {
        if(randomStartRotate) transform.rotation = Quaternion.Euler(transform.rotation.x, Random.Range(0,360), transform.rotation.z);
        LoadFormScriptable();

        player = GameObject.Find("Player");
        playerai = player.GetComponent<PlayerAI>();
        Transform selectedObjFind = transform.Find("Selected");
        selectedObj = selectedObjFind.gameObject;
        selectedObj.transform.localScale = new Vector3(selectedObj.transform.localScale.x * modelSize.x, selectedObj.transform.localScale.y, selectedObj.transform.localScale.z * modelSize.z);
        agent = GetComponent<NavMeshAgent>();

        selfCollider = GetComponent<BoxCollider>();

        agent.speed = speed;
        mainCamera = Camera.main.transform;
        health = maxHealth;

        //Transform particlesObjFinded = transform.Find("Model/Body/DeathParticles");

        Transform skin = transform.Find("Model");
        if (modelSize.x > 0 && modelSize.y > 0 && modelSize.z > 0 && !float.IsNaN(modelSize.x) && !float.IsNaN(modelSize.y) && !float.IsNaN(modelSize.z))
        {
            skin.transform.localScale = modelSize;
        }

        //Renderer skinMat = skin.GetComponent<Renderer>();
        //skinMat.material = skinMaterial;

        sound = GetComponents<AudioSource>();

        Transform modelFind = transform.Find("Model");
        animator = modelFind.GetComponent<Animator>();
        //anim.AddClip(animIdle, animIdle.name);
        //anim.AddClip(animAttack, animAttack.name);
        //anim.AddClip(animWalk, animWalk.name);
        //anim.AddClip(animStartAttack, animStartAttack.name);

        SetModelMaterials();

        healthBar.transform.position = new Vector3(healthBar.transform.position.x, transform.position.y + modelSize.y + 2f, healthBar.transform.position.z);

        weaponScript = weapons.GetComponent<WeaponsScript>();
        Debug.Log("Selected weapon name Enemy: " + selectedWeaponName);
        if(selectedWeaponName != null) weaponScript.SelectWeapon(selectedWeaponName);
        animator.SetBool("bossDeathAnim", selectedScriptable.useBossDeathAnim);

        cameraScript = FindObjectOfType<CameraScript>();
    }

    void SetModelMaterials()
    {
        // Найти все Renderer компоненты в дочерних объектах
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Изменить материал для каждого Renderer
        foreach (Renderer rend in renderers)
        {
            // не назначать если в имени есть
            if (rend.gameObject.name.Contains("HP_Bar") || rend.gameObject.name.Contains("Fill") || rend.gameObject.name.Contains("Selected") || rend.gameObject.name.Contains("DeathParticles") || rend.gameObject.name.Contains("wpngun"))
            {
                //skip
            }
            // назначать
            else if (rend.gameObject.name.Contains("Eye"))
            {
                rend.material = eyeMaterial;
            }
            else
            {
                rend.material = skinMaterial;
            }
        }
    }

    void Update()
    {
        if(!NoAI) DetectPlayer();
        

        /*   скорость анимок, первое число это желаемая длительность, по факту = 1 но я поставил 0.4 что бы
        ускорить анимку   */
        //if(anim.IsPlaying(animIdle.name)) anim[animIdle.name].speed = 1f * (speed/4f);
        //if(anim.IsPlaying(animWalk.name)) anim[animWalk.name].speed = 1f * (speed/4f);
        //if(attackSpawnDelay == 0 && anim.IsPlaying(animStartAttack.name)) anim[animStartAttack.name].speed = 1f;
        //if(attackSpawnDelay != 0 && anim.IsPlaying(animStartAttack.name)) anim[animStartAttack.name].speed = 0.8f / attackSpawnDelay;
        //if(anim.IsPlaying(animAttack.name)) anim[animAttack.name].speed = 1f / attackDelay;

        //скорость новых анимок
        if(animator.GetBool("IsMoving") && !isdead && !isAttacking) animator.speed = 0.5f * (speed/4f);
        if(!animator.GetBool("IsMoving") && !isdead && !isAttacking) animator.speed = 1f * (speed/4f);
        if(animator.GetBool("IsDead") && !isdead) animator.speed = 1f;

        //таймер атаки
        if(attackTimer>0)
        {
            attackTimer -= Time.deltaTime;
        }
        //таймер атаки
        if(attackTimer>0)
        {
            attackPlayed = false;
            animator.SetBool("attackBool", false);
        }
        //атака если таймер <= 0
        if(attackTimer<=0 && isAttacking && !nowIsAttack && !isdead && !attackPlayed)
        {
            if(attackSpawnDelay != 0) animator.SetFloat("startAttackSpeed", 1f / attackSpawnDelay);
            if(attackSpawnDelay == 0) animator.SetFloat("startAttackSpeed", 0.1f);
            animator.SetFloat("attackSpeed", attackDelay / 0.5f);
            animator.SetTrigger("attack");
            animator.SetBool("attackBool", true);

            attackSpawnDelayTimer = attackSpawnDelay;
            nowIsAttack = true;
            attackPlayed = true;
        }

        if(attackSpawnDelayTimer > 0 && nowIsAttack)
        {
            //if(!anim.IsPlaying(animStartAttack.name)){
            //    anim.Stop();
            //    anim.Play(animStartAttack.name);
            //}
            attackSpawnDelayTimer -= Time.deltaTime;
        }
        if(attackSpawnDelayTimer <= 0 && nowIsAttack && !isdead && !playerai.isDead)
        {
            Attack();
        }
        if(nowIsAttack && !isdead)
        {
            agent.ResetPath();
        }

        if(isdead && playerai.selectedEnemy == gameObject)
        {
            playerai.selectedEnemy = null;
        }

        //логика анимок

        //IDLE
        if(!nowIsAttack && !isAttacking && !goToPlayer)
        {
            animator.SetBool("IsMoving", false);
        }
        //WALK
        if(!nowIsAttack && !isAttacking && goToPlayer)
        {
            animator.SetBool("IsMoving", true);
        }
        if(isAttacking)
        {
            animator.SetBool("IsMoving", false);
        }
        if(isAttacking && !nowIsAttack && !goToPlayer)
        {
            animator.SetBool("IsMoving", false);
        }

        if(playerai.selectedEnemy != gameObject) selectedObj.SetActive(false);
        if(playerai.selectedEnemy == gameObject) selectedObj.SetActive(true);

        if(health<=0 && !isdead)
        {
            cameraScript.AddShake(0.1f, 0.2f);
            isdead = true;
            agent.enabled = false;
            animator.speed = 1f;
            SpawnParticles(Resources.Load<GameObject>("Particles/BloodBoom"), false);
            PlaySound(killSound, Random.Range(0.95f,1.05f), 1f, 0);

            if(openingDoorsAfterDeath) Globals.openDoorsRequest = true;
            if(selectedScriptable.storyNotifAfterDeath){
                UI_Elements uielem = FindObjectOfType<UI_Elements>(); 
                PlaySound(selectedScriptable.storyNotifSound, 1f, 0.8f, 3);
                uielem.StoryNotification("<color=#52c8ff><size=24>"+selectedScriptable.storyNotifPrefix+"</size></color>\n"+selectedScriptable.storyNotifText, selectedScriptable.storyNotifTimer);
            }
            selfCollider.enabled = false;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            healthBar.SetActive(false);

            Globals.Money += moneyReward;
            animator.SetBool("IsDead", true);
            if(instantDeathDestroy) Destroy(gameObject);
            if(!animator.GetBool("bossDeathAnim")) Destroy(gameObject, 10f);
            if(animator.GetBool("bossDeathAnim")) Destroy(gameObject, 11f);
        }

        if(isAttacking && !isdead)
        {
            // Поворачиваем врага к игроку
            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.y = 0; // Убираем изменение по высоте, чтобы враг не наклонялся вверх/вниз
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // 10f — скорость поворота
        }

        //bossbar
        if(isBoss && !isdead)
        {
            healthBar.gameObject.SetActive(false);
            if(goToPlayer || isAttacking)
            {
                Globals.BossbarActiveTimer = 1.6f;
                Globals.BossHealthMax = maxHealth;
            }
        }
        if(isBoss)
        {
            if(goToPlayer || isAttacking)
            {
                Globals.BossHealth = health;
                Globals.BossPhase = phase;
                if(selectedScriptable.phasesCount <= 1) Globals.BossMultiplePhases = false;
                if(selectedScriptable.phasesCount > 1) Globals.BossMultiplePhases = true;
            }
        }

        // хп бар
        healthPercentage = (float)health / (float)maxHealth;
        //Debug.Log("HP " + health + "  Max HP " + maxHealth + "  HP Peracentage " + healthPercentage);
        healthBarFill.transform.localScale = new Vector3(healthPercentage*0.9f, 0.5f, 2f);
        healthBarFill.transform.localPosition = new Vector3(-(1 - healthPercentage*0.9f) / 2, healthBarFill.transform.localPosition.y, healthBarFill.transform.localPosition.z);

        // Заставляем Health Bar всегда быть повернутым к камере
        Vector3 directionToCamera = healthBar.transform.position - mainCamera.position;
        Quaternion targetRotation2 = Quaternion.LookRotation(directionToCamera);
        healthBar.transform.rotation = Quaternion.Euler(targetRotation2.eulerAngles.x, targetRotation2.eulerAngles.y, targetRotation2.eulerAngles.z);
        healthBarFill.transform.rotation = Quaternion.Euler(targetRotation2.eulerAngles.x, targetRotation2.eulerAngles.y, targetRotation2.eulerAngles.z);




        //Фазы
        if(selectedScriptable.phasesCount == 1 || selectedScriptable.phasesCount == 0)
        {
            attackPrefab = selectedScriptable.attackPrefab;
        }
        if(selectedScriptable.phasesCount == 2)
        {
            if(health > maxHealth / 2)
            {
                attackPrefab = selectedScriptable.attackPrefab;
                phase = 1;
            }
            else if(health <= maxHealth / 2){
                attackPrefab = selectedScriptable.Attack_phase2;
                attackRange = selectedScriptable.attackRangePhase2;
                attackDelay = selectedScriptable.attackDelayPhase2;
                damage = selectedScriptable.attackDamagePhase2;
                speed = selectedScriptable.speedPhase2;
                agent.speed = speed;
                phase = 2;
            } 
        }
        if(selectedScriptable.phasesCount == 3)
        {
            // Если 3 фазы, проверяем на каждую треть здоровья
            if (health > 2 * maxHealth / 3)
            {
                attackPrefab = selectedScriptable.attackPrefab;
                phase = 1;
            }
            else if (health > maxHealth / 3){
                attackPrefab = selectedScriptable.Attack_phase2;
                attackRange = selectedScriptable.attackRangePhase2;
                attackDelay = selectedScriptable.attackDelayPhase2;
                damage = selectedScriptable.attackDamagePhase2;
                speed = selectedScriptable.speedPhase2;
                agent.speed = speed;
                phase = 2;
            } 
            else{
                attackPrefab = selectedScriptable.Attack_phase3;
                attackRange = selectedScriptable.attackRangePhase3;
                attackDelay = selectedScriptable.attackDelayPhase3;
                damage = selectedScriptable.attackDamagePhase3;
                speed = selectedScriptable.speedPhase3;
                agent.speed = speed;
                phase = 3;
            } 
        }
        if(selectedScriptable.phasesCount == 4)
        {
            // Если 4 фазы, проверяем на каждую четверть здоровья
            if (health > 3 * maxHealth / 4)
            {
                attackPrefab = selectedScriptable.attackPrefab;
                phase = 1;
            }
            else if (health > 2 * maxHealth / 4){
                attackPrefab = selectedScriptable.Attack_phase2;
                attackRange = selectedScriptable.attackRangePhase2;
                attackDelay = selectedScriptable.attackDelayPhase2;
                damage = selectedScriptable.attackDamagePhase2;
                speed = selectedScriptable.speedPhase2;
                agent.speed = speed;
                phase = 2;
            } 
            else if (health > maxHealth / 4){
                attackPrefab = selectedScriptable.Attack_phase3;
                attackRange = selectedScriptable.attackRangePhase3;
                attackDelay = selectedScriptable.attackDelayPhase3;
                damage = selectedScriptable.attackDamagePhase3;
                speed = selectedScriptable.speedPhase3;
                agent.speed = speed;
                phase = 3;
            } 
            else{
                attackPrefab = selectedScriptable.Attack_phase4;
                attackRange = selectedScriptable.attackRangePhase4;
                attackDelay = selectedScriptable.attackDelayPhase4;
                damage = selectedScriptable.attackDamagePhase4;
                speed = selectedScriptable.speedPhase4;
                agent.speed = speed;
                phase = 4;
            } 
        }
    }

    void DetectPlayer()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Проверяем, находится ли игрок в пределах диапазона обнаружения
        if (distanceToPlayer <= viewRange && !isdead && !playerai.isDead)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up, directionToPlayer);
            Debug.DrawRay(ray.origin, ray.direction * viewRange, Color.red);

            // Проводим Raycast для проверки видимости игрока
            if (Physics.Raycast(ray, out hit, viewRange))
            {
                if (hit.transform == player.transform)
                {
                    // Если Raycast попал в игрока, проверяем дистанцию атаки
                    if (distanceToPlayer <= attackRange){
                        isAttacking = true;
                        goToPlayer = false;
                        agent.ResetPath();
                    }
                    else if (distanceToPlayer <= viewRange){
                        // Если игрок вне диапазона атаки, устанавливаем путь к игроку
                        isAttacking = false;
                        goToPlayer = true;
                        agent.SetDestination(player.transform.position);
                    }
                }
            }
        }
        else
        {
            // Если игрок вне диапазона обнаружения, сбрасываем путь
            if(!isdead && !forceDetectPlayer) 
            {
                agent.ResetPath();
                isAttacking = false;
                goToPlayer = false;
            }
        }

        if(forceDetectPlayer && !playerai.isDead)
        {
            goToPlayer = true;
            agent.SetDestination(player.transform.position);
        }
        if(isdead && forceDetectPlayer) 
        {
            agent.ResetPath();
            isAttacking = false;
            goToPlayer = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            isAttacking = true;
            agent.ResetPath();
        }

        AttackPrefab attackComp = other.gameObject.GetComponent<AttackPrefab>();
        //BoxCollider attackBox = other.gameObject.GetComponent<BoxCollider>();
        if(attackComp!=null)
        {
            if(attackComp.playerAttack == true)
            {
                SpawnParticles(Resources.Load<GameObject>("Particles/Damage"), false);
                PlaySound(hitSound, Random.Range(0.95f,1.05f), 1f, 0);
                health -= attackComp.damage;
                CreateDamageText(attackComp.damage, 50, 2f);
                //attackBox.enabled = false;
                Debug.Log(gameObject.name + " Get " + attackComp.damage.ToString() + " damage");
            }
        }
    }

    /*void OnCollisionEnter(Collision other)
    {

    }*/

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isAttacking = false;
            DetectPlayer();
        }
    }

    void Attack()
    {
        //if(!anim.IsPlaying(animAttack.name)){
        //    anim.Stop();
        //    anim.Play(animAttack.name);
        //}
        nowIsAttack = false;
        attackTimer = attackDelay;
        animator.speed = 0.5f / attackDelay;
        animator.Play("Enemy_Attack");

        PlaySound(attackSound, Random.Range(0.95f,1.05f), 0.6f, 1);

        Vector3 attackDirection = transform.forward;
        Vector3 attackPosition = transform.position + transform.forward * attackSpawnOffset;
        GameObject attackObj = Instantiate(attackPrefab, attackPosition, Quaternion.LookRotation(attackDirection));
        attackObj.transform.localScale = new Vector3(attackObj.transform.localScale.x * selectedScriptable.attackSizeMultipWidthX, attackObj.transform.localScale.y * selectedScriptable.attackSizeMultipHeightY, attackObj.transform.localScale.z * selectedScriptable.attackSizeMultipForwardZ);
        AttackPrefab attackPrefabComp = attackObj.GetComponent<AttackPrefab>();
        attackPrefabComp.damage = damage;
        attackPrefabComp.playerAttack = false;
    }

    void LoadFormScriptable()
    {
        viewRange = selectedScriptable.viewRange;
        maxHealth = selectedScriptable.maxHealth;
        speed = selectedScriptable.speed;
        damage = selectedScriptable.damage;
        moneyReward = selectedScriptable.moneyReward;
        skinMaterial = selectedScriptable.skinMaterial;
        eyeMaterial = selectedScriptable.eyesMaterial;
        attackDelay = selectedScriptable.attackDelay;
        attackPrefab = selectedScriptable.attackPrefab;
        attackSpawnOffset = selectedScriptable.attackSpawnOffset;
        attackRange = selectedScriptable.attackRange;
        hitSound = selectedScriptable.hitSound;
        attackSound = selectedScriptable.attackSound;
        killSound = selectedScriptable.killSound;
        attackSpawnDelay = selectedScriptable.attackSpawnDelay;
        attackSize = selectedScriptable.attackPrefabSizeMultip;
        modelSize = selectedScriptable.modelSize;
        openingDoorsAfterDeath = selectedScriptable.openAllDoorsAfterDeath;
        isBoss = selectedScriptable.isBoss;
        //animIdle = selectedScriptable.animIdle;
        //animWalk = selectedScriptable.animWalk;
        //animStartAttack = selectedScriptable.animStartAttack;
        //animAttack = selectedScriptable.animAttack;
        if(selectedScriptable.weapon != null) selectedWeaponName = selectedScriptable.weapon.swordName;
        if(selectedScriptable.weapon == null) selectedWeaponName = "null weapon!";
        if(selectedScriptable.attackSizeMultipWidthX == 0) selectedScriptable.attackSizeMultipWidthX = selectedScriptable.attackPrefabSizeMultip;
        if(selectedScriptable.attackSizeMultipHeightY == 0) selectedScriptable.attackSizeMultipWidthX = selectedScriptable.attackPrefabSizeMultip;
        if(selectedScriptable.attackSizeMultipForwardZ == 0) selectedScriptable.attackSizeMultipWidthX = selectedScriptable.attackPrefabSizeMultip;
    }

    void SpawnParticles(GameObject gameobj, bool spawnForward){
        GameObject itemInstance = Instantiate(gameobj, transform.position, Quaternion.identity);
        if(spawnForward) itemInstance.transform.forward = transform.forward;
        if(!spawnForward) itemInstance.transform.forward = transform.up;
    }

    void PlaySound(AudioClip soundClip, float pitch, float volume, int id)
    {
        sound[id].Stop();
        sound[id].clip = soundClip;
        sound[id].volume = volume * Globals.soundVolume;
        sound[id].pitch = pitch;
        sound[id].Play();
    }

    public void CreateDamageText(float damage_count, float text_size, float destroy_time)
    {
        GameObject dmgTextPrefab = Resources.Load<GameObject>("Prefabs/Scene/Damage");
        GameObject dmgTextInst = Instantiate(dmgTextPrefab);
        DamageText dmgcomp = dmgTextInst.GetComponent<DamageText>();
        dmgTextInst.transform.position = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z);
        dmgcomp.destroy_time = destroy_time;
        dmgcomp.text_size = text_size;
        dmgcomp.damage_count = damage_count;

        // Заставляем Health Bar всегда быть повернутым к камере
        Vector3 directionToCamera = dmgTextInst.transform.position - mainCamera.gameObject.transform.position;
        Quaternion targetRotation2 = Quaternion.LookRotation(directionToCamera);
        dmgTextInst.transform.rotation = Quaternion.Euler(targetRotation2.eulerAngles.x, targetRotation2.eulerAngles.y, targetRotation2.eulerAngles.z);
    }
}
