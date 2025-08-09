using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraGlitchController : MonoBehaviour
{
    public Material glitchMaterial; // Assign material using Distortion.shader
    public float glitchDuration = 0.5f;
    public float maxIntensity = 0.8f;
    public float noiseValue = 4.51f;

    private float glitchTimer;
    private Material materialInstance;

    void Start()
    {
        // Create instance material to avoid modifying original asset
        if (glitchMaterial != null)
        {
            materialInstance = new Material(glitchMaterial);
        }
    }

    void Update()
    {
        if (glitchTimer > 0)
        {
            glitchTimer -= Time.deltaTime;

            // Reduce intensity over time
            float intensity = Mathf.Lerp(0, maxIntensity, glitchTimer / glitchDuration);
            materialInstance.SetFloat("_Intensity", intensity);

            // Randomize noise value for more dynamic effect
            materialInstance.SetFloat("_ValueX", noiseValue + Random.Range(-1f, 1f));

            if (glitchTimer <= 0)
            {
                materialInstance.SetFloat("_Intensity", 0);
            }
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (materialInstance != null && glitchTimer > 0)
        {
            Graphics.Blit(src, dest, materialInstance);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    public void TriggerGlitch()
    {
        glitchTimer = glitchDuration;
        materialInstance.SetFloat("_Intensity", maxIntensity);
        materialInstance.SetFloat("_ValueX", noiseValue);
    }
}