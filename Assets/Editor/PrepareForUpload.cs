using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using VRC.SDK3.Components;
using VRC.SDKBase;

public class PrepareForUpload : EditorWindow
{
    [MenuItem("Tools/Prepare World for VRChat Upload")]
    public static void PrepareWorld()
    {
        if (!EditorUtility.DisplayDialog("Prepare for VRChat Upload",
            "This will verify and prepare your world for upload:\n\n" +
            "✓ Check for VRCWorld descriptor\n" +
            "✓ Set spawn points\n" +
            "✓ Verify scene is saved\n" +
            "✓ Add reference camera\n\n" +
            "Then you can upload via VRChat SDK!\n\n" +
            "Continue?",
            "Yes, Prepare!", "Cancel"))
        {
            return;
        }

        bool allGood = true;

        // Check for VRCSceneDescriptor
        VRC_SceneDescriptor descriptor = GameObject.FindObjectOfType<VRC_SceneDescriptor>();

        if (descriptor == null)
        {
            // Create one
            GameObject worldObject = new GameObject("VRCWorld");
            descriptor = worldObject.AddComponent<VRC_SceneDescriptor>();
            Debug.Log("✓ Created VRCWorld with scene descriptor");
            allGood = true;
        }
        else
        {
            Debug.Log("✓ VRCWorld descriptor found");
        }

        // Setup spawn points
        if (descriptor.spawns == null || descriptor.spawns.Length == 0)
        {
            // Create spawn point
            GameObject spawnPoint = new GameObject("SpawnPoint");
            spawnPoint.transform.position = new Vector3(0, 0, -2); // In front of DJ booth
            spawnPoint.transform.rotation = Quaternion.Euler(0, 0, 0);

            descriptor.spawns = new Transform[] { spawnPoint.transform };
            Debug.Log("✓ Created spawn point");
        }
        else
        {
            Debug.Log($"✓ Spawn points configured ({descriptor.spawns.Length})");
        }

        // Add reference camera if needed
        VRC.SDK3.Components.VRCSceneDescriptor sceneDesc = descriptor as VRC.SDK3.Components.VRCSceneDescriptor;
        if (sceneDesc != null && sceneDesc.ReferenceCamera == null)
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                // Create camera
                GameObject camObj = new GameObject("Reference Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.transform.position = new Vector3(0, 1.6f, -2);
            }
            sceneDesc.ReferenceCamera = cam.gameObject;
            Debug.Log("✓ Reference camera set");
        }

        // Save scene
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("✓ Scene saved");

        // Show success message with instructions
        EditorUtility.DisplayDialog("Ready to Upload!",
            "✅ World is ready for VRChat!\n\n" +
            "NEXT STEPS:\n\n" +
            "1. Top menu: VRChat SDK → Show Control Panel\n" +
            "2. Click 'Authentication' tab → Sign in\n" +
            "3. Click 'Builder' tab\n" +
            "4. Click 'Build & Publish for Windows'\n" +
            "5. Wait for build (~5 mins)\n" +
            "6. Fill in world info:\n" +
            "   • Name: DJ Venue\n" +
            "   • Capacity: 20\n" +
            "   • Tags: Music, Club, Social\n" +
            "7. Set to PRIVATE (friends only)\n" +
            "8. Click Upload!\n\n" +
            "Build takes 5-10 minutes.\n" +
            "Upload takes another 5 minutes.",
            "Let's Do It!");
    }
}
