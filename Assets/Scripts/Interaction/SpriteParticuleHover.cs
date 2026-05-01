using UnityEngine;

public class SpriteParticleHover : MonoBehaviour
{
    [Header("Particules")]
    public float particleSize = 1f;
    public float emissionRate = 5f;
    public float particleSpeed = 1f;
    public float particleLifetime = 1.5f;
    public Color particleColor = Color.white;

    private ParticleSystem particles;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetupParticles();
    }

    void SetupParticles()
    {
        GameObject particleObj = new GameObject("HoverParticles");
        particleObj.transform.SetParent(transform);
        particleObj.transform.localPosition = Vector3.zero;
        particleObj.transform.localScale = Vector3.one;

        particles = particleObj.AddComponent<ParticleSystem>();

        ParticleSystemRenderer psRenderer = particleObj.GetComponent<ParticleSystemRenderer>();
        psRenderer.material = new Material(Shader.Find("Sprites/Default"));
        psRenderer.sortingLayerName = spriteRenderer != null ?
            spriteRenderer.sortingLayerName : "Default";
        psRenderer.sortingOrder = spriteRenderer != null ?
            spriteRenderer.sortingOrder + 1 : 1;

        var main = particles.main;
        main.loop = true;
        main.playOnAwake = false;
        main.startLifetime = particleLifetime;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, particleSpeed);
        main.startSize = new ParticleSystem.MinMaxCurve(particleSize * 0.5f, particleSize);
        main.startColor = particleColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 100;

        var emission = particles.emission;
        emission.rateOverTime = emissionRate;

        var shape = particles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Rectangle;

        if (spriteRenderer != null)
        {
            Vector3 spriteSize = spriteRenderer.bounds.size;
            shape.scale = new Vector3(spriteSize.x, spriteSize.y, 0.1f);
        }
        else
        {
            shape.scale = new Vector3(1f, 1f, 0.1f);
        }

        var velocity = particles.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(0f);
        velocity.y = new ParticleSystem.MinMaxCurve(0.3f);
        velocity.z = new ParticleSystem.MinMaxCurve(0f);

        var sizeOverLifetime = particles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = gradient;

        particles.Stop();
        particleObj.SetActive(false);
    }

    public void ShowParticles()
    {
        if (particles == null) return;
        particles.gameObject.SetActive(true);
        particles.Play();
    }

    public void HideParticles()
    {
        if (particles == null) return;
        particles.Stop();
        particles.gameObject.SetActive(false);
    }
}