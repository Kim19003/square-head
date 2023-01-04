using System;
using UnityEngine;

namespace Assets.Scripts
{
    public enum MovementReadAxis
    {
        Both,
        Horizontal,
        Vertical
    }

    public enum PickableType
    {
        None,
        Heart,
        Star,
        Ammunation
    }

    public enum CustomColor
    {
        Orange,
        DarkOrange,
        OrangeRed,
        Tomato,
        Gold,
        Amber
    }

    public enum Direction
    {
        None,
        Left,
        Right,
        Up,
        Down
    }

    public class Databank
    {
        public static int PickedStars { get; set; } = 0;
        public static int KilledEnemies { get; set; } = 0;
        public static float CompletionTime { get; set; } = 0f;
    }

    public class Helpers
    {
        public static Color32 GetCustomColor(CustomColor customColor)
        {
            switch (customColor)
            {
                case CustomColor.Orange:
                    return new Color32(255, 165, 0, 255);
                case CustomColor.DarkOrange:
                    return new Color32(255, 140, 0, 255);
                case CustomColor.OrangeRed:
                    return new Color32(255, 69, 0, 255);
                case CustomColor.Tomato:
                    return new Color32(255, 99, 71, 255);
                case CustomColor.Gold:
                    return new Color32(255, 215, 0, 255);
                case CustomColor.Amber:
                    return new Color32(255, 191, 0, 255);
                default:
                    return new Color32(255, 255, 255, 255);
            }
        }

        public static Color GetInWaterColor(float alpha)
        {
            return new Color(0.6f, 0.6f, 1f, alpha);
        }

        public static float GetWaterGravityScale()
        {
            return 1f;
        }

        public static void IgnoreCollisions(Collider2D[] collisions1, Collider2D[] collisions2, bool ignore)
        {
            foreach (Collider2D collision1 in collisions1)
            {
                foreach (Collider2D collision2 in collisions2)
                {
                    Physics2D.IgnoreCollision(collision1, collision2, ignore);
                }
            }
        }
    }

    public class TimedUnityAction
    {
        public Action Action
        {
            get
            {
                return action;
            }
            set
            {
                if (value != null)
                {
                    action = value;
                }
            }
        }
        Action action = null;

        public float Interval
        {
            get
            {
                return interval;
            }
            private set
            {
                if (value > 0)
                {
                    interval = value;
                }
            }
        }
        private float interval = 0f;

        float nextActionTime = 0f;

        bool started = false;

        public TimedUnityAction()
        {
            
        }

        public TimedUnityAction(Action action, float interval)
        {
            if (action == null)
            {
                throw new ArgumentException("Action cannot be null.");
            }
            else if (interval <= 0)
            {
                throw new ArgumentException("Interval must be greater than 0.");
            }

            Action = action;
            Interval = interval;
        }

        /// <summary>
        /// Runs {Action} every {Interval} second. Call this in Unity's Update method.
        /// </summary>
        public void Run(float startDelay = 0f)
        {
            if (Action == null)
            {
                throw new ArgumentException("Action cannot be null.");
            }
            else if (Interval <= 0)
            {
                throw new ArgumentException("Interval must be greater than 0.");
            }
            else if (startDelay < 0)
            {
                throw new ArgumentException("Start delay must be greater than or equal to 0.");
            }

            if (Time.timeSinceLevelLoad > (startDelay > 0 && !started ? startDelay : nextActionTime))
            {
                Action();

                nextActionTime += Interval;

                started = true;
            }
        }

        /// <summary>
        /// Runs {action} every {interval} second. Call this in Unity's Update method.
        /// </summary>
        public void Run(Action action, float interval, float startDelay = 0f)
        {
            if (action == null)
            {
                throw new ArgumentException("Action cannot be null.");
            }
            else if (interval <= 0)
            {
                throw new ArgumentException("Interval must be greater than 0.");
            }
            else if (startDelay < 0)
            {
                throw new ArgumentException("Start delay must be greater than or equal to 0.");
            }

            if (Action == null)
            {
                Action = action;
            }
            else if (Action != action)
            {
                throw new ArgumentException("The action has already been initialized, and cannot be changed here. Use the SetAction method to change the action.");
            }

            if (Interval != interval)
            {
                Interval = interval;
            }

            if (Time.timeSinceLevelLoad > (startDelay > 0 && !started ? startDelay : nextActionTime))
            {
                Action();

                nextActionTime += Interval;

                started = true;
            }
        }

        public void SetAction(Action action)
        {
            Action = action;
        }

        public void SetInterval(float interval)
        {
            Interval = interval;
        }
    }

    public static class FloatExtensions
    {
        public static bool IsAny(this float value, params float[] otherValues)
        {
            foreach (float otherValue in otherValues)
            {
                if (value == otherValue)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsIn(this float value, float minValue, float maxValue)
        {
            return value >= minValue && value <= maxValue;
        }
    }

    public static class Collider2DArrayExtensions
    {
        public static void EnableAll(this Collider2D[] collider2Ds)
        {
            foreach (Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = true;
            }
        }

        public static void DisableAll(this Collider2D[] collider2Ds)
        {
            foreach (Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }
        }
    }
}
