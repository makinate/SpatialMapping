// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Text;
using UnityEngine;

#if !UNITY_EDITOR  // for WPA
using System.Threading.Tasks;
using Windows.Storage;
#endif

/// <summary>
/// PTDataFileWriter prints comma-delimited data strings to .csv files. aasd
/// The included functions allow one to keep a .csv file open and repeatedly append lines to it,
/// or to create a .csv, add a string to it, and close it in one call.
/// </summary>
internal class PTDataFileWriter
{
    private StringBuilder stringBuilder;           
    private StreamWriter appendableStreamWriter;
    private string appendableFileNameWithPath;


    /// <summary>
    /// Constructor for PTDataFileWriter
    /// </summary>
    public PTDataFileWriter()
    {
        stringBuilder = new StringBuilder(1000);
    }

    /// <summary>
    /// Add string content to be exported as part of a data file.
    /// </summary>
    /// <param name="stringToAdd">The line to be appended to the string builder</param>
    public void AppendDataStringToStringBuilder(string stringToAdd)
    {
        stringBuilder.Append(stringToAdd);
    }

    /// <summary>
    /// Reset the string builder's buffer.
    /// </summary>
    public void ClearStringBuilder()
    {
        stringBuilder.Length = 0;
    }

    /// <summary>
    /// Create a new file and save the current contents of the string builder.
    /// This function chooses the proper file-writing implementation based on the current platform.
    /// </summary>
    /// <param name="fileName">The name of the the .csv to be written.</param>
    /// <param name="includeTimeStamp"> Whether or not to append the current time to the filename.</param>
    public void WriteToNewFile(string fileName, bool includeTimeStamp)
    {

#if UNITY_EDITOR
        DesktopWriteDataToNewFile(fileName, includeTimeStamp, this.stringBuilder.ToString());
#else
        WsaWriteDataToNewFile(fileName, includeTimeStamp, stringBuilder.ToString());
#endif

    }

#if UNITY_EDITOR || UNITY_STANDALONE
    /// <summary>
    /// The Unity Editor and Standalone implementation of creating a file, writing a string to the file, and closing the file.
    /// </summary>
    /// <param name="fileName">The name of the the .csv to be written.</param>
    /// <param name="includeTimeStamp"> Whether or not to append the current time to the filename.</param>
    /// <param name="stringContents">The string to be saved to the file.</param>
    public void DesktopWriteDataToNewFile(string fileName, bool includeTimeStamp, string stringContents)
    {
        string fileNameWithPath;
        if (includeTimeStamp)
        {
            string timeStamp = string.Format("{0:_yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
            fileNameWithPath = string.Format("{0}/{1}_{2}.csv", Application.dataPath, fileName, timeStamp);
        }
        else
        {
            fileNameWithPath = string.Format("{0}/{1}.csv", Application.dataPath, fileName);
        }

        var sr = File.CreateText(fileNameWithPath);
        sr.Write(stringContents);
        sr.Close();
    }
#else
    /// <summary>
    /// The WSA implementation of creating a file, writing a string to the file, and closing the file.
    /// </summary>
    /// <param name="fileName">The name of the the .csv to be written.</param>
    /// <param name="includeTimeStamp"> Whether or not to append the current time to the filename.</param>
    /// <param name="stringContents">The string to be saved to the file.</param>
    private async Task WsaWriteDataToNewFile(string fileName, bool includeTimeStamp, string stringContents)
    {
        string fileNameWithPath;

        if (includeTimeStamp)
        {
            string timeStamp = string.Format("{0:_yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
            fileNameWithPath = string.Format("{0}_{1}.csv", fileName, timeStamp);
        }
        else
        {
            fileNameWithPath = string.Format("{0}.csv", fileName);
        }
        StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(UnityEngine.Windows.Directory.localFolder);
        StorageFile storageFile = await folder.CreateFileAsync(fileNameWithPath, CreationCollisionOption.OpenIfExists);
        using (StreamWriter sw = new StreamWriter(await storageFile.OpenStreamForWriteAsync()))
        {
            await sw.WriteAsync(stringContents);
        }
    }
#endif


    /// <summary>
    /// Create a new file and keep it open so strings can be repeatedly appended to its end.
    /// This function chooses the proper file-writing implementation based on the current platform.
    /// </summary>
    /// <param name="fileName">The name of the the .csv to be written.</param>
    /// <param name="includeTimeStamp"> Whether or not to append the current time to the filename.</param>
    public void CreateNewAppendableFile(string filename, bool includeTimeStamp)
    {
#if UNITY_EDITOR
        DesktopCreateNewAppendableFile(filename, includeTimeStamp);
#else
        Task fileCreationTask = WsaCreateNewAppendableFile(filename, includeTimeStamp);
        fileCreationTask.Wait();
#endif
    }

    /// <summary>
    /// Add string content to the end of an open .csv file.
    /// </summary>
    /// <param name="stringToAdd">The line to be appended to the open file.</param>
    public void AppendStringToOpenedFile(string stringToAdd)
    {
#if UNITY_EDITOR
        DesktopAppendStringToOpenedFile(stringToAdd);
#else
        WsaAppendStringToOpenedFile(stringToAdd);
#endif
    }

    /// <summary>
    /// Close the open file.
    /// </summary>
    public void CloseAppendableFile()
    {
#if UNITY_EDITOR
        DesktopCloseOpenedFile();
#else
        //WsaCloseOpenedFile();
#endif
    }

#if UNITY_EDITOR
    /// <summary>
    /// The Unity Editor and Standalone implementation of writing a string to a file, with the filename based on the fileNamePrefix followed by a timestamp.
    /// </summary>
    /// <param name="fileName">The name of the the .csv to be written.</param>
    /// <param name="includeTimeStamp"> Whether or not to append the current time to the filename.</param>
    private void DesktopCreateNewAppendableFile(string fileName, bool includeTimeStamp)
    {
        string fileNameWithPath;
        if (includeTimeStamp)
        {
            string timeStamp = string.Format("{0:_yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
            fileNameWithPath = string.Format("{0}/{1}_{2}.csv", Application.dataPath, fileName, timeStamp);
        }
        else
        {
            fileNameWithPath = string.Format("{0}/{1}.csv", Application.dataPath, fileName);
        }
        appendableStreamWriter = File.CreateText(fileNameWithPath);
    }

    /// <summary>
    /// The Unity Editor and Standalone implementation of adding string content to be exported as part of a data file.
    /// </summary>
    /// <param name="stringToAdd">The line to be appended to the string builder</param>
    private void DesktopAppendStringToOpenedFile(string stringToAdd)
    {
        try
        {
            appendableStreamWriter.Write(stringToAdd);
        }
        catch (NullReferenceException e)
        {
            Debug.Log("File must be created before appending text to it.");
        }
        catch (ObjectDisposedException e)
        {
            Debug.Log("Cannot write to closed files.");
        }
    }

    /// <summary>
    ///  The Unity Editor and Standalone implementation of closing the file 
    /// </summary>
    private void DesktopCloseOpenedFile()
    {
        try
        {
            appendableStreamWriter.Close();
        }
        catch (NullReferenceException e)
        {
            Debug.Log("No file available to close.");
        }
    }

#else

    private async Task WsaCreateNewAppendableFile(string fileName, bool includeTimeStamp)
    {
        string appendableFileName;
        if (includeTimeStamp)
        {
            string timeStamp = string.Format("{0:_yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
            appendableFileName = string.Format("{0}_{1}.csv",fileName, timeStamp);
        }
        else
        {
            appendableFileName = string.Format("{0}.csv",fileName);
        }
        StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(UnityEngine.Windows.Directory.localFolder);
        StorageFile liveStorageFile = await folder.CreateFileAsync(appendableFileName, CreationCollisionOption.ReplaceExisting);
        appendableFileNameWithPath = liveStorageFile.Path;
    }

    /// <summary>
    /// The WSA implementation of adding string content to be exported as part of a data file.
    /// </summary>
    /// <param name="stringToAdd">The line to be appended to the string builder</param>
    private async Task WsaAppendStringToOpenedFile(string str)
    {
        if (appendableFileNameWithPath != null)
        {
            if (File.Exists(appendableFileNameWithPath)) 
            {
                using (StreamWriter sw = File.AppendText(appendableFileNameWithPath)) 
                {
                    sw.Write(str);
                }	
            }
            else        
            {
                Debug.Log("Appendable file must be created before appending text to it.");
            }
        }
        else
        {
            Debug.Log("Appendable file must be created before appending text to it.");
        }
    }

    ///// <summary>
    /////  The WSA implementation of closing the file 
    ///// </summary>
    //private void WsaCloseOpenedFile()
    //{
    //    try
    //    {
    //        appendableStreamWriter.Dispose();
    //    }
    //    catch (NullReferenceException e)
    //    {
    //        Debug.Log("No opened file available to close.");
    //    }
    //}

#endif
}
