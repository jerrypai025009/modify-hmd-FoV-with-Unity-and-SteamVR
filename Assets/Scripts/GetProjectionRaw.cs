using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class GetProjectionRaw : MonoBehaviour
{
    public CVRSystem hmd;
    public CVRHeadsetView view;
    float tL, bL, lL, rL;

    private void Awake()
    {
        var err = EVRInitError.None;
        //hmd = OpenVR.Init(ref err, EVRApplicationType.VRApplication_Other);
        hmd = OpenVR.System;
        //Debug.Log(OpenVR.k_pch_SteamVR_IpdOffset_Float);
        if(err != EVRInitError.None)
        {
            Debug.Log("Init Error");
        }

        view = OpenVR.HeadsetView;

    }
    uint w, h;

    // Start is called before the first frame update
    void Start()
    {
        /*
        Debug.Log(view.GetHeadsetViewAspectRatio());
        Debug.Log(view.GetHeadsetViewCropped());
        view.GetHeadsetViewSize(ref w, ref h);
        float width = (float)w;
        float height = (float)h;
        Debug.Log(width);
        Debug.Log(height);
        */
    }

    // Update is called once per frame
    void Update()
    {
    }
}
