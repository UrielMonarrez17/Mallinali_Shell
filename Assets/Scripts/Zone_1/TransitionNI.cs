using System;
using TMPro;
using Unity.Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TransitionNI : MonoBehaviour
{
    [SerializeField] PolygonCollider2D mapBoundry;
    CinemachineConfiner2D confiner;
    [SerializeField] Direction direction;
    enum Direction { Up, Down, Left, Right };

   [SerializeField]
   private TextMeshProUGUI interactText;

   private bool interactAllowed;

   private GameObject player;
   private bool haveText;
    [SerializeField] int distance;
    private void Awake()
    {
        confiner = FindAnyObjectByType<CinemachineConfiner2D>();
        haveText = interactText!=null;
        if(haveText)
            interactText.gameObject.SetActive(false);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
 if (other.gameObject.CompareTag("Principal")||other.gameObject.CompareTag("Principal"))
        {
            if(haveText)
                interactText.gameObject.SetActive(true);
            player = other.gameObject;
            beInteracted();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        interactAllowed = false;
        if(haveText)
            interactText.gameObject.SetActive(false);
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position;
        switch (direction)
        {
            case Direction.Up:
            newPos.y += distance;
            break;
            case Direction.Down:
            newPos.y -= distance;
            break;
            case Direction.Left:
            newPos.x -= distance;
            break;
            case Direction.Right:
            newPos.x += distance;
            break;

        }     
        player.transform.position = newPos; 
    }

        public void beInteracted()
    {
        confiner.BoundingShape2D = mapBoundry;
        UpdatePlayerPosition(player);
        if(haveText)
            interactText.gameObject.SetActive(false);
    }

}
