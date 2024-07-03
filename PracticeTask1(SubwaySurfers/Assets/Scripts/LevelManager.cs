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

    int itemSpace = 20; // Расстояние между препятствиями. Можно трогать
    int itemCountInMap = 5; // Кол-во препятствий на карте. Тоже можно трогать, только очень нежно


    private Dictionary<String, float> RoadPosition = new Dictionary<string, float> // Словарь хранящий координаты для спавна препятствия в зависимости от линии
    {
        {"left", -2.575f},
        {"mid", 0},
        {"right", 2.575f}
    };

    [SerializeField] public GameObject ObstaclePrefab; // Префаб препятствия
    public List<GameObject> maps = new List<GameObject>(); // Лист из карт



    private Queue<GameObject> sectionsQueue = new Queue<GameObject>(); //Очередь секций уровня
    private float spawnZ = 0.0f; //Позиция по оси Z для появления новых секций

    private void Start() 
    {
        for(int i = 0; i < initialSections; i++)
        {
            SpawnSection();
        }

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
            map.transform.position -= new Vector3(0, 0, levelSpeed * Time.deltaTime);
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
        result.transform.SetParent(transform);
        for (int i = 0; i < itemCountInMap; i++)
        {
            GameObject obstacle = null;
            float roadPos =  RoadPosition["mid"];

            if(i==2) {roadPos = RoadPosition["left"]; obstacle = ObstaclePrefab;}
            else if(i==3) {roadPos = RoadPosition["mid"]; obstacle = ObstaclePrefab;}
            else if(i==4) {roadPos = RoadPosition["right"]; obstacle = ObstaclePrefab;}

            Vector3 obstaclePos = new Vector3(roadPos, 0, i*itemSpace);
            if (obstacle != null)
            {
                GameObject go = Instantiate(obstacle, obstaclePos, Quaternion.identity);
                go.transform.SetParent(result.transform);
            }
        }
        return result;
    }
    


}
