using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISubscriber <T, U> {
    public void ReceiveMessage(T data1, U data2);
}
