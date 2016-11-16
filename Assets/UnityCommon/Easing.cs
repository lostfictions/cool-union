using UnityEngine;
using System.Collections;

public static class Easing
{
    // Adapted from source : http://www.robertpenner.com/easing/

    const float PI_OVER2 = Mathf.PI / 2;

    public static float Ease(float linearStep, AnimationCurve easingCurve)
    {
        return easingCurve.Evaluate(linearStep);
    }

    public static float Ease(float linearStep, float acceleration, EaseType type)
    {
        float easedStep = acceleration > 0
            ? EaseIn(linearStep, type)
            : acceleration < 0
                ? EaseOut(linearStep, type)
                : linearStep;

        return Mathf.Lerp(linearStep, easedStep, Mathf.Abs(acceleration));
    }

    public static float EaseIn(float linearStep, EaseType type)
    {
        switch(type) {
            case EaseType.None:
                return 1;
            case EaseType.Linear:
                return linearStep;
            case EaseType.Sine:
                return Sine.EaseIn(linearStep);
            case EaseType.Quad:
                return Power.EaseIn(linearStep, 2);
            case EaseType.Cubic:
                return Power.EaseIn(linearStep, 3);
            case EaseType.Quartic:
                return Power.EaseIn(linearStep, 4);
            case EaseType.Quintic:
                return Power.EaseIn(linearStep, 5);
            case EaseType.Circ:
                return Circ.EaseIn(linearStep);
            case EaseType.Bounce:
                return Bounce.EaseIn(linearStep);
            case EaseType.Back:
                return Back.EaseIn(linearStep);
            case EaseType.Elastic:
                return Elastic.EaseIn(linearStep);
        }
        Debug.LogError("Um.");
        return 0;
    }

    public static float EaseOut(float linearStep, EaseType type)
    {
        switch(type) {
            case EaseType.None:
                return 1;
            case EaseType.Linear:
                return linearStep;
            case EaseType.Sine:
                return Sine.EaseOut(linearStep);
            case EaseType.Quad:
                return Power.EaseOut(linearStep, 2);
            case EaseType.Cubic:
                return Power.EaseOut(linearStep, 3);
            case EaseType.Quartic:
                return Power.EaseOut(linearStep, 4);
            case EaseType.Quintic:
                return Power.EaseOut(linearStep, 5);
            case EaseType.Circ:
                return Circ.EaseOut(linearStep);
            case EaseType.Bounce:
                return Bounce.EaseOut(linearStep);
            case EaseType.Back:
                return Back.EaseOut(linearStep);
            case EaseType.Elastic:
                return Elastic.EaseOut(linearStep);
        }
        Debug.LogError("Um.");
        return 0;
    }

    public static float EaseInOut(
        float linearStep,
        EaseType easeInType,
        float acceleration,
        EaseType easeOutType,
        float deceleration)
    {
        return linearStep < 0.5
            ? Mathf.Lerp(linearStep, EaseInOut(linearStep, easeInType), acceleration)
            : Mathf.Lerp(linearStep, EaseInOut(linearStep, easeOutType), deceleration);
    }

    public static float EaseInOut(float linearStep, EaseType easeInType, EaseType easeOutType)
    {
        return linearStep < 0.5 ? EaseInOut(linearStep, easeInType) : EaseInOut(linearStep, easeOutType);
    }

    public static float EaseInOut(float linearStep, EaseType type)
    {
        switch(type) {
            case EaseType.None:
                return 1;
            case EaseType.Linear:
                return linearStep;
            case EaseType.Sine:
                return Sine.EaseInOut(linearStep);
            case EaseType.Quad:
                return Power.EaseInOut(linearStep, 2);
            case EaseType.Cubic:
                return Power.EaseInOut(linearStep, 3);
            case EaseType.Quartic:
                return Power.EaseInOut(linearStep, 4);
            case EaseType.Quintic:
                return Power.EaseInOut(linearStep, 5);
            case EaseType.Circ:
                return Circ.EaseInOut(linearStep);

            case EaseType.Bounce:
                return Bounce.EaseInOut(linearStep);
            case EaseType.Back:
                return Back.EaseInOut(linearStep);
            case EaseType.Elastic:
                return Elastic.EaseInOut(linearStep);
        }
        Debug.LogError("Um.");
        return 0;
    }

    static class Sine
    {
        public static float EaseIn(float s)
        {
            return Mathf.Sin(s * PI_OVER2 - PI_OVER2) + 1;
        }

        public static float EaseOut(float s)
        {
            return Mathf.Sin(s * PI_OVER2);
        }

        public static float EaseInOut(float s)
        {
            return (Mathf.Sin(s * Mathf.PI - PI_OVER2) + 1) / 2;
        }
    }

    static class Power
    {
        public static float EaseIn(float s, int power)
        {
            return Mathf.Pow(s, power);
        }

        public static float EaseOut(float s, int power)
        {
            var sign = power % 2 == 0 ? -1 : 1;
            return (sign * (Mathf.Pow(s - 1, power) + sign));
        }

        public static float EaseInOut(float s, int power)
        {
            s *= 2;
            if(s < 1) {
                return EaseIn(s, power) / 2;
            }
            var sign = power % 2 == 0 ? -1 : 1;
            return (sign / 2.0f * (Mathf.Pow(s - 2, power) + sign * 2));
        }
    }

    static class Circ
    {
        public static float EaseIn(float s)
        {
            return -(Mathf.Sqrt(1 - s * s) - 1);
        }

        public static float EaseOut(float s)
        {
            return Mathf.Sqrt(1 - (s - 1) * s);
        }

        public static float EaseInOut(float s)
        {
            s *= 2;
            if(s < 1) {
                return EaseIn(s) / 2;
            }
            return ((Mathf.Sqrt(1 - (s - 2) * s)) + 1) / 2;
        }
    }

    //TODO: sanity check on these
    static class Bounce
    {
        public static float EaseIn(float s)
        {
            return 1f - EaseOut(1f - s);
        }

        public static float EaseOut(float s)
        {
            if(s < (1 / 2.75f)) {
                return (7.5625f * s * s);
            }
            else if(s < (2 / 2.75f)) {
                s -= (1.5f / 2.75f);
                return (7.5625f * s * s + .75f);
            }
            else if(s < (2.5 / 2.75)) {
                s -= (2.25f / 2.75f);
                return (7.5625f * s * s + .9375f);
            }
            else {
                s -= (2.625f / 2.75f);
                return (7.5625f * s * s + .984375f);
            }
        }

        public static float EaseInOut(float s)
        {
            if(s < 0.5f) {
                return EaseIn(s * 2) * 0.5f;
            }
            else {
                return EaseOut(s) * 0.5f + 0.5f;
            }
        }
    }

    static class Back
    {
        public static float EaseIn(float s)
        {
            return s * s * (2.70158f * s - 1.70158f);
        }

        public static float EaseOut(float s)
        {
            s = s - 1;
            return (s * s * (2.70158f * s + 1.70158f) + 1);
        }

        public static float EaseInOut(float s)
        {
            s = s * 2f;
            if(s < 1) {
                return 1f / 2 * (s * s * (((1.70158f * 1.525f) + 1) * s - (1.70158f * 1.525f)));
            }
            s -= 2;
            return 1f / 2 * ((s) * s * (((1.70158f * 1.525f) + 1) * s + (1.70158f * 1.525f)) + 2);
        }
    }

    static class Elastic
    {
        public static float EaseIn(float linearStep)
        {
            if(linearStep == 0) {
                return 0;
            }
            if(linearStep == 1) {
                return 1f;
            }

            return -(Mathf.Pow(2, 10 * (linearStep - 1)) * Mathf.Sin((linearStep - 1.075f) * (2 * Mathf.PI) / 0.3f));
        }

        public static float EaseOut(float linearStep)
        {
            if(linearStep == 0) {
                return 0;
            }
            if(linearStep == 1) {
                return 1f;
            }

            return Mathf.Pow(2, -10 * linearStep) * Mathf.Sin((linearStep - 0.075f) * (2 * Mathf.PI) / 0.3f) + 1f;
        }

        public static float EaseInOut(float linearStep)
        {
            if(linearStep == 0) {
                return 0;
            }
            if(linearStep == 1) {
                return 1f;
            }

            linearStep *= 2f;

            if(linearStep < 1) {
                return -0.5f *
                       (Mathf.Pow(2, 10 * (linearStep - 1f)) * Mathf.Sin((linearStep - 1.075f) * (2 * Mathf.PI) / 0.3f));
            }
            return Mathf.Pow(2, -10 * (linearStep - 1f)) * Mathf.Sin((linearStep - 1.075f) * (2 * Mathf.PI) / 0.3f) *
                   1.5f;
        }
    }
}

public enum EaseType
{
    None,
    Linear,
    Sine,
    Quad,
    Cubic,
    Quartic,
    Quintic,
    Circ,
    Bounce,
    Back,
    Elastic
}
