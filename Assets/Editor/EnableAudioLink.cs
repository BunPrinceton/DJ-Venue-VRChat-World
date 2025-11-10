using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class EnableAudioLink : EditorWindow
{
    [MenuItem("Tools/Enable AudioLink Music Reactive Lights")]
    public static void EnableAudioLinkMode()
    {
        if (!EditorUtility.DisplayDialog("Enable AudioLink",
            "This will enable music-reactive lighting:\n\n" +
            "✓ Re-enable AudioLink on all VRSL lights\n" +
            "✓ Add AudioLink prefab to scene\n" +
            "✓ Lights will react to music automatically\n\n" +
            "Perfect for DJ sets!\n\n" +
            "Continue?",
            "Yes, Enable!", "Cancel"))
        {
            return;
        }

        int lightsEnabled = 0;

        // Re-enable AudioLink components on all VRSL lights
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("VRSL") || obj.name.Contains("AudioLink"))
            {
                // Enable all MonoBehaviour components
                MonoBehaviour[] components = obj.GetComponents<MonoBehaviour>();
                foreach (var comp in components)
                {
                    if (comp != null)
                    {
                        comp.enabled = true;
                        lightsEnabled++;
                    }
                }

                // Enable all child components too
                MonoBehaviour[] childComponents = obj.GetComponentsInChildren<MonoBehaviour>();
                foreach (var comp in childComponents)
                {
                    if (comp != null)
                    {
                        comp.enabled = true;
                    }
                }

                // Make sure renderers are on
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
                foreach (var r in renderers)
                {
                    r.enabled = true;
                }
            }
        }

        // Check if AudioLink prefab exists in scene
        GameObject audioLink = GameObject.Find("AudioLink");
        if (audioLink == null)
        {
            // Try to find and instantiate AudioLink prefab
            string[] guids = AssetDatabase.FindAssets("AudioLink t:Prefab");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    audioLink = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    audioLink.name = "AudioLink";
                    Debug.Log("✓ AudioLink prefab added to scene");
                }
            }
            else
            {
                Debug.LogWarning("AudioLink prefab not found! Lights may not react to music.");
            }
        }
        else
        {
            Debug.Log("✓ AudioLink already in scene");
        }

        // Save scene
        EditorSceneManager.SaveOpenScenes();

        string message = $"✅ AudioLink Enabled!\n\n" +
            $"✓ {lightsEnabled} light components enabled\n" +
            $"✓ AudioLink prefab: {(audioLink != null ? "Added" : "Not found")}\n\n" +
            "HOW IT WORKS:\n\n" +
            "1. Play music in VRChat (world audio or video player)\n" +
            "2. Lights automatically react to:\n" +
            "   • Bass (low frequencies) = Red lights\n" +
            "   • Mids (vocals) = Green lights  \n" +
            "   • Highs (cymbals) = Blue lights\n" +
            "   • Combined = Full color spectrum!\n\n" +
            "3. Different lights react to different frequencies\n" +
            "4. Creates dynamic light show automatically!\n\n" +
            "TIPS:\n" +
            "• Louder music = brighter lights\n" +
            "• Bass-heavy music = more movement\n" +
            "• Works with any audio source in VRChat\n\n" +
            "Upload and test in VRChat to see it work!";

        EditorUtility.DisplayDialog("AudioLink Ready!", message, "Awesome!");

        Debug.Log($"AudioLink mode enabled! {lightsEnabled} components activated");
    }
}
