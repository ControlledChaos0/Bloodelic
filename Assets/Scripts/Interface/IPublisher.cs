using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public interface IPublisher <T, U> {
    public void Publish(T data, U data2);
    public void Subscribe(MonoBehaviour subscriber);
    public void Unsubscribe(MonoBehaviour subscriber);
}