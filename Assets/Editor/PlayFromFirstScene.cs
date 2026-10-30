using UnityEditor;
using UnityEditor.SceneManagement;

// Play always starts from the first enabled scene in Build Settings (Core), whichever scenes are open,
// so the zones never run without the core managers. Core then loads the zone scenes itself.
[InitializeOnLoad]
public static class PlayFromFirstScene
{
    static PlayFromFirstScene()
    {
        // Build Settings may not be readable yet inside the static constructor
        EditorApplication.delayCall += Apply;
        EditorBuildSettings.sceneListChanged += Apply;
    }

    private static void Apply()
    {
        SceneAsset firstScene = null;
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            firstScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
            break;
        }

        EditorSceneManager.playModeStartScene = firstScene;
    }
}
