using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


// creates content for dropdown menu to select participants

public class DropDownHandler_shader : MonoBehaviour
{
    List<string> names = new List<string>() { "no_shader","shader_00", "shader_01", "shader_02", "shader_03" }; 

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
