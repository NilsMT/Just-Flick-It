using System.Collections;
using UnityEngine;
using UnityEngine.XR;

//Credit : https://gist.github.com/ftvs/5822103

public class CameraAnimation : MonoBehaviour
{
    Transform camTransform;
    Light camLight;

    public float animationtime = 5f;
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

     // Reference to the light component

    Vector3 originalPos;
    float originalIntensity;
    float originalanimationtime;
    float originalshakeAmount;
    string status;
    bool wblink;

    void reset()
    {
        wblink = false;
        gameObject.transform.localPosition = originalPos;
        camLight.enabled = true;
        camLight.intensity = originalIntensity;
        shakeAmount = originalshakeAmount;
        animationtime = originalanimationtime;
    }

    void save()
    {
        originalanimationtime = animationtime;
        originalshakeAmount = shakeAmount;
        originalPos = camTransform.localPosition;
        originalIntensity = camLight.intensity;
    }

    void blinkOnce()
    {
        if (Random.value < 0.05f) // Adjust the probability as needed
        {
            camLight.enabled = !camLight.enabled;
            if (camLight.enabled)
            {
                camLight.intensity = Random.Range(originalIntensity * 0.5f, originalIntensity);
            }
        }
    }

    void shakeOnce()
    {
        gameObject.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
        shakeAmount -= Time.deltaTime * decreaseFactor;
        if (shakeAmount <= 0) shakeAmount = 0;
        if (wblink)
        {
            blinkOnce();
        }
    }

    public void shake(bool withblink)
    {
        save();
        wblink = withblink;
        status = "shake";
    }

    public void blink()
    {
        save();
        status = "blink";
    }

    public void changeColor(Color color) 
    {
        camLight.color = color; 
    }

    public void enableLight(bool enable)
    {
        camLight.enabled = enable;
    }

    private void Start()
    {
        camTransform = gameObject.GetComponent<Transform>();
        camLight = gameObject.GetComponent<Light>();
    }

    private void Update()
    {
        if (animationtime > 0)
        {    
            switch (status)
            {
                case "shake":
                    animationtime -= Time.deltaTime * decreaseFactor;
                    shakeOnce();
                    break;
                case "blink":
                    animationtime -= Time.deltaTime * decreaseFactor;
                    blinkOnce();
                    break;
                default:
                    break;
            }            
        } else
        {
            status = "";
            reset();
        }
    }

    private void OnDisable()
    {
        reset();
    }
}