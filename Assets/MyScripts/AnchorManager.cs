using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
using UnityEngine.XR.WSA.Sharing;

[Serializable]
public class AnchorManager : MonoBehaviour
{
    public WorldAnchor anchor;
    public string Name;
    public WorldAnchorManager wam;
    WorldAnchorStore anchorStore;
    //public WebSocketClient client;
    byte[] importedData = new byte[1];
    int retryCount = 3;
    Queue<IEnumerator> actions;

    // Start is called before the first frame update
    void Start()
    {
        actions = new Queue<IEnumerator>();
        try
        {
            wam.AttachAnchor(gameObject, Name);
            //client.OnReceive += client_OnReceive;
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error when start: " + ex.Message + "\r\n");
        }

        //WorldAnchorStore.GetAsync(AnchorLoaded);
    }

    private void Update()
    {
        if (actions.Count > 0)
        {
            var func = actions.Dequeue();
            StartCoroutine(func);
        }
    }

    private void client_OnReceive(object sender, byte[] e)
    {
        importedData = new byte[e.Length];
        for (int i = 0; i < e.Length; i++)
        {
            importedData[i] = e[i];
        }

        WorldAnchorTransferBatch.ImportAsync(importedData, OnImportComplete);
    }

    private void OnImportComplete(SerializationCompletionReason completionReason, WorldAnchorTransferBatch deserializedTransferBatch)
    {
        if (completionReason != SerializationCompletionReason.Succeeded)
        {
            Debug.Log("Failed to import: " + completionReason.ToString());
            if (retryCount > 0)
            {
                retryCount--;
                WorldAnchorTransferBatch.ImportAsync(importedData, OnImportComplete);
            }
            return;
        }

        anchor = deserializedTransferBatch.LockObject(Name, gameObject);
    }

    //public IEnumerator TransferAnchor()
    //{
    //    try
    //    {
    //        DebugText.text += "start transfer anchor on " + Name + ":\r\n";

    //        anchor = GetComponent<WorldAnchor>();
    //        var data = ByteUtils.AnchorToByteArray(anchor);
    //        actions.Enqueue(client.SendAnchor(data));
    //        DebugText.text += "sending " + data.Length + " bytes of data...\r\n";
    //        //WorldAnchorTransferBatch batch = new WorldAnchorTransferBatch();
    //        //batch.AddWorldAnchor(Name, anchor);
    //        //WorldAnchorTransferBatch.ExportAsync(batch, OnExportDataAvailable, OnExportComplete);
    //    }
    //    catch (Exception ex)
    //    {
    //        DebugText.text += "error when transfer anchor on " + Name + ":\r\n";
    //        DebugText.text += ex.Message + "\r\n";
    //        DebugText.text += ex.StackTrace + "\r\n";
    //    }

    //    yield return null;
    //}

    private void OnExportDataAvailable(byte[] data)
    {
        Debug.Log("sending " + data.Length + " bytes of data...\r\n");
        //UnityEngine.WSA.Application.InvokeOnUIThread(() =>
        //{
        //    DebugText.text += "sending " + data.Length + " bytes of data...\r\n";
        //    actions.Enqueue(client.SendAnchor(data));
        //}, false);
    }

    private void OnExportComplete(SerializationCompletionReason completionReason)
    {
        Debug.Log("Export complete...\r\n");
        if (completionReason != SerializationCompletionReason.Succeeded)
        {
            //SendExportFailedToClient();
        }
        else
        {
            //SendExportSucceededToClient();
        }
    }

    public void DeleteAnchor()
    {
        UnityEngine.WSA.Application.InvokeOnUIThread(() =>
        {
            Debug.Log("start delete anchor on " + Name + "...\r\n");
            try
            {
                wam.RemoveAnchor(Name);
                //var a = GetComponent<WorldAnchor>();
                Debug.Log("after delete anchor on " + Name + ": \r\n");
                Debug.Log("World Anchor = " + gameObject.GetComponent<WorldAnchor>());
            }
            catch (System.Exception ex)
            {
                Debug.Log("Error when delete anchor: " + ex.Message + "\r\n");
                Debug.Log(ex.StackTrace + "\r\n");
            }
        }, false);
    }

    public IEnumerator SaveAnchor()
    {
        try
        {
            Debug.Log("start save anchor on " + Name + "...\r\n");
            wam.AttachAnchor(gameObject, Name);
            Debug.Log("after save anchor on " + Name + "\r\n");
            Debug.Log("World Anchor = " + gameObject.GetComponent<WorldAnchor>());
        }
        catch (System.Exception ex)
        {
            Debug.Log("error when save anchor on " + Name + ":\r\n");
            Debug.Log(ex.Message + "\r\n");
            Debug.Log(ex.StackTrace + "\r\n");
        }
        yield return null;
    }

    public void BeginSaveAnchor()
    {
        UnityEngine.WSA.Application.InvokeOnUIThread(() =>
        {
            actions.Enqueue(SaveAnchor());
        }, false);
    }

    //public void BeginTransferAnchor()
    //{
    //    UnityEngine.WSA.Application.InvokeOnUIThread(() =>
    //    {
    //        actions.Enqueue(TransferAnchor());
    //    }, false);
    //}

    //public void DeleteAnchor()
    //{
    //    try
    //    {
    //        DebugText.text += "Start delete anchor on: " + Name + "\r\n";
    //        anchorStore.Delete(Name);
    //        wam.RemoveAllAnchors();
    //    }
    //    catch (System.Exception ex)
    //    {
    //        DebugText.text += "Error when delete anchor: " + ex.Message + "\r\n";
    //    }
    //}

    //private void AnchorLoaded(WorldAnchorStore anchorS)
    //{
    //    anchorStore = anchorS;
    //    LoadAnchor();
    //}

    //public void SaveAnchor()
    //{
    //    try
    //    {
    //        var res = anchorStore.Save(Name, anchor);
    //        DebugText.text += "save anchor on " + Name + ": " + res + "\r\n";
    //    }
    //    catch (System.Exception ex)
    //    {
    //        DebugText.text += "Error when save anchor: " + ex.Message + "\r\n";
    //    }
    //}

    //public void LoadAnchor()
    //{
    //    try
    //    {
    //        anchor = anchorStore.Load(Name, gameObject);
    //        if (anchor == null)
    //        {
    //            DebugText.text += "add new to " + Name + "\r\n";
    //            //gameObject.AddComponent<WorldAnchor>();
    //            //anchor = GetComponent<WorldAnchor>();
    //            anchorStore.Delete(Name);
    //            wam.AttachAnchor(gameObject, Name);
    //            SaveAnchor();
    //        }
    //    }
    //    catch (System.Exception ex)
    //    {
    //        DebugText.text += "Error when Load anchor: " + ex.Message + "\r\n";
    //    }
    //}
}
