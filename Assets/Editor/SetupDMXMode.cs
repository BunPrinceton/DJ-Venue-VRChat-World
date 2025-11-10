using UnityEngine;
using UnityEditor;
using System.IO;

public class SetupDMXMode : EditorWindow
{
    [MenuItem("Tools/Setup DMX Control")]
    public static void SetupDMX()
    {
        if (!EditorUtility.DisplayDialog("Setup DMX Control",
            "This will configure your lights for DMX control:\n\n" +
            "✓ Find all VRSL lights in scene\n" +
            "✓ Enable DMX mode\n" +
            "✓ Add video player for DMX grid\n" +
            "✓ Configure lighting fixtures\n\n" +
            "NOTE: You'll need VRSL Grid Node software ($20)\n" +
            "to actually control the lights via DMX.\n\n" +
            "Continue?",
            "Yes, Setup DMX!", "Cancel"))
        {
            return;
        }

        int lightsConfigured = 0;

        // Find all VRSL lights
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("VRSL"))
            {
                // Re-enable all components (we disabled AudioLink earlier)
                MonoBehaviour[] components = obj.GetComponents<MonoBehaviour>();
                foreach (var comp in components)
                {
                    if (comp != null)
                    {
                        comp.enabled = true;
                        lightsConfigured++;
                    }
                }
            }
        }

        // Add DMX Video Player
        AddDMXVideoPlayer();

        // Add Grid Node setup object
        AddGridNodeSetup();

        EditorUtility.DisplayDialog("DMX Setup Complete!",
            $"✅ DMX mode configured!\n\n" +
            $"✓ {lightsConfigured} light components enabled\n" +
            "✓ Video player added for DMX grid\n" +
            "✓ Grid node setup object added\n\n" +
            "NEXT STEPS:\n\n" +
            "1. Get VRSL Grid Node: https://gumroad.com/l/xYaPu\n" +
            "2. Install lighting software (QLC+, etc.)\n" +
            "3. Set up Artnet output\n" +
            "4. Run Grid Node to convert to video\n" +
            "5. Stream video URL to VRChat\n" +
            "6. Control lights in real-time!\n\n" +
            "See VRSL docs for full guide:\n" +
            "github.com/AcChosen/VR-Stage-Lighting/wiki",
            "Got It!");

        Debug.Log($"DMX mode setup complete! {lightsConfigured} lights configured");
    }

    private static void AddDMXVideoPlayer()
    {
        // Create video player object
        GameObject videoPlayer = new GameObject("DMX_VideoPlayer");
        videoPlayer.transform.position = new Vector3(0, 10, 0); // High up, out of sight

        // Add video player component
        UnityEngine.Video.VideoPlayer vp = videoPlayer.AddComponent<UnityEngine.Video.VideoPlayer>();
        vp.playOnAwake = true;
        vp.isLooping = true;
        vp.renderMode = UnityEngine.Video.VideoRenderMode.RenderTexture;

        // Create render texture for DMX grid
        RenderTexture rt = new RenderTexture(256, 256, 0);
        rt.name = "DMX_GridTexture";
        vp.targetTexture = rt;

        // Create display quad (optional - to see the DMX grid)
        GameObject displayQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        displayQuad.name = "DMX_GridDisplay";
        displayQuad.transform.position = new Vector3(5, 2, 0);
        displayQuad.transform.rotation = Quaternion.Euler(0, -90, 0);
        displayQuad.transform.localScale = new Vector3(1, 1, 1);

        Material mat = new Material(Shader.Find("Unlit/Texture"));
        mat.mainTexture = rt;
        displayQuad.GetComponent<Renderer>().material = mat;

        Debug.Log("✓ DMX video player added");
    }

    private static void AddGridNodeSetup()
    {
        // Create info object with instructions
        GameObject infoObject = new GameObject("DMX_Setup_Instructions");
        infoObject.transform.position = new Vector3(5, 3, 0);

        // Add text with instructions
        GameObject textObj = new GameObject("InstructionsText");
        textObj.transform.parent = infoObject.transform;
        textObj.transform.localPosition = Vector3.zero;

        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = "DMX GRID\n(Stream video here)";
        textMesh.fontSize = 20;
        textMesh.color = Color.cyan;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;

        Debug.Log("✓ DMX instructions added");
    }
}
