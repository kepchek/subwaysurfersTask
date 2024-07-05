using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject levelSectionPrefab; 
    [SerializeField] private int initialSections = 6; //Кол-во секций вначале  <-- Можно трогать
    [SerializeField] private float sectionLength = 20.0f;   //Не трогать
    [SerializeField] private float levelSpeed = 5.0f; //Тоже можно трогать
    [SerializeField] private Transform player;
    [SerializeField] private Transform Obstacles;
    [SerializeField] private float acceleration = 0.003f;

    int itemSpace = 8; // Расстояние между препятствиями. Можно трогать
    int ObstacleCountInMap = 25; // Кол-во препятствий на карте. Тоже можно трогать, только очень нежно
    
    float ObstacleSpawnPoint;

    private Dictionary<int, float> RoadPosition = new Dictionary<int, float> // Словарь хранящий координаты для спавна препятствия в зависимости от линии, 0 - первая дорога, 1 - вторая, 2 - третья, счёт слева
    {
        {0, -2.575f},
        {1, 0},
        {2, 2.575f}
    };

    [SerializeField] public Obstacle ObstaclePrefab; // Префаб препятствия

    private Pool<Obstacle> poolObstacles;

    private Queue<GameObject> sectionsQueue = new Queue<GameObject>(); //Очередь секций уровня
    private Queue<Obstacle> obstaclesQueue = new Queue<Obstacle>(); // Очередь препятствий
    private float spawnZ = 0.0f; //Позиция по оси Z для появления новых секций

    private void Start() 
    {
        for (int i = 0; i < initialSections; i++)
        {
            SpawnSection();
        }

        poolObstacles = new Pool<Obstacle>(ObstaclePrefab, ObstacleCountInMap, Obstacles.transform); // создание пула препятствий
        ObstacleSpawnPoint = 100; // точка спавна новых препятствий
        MakeObstacles(); 
    }

    private void Update() 
    {
        MoveSections();

        if (sectionsQueue.Peek().transform.position.z < player.position.z - sectionLength)
        {
            RemoveSection();
            FixSpawnSection();
        }

        MoveObstacles();

        if (obstaclesQueue.Peek().transform.position.z < player.position.z - itemSpace) // перемещение пройденного игроком объекта в конец игрового поля
        {
            Obstacle passedObstacle = obstaclesQueue.Dequeue();
            int rnd = UnityEngine.Random.Range(0, 3);
            passedObstacle.transform.position = new Vector3(RoadPosition[rnd], 0, ObstacleSpawnPoint);
            obstaclesQueue.Enqueue(passedObstacle);

        }
    }

    private void FixedUpdate() 
    {
        if (levelSpeed < 25)
        {
            levelSpeed += acceleration;
        }
    }

    void SpawnSection()
    {
        GameObject newSection = Instantiate(levelSectionPrefab, new Vector3(0, 0, spawnZ), Quaternion.identity);
        sectionsQueue.Enqueue(newSection);
        spawnZ += sectionLength;
    }

    void FixSpawnSection()
    {
        GameObject newSection = Instantiate(levelSectionPrefab, new Vector3(0, 0, spawnZ - 20), Quaternion.identity);
        sectionsQueue.Enqueue(newSection);
    }

    void MoveSections()
    {
        foreach (var section in sectionsQueue)
        {
            section.transform.position -= new Vector3(0, 0, levelSpeed * Time.deltaTime);
        }
    }

    void MoveObstacles()
    {
        foreach (var obstacle in poolObstacles)
        {
            if (obstacle.isActiveAndEnabled)
            {
                obstacle.transform.position -= new Vector3(0, 0, levelSpeed * Time.deltaTime);
            }
        }
    }

    void RemoveSection()
    {
        GameObject oldSection = sectionsQueue.Dequeue();
        Destroy(oldSection);
    }

    void MakeObstacles()
    {
        for (int i = 0; i < ObstacleCountInMap / 2; i++)
        {
            int rnd = UnityEngine.Random.Range(0, 3);
            int rnd2 = UnityEngine.Random.Range(1, 3);
            Vector3 firstObstaclePos = new Vector3(RoadPosition[rnd], 0, i * itemSpace + 20);
            Vector3 secondObstaclePos = new Vector3(RoadPosition[Math.Abs(rnd - rnd2)], 0, i * itemSpace + 20);

            Obstacle firstObs = poolObstacles.GetFreeElement();
            firstObs.transform.position = firstObstaclePos;
            firstObs.transform.SetParent(Obstacles.transform);
            obstaclesQueue.Enqueue(firstObs);

            Obstacle secondObs = poolObstacles.GetFreeElement();
            secondObs.transform.position = secondObstaclePos;
            secondObs.transform.SetParent(Obstacles.transform);
            obstaclesQueue.Enqueue(secondObs);
        }
    }
}
