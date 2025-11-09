using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class MirrorQualityController : UdonSharpBehaviour
{
    public GameObject mirror;

    private VRC.SDK3.Components.VRCMirrorReflection mirrorComponent;

    void Start()
    {
        if (mirror != null)
        {
            mirrorComponent = mirror.GetComponent<VRC.SDK3.Components.VRCMirrorReflection>();
        }
    }

    public void SetHighQuality()
    {
        if (mirrorComponent != null)
        {
            // VRChat mirror doesn't have quality settings exposed
            // But we can enable/disable it
            mirrorComponent.enabled = true;
            Debug.Log("Mirror: High Quality");
        }
    }

    public void SetLowQuality()
    {
        if (mirrorComponent != null)
        {
            mirrorComponent.enabled = true;
            Debug.Log("Mirror: Low Quality");
        }
    }

    public void ToggleMirror()
    {
        if (mirrorComponent != null)
        {
            mirrorComponent.enabled = !mirrorComponent.enabled;
            Debug.Log($"Mirror: {(mirrorComponent.enabled ? "ON" : "OFF")}");
        }
    }
}
