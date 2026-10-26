using System.Collections;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    public class OrderTemplateUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI orderNameText;
        [SerializeField] private Transform iconsContainer;
        [SerializeField] private Transform iconTemplate;
        [SerializeField] private ProductRangeColorsSO rangeColorsSO;
    
        private Dish _order;
        private Coroutine shakeRoutine;
        
        private void Awake()
        {
            iconTemplate.gameObject.SetActive(false);
        }
    
        public void SetOrder(Dish order)
        {
            _order = order;
            orderNameText.text = order.Name;

            foreach (var ingredient in _order.ingredients)
            {
                Transform newIcon = Instantiate(iconTemplate, iconsContainer);
                newIcon.gameObject.SetActive(true);
                newIcon.GetComponent<Image>().sprite = ingredient.productSO.icon;
                if (ingredient.productSO.isMergable && ingredient.productLevel > 0)
                {
                    var rarityImage = newIcon.GetChild(0);
                    rarityImage.gameObject.SetActive(true);
                    rarityImage.GetComponent<Image>().color = rangeColorsSO.GetColor(ingredient.productLevel);
                }
            }
        }
    
        public Dish GetOrder()
        {
            return _order;
        }

        // A short wobble. Rotation is used because the cards' positions are controlled by a layout group
        public void Shake()
        {
            if (shakeRoutine != null) StopCoroutine(shakeRoutine);
            shakeRoutine = StartCoroutine(ShakeRoutine());
        }

        private IEnumerator ShakeRoutine()
        {
            const float duration = 0.4f;
            const float maxAngle = 8f;
            const float frequency = 30f;

            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                float damping = 1f - t / duration;
                transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * frequency) * maxAngle * damping);
                yield return null;
            }

            transform.localRotation = Quaternion.identity;
            shakeRoutine = null;
        }
    }
}
