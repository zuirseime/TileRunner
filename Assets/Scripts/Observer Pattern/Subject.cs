using System.Collections.Generic;
using UnityEngine;

public abstract class Subject : MonoBehaviour {
    private List<Observer> observers = new List<Observer>();

    public void AddObserver(Observer observer) {
        observers.Add(observer);
    }

    public void RemoveObserver(Observer observer) {
        observers.Remove(observer);
    }

    protected void Notify(NotificationType notification) {
        observers.ForEach(observer => observer.OnNotify(notification));
    }
}
