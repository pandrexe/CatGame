using UnityEngine;
using Unity.Cinemachine; 
using System.Collections;

public class RoomsTransition : MonoBehaviour
{
    [SerializeField] BoxCollider2D mapBoundry;
    CinemachineConfiner2D confiner; 
    [SerializeField] Direction direction;
    [SerializeField] float additivePos = 2f;

    enum Direction
    {
        Left,
        Right
    }


    private void Awake()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            confiner.BoundingShape2D = mapBoundry;

            confiner.InvalidateBoundingShapeCache();
        }
            UpdatePlayerPosition(collision.gameObject);
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPosition = player.transform.position;

        if (direction == Direction.Left)
        {
            newPosition.x -= additivePos;
        }
        else if (direction == Direction.Right)
        {
            newPosition.x += additivePos;
        }
        player.transform.position = newPosition;
    }   
}