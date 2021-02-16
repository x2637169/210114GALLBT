using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Mod_VersionChangeObject : MonoBehaviour
{
    public GameObject[] ServerNeedObjects;
    public GameObject[] SingleNeedObjects;

    void Start()
    {
#if Server
        #region Server
        DeActiveSingleObjects();
        ActiveServerObjects();
        #endregion
#else
        #region !Serve
        DeActiveServerObjects();
        ActiveSingleObjects();
        #endregion
#endif
    }

    public void ActiveServerObjects()
    {
        for (int i = 0; i < ServerNeedObjects.Length; i++)
        {
            ServerNeedObjects[i].SetActive(true);
        }
    }

    public void DeActiveServerObjects()
    {
        for (int i = 0; i < ServerNeedObjects.Length; i++)
        {
            ServerNeedObjects[i].SetActive(false);
        }
    }

    public void ActiveSingleObjects()
    {
        for (int i = 0; i < SingleNeedObjects.Length; i++)
        {
            SingleNeedObjects[i].SetActive(true);
        }
    }

    public void DeActiveSingleObjects()
    {
        for (int i = 0; i < SingleNeedObjects.Length; i++)
        {
            SingleNeedObjects[i].SetActive(false);
        }
    }
}
