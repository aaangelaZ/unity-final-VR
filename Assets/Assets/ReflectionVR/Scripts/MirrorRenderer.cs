using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;
using System;
using UnityEngine.Rendering.Universal;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Fragilem17.MirrorsAndPortals
{
    [ExecuteInEditMode]
    public class MirrorRenderer : MonoBehaviour
    {
        public static List<MirrorRenderer> mirrorRendererInstances;

        [Tooltip("The source material, disable and re-enable this component if you make changes to the material")]
        public List<MirrorSurface> mirrorSurfaces;

        [Tooltip("How many times can this surface reflect back onto the recursiveSurface.\nFrom 1 till the texturememory runs out.")]
        [MinAttribute(1)]
        public int recursions = 1;

        [Space(10)] // 10 pixels of spacing here.

        [Header("Other")]

        [Tooltip("In VR this should probably always be 0")]
        [MinAttribute(0)]
        public int framesNeededToUpdate = 0;

        [Tooltip("The layer mask of the reflection camera")]
        public LayerMask RenderTheseLayers = -1;



        [Space(10)]
        [Header("Quality Settings")]
        public Vector2 textureSize = Vector2.one * 128f;

        public bool useScreenScaleFactor = true;

        [Range(0.01f, 1f)]
        public float screenScaleFactor = 0.5f;

        public AA antiAliasing = AA.Low;

        public bool disablePixelLights = true;


        [Space(10)]
        [Header("Events")]
        public UnityEvent onStartRendering;
        public UnityEvent onFinishedRendering;


        private List<PooledTexture> _pooledTextures = new List<PooledTexture>();
        
        private static Dictionary<Camera, Camera> _reflectionCameras = new Dictionary<Camera, Camera>();

        private static InputDevice _centerEye;
        private static float _IPD;
        private static Vector3 _leftEyePosition;
        private static Vector3 _rightEyePosition;

        private static Camera _reflectionCamera;

        private int _frameCounter = 0;

        private AA _oldAntiAliasing = AA.Low;
        private int _oldTextureSize = 0;
        private bool _oldUseScreenScaleFactor = true;
        private float _oldScreenScaleFactor = 0.5f;

        private List<CameraMatrices> cameraMatricesInOrder = new List<CameraMatrices>();

        private static MirrorRenderer _master;
        private UniversalAdditionalCameraData _uac;


        [Tooltip("When checked, the reflection will stop rendering but the materials will still update their position and blending")]
        public bool disableRenderingWhileStillUpdatingMaterials = false;

        [Space(10)]
        [Header("Debugging")]

        public bool showDebuggingInfo = false;
#if UNITY_EDITOR_OSX
        [Tooltip("When checked, in Unity for MacOSX, the console will be spammed with a message each time a mirror renders, this is a workarround to a Unity Bug that instantly crashes the editor. (disable at your own peril)")]
        public bool enableMacOSXTemporaryLogsToAvoidCrashingTheEditor = true;
#endif
        //[Tooltip("If you have multiple sceneViews in the editor open, you can select where the mirrors should render, and where you want to see debug info")]
        //public int SceneViewIndex = 0;

        public enum AA
        {
            None = 1,
            Low = 2,
            Medium = 4,
            High = 8
        }

        private void OnEnable()
        {
            if (mirrorRendererInstances == null)
			{
                mirrorRendererInstances = new List<MirrorRenderer>();
			}
            mirrorRendererInstances.Add(this);

            RenderPipeline.beginCameraRendering += UpdateCamera;
        }


        private void UpdateCamera(UnityEngine.Rendering.ScriptableRenderContext src, Camera renderCamera)
        {
            bool renderNeeded = false;
#if UNITY_EDITOR
            // only render the first sceneView (so we can see debug info in a second sceneView)
            //int index = Mathf.Clamp(SceneViewIndex, 0, SceneView.sceneViews.Count - 1);
            //SceneView view = SceneView.sceneViews[index] as SceneView;
            renderNeeded = renderCamera.tag == "MainCamera" || (renderCamera.cameraType == CameraType.SceneView && renderCamera.name.IndexOf("Preview Camera") == -1); //  && view.camera == renderCamera
#else
            renderNeeded = renderCamera.tag == "MainCamera";
#endif

            if (!renderNeeded)
            {
                if (_master == this) { _master = null; }
                return;
            }

            if (!enabled || !renderCamera)
            {
                if (_master == this) { _master = null; }
                return;
            }

            if (mirrorSurfaces == null || mirrorSurfaces.Count == 0)
            {
                if (_master == this) { _master = null; }
                return;
            }

            if (_frameCounter > 0)
            {
                _frameCounter--;
                return;
            }
            _frameCounter = framesNeededToUpdate;



            // check the distance
            renderNeeded = false;
            for (int i = 0; i < mirrorSurfaces.Count; i++)
            {
                MirrorSurface ms = mirrorSurfaces[i];
				if (ms)
				{
                    renderNeeded = mirrorSurfaces[i].VisibleFromCamera(renderCamera, false) || renderNeeded;
				}
            }
			if (!renderNeeded) {
                //Debug.Log("stop rendering exit " + name);
                if (_master == this) { _master = null; }
                return;
            }


			if (disableRenderingWhileStillUpdatingMaterials && cameraMatricesInOrder != null)
			{
                for (int i = 0; i < mirrorSurfaces.Count; i++)
                {
                    MirrorSurface ms = mirrorSurfaces[i];

                    float myDistance = Vector3.Distance(renderCamera.transform.position, ms.ClosestPoint(renderCamera.transform.position));
                    ms.UpdateMaterial(Camera.StereoscopicEye.Left, null, this, 1, myDistance);
                }
                if (_master == this) { _master = null; }
                return;
			}

            if (!_master) { _master = this; }

            CreateMirrorCameras(renderCamera, out _reflectionCamera);

            _reflectionCamera.CopyFrom(renderCamera);
            _reflectionCamera.cullingMask = RenderTheseLayers.value;

            _uac = renderCamera.GetComponent<UniversalAdditionalCameraData>();

            if (XRSettings.enabled && (_master == this) && _uac.allowXRRendering)
            {
                // get the IPD
                _centerEye = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
                _centerEye.TryGetFeatureValue(CommonUsages.leftEyePosition, out _leftEyePosition);
                _centerEye.TryGetFeatureValue(CommonUsages.rightEyePosition, out _rightEyePosition);

                _IPD = Vector3.Distance(_leftEyePosition, _rightEyePosition) * renderCamera.transform.lossyScale.x;
            }


            if (XRSettings.enabled && _uac.allowXRRendering)
            {
                Vector3 originalPos = renderCamera.transform.position;
                renderCamera.transform.position -= (renderCamera.transform.right * _IPD / 2);
                _reflectionCamera.transform.SetPositionAndRotation(renderCamera.transform.position, renderCamera.transform.rotation);
                _reflectionCamera.worldToCameraMatrix = renderCamera.worldToCameraMatrix;
                renderCamera.transform.position = originalPos;

                _reflectionCamera.projectionMatrix = renderCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
            }
            else
            {
                _reflectionCamera.transform.SetPositionAndRotation(renderCamera.transform.position, renderCamera.transform.rotation);
                _reflectionCamera.worldToCameraMatrix = renderCamera.worldToCameraMatrix;
                _reflectionCamera.projectionMatrix = renderCamera.projectionMatrix;
            }


            cameraMatricesInOrder.Clear();

            onStartRendering.Invoke();

            //Debug.Log("START SEARCH !! Mirror Cameras on Root: " + name + " : " + renderCamera.name + " : " + mirrorSurfaces.Count);
            RecusiveFindMirrorsInOrder(renderCamera, cameraMatricesInOrder, 1, Camera.StereoscopicEye.Left);
            
            RenderMirrorCamera(src, _reflectionCamera, cameraMatricesInOrder, Camera.StereoscopicEye.Left);

            if (XRSettings.enabled && _uac.allowXRRendering)
            {
                Vector3 originalPos = renderCamera.transform.position;
                renderCamera.transform.position += (renderCamera.transform.right * _IPD / 2f);
                _reflectionCamera.transform.SetPositionAndRotation(renderCamera.transform.position, renderCamera.transform.rotation);
                _reflectionCamera.worldToCameraMatrix = renderCamera.worldToCameraMatrix;
                renderCamera.transform.position = originalPos;

                _reflectionCamera.projectionMatrix = renderCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);

                cameraMatricesInOrder.Clear();
                RecusiveFindMirrorsInOrder(renderCamera, cameraMatricesInOrder, 1, Camera.StereoscopicEye.Right);
                
                RenderMirrorCamera(src, _reflectionCamera, cameraMatricesInOrder, Camera.StereoscopicEye.Right);
            }

            onFinishedRendering.Invoke();


            foreach (MirrorSurface ms in mirrorSurfaces)
			{
				if (ms != null)
				{
                    ms.TurnOffForceEye();
				}
			}
        }

		void OnDrawGizmosSelected()
        {
            if (!showDebuggingInfo)
			{
                return;
			}

            // Draw a yellow sphere at the transform's position
            int index = 0;
            foreach (CameraMatrices matrices in cameraMatricesInOrder)
            {
                index++;
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(matrices.camPos, 0.16f * matrices.depth);
                //Gizmos.color = Color.red;
                //Gizmos.DrawWireSphere(matrices.camPos, 0.18f * matrices.distance);
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(matrices.camPos, 0.20f * index);
            }
        }

        private void RecusiveFindMirrorsInOrder(Camera renderCamera, List<CameraMatrices> cameraMatricesInOrder, int depth, Camera.StereoscopicEye eye, 
            MirrorSurface parentSurface = null, 
            MirrorSurface parentsParentSurface = null,
            MirrorSurface parentsParentsParentSurface = null,
            MirrorSurface parentsParentsParentsParentSurface = null)
        {
            // look one deeper to know which deepest mirrors to turn dark
            if (depth > recursions+1)
            {
                return;
            }

            Vector3 eyePosition = _reflectionCamera.transform.position;
            //Quaternion eyeRotation = _reflectionCamera.transform.rotation;
            Matrix4x4 worldToCameraMatrix = _reflectionCamera.worldToCameraMatrix;
            Matrix4x4 projectionMatrix = _reflectionCamera.projectionMatrix;
            if (XRSettings.enabled && _uac.allowXRRendering)
            {
                projectionMatrix = _reflectionCamera.GetStereoProjectionMatrix(eye);
            }
           
            Vector3 planeIntersection = Vector3.zero;
            //Debug.DrawLine(_reflectionCamera.transform.position, _reflectionCamera.transform.position + _reflectionCamera.transform.forward, Color.red, 0, false);

            Vector3 mirrorPos;
            Vector3 mirrorNormal;
            float myDistance;

            foreach (MirrorSurface reflectionMs in mirrorSurfaces)
            {
                //Debug.Log("Should I Add? " + reflectionMs.name);
                if (reflectionMs != null && reflectionMs != parentSurface && reflectionMs.VisibleFromCamera(_reflectionCamera, true)
                    ) // && (depth != 2 || (depth == 2 && reflectionMs.VisibleInBoundsParent(parentSurface, _reflectionCamera, renderCamera)))
                {
                                        
                    mirrorPos = reflectionMs.transform.position;
                    mirrorNormal = reflectionMs.transform.forward * -1;

                    if (showDebuggingInfo)
                    {
                        Plane m_Plane = new Plane(mirrorNormal, mirrorPos);
                        float enter = 0.0f;
                        Ray ray = _reflectionCamera.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f));
                        if (m_Plane.Raycast(ray, out enter))
                        {
                            planeIntersection = ray.GetPoint(enter);
                            Debug.DrawLine(eyePosition, planeIntersection, Color.red, 0.15f, false); 
                        }
					}

                    // todo: calculate closest point in the bounds of the mirror renderer surface instead of the mirror center
                    myDistance = Vector3.Distance(eyePosition, reflectionMs.ClosestPoint(eyePosition));

                    // if we're using useRecursiveDarkening, we might be rendering deeper then the maxDistance allows
                    if (myDistance <= reflectionMs.maxRenderingDistance || reflectionMs.useRecursiveDarkening)
					{
                        // Render reflection
                        // Reflect camera around reflection plane
                        float d = (-Vector3.Dot(mirrorNormal, mirrorPos) - reflectionMs.clippingPlaneOffset);
                        Vector4 reflectionPlane = new Vector4(mirrorNormal.x, mirrorNormal.y, mirrorNormal.z, d);

                        Matrix4x4 reflection = Matrix4x4.zero;
                        CalculateReflectionMatrix(ref reflection, reflectionPlane);

                        // no need to update the transforms, the matrix contains where to render, but it helps with debugging
                        Vector3 newEyePos = reflection.MultiplyPoint(eyePosition);
                        _reflectionCamera.transform.position = newEyePos;
                        //_reflectionCamera.transform.LookAt(planeIntersection);
                        

                        Matrix4x4 newWorldToCameraMatrix = _reflectionCamera.worldToCameraMatrix * reflection;
                        _reflectionCamera.worldToCameraMatrix = newWorldToCameraMatrix;

                        //offset a bit so we can see it next to the red one
                        if (showDebuggingInfo) Debug.DrawLine(_reflectionCamera.transform.position + (Vector3.one * 0.1f), planeIntersection + (Vector3.one * 0.1f), Color.cyan, 0.15f, false);

                        Vector4 clipPlane = CameraSpacePlane(newWorldToCameraMatrix, mirrorPos, mirrorNormal, 1.0f, reflectionMs.clippingPlaneOffset);

                        Matrix4x4 newProjectionMatrix;
                        if (XRSettings.enabled && _uac.allowXRRendering)
                        {
                            newProjectionMatrix = _reflectionCamera.GetStereoProjectionMatrix(eye);
                        }
                        else
                        {
                            newProjectionMatrix = _reflectionCamera.projectionMatrix;
                        }

                        MakeProjectionMatrixOblique(ref newProjectionMatrix, clipPlane);
                        _reflectionCamera.projectionMatrix = newProjectionMatrix;
                        Matrix4x4 newCullingMatrix = newProjectionMatrix * newWorldToCameraMatrix;

                        //Debug.Log("Search Mirror Cameras seen by " + reflectionMs.name + " : " + depth);
                        RecusiveFindMirrorsInOrder(renderCamera, cameraMatricesInOrder, depth + 1, eye, reflectionMs, parentSurface, parentsParentSurface, parentsParentsParentSurface);

                        // we might have moved the reflection camera in a previous iteration
                        // reset it for the next 
                        _reflectionCamera.transform.position = eyePosition;
                        //_reflectionCamera.transform.rotation = eyeRotation;
                        _reflectionCamera.worldToCameraMatrix = worldToCameraMatrix;
                        _reflectionCamera.projectionMatrix = projectionMatrix;


                        //Debug.Log("Found Mirror: " + reflectionMs.name + " depth: " + depth + " parent: " + parentSurface?.name);
                        cameraMatricesInOrder.Add(new CameraMatrices(newProjectionMatrix, newWorldToCameraMatrix, newCullingMatrix, reflectionMs, depth % 2 != 0, newEyePos, depth, myDistance, parentSurface, parentsParentSurface, parentsParentsParentSurface, parentsParentsParentsParentSurface));
					}
					else
					{
                        //Debug.Log("to far: " + reflectionMs.name + " : " + depth + " : " + myDistance + " : " + reflectionMs.maxRenderingDistance);
                        cameraMatricesInOrder.Add(new CameraMatrices(Matrix4x4.identity, Matrix4x4.identity, Matrix4x4.identity, reflectionMs, true, Vector3.zero, recursions+1, myDistance, parentSurface, parentsParentSurface, parentsParentsParentSurface, parentsParentsParentsParentSurface));
                    }
				}
            }

            //Debug.Log("stop search: " + depth + " : " + parentSurface?.name);
        }

        private void RenderMirrorCamera(UnityEngine.Rendering.ScriptableRenderContext src, Camera reflectionCamera, List<CameraMatrices> cameraMatricesInOrder, Camera.StereoscopicEye eye)
        {
            //Debug.Log("RenderMirrorCamera");

            // Optionally disable pixel lights for reflection
            int oldPixelLightCount = QualitySettings.pixelLightCount;
            if (disablePixelLights)
            {
                QualitySettings.pixelLightCount = 0;
            }

            PooledTexture _ptex = null;
            // position and render the camera
            CameraMatrices matrices = null;


            for (int i = 0; i < cameraMatricesInOrder.Count; i++)
			{
                matrices = cameraMatricesInOrder[i];
  
                if (matrices.depth >= recursions+1)
				{
                    //Debug.Log(" render surface lite: " + matrices.mirrorSurface.name + " de: " + matrices.depth + " pa: " + matrices.parentMirrorSurface?.name + " di: " + matrices.distance);
                    // make it completely blended, no need to render these
                    matrices.mirrorSurface.UpdateMaterial(eye, null, this, matrices.depth, Mathf.Infinity);


                }
                else
				{
                    GetFreeTexture(out _ptex, eye);
                    _ptex.matrices = matrices;

                    //Debug.Log(" depth: " + matrices.depth + " render op: " + matrices.mirrorSurface.name + " VOOR parent: " + matrices.parentMirrorSurface?.name + " using tex: " + _ptex.texture.name + " parentsParent: "+ matrices.parentsParentMirrorSurface);

                    _ptex.liteLock = true;

					if (matrices.parentMirrorSurface == null)
					{
                        _pooledTextures.ForEach(pTex => {
                            pTex.liteLock = false;
                        });
                        _ptex.fullLock = true;
                    }

                    matrices.mirrorSurface.UpdateMaterial(eye, _ptex.texture, this, matrices.depth, matrices.distance);
                    reflectionCamera.targetTexture = _ptex.texture;
                    
                    reflectionCamera.worldToCameraMatrix = matrices.worldToCameraMatrix;
                    reflectionCamera.projectionMatrix = matrices.projectionMatrix;

                    if (matrices.even)
                    {
                        GL.invertCulling = true; 
                    }

                    //Debug.Log(" render surface heav: " + matrices.mirrorSurface.name + " de : " + matrices.depth + " pa: " + matrices.parentMirrorSurface?.name + " di: " + matrices.distance + " tex: " + _ptex.texture.name);
                    reflectionCamera.useOcclusionCulling = false;
                    reflectionCamera.cullingMatrix = matrices.cullingMatrix;

#if UNITY_EDITOR_OSX
                    if(enableMacOSXTemporaryLogsToAvoidCrashingTheEditor){
                        Debug.Log(" a bug in Unity for MacOSX causes the editor to crash if this message is not here. Terribly sorry about this");
                    }
#endif

                    UniversalRenderPipeline.RenderSingleCamera(src, reflectionCamera);

                    if (matrices.even)
                    {
                        GL.invertCulling = false;
                    }

                    // reset the material to the one with the lowest depth
                    List<CameraMatrices> li = cameraMatricesInOrder.FindAll(x => x.depth == matrices.depth 
                        && x.depth == matrices.depth 
                        && x.parentMirrorSurface == matrices.parentMirrorSurface 
                        && x.parentsParentMirrorSurface == matrices.parentsParentMirrorSurface 
                        && x.parentsParentsParentMirrorSurface == matrices.parentsParentsParentMirrorSurface
                        && x.parentsParentsParentsParentMirrorSurface == matrices.parentsParentsParentsParentMirrorSurface);
                    //Debug.Log("how many?" + li.Count);

                    if (li.Count > 0)
                    {
                        foreach (CameraMatrices cm in li)
                        {
                            if (cm != matrices)
                            {
                                PooledTexture p = _pooledTextures.Find(ptex => ptex.matrices.mirrorSurface == cm.mirrorSurface 
                                    && ptex.matrices.parentMirrorSurface == cm.parentMirrorSurface 
                                    && ptex.matrices.parentsParentMirrorSurface == cm.parentsParentMirrorSurface 
                                    && ptex.matrices.parentsParentsParentMirrorSurface == cm.parentsParentsParentMirrorSurface
                                    && ptex.matrices.parentsParentsParentsParentMirrorSurface == cm.parentsParentsParentsParentMirrorSurface
                                    && ptex.matrices.depth == cm.depth && ptex.eye == eye);
                                if (p != null)
                                {
                                    cm.mirrorSurface.UpdateMaterial(eye, p.texture, this, cm.depth, cm.distance);
								}
                            }
                        }
                    }
			
                    
                    // turn on occlusionCulling even though there is an issue with the cullingMatrix for mirrors
                    // the RecusiveFindMirrorsInOrder will use VisibleFromCamera and that can early exit 
                    reflectionCamera.useOcclusionCulling = true;
                }
            }
           

            // break the textureLocks
            _pooledTextures.ForEach(pTex => {
                pTex.liteLock = false;
                pTex.fullLock = false;
            });
            
            // Restore pixel light count
            if (disablePixelLights)
            { 
                QualitySettings.pixelLightCount = oldPixelLightCount;
            }
        }

        private void GetFreeTexture(out PooledTexture textureOut, Camera.StereoscopicEye eye = Camera.StereoscopicEye.Left) 
        {
            PooledTexture tex = _pooledTextures.Find(tex => !tex.fullLock && !tex.liteLock && tex.eye == eye);
            if (tex == null)
			{
                tex = new PooledTexture();
                tex.eye = eye;
                _pooledTextures.Add(tex);

                // create the texture
                //Debug.Log("creating new pooledTexture: " + _pooledTextures.Count);

                if (useScreenScaleFactor && screenScaleFactor > 0)
                {
                    float scale = screenScaleFactor; // * (1f / depth);
                    textureSize = new Vector2(Screen.width * scale, Screen.height * scale);
                }

                RenderTextureDescriptor desc = new RenderTextureDescriptor((int)textureSize.x, (int)textureSize.y, RenderTextureFormat.ARGB32, 1);
                desc.useMipMap = false;
                //desc.autoGenerateMips = true;

                desc.msaaSamples = (int)antiAliasing;

                tex.texture = RenderTexture.GetTemporary(desc);
                tex.texture.wrapMode = TextureWrapMode.Mirror;
                tex.texture.name = "_Tex" + gameObject.name + "_" + _pooledTextures.Count;
                tex.texture.hideFlags = HideFlags.DontSave;
                //tex.texture.stencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.A2B10G10R10_UNormPack32;
                //tex.texture.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.A2B10G10R10_UNormPack32;
            }

            textureOut = tex;
        }

		private void Update()
		{
            if (_oldTextureSize != ((int)textureSize.x + (int)textureSize.y)
                || _oldScreenScaleFactor != screenScaleFactor
                || _oldAntiAliasing != antiAliasing
                || _oldUseScreenScaleFactor != useScreenScaleFactor)
            {
                _oldUseScreenScaleFactor = useScreenScaleFactor;
                _oldAntiAliasing = antiAliasing;
                _oldScreenScaleFactor = screenScaleFactor;
                _oldTextureSize = ((int)textureSize.x + (int)textureSize.y);

                foreach (PooledTexture tex in _pooledTextures)
                {
                    DestroyImmediate(((RenderTexture)tex.texture));
                }
                _pooledTextures.Clear();

            }

			if (recursions > 8)
			{
                recursions = 8;

            }
        }

        private void CreateMirrorCameras(Camera renderCamera, out Camera reflectionCamera)
        {
            reflectionCamera = null;

            // Camera for reflection
            Camera reflectionCam;
            _reflectionCameras.TryGetValue(renderCamera, out reflectionCam);

            if (reflectionCam == null)
            {
                //Debug.Log("new reflection camera for " + renderCamera.name);
                GameObject go = new GameObject("Reflection Camera for " + renderCamera.name, typeof(Camera), typeof(Skybox));
                reflectionCamera = go.GetComponent<Camera>();
                reflectionCamera.useOcclusionCulling = true;
                reflectionCamera.enabled = false;
                reflectionCamera.transform.position = transform.position;
                reflectionCamera.transform.rotation = transform.rotation;
                reflectionCamera.gameObject.AddComponent<FlareLayer>();

                //reflectionCamera.clearFlags = CameraClearFlags.Nothing;
                //reflectionCamera.depthTextureMode = DepthTextureMode.None;

                _uac = renderCamera.GetComponent<UniversalAdditionalCameraData>();
                if (_uac) { 
                    UniversalAdditionalCameraData reflectionCameraUac = reflectionCamera.gameObject.AddComponent<UniversalAdditionalCameraData>();
                    reflectionCameraUac.allowXRRendering = _uac.allowXRRendering;
                }

                go.hideFlags = HideFlags.DontSave;

				if (_reflectionCameras.ContainsKey(renderCamera))
				{
                    _reflectionCameras[renderCamera] = reflectionCamera;
				}
				else
				{
                    _reflectionCameras.Add(renderCamera, reflectionCamera);
				}
            }
            else
            {
                reflectionCamera = reflectionCam;
            }
        }


        private void OnDestroy()
        {
            OnDisable();
            mirrorRendererInstances.Remove(this);
        }

        // Cleanup all the objects we possibly have created
        void OnDisable()
        {
            //Debug.Log("OnDisable");
            RenderPipeline.beginCameraRendering -= UpdateCamera;

            if (_master == this)
            {
                _master = null;
            }

            if (!Application.isPlaying)
			{
                foreach (var pTex in _pooledTextures)
                {
                    DestroyImmediate(((RenderTexture)pTex.texture));
                }
                _pooledTextures.Clear();

                foreach (var kvp in _reflectionCameras)
                {
                    DestroyImmediate(((Camera)kvp.Value).gameObject);
                }
                _reflectionCameras.Clear();
            }
			else
			{
                foreach (var kvp in _reflectionCameras)
                {
                    Destroy(((Camera)kvp.Value).gameObject);
                }
                _reflectionCameras.Clear();

                foreach (var pTex in _pooledTextures)
                {
                    Destroy(((RenderTexture)pTex.texture));
                }
                _pooledTextures.Clear();
			}
        }


#if UNITY_EDITOR
        public void SurfaceGotDeselectedInEditor()
        {
			// notify the other surfaces as the material might have changed, to update their materials
			if (mirrorSurfaces == null)
			{
                return;
			}

            foreach (MirrorSurface ms in mirrorSurfaces)
            {
				if (ms)
				{
                    ms.RefreshMaterialInEditor();
				}
            }
        }
#endif


        // Extended sign: returns -1, 0 or 1 based on sign of a
        private static float sgn(float a)
        {
            if (a > 0.0f) return 1.0f;
            if (a < 0.0f) return -1.0f;
            return 0.0f;
        }

        // Given position/normal of the plane, calculates plane in camera space.
        private Vector4 CameraSpacePlane(Matrix4x4 worldToCameraMatrix, Vector3 pos, Vector3 normal, float sideSign, float clippingPlaneOffset)
        {
            Vector3 offsetPos = pos + normal * clippingPlaneOffset;
            Vector3 cpos = worldToCameraMatrix.MultiplyPoint(offsetPos);
            Vector3 cnormal = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
            return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        }

        // Calculates reflection matrix around the given plane
        private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
        {
            reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
            reflectionMat.m01 = (-2F * plane[0] * plane[1]);
            reflectionMat.m02 = (-2F * plane[0] * plane[2]);
            reflectionMat.m03 = (-2F * plane[3] * plane[0]);

            reflectionMat.m10 = (-2F * plane[1] * plane[0]);
            reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
            reflectionMat.m12 = (-2F * plane[1] * plane[2]);
            reflectionMat.m13 = (-2F * plane[3] * plane[1]);

            reflectionMat.m20 = (-2F * plane[2] * plane[0]);
            reflectionMat.m21 = (-2F * plane[2] * plane[1]);
            reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
            reflectionMat.m23 = (-2F * plane[3] * plane[2]);

            reflectionMat.m30 = 0F;
            reflectionMat.m31 = 0F;
            reflectionMat.m32 = 0F;
            reflectionMat.m33 = 1F;
        }

        // taken from http://www.terathon.com/code/oblique.html
        private static void MakeProjectionMatrixOblique(ref Matrix4x4 matrix, Vector4 clipPlane)
        {
            Vector4 q;

            // Calculate the clip-space corner point opposite the clipping plane
            // as (sgn(clipPlane.x), sgn(clipPlane.y), 1, 1) and
            // transform it into camera space by multiplying it
            // by the inverse of the projection matrix

            q.x = (sgn(clipPlane.x) + matrix[8]) / matrix[0];
            q.y = (sgn(clipPlane.y) + matrix[9]) / matrix[5];
            q.z = -1.0F;
            q.w = (1.0F + matrix[10]) / matrix[14];

            // Calculate the scaled plane vector
            Vector4 c = clipPlane * (2.0F / Vector3.Dot(clipPlane, q));

            // Replace the third row of the projection matrix
            matrix[2] = c.x;
            matrix[6] = c.y;
            matrix[10] = c.z + 1.0F;
            matrix[14] = c.w;
        }

    }

    public class PooledTexture
    {
        public bool liteLock;
        public bool fullLock;
        public CameraMatrices matrices;
        //public MirrorSurface mirrorSurface;
        //public MirrorSurface parentSurface;
        //public MirrorSurface parentsParentSurface;
        public RenderTexture texture;
        //public int depth;
        public Camera.StereoscopicEye eye = Camera.StereoscopicEye.Left;

        public PooledTexture()
        {
        }
    }

    public class CameraMatrices
    {
        public Matrix4x4 projectionMatrix;
        public Matrix4x4 worldToCameraMatrix;
        public Matrix4x4 cullingMatrix;
        public MirrorSurface mirrorSurface;
        public MirrorSurface parentMirrorSurface;
        public MirrorSurface parentsParentMirrorSurface;
        public MirrorSurface parentsParentsParentMirrorSurface;
        public MirrorSurface parentsParentsParentsParentMirrorSurface;
        public bool even;
        public Vector3 camPos;
        public int depth;
        public float distance;

        public CameraMatrices(Matrix4x4 projectionMatrix, Matrix4x4 worldToCameraMatrix, Matrix4x4 cullingMatrix, MirrorSurface mirrorSurface, bool even, 
            Vector3 camPos, int depth, float distance,
            MirrorSurface parentMirrorSurface,
            MirrorSurface parentsParentMirrorSurface,
            MirrorSurface parentsParentsParentMirrorSurface,
            MirrorSurface parentsParentsParentsParentMirrorSurface)
        {
            this.projectionMatrix = projectionMatrix;
            this.worldToCameraMatrix = worldToCameraMatrix;
            this.mirrorSurface = mirrorSurface;
            this.even = even;
            this.camPos = camPos;
            this.depth = depth;
            this.distance = distance;
            this.parentMirrorSurface = parentMirrorSurface;
            this.parentsParentMirrorSurface = parentsParentMirrorSurface;
            this.parentsParentsParentMirrorSurface = parentsParentsParentMirrorSurface;
            this.parentsParentsParentsParentMirrorSurface = parentsParentsParentsParentMirrorSurface;
            this.cullingMatrix = cullingMatrix;
        }
    }
}