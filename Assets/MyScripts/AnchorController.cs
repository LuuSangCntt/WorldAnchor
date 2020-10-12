using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;

public class AnchorController : MonoBehaviour
{
    public WorldAnchorManager MyWorldAnchorManager;
    public WorldAnchor MyWorldAnchor;
    public string AnchorId;
    public bool LoadOnStart = true;

    #region Unity Functions
    private void Start()
    {
        if (LoadOnStart == true)
        {
            LoadAnchor();
        }
    }
    #endregion


    #region Public Functions
    public void LoadAnchor()
    {
        try
        {
            MyWorldAnchorManager.AttachAnchor(this.gameObject, AnchorId);
            Debug.Log("Load anchor for gameobject " + gameObject.name);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error when start: " + e.Message + "\r\n");
        }
    }
    public void EnableEditMode()
    {
        try
        {
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                DeleteAnchor();
            }, false);
            Debug.Log("EditMode is enable");
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error when start edit: " + ex.Message + "\r\n");
        }
    }
    public void DisableEditModeAndSave()
    {
        try
        {
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                SaveAnchor();
            }, false);
            Debug.Log("EditMode is disable");
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error on Saving Anchor: " + ex.Message);
        }
    }
    public void SendAchor()
    {

    }
    #endregion

    #region private functions
    private void SaveAnchor()
    {
        MyWorldAnchorManager.AttachAnchor(gameObject, AnchorId);
        Debug.Log("Save anchor " + AnchorId + "\r\n");
    }
    private void DeleteAnchor()
    {
        MyWorldAnchorManager.RemoveAnchor(AnchorId);
        //var a = GetComponent<WorldAnchor>();
        Debug.Log("Delete anchor " + AnchorId + " \r\n");
    }
    #endregion

}
