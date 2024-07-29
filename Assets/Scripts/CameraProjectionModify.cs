using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Valve.VR;
public class CameraProjectionModify : MonoBehaviour
{
    public CVRSystem system;
    private void Awake()
    {
        //Get cvrsystem component
        var error = EVRInitError.None;
        system = OpenVR.Init(ref error, EVRApplicationType.VRApplication_Scene);
        if (error != EVRInitError.None)
            Debug.Log("Init Error");
    }
    private void OnEnable() {
        RenderPipelineManager.beginFrameRendering += ScenePreCull;
    }

    private void OnDisable() {
        RenderPipelineManager.beginFrameRendering -= ScenePreCull;
        GetComponent<Camera>().ResetStereoProjectionMatrices();
    }

    private void ScenePreCull(ScriptableRenderContext context, Camera[] cam)
    {
        OnPreCull();
    }

    Camera cam;
    //the matrices which is going to be customized
    Matrix4x4 matR, matL;
    
    //current right eye field of view and aspect ratio
    [Range(0.01f, 179.99f)]
    public float fovxR;
    [Range(0.01f, 179.99f)]
    public float fovyR;
    public float aspectR;

    //current left eye field of view and aspect ratio
    [Range(0.01f, 179.99f)]
    public float fovxL;
    [Range(0.01f, 179.99f)]
    public float fovyL;
    public float aspectL;

    //value for building the view frustum
    public float near = 0.01f;
    public float far = 1000f;

    private float topL, bottomL;
    private float leftL, rightL;

    private float topR, bottomR;
    private float leftR, rightR;

    //gain
    public float gain = 1f;

    //optical field of view
    private float fovx;
    private float fovy;

    //state:
    //  0: modify both horizontal and vertical field of view
    //  1: modify only vertical field of view
    //  2: modify only horizontal field of view
    public int state = 0;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();

        //to get default near and far
        near = cam.nearClipPlane;
        far = cam.farClipPlane;

        //to get the default left, right, bottom, top value
        //the function return the tangent value, so must times near value
        system.GetProjectionRaw(EVREye.Eye_Left, ref leftL, ref rightL, ref bottomL, ref topL);    
        system.GetProjectionRaw(EVREye.Eye_Right, ref leftR, ref rightR, ref bottomR, ref topR);

        leftL *= near;
        rightL *= near;
        topL *= near;
        bottomL *= near;

        leftR *= near;
        rightR *= near;
        topR *= near;
        bottomR *= near;

        //to get optical field of view
        fovx = (Mathf.Atan(rightR / near) - Mathf.Atan(leftR / near)) * Mathf.Rad2Deg;
        fovy = (Mathf.Atan(topR / near) - Mathf.Atan(bottomR / near)) * Mathf.Rad2Deg;

        state = 0;
    }

    //the function to get frustum with given fov and edge values
    void getFrustumValue(float angle, float x, float y)
    {
        y = Mathf.Abs(y);
        float a = 1;
        float b = -1 / Mathf.Tan(angle * Mathf.Deg2Rad) * (x + y);
        float c = -1 * x * y;
        float tl = topL;
        float bl = bottomL;
        float ll = leftL;
        float rl = rightL;
        float tr = topR;
        float br = bottomR;
        float lr = leftL;
        float rr = rightL;

        float n = (-1 * b + Mathf.Pow(b * b - 4 * a * c, 0.5f)) / (2 * a);
        if (state == 0 || state == 1)
        {
            topL *= (near / n);
            bottomL *= (near / n);
            topR *= (near / n);
            bottomR *= (near / n);
        }
        if (state == 0 || state == 2)
        {
            leftL *= (near / n);
            rightL *= (near / n);
            leftR *= (near / n);
            rightR *= (near / n);
        }

        float fovxR = (Mathf.Atan(rightR / near) - Mathf.Atan(leftR / near)) * Mathf.Rad2Deg;
        float fovyR = (Mathf.Atan(topR / near) - Mathf.Atan(bottomR / near)) * Mathf.Rad2Deg;

        float fovxL = (Mathf.Atan(rightL / near) - Mathf.Atan(leftL / near)) * Mathf.Rad2Deg;
        float fovyL = (Mathf.Atan(topL / near) - Mathf.Atan(bottomL / near)) * Mathf.Rad2Deg;

        if(fovxR > 179.99f || fovyR > 179.99f || fovxL > 179.99f || fovyL > 179.99f)
        {
            topL = tl;
            bottomL = bl;
            leftL = ll;
            rightL = rl;

            topR = tr;
            bottomR = br;
            leftR = lr;
            rightR = rr;
        }
        /*
        float a = Mathf.Tan(fovyR * Mathf.Deg2Rad);
        float b = -1 * (topR + Mathf.Abs(bottomR));
        float c = -1 * a * topR * Mathf.Abs(bottomR);

        float newNear1 = (-1 * b + Mathf.Pow(b * b - 4 * a * c, 0.5f)) / (2*a);
        float newNear2 = (-1 * b - Mathf.Pow(b * b - 4 * a * c, 0.5f)) / (2*a);
        near = (newNear1 > 0) ? newNear1 : newNear2;*/
    }

    void Update()
    {
        //to prevent the near and far from being invalid
        if (near <= 0f) near = 0.001f;
        if (far <= near) far = near + 0.001f;

        //get current aspect ratio
        float heightR = topR - bottomR;
        float widthR = rightR - leftR;
        aspectR = widthR / heightR;

        float heightL = topL - bottomL;
        float widthL = rightL - leftL;
        aspectL = widthL / heightL;

        //get current field of view for both eyes
        fovxR = (Mathf.Atan(rightR / near) - Mathf.Atan(leftR / near)) * Mathf.Rad2Deg;
        fovyR = (Mathf.Atan(topR / near) - Mathf.Atan(bottomR / near)) * Mathf.Rad2Deg;
        
        fovxL = (Mathf.Atan(rightL / near) - Mathf.Atan(leftL / near)) * Mathf.Rad2Deg;
        fovyL = (Mathf.Atan(topL / near) - Mathf.Atan(bottomL / near)) * Mathf.Rad2Deg;
        
        //Modify the vertical field of view by degrees
        if (Input.GetKey(KeyCode.UpArrow) && (state == 0 || state == 1))
        {
            fovyR += 1f;
            fovyR = Mathf.Clamp(fovyR, 0.01f, 179.99f);

            getFrustumValue(fovyR, topR, bottomR);
        }
        if (Input.GetKey(KeyCode.DownArrow) && (state == 0 || state == 1))
        {
            fovyR -= 1f;
            fovyR = Mathf.Clamp(fovyR, 0.01f, 179.99f);

            getFrustumValue(fovyR, topR, bottomR);
        }

        //Modify the horizontal field of view by degrees
        if (Input.GetKey(KeyCode.RightArrow) && (state == 0 || state == 2))
        {
            fovxR += 1f;
            fovxR = Mathf.Clamp(fovxR, 0.01f, 179.99f);

            getFrustumValue(fovxR, rightR, leftR);
        }
        if (Input.GetKey(KeyCode.LeftArrow) && (state == 0 || state == 2))
        {
            fovxR -= 1f;
            fovxR = Mathf.Clamp(fovxR, 0.01f, 179.99f);

            getFrustumValue(fovxR, rightR, leftR);
        }
        /*
        //Modify the vertical field of view by gains
        if (Input.GetKeyDown(KeyCode.W))
        {
            gain += 0.05f;
            fovyR = fovy * gain;
            getFrustumValue(fovyR, topR, bottomR);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            gain -= 0.05f;
            fovyR = fovy * gain; 
            getFrustumValue(fovyR, topR, bottomR);
        }*/

        //change modification state
        if(Input.GetKeyDown(KeyCode.Space))
        {
            state = (state + 1) % 3;
        }
    }

    private void OnPreCull() {
        //customize perspective projection matrix by the frustum value
        matR = Matrix4x4.Frustum(leftR, rightR, bottomR, topR, near, far);
        matL = Matrix4x4.Frustum(leftL, rightL, bottomL, topL, near, far);
        
        //set my customized projection matrix into the cam projection matrix
        cam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, matR);
        cam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, matL);
    }
}
