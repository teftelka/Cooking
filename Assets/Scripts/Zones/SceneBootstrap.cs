using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

// Lives in the Core scene. Loads the zone scenes on top of it and keeps them all loaded:
// the kitchen keeps cooking and the garden keeps growing while another zone is on screen.
// The screen stays dark until everything is loaded.
public class SceneBootstrap : MonoBehaviour
{
    [Tooltip("Zone scenes loaded on top of Core. They must be in Build Settings")]
    [SerializeField] private List<string> zoneScenes = new List<string> { "Kitchen", "Garden", "Pond" };

    [Tooltip("Objects created during play (products, plates...) go into this scene")]
    [SerializeField] private string activeScene = "Kitchen";

    private IEnumerator Start()
    {
        var areaManager = AreaManager.Instance;
        if (areaManager != null) areaManager.CoverScreen();

        foreach (var sceneName in zoneScenes)
        {
            if (string.IsNullOrEmpty(sceneName)) continue;
            if (SceneManager.GetSceneByName(sceneName).isLoaded) continue;

            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"{name}: scene '{sceneName}' is not in Build Settings", this);
                continue;
            }

            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        var active = SceneManager.GetSceneByName(activeScene);
        if (active.IsValid() && active.isLoaded)
            SceneManager.SetActiveScene(active);

        if (areaManager != null) yield return areaManager.RevealScreen();
    }
}
