using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System {
    public class ArgumentOutOfRangeExceptionUse : ArgumentOutOfRangeException
    {
        public static void ThrowIfNegative(int value) {
            if (value < 0) {
                throw new ArgumentOutOfRangeException("Value cannot be negative");
            }
        }
    }
}
