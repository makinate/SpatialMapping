using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


// creates content for dropdown menu to select participants

public class DropDownHandler_sbj : MonoBehaviour
{
    List<string> names = new List<string>() {   "pilot1", "pilot2", "pilot3", "pilot4",
                                                "pilot5", "pilot6", "sbj000",
                                                "sbj001", "sbj002", "sbj003", "sbj004", 
                                                "sbj005", "sbj006", "sbj007", "sbj008", 
                                                "sbj009", "sbj010", "sbj011", "sbj012", 
                                                "sbj013", "sbj014", "sbj015", "sbj016", 
                                                "sbj017", "sbj018", "sbj019", "sbj020"}; 

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
