using System.Collections;
using GameLocalization;
using Managers;
using Tables;
using TMPro;
using UnityEngine;

namespace UIScripts
{
    // Short message above an order counter when it refuses a plate
    public class OrderFeedbackUI : MonoBehaviour
    {
        [Tooltip("The counter this message belongs to. Found in the parents if left empty")]
        [SerializeField] private OrderTable table;
        [SerializeField] private TMP_Text label;
        [SerializeField] private LocalizationTableSO texts;
        [SerializeField] private float showSeconds = 1.5f;
        [SerializeField] private float fadeSeconds = 0.3f;

        private Coroutine showRoutine;

        private void Awake()
        {
            if (table == null)
                table = GetComponentInParent<OrderTable>();
            label.alpha = 0f;
        }

        private void Start()
        {
            OrderManager.Instance.OnOrderRejected += HandleOrderRejected;
        }

        private void OnDestroy()
        {
            if (OrderManager.Instance != null)
                OrderManager.Instance.OnOrderRejected -= HandleOrderRejected;
        }

        private void HandleOrderRejected(object sender, OrderManager.OnOrderRejectedEventArgs e)
        {
            if (e.table != table) return;

            string key = OrderRejectReasonKeys.Get(e.reason);
            label.text = texts != null ? texts.Get(key) : key;

            if (showRoutine != null) StopCoroutine(showRoutine);
            showRoutine = StartCoroutine(ShowRoutine());
        }

        private IEnumerator ShowRoutine()
        {
            label.alpha = 1f;
            yield return new WaitForSeconds(showSeconds);

            for (float t = 0f; t < fadeSeconds; t += Time.deltaTime)
            {
                label.alpha = 1f - t / fadeSeconds;
                yield return null;
            }

            label.alpha = 0f;
            showRoutine = null;
        }
    }
}
