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
    [SerializeField] private Transform Obstacles;
    [SerializeField] private float acceleration = 0.003f;

    int itemSpace = 10; // Расстояние между препятствиями. Можно трогать
    int ObstacleCountInMap = 10; // Кол-во препятствий на карте. Тоже можно трогать, только очень нежно



    private Dictionary<int, float> RoadPosition = new Dictionary<int, float> // Словарь хранящий координаты для спавна препятствия в зависимости от линии, 1 - первая дорога, 2 - вторая, 3 - третья, счёт слева
    {
        {1, -2.575f},
        {2, 0},
        {3, 2.575f}
    };

    [SerializeField] public Obstacle ObstaclePrefab; // Префаб препятствия

    private Pool<Obstacle> pool;
    public List<GameObject> maps = new List<GameObject>(); // Лист из карт



    private Queue<GameObject> sectionsQueue = new Queue<GameObject>(); //Очередь секций уровня
    private float spawnZ = 0.0f; //Позиция по оси Z для появления новых секций

    private void Start() 
    {

        for(int i = 0; i < initialSections; i++)
        {
            SpawnSection();
        }

        pool = new Pool<Obstacle>(ObstaclePrefab, ObstacleCountInMap, Obstacles.transform);

        maps.Add(MakeMap()); // Создание карты

        
    }

    private void Update() 
    {
        MoveSections();

        if(sectionsQueue.Peek().transform.position.z < player.position.z - sectionLength)
        {
            RemoveSection();
            FixSpawnSection();//Почему здесь фиксспавнсекшн? Потому что я дебил, а ещё есть баг, в том что метод не универсальный и я спёкся его чинить и придумал крутой костыль! Завтра попробую починить, удачи!
        }

        foreach(var map in maps) // Хуепуталы двигаются в сторону игрока
        {
            if(map.transform.position.z < player.position.z - sectionLength*5)
            {
                map.SetActive(false);
            }
            Obstacles.transform.position -= new Vector3(0, 0, levelSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate() 
    {
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

    void RemoveSection()
    {
        GameObject oldSection = sectionsQueue.Dequeue();
        Destroy(oldSection);
    }

    GameObject MakeMap()
    {
        GameObject result = new GameObject("map");
        result.transform.SetParent(Obstacles);
        for (int i = 0; i < ObstacleCountInMap; i++)
        {
            int rnd = UnityEngine.Random.Range(1,4);
            Vector3 obstaclePos = new Vector3(RoadPosition[rnd], 0, i*itemSpace ); // +20 для того, чтобы первое препятствие не спавнилось прям перед игроков
            Obstacle go = pool.GetFreeElement();
            go.transform.position = obstaclePos;
            go.transform.SetParent(result.transform);
        }
        return result;
    }

/*    GameObject MakeMap()
    {
        GameObject result = new GameObject("map");
        result.transform.SetParent(Obstacles);
        for (int i = 0; i < ObstacleCountInMap; i++)
        {
            Obstacle obstacle = null;
            float roadPos =  RoadPosition["mid"];

            if(i==2) {roadPos = RoadPosition["left"]; obstacle = ObstaclePrefab;}
            else if(i==3) {roadPos = RoadPosition["mid"]; obstacle = ObstaclePrefab;}
            else if(i==4) {roadPos = RoadPosition["right"]; obstacle = ObstaclePrefab;}

            Vector3 obstaclePos = new Vector3(roadPos, 0, i*itemSpace);
            if (obstacle != null)
            {
                Obstacle go = pool.GetFreeElement();
                go.transform.position = obstaclePos;
                go.transform.SetParent(result.transform);
            }
        }
        return result;
    }
*/


}
