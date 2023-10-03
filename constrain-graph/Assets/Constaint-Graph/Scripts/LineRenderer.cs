using UnityEngine;

namespace Shahant.Constraint
{
    public class LineRenderer : MonoBehaviour
    {
        [SerializeField] GameObject _linePrefab;
        [SerializeField] Transform _container;

        public void GenerateLine(Vector2 start, Vector2 end)
        {
            var length = (end - start).magnitude;
            var angle = Vector2.SignedAngle(Vector2.right, end - start);

            var line = Instantiate(_linePrefab, _container, false);
            var lineRect = line.transform as RectTransform;
            lineRect.pivot = new Vector2(0, 0.5f);
            lineRect.anchorMin = lineRect.anchorMax = Vector2.up;
            lineRect.anchoredPosition = start;
            lineRect.sizeDelta = new Vector2(length, lineRect.sizeDelta.y);
            lineRect.localRotation = Quaternion.Euler(0, 0, angle);
        }

        public void Clear()
        {
            for (; _container.childCount > 0;)
            {
                DestroyImmediate(_container.GetChild(0).gameObject);
            }
        }
    }
}
