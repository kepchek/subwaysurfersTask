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
    int ObstacleCountInMap = 10; // Кол-во препятствий на карте. Тоже можно трогать, только очень нежно
    
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
    private float spawnZ = 0.0f; //Позиция по оси Z для появления новых секций

    private void Start() 
    {

        for(int i = 0; i < initialSections; i++)
        {
            SpawnSection();
        }

        poolObstacles = new Pool<Obstacle>(ObstaclePrefab, ObstacleCountInMap, Obstacles.transform);
        LastActiveObsNow = 40;
        Debug.Log(LastActiveObsNow);

        MakeObstacles(); 
    }

    private void Update() 
    {
        MoveSections();

        if(sectionsQueue.Peek().transform.position.z < player.position.z - sectionLength)
        {
            RemoveSection();
            FixSpawnSection();//Почему здесь фиксспавнсекшн? Потому что я дебил, а ещё есть баг, в том что метод не универсальный и я спёкся его чинить и придумал крутой костыль! Завтра попробую починить, удачи!
        }

        MoveObstacles();
    }

    private void FixedUpdate() 
    {
        CheckWhereObstacle();
        if(levelSpeed < 25)
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
        GameObject newSection = Instantiate(levelSectionPrefab, new Vector3(0, 0, spawnZ-20), Quaternion.identity);
        sectionsQueue.Enqueue(newSection);
    }

    void MoveSections()
    {
        foreach(var section in sectionsQueue)
        {
            section.transform.position -= new Vector3(0, 0, levelSpeed * Time.deltaTime);
        }
    }

    void MoveObstacles()
    {
        foreach(var obstacle in poolObstacles)
        {
            if(obstacle.isActiveAndEnabled)
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
        for (int i = 0; i < ObstacleCountInMap/2; i++)
        {
            int rnd = UnityEngine.Random.Range(0,3);
            int rnd2 = UnityEngine.Random.Range(1,3);
            Debug.Log(rnd);
            Debug.Log(rnd2);
            Vector3 firstObstaclePos = new Vector3(RoadPosition[rnd], 0, i*itemSpace ); // +20 для того, чтобы первое препятствие не спавнилось прям перед игроков
            Vector3 secondObstaclePos = new Vector3(RoadPosition[Math.Abs(rnd-rnd2)], 0, i*itemSpace );

            Obstacle firstObs = poolObstacles.GetFreeElement();
            firstObs.transform.position = firstObstaclePos;
            firstObs.transform.SetParent(Obstacles.transform);

            Obstacle secondObs = poolObstacles.GetFreeElement();
            secondObs.transform.position = secondObstaclePos;
            secondObs.transform.SetParent(Obstacles.transform);
        }
    }
    
    void CheckWhereObstacle()
    {
        int rnd = UnityEngine.Random.Range(0,3);
        int rnd2 = UnityEngine.Random.Range(1,3);
        foreach(var obstacle in poolObstacles)
        {
            if(obstacle.isActiveAndEnabled && obstacle.transform.position.z < player.position.z - itemSpace)
            {
                obstacle.transform.position = new Vector3(RoadPosition[Math.Abs(rnd - rnd2)], 0, LastActiveObsNow);
            }
        }
        LastActiveObsNow += itemSpace;
    }

  /* 
    
    void MakeObstacles()
    {
        for (int i = 0; i < ObstacleCountInMap; i++)
        {
            int rnd = UnityEngine.Random.Range(1,4);
            Vector3 obstaclePos = new Vector3(RoadPosition[rnd], 0, i*itemSpace ); // +20 для того, чтобы первое препятствие не спавнилось прям перед игроков
            Obstacle go = poolObstacles.GetFreeElement();
            go.transform.position = obstaclePos;
            go.transform.SetParent(Obstacles.transform);
        }
    }

    void CheckWhereObstacle()
    {
        foreach(var obstacle in poolObstacles)
        {
            int rnd = UnityEngine.Random.Range(1,4);
            if(obstacle.isActiveAndEnabled && obstacle.transform.position.z < player.position.z - itemSpace)
            {
                obstacle.transform.position = new Vector3(RoadPosition[rnd], 0, LastActiveObsNow);
                LastActiveObsNow += itemSpace;
            }
        }
    }
    */


}
