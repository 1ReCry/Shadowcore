using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoreAI : MonoBehaviour
{
    [Header("Свойства")]
    public float health;
    public float healthMax;
    public float detectRange;
    public float spawnerTimerPhase1;
    public float spawnerTimerPhase2;
    public float spawnerTimerPhase3;
    public float heal_by_towers;
    public bool endCutsceneAfterDeath;
    public float to_cutscene_timer;
    [Header("Привязки спавнер поинтов")]
    public GameObject spawner_point1;
    public GameObject spawner_point2;
    public GameObject spawner_point3;
    public GameObject spawner_point4;
    [Header("Враги для спавна")]
    public GameObject enemy1Prefab;
    public GameObject enemy2Prefab;
    public GameObject enemy3Prefab;
    [Header("Партиклы спавна")]
    public GameObject spawnParticles;

    [Header("\nПривязки хил башен")]
    public GameObject heal_tower1;
    public GameObject heal_tower2;
    public GameObject heal_tower3;
    public GameObject heal_tower4;

    private int phase = 1;
    private bool playerOnceDetected;
    private bool deadAlready;
    

    private PlayerAI playerai;
    private GameObject mainCamera;
    private CameraScript cameraScript;
    private float spawnerTimer;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        spawnerTimer = spawnerTimerPhase1 / 2;
        playerai = FindObjectOfType<PlayerAI>();
        mainCamera = Camera.main.gameObject;
        cameraScript = mainCamera.GetComponent<CameraScript>();
    }

    void Update()
    {
        if(!playerOnceDetected)
        {
            DetectPlayer();
        }
        if(playerOnceDetected)
        {
            Globals.BossHealth = (int)health;
            Globals.BossHealthMax = healthMax;
            Globals.BossPhase = phase;
            Globals.BossbarActiveTimer = 2f;
            Globals.BossMultiplePhases = true;
        }

        if(Input.GetKeyDown(KeyCode.C) && Application.isEditor)
        {
            health /= 2;
        }

        if(spawnerTimer > 0 && playerOnceDetected && !playerai.isDead)
        {
            spawnerTimer -= Time.deltaTime;
        }
        if(spawnerTimer <= 0)
        {
            if(phase == 1)
            {
                spawnerTimer = spawnerTimerPhase1;
                SpawnEnemy(spawner_point1, enemy1Prefab);
                SpawnEnemy(spawner_point2, enemy1Prefab);
            }
            if(phase == 2)
            {
                spawnerTimer = spawnerTimerPhase2;
                SpawnEnemy(spawner_point1, enemy2Prefab);
                SpawnEnemy(spawner_point2, enemy2Prefab);
            }
            if(phase == 3)
            {
                spawnerTimer = spawnerTimerPhase3;
                SpawnEnemy(spawner_point1, enemy2Prefab);
                SpawnEnemy(spawner_point2, enemy2Prefab);
                SpawnEnemy(spawner_point3, enemy2Prefab);
                SpawnEnemy(spawner_point4, enemy3Prefab);
            }
        }

        if(health < healthMax)
        {
            if(heal_tower1 != null)
            {
                health += heal_by_towers * Time.deltaTime;
                Debug.Log("Heal by tower 1  " + heal_by_towers * Time.deltaTime);
            }
            if(heal_tower2 != null)
            {
                health += heal_by_towers * Time.deltaTime;
                Debug.Log("Heal by tower 2  " + heal_by_towers * Time.deltaTime);
            }
            if(heal_tower3 != null)
            {
                health += heal_by_towers * Time.deltaTime;
                Debug.Log("Heal by tower 3  " + heal_by_towers * Time.deltaTime);
            }
            if(heal_tower4 != null)
            {
                health += heal_by_towers * Time.deltaTime;
                Debug.Log("Heal by tower 4  " + heal_by_towers * Time.deltaTime);
            }
        }
        else
        {
            health = healthMax;
        }

        if(health <= 0)
        {
            deadAlready = true;
        }
        if(deadAlready)
        {
            if(endCutsceneAfterDeath && to_cutscene_timer>0)
            {
                health = 0f;
                Time.timeScale = 0.1f;
            }
            else if(endCutsceneAfterDeath && to_cutscene_timer <=0)
            {
                Globals.completedLevelID += 1;
                Time.timeScale = 1f;
                SceneManager.LoadScene("EndCutscene");
            }

            if(!endCutsceneAfterDeath)
            {
                Destroy(gameObject);
            }

            if(endCutsceneAfterDeath && to_cutscene_timer > 0)
            {
                to_cutscene_timer -= Time.deltaTime;
                cameraScript.AddShake(0.3f,0.01f);
            }
        }

        if(heal_tower1 != null || heal_tower2 != null || heal_tower3 != null || heal_tower4 != null)
        {
            phase = 1;
        }
        else if(heal_tower1 == null && heal_tower2 == null && heal_tower3 == null && heal_tower4 == null && health > healthMax/2)
        {
            phase = 2;
        }
        else if(heal_tower1 == null && heal_tower2 == null && heal_tower3 == null && heal_tower4 == null && health <= healthMax/2)
        {
            phase = 3;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        AttackPrefab attackComp = other.gameObject.GetComponent<AttackPrefab>();
        //BoxCollider attackBox = other.gameObject.GetComponent<BoxCollider>();
        if(attackComp!=null)
        {
            if(attackComp.playerAttack == true)
            {
                health -= attackComp.damage;
                animator.SetTrigger("Damaged");
                CreateDamageText(attackComp.damage, 50, 2f);
                cameraScript.AddShake(0.1f,0.15f);
                //attackBox.enabled = false;
                Debug.Log(gameObject.name + " Get " + attackComp.damage.ToString() + " damage");
            }
        }
    }

    void DetectPlayer()
    {
        Vector3 directionToPlayer = playerai.gameObject.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        if (distanceToPlayer <= detectRange && !playerai.isDead)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up, directionToPlayer);
            Debug.DrawRay(ray.origin, ray.direction * detectRange, Color.red);
            if (Physics.Raycast(ray, out hit, detectRange))
            {
                if (hit.transform == playerai.gameObject.transform)
                {
                    playerOnceDetected = true;
                }
            }
        }
        else
        {
            playerOnceDetected = false;
        }
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

    private void SpawnEnemy(GameObject spawner, GameObject enemy_gameobj)
    {
        animator.SetTrigger("Action");
        Instantiate(spawnParticles, spawner.transform.position, Quaternion.identity);
        GameObject enemyInst = Instantiate(enemy_gameobj, spawner.transform.position, spawner.transform.rotation);
        enemyInst.transform.SetParent(null);
        EnemyAI enemyai = enemyInst.GetComponent<EnemyAI>();
        enemyai.forceDetectPlayer = true;
    }
}
