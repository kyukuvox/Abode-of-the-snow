using UnityEngine;

public class CameraGround : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float smoothSpeed = 0.3f;

    void Start()
    {
        player = GameObject.Find("JOUEUR");
    }

    void FixedUpdate()
    {
        Vector3 desiredPosition = new Vector3(player.transform.position.x + 0, player.transform.position.y  +2, -10);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
