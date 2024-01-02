using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System {
    public class ArgumentNullExceptionUse : ArgumentNullException
    {
        public static void ThrowIfNull(object? arg) {
            if (arg == null) {
                throw new ArgumentOutOfRangeException("Object cannot be null");
            }
        }
    }
}
