using UnityEngine;

public class GameCursor : MonoBehaviour
{
    public float detectionRadius = 1.2f; // Радиус для обнаружения врагов
    public bool isMoving; // Переменная для проверки состояния движения
    public LayerMask enemyLayerMask;

    private void Awake()
    {
        LayerMask enemyLayerMask = LayerMask.GetMask("Enemy"); // Маска слоя для врагов
    }

    private void FixedUpdate()
    {
        // Проверяем наличие врагов в радиусе обнаружения
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayerMask);

        if (enemiesInRange.Length > 0)
        {
            // Инициализируем переменные для нахождения ближайшего врага
            GameObject nearestEnemy = null;
            float nearestDistance = Mathf.Infinity;

            // Перебираем всех врагов в радиусе обнаружения
            foreach (Collider enemyCollider in enemiesInRange)
            {
                // Получаем самый главный объект (родителя) для каждого врага
                GameObject enemyObject = GetRootEnemyObject(enemyCollider.gameObject);

                // Вычисляем расстояние до самого главного объекта
                float distanceToEnemy = Vector3.Distance(transform.position, enemyObject.transform.position);

                // Если текущий враг ближе, чем предыдущий, обновляем ближайшего врага
                if (distanceToEnemy < nearestDistance)
                {
                    nearestDistance = distanceToEnemy;
                    nearestEnemy = enemyObject;
                }
            }

            // Если ближайший враг был найден
            if (nearestEnemy != null)
            {
                // Устанавливаем ближайшего врага в компонент PlayerAI или другой соответствующий скрипт
                PlayerAI playerAI = FindObjectOfType<PlayerAI>(); // Найти компонент PlayerAI на сцене
                if (playerAI != null && nearestEnemy.name.Contains("Enemy") && !playerAI.isDead)
                {
                    playerAI.selectedEnemy = nearestEnemy;
                }

                //Debug.Log("Nearest enemy detected: " + nearestEnemy.name);
            }
        }
        else if (isMoving)
        {
            // Если врагов не обнаружено и isMoving == true, устанавливаем selectedEnemy в null
            PlayerAI playerAI = FindObjectOfType<PlayerAI>(); // Найти компонент PlayerAI на сцене
            if (playerAI != null)
            {
                playerAI.selectedEnemy = null;
            }

            //Debug.Log("No enemies in range. Setting selectedEnemy to null.");
        }
    }

    private GameObject GetRootEnemyObject(GameObject child)
    {
        // Ищем самый главный объект, пока не найдем объект с названием содержащим "Enemy"
        Transform currentTransform = child.transform;
        while (currentTransform.parent != null)
        {
            currentTransform = currentTransform.parent;
        }

        // Если объект имеет название "Enemy", возвращаем его
        if (currentTransform != null && currentTransform.name.Contains("Enemy"))
        {
            return currentTransform.gameObject;
        }

        // Если название не содержит "Enemy", возвращаем null
        return null;
    }

    private void OnDrawGizmos()
    {
        // Устанавливаем цвет для сферы
        Gizmos.color = Color.red;
        
        // Рисуем сферу в позиции курсора
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
