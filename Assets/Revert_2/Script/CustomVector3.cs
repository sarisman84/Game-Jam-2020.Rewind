using UnityEngine;

namespace Revert_2.Script
{
    public static class CustomVector3
    {
        public enum Axis
        {
            XY,
            YZ,
            ZX,
        }

        public static Vector3 AddWithClampedMagnitude(Vector3 firstValue, Vector3 secondValue, float maxMagnitude)
        {
            firstValue += secondValue;
            firstValue = Vector3.ClampMagnitude(firstValue, maxMagnitude);
            return firstValue;
        }

        public static Vector3 ToVector3(this Vector2 value, Axis axis)
        {
            switch (axis)
            {
                case Axis.XY:
                    return new Vector3(value.x, value.y, 0);
                case Axis.YZ:
                    return new Vector3(0, value.y, value.x);
                case Axis.ZX:
                    return new Vector3(value.x, 0, value.y);
            }

            return Vector3.zero;
        }
    }
}
