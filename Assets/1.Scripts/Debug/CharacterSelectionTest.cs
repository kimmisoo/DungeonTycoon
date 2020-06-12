using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionTest : MonoBehaviour
{
    public int spAdvIdx;
    public Dropdown dropdown;
    public Text selected;

    void Start()
    {
        
    }
    // Update is called once per frame
    void Update ()
    {
        dropdown.ClearOptions();

        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach(GameObject spAdv in GameManager.Instance.specialAdventurers)
        {
            options.Add(new Dropdown.OptionData(spAdv.name));
        }

        dropdown.AddOptions(options);
        spAdvIdx = dropdown.value;

        selected.text = GameManager.Instance.playerSpAdvIndex.ToString();
    }

    public void OnDropBoxValueChanged(int num)
    {
        spAdvIdx = num;
    }

    public void ChooseSpAdv()
    {
        GameManager.Instance.ChooseSpAdv(spAdvIdx);
    }
}
