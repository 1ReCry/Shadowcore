using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalManager : MonoBehaviour
{
    [Header("Настройки Таймера")]
    public float spawn_timer_max;
    public float spawn_time;
    public float spawn_timer_min;
    public float spawn_timer_increasing;

    [Header("Настройки сложности")]
    public float difficulty_multiplier;
    public float difficulty_float = 1;
    public int difficulty = 1;

    [Header("Настройки Enemy")]
    public GameObject enemyPrefab;

    public EnemyScriptable enemy1;
    public EnemyScriptable enemy2;
    public EnemyScriptable enemy3;
    public EnemyScriptable enemy4;
    public EnemyScriptable enemy5;

    [Header("Настройки спавнеров")]
    public GameObject spawner1;
    public GameObject spawner2;
    public GameObject spawner3;
    public GameObject spawner4;
    public Vector3 spawn_offset;

    //приватные переменные
    private float time;
    private float spawn_timer_value;
    private int random_spawner_picked;
    private int random_enemy_picked;
    private EnemyScriptable picked_enemy_scriptable;
    

    // Start is called before the first frame update
    void Start()
    {
        Globals.survivalMode = true;
        spawn_time = spawn_timer_max;
        spawn_timer_value = spawn_time / 4;
        Globals.Money = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(spawn_time > spawn_timer_min)
        {
            spawn_time -= spawn_timer_increasing * Time.deltaTime;
        }

        spawn_timer_value -= Time.deltaTime;
        difficulty_float += difficulty_multiplier * Time.deltaTime;
        difficulty = Mathf.RoundToInt(difficulty_float);
        if(spawn_timer_value <= 0)
        {
            GameObject[] spawners = { spawner1, spawner2, spawner3, spawner4 };
            GameObject picked_spawner = spawners[Random.Range(0,spawners.Length)];

            EnemyScriptable[] enemies = { enemy1, enemy2, enemy3, enemy4 , enemy5 };
            int difficulty_to_length = Mathf.Clamp(difficulty, 0, enemies.Length);
            EnemyScriptable picked_enemy = enemies[Random.Range(0,difficulty_to_length)];

            SpawnEnemy(picked_spawner, enemyPrefab, picked_enemy);

            spawn_timer_value = spawn_time;
        }
    }

    private void SpawnEnemy(GameObject spawner, GameObject enemy_prefab, EnemyScriptable enemy_scriptable)
    {
        GameObject enemyInst = Instantiate(enemy_prefab, spawner.transform.position, spawner.transform.rotation);
        enemyInst.transform.SetParent(null);
        EnemyAI enemyai = enemyInst.GetComponent<EnemyAI>();
        enemyai.selectedScriptable = enemy_scriptable;
        enemyai.forceDetectPlayer = true;
    }
}
