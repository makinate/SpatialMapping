using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


// creates content for dropdown menu to select participants

public class DropDownHandler_layout : MonoBehaviour
{
    List<string> names = new List<string>() {   "select_layout", "layout_01", "layout_02", "layout_03", "layout_04"}; 

    public Dropdown dropdown;
    public Text selectedParticipant; 

    public void Dropdown_IndexChanged (int index)
    {
        selectedParticipant.text = names[index];
         
        Debug.Log(names[index] + " selected");
    }


    void Start ()
    {
        PopulateList();
    }


    void PopulateList()
    {
        //  TODO: read these values from the web site 

        dropdown.AddOptions(names);



    }
}
