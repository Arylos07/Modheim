using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModpackListElement : MonoBehaviour
{
    public int index;
    public Text PackName;
    public Text PackAuthor;
    public Image EnabledImage;
    public Button ElementButton;

    public void Select()
    {
        ModpackList.instance.SelectPack(index);
    }

    private void OnMouseOver()
    {
        Debug.Log("mouse");
    }
}
