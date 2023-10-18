using UnityEngine;

namespace Shahant.PathFinding
{
    public enum Direction
    {
        None = 0,
        Up = 1,
        Right = 2,
        Down = 3,
        Left = 4
    }

    public static class DirectionExtension
    {
        public static Direction ToDirection(this Vector2Int directionVector)
        {
            if (directionVector.x * directionVector.y != 0) return Direction.None;
            var x = directionVector.x == 0 ? 0 : (directionVector.x < 0 ? -1 : 1);
            var y = directionVector.y == 0 ? 0 : (directionVector.y < 0 ? -1 : 1);

            switch (y)
            {
                case -1: return Direction.Down;
                case 1: return Direction.Up;
            }

            switch (x)
            {
                case -1: return Direction.Left;
                case 1: return Direction.Right;
            }

            return Direction.None;

        }
        public static Direction TurnBack(this Direction direction)
        {
            return direction switch
            {
                Direction.None => Direction.None,
                Direction.Down => Direction.Up,
                Direction.Up => Direction.Down,
                Direction.Right => Direction.Left,
                Direction.Left => Direction.Right,
                _ => Direction.None,
            };
        }
        public static Vector2Int ToVector(this Direction direcion)
        {
            return direcion switch
            {
                Direction.None => Vector2Int.zero,
                Direction.Down => Vector2Int.down,
                Direction.Up => Vector2Int.up,
                Direction.Left => Vector2Int.left,
                Direction.Right => Vector2Int.right,
                _ => Vector2Int.zero,
            };

        }
    }
}


