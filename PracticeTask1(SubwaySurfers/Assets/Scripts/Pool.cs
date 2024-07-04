using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> : IEnumerable<T> where T : MonoBehaviour
{
    public T prefab { get; }    
    
    public Transform container {get;}

    private List<T> poolList;

    private int count;

    public Pool(T prefab, int count, Transform container)
    {
        this.prefab = prefab;
        this.container = container;
        this.count = count;
        this.CreatePool(count);
    }

    private void CreatePool(int count)
    {
        this.poolList = new List<T>();

        for (int i = 0; i < count; i++)
        {
            CreateObj();
        }
    }

    private T CreateObj()
    {
        var createdObj = Object.Instantiate(this.prefab, this.container);
        createdObj.gameObject.SetActive(false);
        this.poolList.Add(createdObj);
        return createdObj;
    }

    public bool HasFreeElement(out T element) // Проверяет, есть ли свободный (не активный) элемент
    {
        foreach ( var mono in poolList)
        {
            if(!mono.gameObject.activeInHierarchy)
            {
                element = mono;
                mono.gameObject.SetActive(true);
                return true;
            }
        }
        
        element = null;
        return false;
    }

    public T GetFreeElement() // Возвращает свободный (не активный) элемент
    {
        if(HasFreeElement(out var element))
        {
            return element;
        }

        throw new System.Exception($"Дурила, нет у тебя свободных элементов");

    }

    public T GetForIndex(int index)
    {
        return poolList[index];
    }

    public IEnumerator<T> GetEnumerator()
    {
        return poolList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

}
