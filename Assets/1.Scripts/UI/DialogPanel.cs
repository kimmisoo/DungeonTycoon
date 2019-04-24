using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogPanel : UIObject {

    public GameObject nameText;
    public GameObject characterImage1;
    public GameObject characterIamge2;
    public GameObject dialogText;

    public float delay = 0.02f;
    public string currentText;

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();   
    }

    public void ShowTemporalText()
    {

    }

    public void OnDialogTouch()
    {

    }



    IEnumerator ShowCurrentText()
    {
        yield return null;
    }

}
