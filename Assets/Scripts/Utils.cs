using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public static class Utils
{
    // UI SetListener extension that removes previous and then adds new listener
    // (this version is for onClick etc.)
    public static void SetListener(this UnityEvent uEvent, UnityAction call)
    {
        uEvent.RemoveAllListeners();
        uEvent.AddListener(call);
    }
}

public static class UIUtils
{
    // instantiate/remove enough prefabs to match amount
    public static void BalancePrefabs(GameObject prefab, int amount, Transform parent)
    {
        // instantiate until amount
        for (int i = parent.childCount; i < amount; ++i)
        {
            GameObject go = GameObject.Instantiate(prefab);
            go.transform.SetParent(parent, false);
        }

        // delete everything that's too much
        // (backwards loop because Destroy changes childCount)
        for (int i = parent.childCount - 1; i >= amount; --i)
            GameObject.Destroy(parent.GetChild(i).gameObject);
    }

    /// <summary>
    /// takes a substring between two anchor strings (or the end of the string if that anchor is null)
    /// </summary>
    /// <param name="this">a string</param>
    /// <param name="from">an optional string to search after</param>
    /// <param name="until">an optional string to search before</param>
    /// <param name="comparison">an optional comparison for the search</param>
    /// <returns>a substring based on the search</returns>
    public static string Substring(this string @this, string from = null, string until = null, StringComparison comparison = StringComparison.InvariantCulture)
    {
        var fromLength = (from ?? string.Empty).Length;
        var startIndex = !string.IsNullOrEmpty(from)
            ? @this.IndexOf(from, comparison) + fromLength
            : 0;

        if (startIndex < fromLength) { throw new ArgumentException("from: Failed to find an instance of the first anchor"); }

        var endIndex = !string.IsNullOrEmpty(until)
        ? @this.IndexOf(until, startIndex, comparison)
        : @this.Length;

        if (endIndex < 0) { throw new ArgumentException("until: Failed to find an instance of the last anchor"); }

        var subString = @this.Substring(startIndex, endIndex - startIndex);
        return subString;
    }

    // find out if any input is currently active by using Selectable.all
    // (FindObjectsOfType<InputField>() is far too slow for huge scenes)
    public static bool AnyInputActive()
    {
        return Selectable.allSelectablesArray.Any(
            sel => sel is InputField && ((InputField)sel).isFocused
        );
    }

    // deselect any UI element carefully
    // (it throws an error when doing it while clicking somewhere, so we have to
    //  double check)
    public static void DeselectCarefully()
    {
        if (!Input.GetMouseButton(0) &&
            !Input.GetMouseButton(1) &&
            !Input.GetMouseButton(2))
            EventSystem.current.SetSelectedGameObject(null);
    }
}
