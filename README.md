# modifying hmd FoV with Unity & steamVR plugin
 
 Software:
 <b>Unity version<b>: 2021.3.3f1
 <b>steamVR version<b>: 2.8.0
 
 Hardware:
 <b>Head-mounted display<b>: HTC VIVE pro
 <b>Graphic card<b>: 

 Description:
 Using GetProjectionRaw() & SetStereoProjectionMatrix() to modify perspective projection for hmd.
 
 With three states: 0, 1, 2
 state 0: modify both horizontal & vertical field of view(fixed aspect ratio)
 state 1: modify only vertical field of view
 state 2: modify only horizontal field of view
 
 Control config(Keyboard):
 Press Space button: next state.
 Press Up Arrow: vertical field of view angle + 1 degree
 Press Down Arrow: vertical field of view angle - 1 degree
 Press Left Arrow: horizontal field of view angle + 1 degree
 Press Right Arrow: horizontal field of view angle - 1 degree

 Constraints:
 Must set to <b>MultiPass<b> render mode.
 Edge distortion problem occurs with wide field of view.