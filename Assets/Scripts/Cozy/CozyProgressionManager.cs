using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Cozy
{
    // PROTOTYPE (cozy test branch): endless orders, no timer. Served orders open new recipes
    // stage by stage; a just-opened recipe is ordered right away. Progress is kept between
    // sessions (PlayerPrefs). Remove this component from the scene to get the plain endless mode back.
    public class CozyProgressionManager : MonoBehaviour
    {
        private const string ServedOrdersKey = "cozy.servedOrders";
        private const float NoticeDuration = 4f;

        [SerializeField] private CozyProgressionSO progression;

        private readonly List<RecipeSO> menu = new List<RecipeSO>();
        private int servedOrders;
        private int openedStages;   // how many stages are open

        private string notice;
        private float noticeTimeLeft;

        private GUIStyle textStyle;
        private GUIStyle noticeStyle;
        private GUIStyle buttonStyle;

        private void Start()
        {
            if (progression == null || progression.stages.Count == 0 || OrderManager.Instance == null)
            {
                Debug.LogError($"{name}: no progression set, or no OrderManager in the scene", this);
                enabled = false;
                return;
            }

            servedOrders = PlayerPrefs.GetInt(ServedOrdersKey, 0);
            OpenReachedStages(announce: false);
            OrderManager.Instance.OnOrderComplete += HandleOrderComplete;
        }

        private void OnDestroy()
        {
            if (OrderManager.Instance != null)
                OrderManager.Instance.OnOrderComplete -= HandleOrderComplete;
        }

        private void Update()
        {
            if (noticeTimeLeft > 0f) noticeTimeLeft -= Time.deltaTime;
        }

        private void HandleOrderComplete(object sender, OrderManager.OnOrderCompleteEventArgs e)
        {
            AddServedOrders(1);
        }

        private void AddServedOrders(int amount)
        {
            servedOrders += amount;
            PlayerPrefs.SetInt(ServedOrdersKey, servedOrders);
            PlayerPrefs.Save();
            OpenReachedStages(announce: true);
        }

        private void OpenReachedStages(bool announce)
        {
            bool menuChanged = false;
            while (openedStages < progression.stages.Count &&
                   servedOrders >= progression.stages[openedStages].requiredOrders)
            {
                var stage = progression.stages[openedStages];
                openedStages++;

                foreach (var recipe in stage.recipes)
                {
                    if (recipe == null || menu.Contains(recipe)) continue;
                    menu.Add(recipe);
                    menuChanged = true;

                    if (announce)
                    {
                        OrderManager.Instance.QueueRecipe(recipe);
                        ShowNotice($"Новый рецепт: {recipe.recipeName}");
                    }
                }
            }

            if (menuChanged || openedStages > 0)
                OrderManager.Instance.SetMenu(menu);
        }

        private void ShowNotice(string text)
        {
            notice = noticeTimeLeft > 0f && !string.IsNullOrEmpty(notice) ? notice + "\n" + text : text;
            noticeTimeLeft = NoticeDuration;
        }

        // For testing
        private void ResetProgress()
        {
            PlayerPrefs.DeleteKey(ServedOrdersKey);
            servedOrders = 0;
            openedStages = 0;
            menu.Clear();
            OpenReachedStages(announce: false);
            ShowNotice("Прогресс сброшен");
        }

        // ---- Temporary on-screen UI (IMGUI): no scene setup needed ----

        private void OnGUI()
        {
            if (progression == null) return;
            CreateStyles();

            float scale = Screen.height / 1080f;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
            float width = Screen.width / scale;

            var rect = new Rect(width / 2f - 380f, 10f, 760f, 90f);
            GUI.Box(rect, GUIContent.none);
            GUI.Label(new Rect(rect.x + 20f, rect.y + 8f, rect.width - 40f, 40f),
                $"Заказов выполнено: {servedOrders}    Рецептов в меню: {menu.Count}", textStyle);
            GUI.Label(new Rect(rect.x + 20f, rect.y + 46f, rect.width - 40f, 40f), NextStageText(), textStyle);

            if (noticeTimeLeft > 0f && !string.IsNullOrEmpty(notice))
            {
                var noticeRect = new Rect(width / 2f - 400f, 130f, 800f, 120f);
                GUI.Box(noticeRect, GUIContent.none);
                GUI.Box(noticeRect, GUIContent.none);
                GUI.Label(noticeRect, notice, noticeStyle);
            }

            // Test buttons
            if (GUI.Button(new Rect(10f, 10f, 150f, 50f), "+1 заказ", buttonStyle))
                AddServedOrders(1);
            if (GUI.Button(new Rect(170f, 10f, 150f, 50f), "Сброс", buttonStyle))
                ResetProgress();
        }

        private string NextStageText()
        {
            if (openedStages >= progression.stages.Count)
                return "Открыты все рецепты";

            var next = progression.stages[openedStages];
            int left = Mathf.Max(next.requiredOrders - servedOrders, 0);
            return $"Дальше: «{next.stageName}» через {left} заказ(а/ов)";
        }

        private void CreateStyles()
        {
            if (textStyle != null) return;

            textStyle = new GUIStyle(GUI.skin.label) { fontSize = 28, alignment = TextAnchor.MiddleCenter };
            textStyle.normal.textColor = Color.white;

            noticeStyle = new GUIStyle(textStyle) { fontSize = 36, fontStyle = FontStyle.Bold, wordWrap = true };
            noticeStyle.normal.textColor = new Color(1f, 0.9f, 0.5f);

            buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 24 };
        }
    }
}
