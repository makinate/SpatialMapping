using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


// creates content for dropdown menu to select participants

public class DropDownHandler_trainingOrTrial : MonoBehaviour
{
    List<string> names = new List<string>() {"TrainingOrTrial",   "Training", "Trial"}; 

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
