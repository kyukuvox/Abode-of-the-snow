using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    public static ParallaxManager Instance;

    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layer;
        [Range(0, 1)] public float parallaxFactor;
        [HideInInspector] public Vector3 originPosition;
    }

    public ParallaxLayer[] layers;
    public Transform camTransform;
    private Vector3 lastCameraPosition;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        lastCameraPosition = camTransform.position;

        foreach (ParallaxLayer layer in layers)
            layer.originPosition = layer.layer.position;
    }

    void LateUpdate()
    {
        Vector3 cameraDelta = camTransform.position - lastCameraPosition;

        foreach (ParallaxLayer layer in layers)
        {
            float moveX = cameraDelta.x * layer.parallaxFactor;
            float moveY = cameraDelta.y * layer.parallaxFactor;
            layer.layer.position += new Vector3(moveX, moveY, 0);
        }

        lastCameraPosition = camTransform.position;
    }

    public void ResetToOrigin()
    {
        foreach (ParallaxLayer layer in layers)
            layer.layer.position = layer.originPosition;

        lastCameraPosition = camTransform.position;
    }
}