using System.Collections.Generic;
using UnityEngine;

public abstract class Subject : MonoBehaviour {
    private List<IObserver> observers = new();

    public void AddObserver(IObserver observer) {
        observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer) {
        observers.Remove(observer);
    }

    protected void NotifyObservers(NotificationType notification) {
        observers.ForEach(o => o.OnNotify(notification));
    }
}
