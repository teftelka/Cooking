using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Levels
{
    // PROTOTYPE (levels test branch): splits the game into timed levels of growing difficulty.
    // Each level reloads the scene, so the kitchen, stock and money start clean.
    // Remove this component from the scene to get the endless orders back.
    public class LevelManager : MonoBehaviour
    {
        [Tooltip("From easy to hard")]
        [SerializeField] private List<LevelSO> levels = new List<LevelSO>();

        // Survives the scene reload between levels
        private static int currentIndex;

        // Reset on entering Play mode, also when domain reload is turned off
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => currentIndex = 0;

        private LevelSO level;
        private float timeLeft;
        private int completedOrders;
        private bool finished;

        private GUIStyle textStyle;
        private GUIStyle bigTextStyle;
        private GUIStyle buttonStyle;

        private void Start()
        {
            Time.timeScale = 1f;

            if (levels.Count == 0 || OrderManager.Instance == null)
            {
                Debug.LogError($"{name}: no levels set, or no OrderManager in the scene", this);
                enabled = false;
                return;
            }

            currentIndex = Mathf.Clamp(currentIndex, 0, levels.Count - 1);
            level = levels[currentIndex];
            timeLeft = level.duration;

            OrderManager.Instance.ConfigureForLevel(level.recipes, level.maxActiveOrders, level.randomIngredientLevels);
            OrderManager.Instance.OnOrderComplete += HandleOrderComplete;
        }

        private void OnDestroy()
        {
            if (OrderManager.Instance != null)
                OrderManager.Instance.OnOrderComplete -= HandleOrderComplete;
        }

        private void Update()
        {
            if (finished) return;

            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f)
            {
                timeLeft = 0f;
                FinishLevel();
            }
        }

        private void HandleOrderComplete(object sender, OrderManager.OnOrderCompleteEventArgs e)
        {
            if (!finished) completedOrders++;
        }

        private void FinishLevel()
        {
            finished = true;
            OrderManager.Instance.SetSpawning(false);
            Time.timeScale = 0f;   // stops cooking, cutting and all other timers
        }

        private void LoadLevel(int index)
        {
            currentIndex = index;
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // ---- Temporary on-screen UI (IMGUI): no scene setup needed ----

        private void OnGUI()
        {
            if (level == null) return;
            CreateStyles();

            // Draw as if the screen were 1920x1080
            float scale = Screen.height / 1080f;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
            float width = Screen.width / scale;

            DrawTopBar(width);
            DrawLevelButtons();
            if (finished) DrawResults(width);
        }

        private void DrawTopBar(float width)
        {
            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);
            string thresholds = string.Join(" / ", level.starThresholds);

            var rect = new Rect(width / 2f - 360f, 10f, 720f, 90f);
            GUI.Box(rect, GUIContent.none);
            GUI.Label(new Rect(rect.x + 20f, rect.y + 8f, rect.width - 40f, 40f),
                $"Уровень {currentIndex + 1}/{levels.Count}: {level.levelName}      {minutes}:{seconds:00}", textStyle);
            GUI.Label(new Rect(rect.x + 20f, rect.y + 46f, rect.width - 40f, 40f),
                $"Заказов выполнено: {completedOrders}   (звёзды за {thresholds})", textStyle);
        }

        // For testing: jump to any level
        private void DrawLevelButtons()
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (GUI.Button(new Rect(10f + i * 70f, 10f, 60f, 50f), (i + 1).ToString(), buttonStyle))
                    LoadLevel(i);
            }
        }

        private void DrawResults(float width)
        {
            int stars = level.GetStars(completedOrders);
            bool hasNext = currentIndex + 1 < levels.Count;

            var rect = new Rect(width / 2f - 300f, 300f, 600f, 380f);
            GUI.Box(rect, GUIContent.none);
            GUI.Box(rect, GUIContent.none);   // twice for a darker background

            GUI.Label(new Rect(rect.x, rect.y + 30f, rect.width, 60f), "Смена окончена", bigTextStyle);
            GUI.Label(new Rect(rect.x, rect.y + 110f, rect.width, 50f),
                $"Заказов: {completedOrders}    Звёзды: {stars} из {level.starThresholds.Count}", textStyle);

            if (GUI.Button(new Rect(rect.x + 50f, rect.y + 200f, 230f, 70f), "Повторить", buttonStyle))
                LoadLevel(currentIndex);

            GUI.enabled = hasNext && stars > 0;
            string nextText = !hasNext ? "Последний уровень" : stars > 0 ? "Следующий" : "Нужна 1 звезда";
            if (GUI.Button(new Rect(rect.x + 320f, rect.y + 200f, 230f, 70f), nextText, buttonStyle))
                LoadLevel(currentIndex + 1);
            GUI.enabled = true;
        }

        private void CreateStyles()
        {
            if (textStyle != null) return;

            textStyle = new GUIStyle(GUI.skin.label) { fontSize = 28, alignment = TextAnchor.MiddleCenter };
            textStyle.normal.textColor = Color.white;

            bigTextStyle = new GUIStyle(textStyle) { fontSize = 44, fontStyle = FontStyle.Bold };

            buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 26 };
        }
    }
}
