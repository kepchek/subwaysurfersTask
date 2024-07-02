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

    private Queue<GameObject> sectionsQueue = new Queue<GameObject>(); //Очередь секций уровня
    private float spawnZ = 0.0f; //Позиция по оси Z для появления новых секций

    private void Start() 
    {
        for(int i = 0; i < initialSections; i++)
        {
            SpawnSection();
        }
    }

    private void Update() 
    {
        MoveSections();

        if(sectionsQueue.Peek().transform.position.z < player.position.z - sectionLength)
        {
            RemoveSection();
            FixSpawnSection();//Почему здесь фиксспавнсекшн? Потому что я дебил, а ещё есть баг, в том что метод не универсальный и я спёкся его чинить и придумал крутой костыль! Завтра попробую починить, удачи!
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

    


}
