using UnityEngine;
using UnityEngine.UI;

namespace Shahant.PathFinding
{
    public class NodeView : MonoBehaviour
    {
        [SerializeField] Node _data;
        [SerializeField] GameObject[] _walls;

        public Node Data => _data;

        public event System.Action<NodeView> OnButtonClicked;

        public void Setup(Node data)
        {
            _data = data;
            UpdateWalls();
            
        }

        public void SetColor(Color color)
        {
            GetComponent<Image>().color = color;
        }

        public void UpdateWalls()
        {
            for (int i = 0; i < _walls.Length; ++i) _walls[i].SetActive(false);
            for(int i = 0; i < Data.Walls.Count; ++i)
            {
                _walls[(int)Data.Walls[i] - 1].SetActive(true);
            }
        }

        public void OnClicked()
        {
            OnButtonClicked?.Invoke(this);
        }
    }
}
