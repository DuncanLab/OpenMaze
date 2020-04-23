using System;
using System.Reflection;
using UnityEngine;

namespace Tests
{
    // Series of utility functions for UnityTests
    public class TestUtils
    {
        public static T Construct<T>(Type[] paramTypes, object[] paramValues)
        {
            var t = typeof(T);

            var ci = t.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null, paramTypes, null);

            if (ci == null)
            {
                Debug.LogError("constructor doesn't exist");
                throw new NullReferenceException();
            }

            return (T) ci.Invoke(paramValues);
        }
    }
}
