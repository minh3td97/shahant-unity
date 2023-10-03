using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace Shahant.Constraint
{
    public class ConstraintGraphLayout : MonoBehaviour
    {
        
        [SerializeField] Corner _startCorner;
        [SerializeField] Axis _startAxis;
        [SerializeField] TextAnchor _childAlignment;
        [SerializeField] Vector2 _spacing;
        [SerializeField] Vector2 _offset;

        private Rect _rect;

        public Vector2 GetStartPosition(RectTransform containerRT, Vector2 itemSize)
        {
            float width = _startAxis == Axis.Horizontal ? _rect.width : _rect.height;
            float height = _startAxis == Axis.Horizontal ? _rect.height : _rect.width;
            Vector2 rectSize = (itemSize +_spacing) * _rect.size + itemSize;

            Rect containerRect = containerRT.rect;
            Rect contentRect = new Rect();
            contentRect.size = rectSize;

            return CalculateContentPos(containerRect, contentRect) 
                + CalculateStartPosInContent(contentRect, itemSize) 
                + _offset;
        } 

        private Vector2 CalculateContentPos(Rect containerRect, Rect contentRect)
        {
            Vector2 contentPos = Vector2.zero;
            switch (_childAlignment)
            {
                case TextAnchor.UpperLeft:
                    contentPos = Vector2.zero;
                    break;
                case TextAnchor.UpperCenter:
                    contentPos.x = 0.5f * (containerRect.width - contentRect.width);
                    break;
                case TextAnchor.UpperRight:
                    contentPos.x = containerRect.width - contentRect.width;
                    break;
                case TextAnchor.MiddleLeft:
                    contentPos.y = -0.5f * (containerRect.height - contentRect.height);
                    break;
                case TextAnchor.MiddleCenter:
                    contentPos.x = 0.5f * (containerRect.width - contentRect.width);
                    contentPos.y = -0.5f * (containerRect.height - contentRect.height);
                    break;
                case TextAnchor.MiddleRight:
                    contentPos.x = containerRect.width - contentRect.width;
                    contentPos.y = -0.5f * (containerRect.height - contentRect.height);
                    break;
                case TextAnchor.LowerLeft:
                    contentPos.y = -containerRect.height + contentRect.height;
                    break;
                case TextAnchor.LowerCenter:
                    contentPos.x = 0.5f * (containerRect.width - contentRect.width);
                    contentPos.y = -containerRect.height + contentRect.height;
                    break;
                case TextAnchor.LowerRight:
                    contentPos.x = containerRect.width - contentRect.width;
                    contentPos.y = -containerRect.height + contentRect.height;
                    break;
                
                default:
                    contentPos = Vector2.zero;
                    break;
            }
            return contentPos;
        }
        private Vector2 CalculateStartPosInContent(Rect contentRect, Vector2 itemSize)
        {
            return _startCorner switch
            {
                Corner.UpperLeft => Vector2.zero,
                Corner.UpperRight => new Vector2(contentRect.width - itemSize.x, 0),
                Corner.LowerLeft => new Vector2(0, -contentRect.height + itemSize.y),
                Corner.LowerRight => new Vector2(contentRect.width - itemSize.x, -contentRect.height + itemSize.y),
                _ => Vector2.zero,
            };
        }

        public Vector2 CalculatePos(Vector2 coord, Vector2 itemSize, Vector2 startPos)
        {
            return startPos + (itemSize + _spacing) * CalculateCoord(coord) + itemSize * new Vector2(0.5f, -0.5f);
        }
        public Vector2 CalculateCoord(Vector2 coord)
        {
            float x = coord.x - _rect.xMin, y = coord.y - _rect.yMax;
            switch (_startCorner)
            {
                case Corner.UpperRight:
                    x = -x;
                    break;
                case Corner.LowerLeft:
                    y = -y;
                    break;
                case Corner.LowerRight:
                    x = -x;
                    y = -y;
                    break;
                default:
                    break;
            }
            if (_startAxis == Axis.Horizontal) return new Vector2(x, y);
            else return new Vector2(-y, -x);
        }

        public void SetGraphCoordRect(Rect rect)
        {
            _rect = rect;
        }

    }
}
