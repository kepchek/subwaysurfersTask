using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int playerPos = 1; //Хранит позицию игрока на конкретной линии line0 = 0, line1 = 1, line2 = 2
    private int LeftEdge = 0; //Переменные, обозначающие крайние линии(для избежания magic numbers)
    private int RightEdge = 2;
    [SerializeField] private float moveDistance = 2.575f; //Расстояние на которое перемещается игрок при нажатии стрелки
    [SerializeField] private float moveSpeed = 5.0f; //Скорость перемещения игрока
    [SerializeField] private GameObject GameoverMenu;

    private Vector3 targetPos; //Коорды целевой позиции

    private void Start() 
    {
        targetPos = transform.position;
        Time.timeScale = 1;
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow) & playerPos > LeftEdge)
        {
            targetPos -= new Vector3(moveDistance, 0, 0);
            playerPos--;
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow) & playerPos < RightEdge)
        {
            targetPos += new Vector3(moveDistance, 0, 0);
            playerPos++;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        
    
    }
    private void OnCollisionEnter(Collision other) {
    if(other.gameObject.tag == "Respawn")
    {
        Time.timeScale = 0;
        GameoverMenu.SetActive(true);
    }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }



    
}
