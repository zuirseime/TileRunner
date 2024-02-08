using System;
using UnityEngine.UI;

public static class ButtonExtention {
    public static void AddEventListener<T>(this Button btn, T param, Action<T> OnClick) {
        btn.onClick.AddListener(() => { OnClick(param); });
    }

    public static void AddEventListener<T1, T2>(this Button btn, T1 param1, T2 param2, Action<T1, T2> OnClick) {
        btn.onClick.AddListener(() => { OnClick(param1, param2); });
    }

    public static void AddEventListener<T1, T2, T3>(this Button btn, T1 param1, T2 param2, T3 param3, Action<T1, T2, T3> OnClick) {
        btn.onClick.AddListener(() => { OnClick(param1, param2, param3); });
    }
}