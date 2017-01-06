using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if WINDOWS_UWP
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using System;
using System.Collections;
using UnityEngine;
#endif



public class TextMaker : MonoBehaviour
{

    void MyGameMethod()
    {
        Debug.Log("The TexMaker is working!");
    }
    /* DO NOTHING 
    private void txtTextMesh;

    string plainText;
    void Start()
    {
        Debug.Log("Please create a file!!!");
        
        #if WINDOWS_UWP

                    Task task = new Task(

                        async () =>
                        {                              
                            StorageFile textFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///MaxText.txt"));
                            plainText = await FileIO.ReadTextAsync(textFile);

                            txtTextMesh.text = plainText;

                        });
                    task.Start();
                    task.Wait();

        #endif
    }
    */
}