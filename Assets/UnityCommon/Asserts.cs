using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using System.Diagnostics;
using System.Linq.Expressions;

namespace UnityCommon.Assert
{
    public static class Assert
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
        public static void That(Expression<Func<bool>> predicate)
        {
            if(!predicate.Compile().Invoke()) {
                Debug.LogError("Assertion failed!\n" + predicate.Body);
                Debug.Break();
            }
        }
    }
}
