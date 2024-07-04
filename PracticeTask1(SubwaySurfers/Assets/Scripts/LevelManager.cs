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
    [SerializeField] private Transform player;  //А тут нехуй трогать
    [SerializeField] private Transform Obstacles; //А тут хуй трогать
    [SerializeField] private float acceleration = 0.003f;

    int itemSpace = 8; // Расстояние между препятствиями. Можно трогать
    int ObstacleCountInMap = 25; // Кол-во препятствий на карте. Тоже можно трогать, только очень нежно

    int counter = 0; // 
    
    float LastActiveObsNow;

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

        poolObstacles = new Pool<Obstacle>(ObstaclePrefab, ObstacleCountInMap, Obstacles.transform);
        LastActiveObsNow = 40;
        MakeObstacles(); 

        foreach (var obstacle in poolObstacles)
        {
            Debug.Log(obstacle.transform.position.z);
        }
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

        if (obstaclesQueue.Peek().transform.position.z < player.position.z - itemSpace)
        {
            Obstacle passedObstacle = obstaclesQueue.Dequeue();
            int rnd = UnityEngine.Random.Range(0, 3);
            passedObstacle.transform.position = new Vector3(RoadPosition[rnd], 0, LastActiveObsNow);
            obstaclesQueue.Enqueue(passedObstacle);
            counter++;

            if(counter%2 == 0) LastActiveObsNow += itemSpace;
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
            Vector3 firstObstaclePos = new Vector3(RoadPosition[rnd], 0, i * itemSpace);
            Vector3 secondObstaclePos = new Vector3(RoadPosition[Math.Abs(rnd - rnd2)], 0, i * itemSpace);

            Obstacle firstObs = poolObstacles.GetFreeElement();
            firstObs.transform.position = firstObstaclePos;
            firstObs.transform.SetParent(Obstacles.transform);
            obstaclesQueue.Enqueue(firstObs);

            Obstacle secondObs = poolObstacles.GetFreeElement();
            secondObs.transform.position = secondObstaclePos;
            secondObs.transform.SetParent(Obstacles.transform);
            obstaclesQueue.Enqueue(secondObs);
        }

        // Ensure the last obstacle position is correctly set
        LastActiveObsNow = obstaclesQueue.Peek().transform.position.z + itemSpace * (ObstacleCountInMap / 2);
    }

    void RepositionObstacle()
    {
        Obstacle passedObstacle = obstaclesQueue.Dequeue();
        int rnd = UnityEngine.Random.Range(0, 3);
        passedObstacle.transform.position = new Vector3(RoadPosition[rnd], 0, LastActiveObsNow);
        obstaclesQueue.Enqueue(passedObstacle);

        LastActiveObsNow += itemSpace;
    }
}
