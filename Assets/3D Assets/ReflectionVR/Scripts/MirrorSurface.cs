using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;
using System;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Fragilem17.MirrorsAndPortals
{

    [ExecuteInEditMode]
    public class MirrorSurface : MonoBehaviour
    {

        [Tooltip("The source material, disable and re-enable this component if you make changes to the material")]
        public Material Material;

        [Tooltip("When the camera is further from this distance, the surface stops updating it's texture.")]
        [MinAttribute(0)]
        public float maxRenderingDistance = 5f;

        [Tooltip("The % of maxRenderingDistance over which the mirror starts to darkens.")]
        [Range(0, 1)]
        public float fadeDistance = 0.5f;

        [Tooltip("How much reflection is allowed to blend in the color when you're closer than maxRenderingDistance-fadeDistance.")]
        [Range(0, 1)]
        public float maxBlend = 1f;

        public Color FadeColor = Color.black;


        [Space(10)]

        [Tooltip("When enabled each recursion can be used to darken the reflection, disabled the fadeDistance will be used to darken.")]
        public bool useRecursiveDarkening = true;

        public AnimationCurve recursiveDarkeningCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Other")]

        public float clippingPlaneOffset = 0.0f;


        public MeshRenderer myMeshRenderer;
        private Material _material;
        private MirrorRenderer _myRenderer;
        private Color _oldFadeColor = Color.black;
        private Material _oldMaterial;
        private bool _wasToFar = false;

        private bool _isSelectedInEditor = false;


        private void OnEnable()
        {
			if (myMeshRenderer == null)
			{
                myMeshRenderer = GetComponent<MeshRenderer>();
			}

            _wasToFar = false;

            if (!Material && myMeshRenderer)
            {
                Material = myMeshRenderer.sharedMaterial;
            }


            if (myMeshRenderer && Material)
            {
                _oldMaterial = Material;

			    if (_isSelectedInEditor)
                {
                    // make sure we're editing the source materials, not the instance
                    Material.SetColor("_FadeColor", FadeColor);
                    myMeshRenderer.sharedMaterial = Material;
                    _material = Material;
                }
				else
				{
                    _material = new Material(Material);
                    _material.name += " (for " + gameObject.name + ")";
                    _material.SetColor("_FadeColor", FadeColor);
                    myMeshRenderer.material = _material;
				}
            }
            


#if UNITY_EDITOR
            Selection.selectionChanged += OnSelectionChange;
#endif
        }
        private void OnDisable()
        {
#if UNITY_EDITOR
            Selection.selectionChanged -= OnSelectionChange;
#endif
            if (_material != Material)
            {
                DestroyImmediate(_material, true);
            }
            if (myMeshRenderer)
            {
                myMeshRenderer.material = Material;
            }
        }

#if UNITY_EDITOR
		private void OnDestroy()
		{
            _isSelectedInEditor = false;
            Selection.selectionChanged -= OnSelectionChange;
        }
#endif

        internal Bounds GetBounds()
		{
            return myMeshRenderer.bounds;
		}

        /*
        public bool VisibleInBoundsParent(MirrorSurface parentSurface, Camera reflectionCamera, Camera renderCamera) 
        {
			if (parentSurface == null)
			{
                return true;
			}

            float scale = 1;
            if (_myRenderer)
			{
                scale = _myRenderer.screenScaleFactor;
            }

            Rect myRect = RendererBoundsInScreenSpace(reflectionCamera, _renderer);
            Rect parentRect = RendererBoundsInScreenSpace(reflectionCamera, parentSurface._renderer);

			if (parentRect.width > Screen.width || parentRect.height > Screen.height)
			{
                return true;
			}
            
            //Vector3 pxb = renderCamera.ScreenToWorldPoint(new Vector3(myRect.x, myRect.y, renderCamera.nearClipPlane+0.1f));
            //Vector3 pxt = renderCamera.ScreenToWorldPoint(new Vector3(myRect.x, myRect.y + myRect.height, renderCamera.nearClipPlane + 0.1f));
            //Vector3 pyb = renderCamera.ScreenToWorldPoint(new Vector3(myRect.x + myRect.width, myRect.y, renderCamera.nearClipPlane + 0.1f));
            //Vector3 pyt = renderCamera.ScreenToWorldPoint(new Vector3(myRect.x + myRect.width, myRect.y + myRect.height, renderCamera.nearClipPlane + 0.1f));
   
            //Debug.DrawLine(pxt, pyb, Color.red);
            //Debug.DrawLine(pxt, pxb, Color.red);
            //Debug.DrawLine(pxb, pyt, Color.red);

            //pxb = renderCamera.ScreenToWorldPoint(new Vector3(parentRect.x, parentRect.y, renderCamera.nearClipPlane + 0.1f));
            //pyt = renderCamera.ScreenToWorldPoint(new Vector3(parentRect.x + parentRect.width, parentRect.y + parentRect.height, renderCamera.nearClipPlane + 0.1f));
            //pxt = renderCamera.ScreenToWorldPoint(new Vector3(parentRect.x, parentRect.y + parentRect.height, renderCamera.nearClipPlane + 0.1f));
            //pyb = renderCamera.ScreenToWorldPoint(new Vector3(parentRect.x + parentRect.width, parentRect.y, renderCamera.nearClipPlane+0.1f));

            //Debug.DrawLine(pxt, pyb, Color.green);
            //Debug.DrawLine(pxt, pxb, Color.green);
            //Debug.DrawLine(pxb, pyt, Color.green);
            


            if (myRect.Overlaps(parentRect, true))
            {
                //Debug.Log("Limiting recursion, no overlapping!");
                return true;
            }
            //Debug.Log("Limiting recursion, no overlapping!");
            return false;
        }

        private static Vector3[] screenSpaceCorners;
        static Rect RendererBoundsInScreenSpace(Camera theCamera, Renderer r)
        {
            // This is the space occupied by the object's visuals
            // in WORLD space.
            Bounds bigBounds = r.bounds;
            
            if (screenSpaceCorners == null)
                screenSpaceCorners = new Vector3[8];


            // For each of the 8 corners of our renderer's world space bounding box,
            // convert those corners into screen space.
            screenSpaceCorners[0] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[1] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
            screenSpaceCorners[2] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[3] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
            screenSpaceCorners[4] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[5] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
            screenSpaceCorners[6] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[7] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));

            // Now find the min/max X & Y of these screen space corners.
            float min_x = screenSpaceCorners[0].x;
            float min_y = screenSpaceCorners[0].y;
            float max_x = screenSpaceCorners[0].x;
            float max_y = screenSpaceCorners[0].y;

            for (int i = 1; i < 8; i++)
            {
                if (screenSpaceCorners[i].x < min_x)
                {
                    min_x = screenSpaceCorners[i].x;
                }
                if (screenSpaceCorners[i].y < min_y)
                {
                    min_y = screenSpaceCorners[i].y;
                }
                if (screenSpaceCorners[i].x > max_x)
                {
                    max_x = screenSpaceCorners[i].x;
                }
                if (screenSpaceCorners[i].y > max_y)
                {
                    max_y = screenSpaceCorners[i].y;
                }
            }

            return Rect.MinMaxRect(min_x, min_y, max_x, max_y);
        }
        */

        public bool VisibleFromCamera(Camera renderCamera, bool ignoreDistance = true)
		{
			//Debug.Log(name + " : " + _renderer.isVisible);

			if (!enabled || !myMeshRenderer || !_material || !gameObject.activeInHierarchy)
			{
                return false;
			}

			if (!myMeshRenderer.isVisible)
			{
                return false;
			}

            // check the normal of the mirror. if the camera is behind it, return early
            Vector3 forward = -1 * transform.forward; //transform.TransformDirection(Vector3.forward);
            Vector3 toOther = renderCamera.transform.position - transform.position;

            if (Vector3.Dot(forward, toOther) < 0) // if we're behind the mirror 
            {
                if (!_wasToFar)
                {
                    _wasToFar = true;

                    // blend the surface
                    if (_material.HasProperty("_DistanceBlend"))
                    {
                        _material.SetFloat("_DistanceBlend", 0);
                    }
                }

                return false;
			}
			else
			{
                if (_wasToFar)
                {
                    _wasToFar = false;
                }
            }

			if (!ignoreDistance)
			{
                bool toFar = Vector3.Distance(ClosestPoint(renderCamera.transform.position), renderCamera.transform.position) > maxRenderingDistance;
                if (toFar && !_wasToFar)
                {
                    _wasToFar = true;

                    // blend the surface
                    if (_material.HasProperty("_DistanceBlend"))
                    {
                        _material.SetFloat("_DistanceBlend", 0);
                        //Debug.Log(gameObject.name + " blend " + blend + " distance: " + distance);
                    }
                }
                if (!toFar && _wasToFar)
                {
                    _wasToFar = false;
                }

			    if (toFar)
                {
                    return false;
			    }
			}

            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(renderCamera);
            bool inBounds = GeometryUtility.TestPlanesAABB(planes, myMeshRenderer.bounds);
            return inBounds;
        }
        public bool ShouldRenderBasedOnDistance(Camera renderCamera)
        {
            if (!enabled)
            {
                return false;
            }

			if (!_material)
			{
                return false;
			}

            bool toFar = Vector3.Distance(ClosestPoint(renderCamera.transform.position), renderCamera.transform.position) > maxRenderingDistance;

			if (toFar && !_wasToFar)
			{
                _wasToFar = true;
                
                // blend the surface
                if (_material.HasProperty("_DistanceBlend"))
                {
                    _material.SetFloat("_DistanceBlend", 0);
                    //Debug.Log(gameObject.name + " blend " + blend + " distance: " + distance);
                }
            }

			if (!toFar && _wasToFar)
            {
                _wasToFar = false;
            }

            return !toFar;
        }

        public Vector3 ClosestPoint(Vector3 toPos)
		{
            Vector3 p = myMeshRenderer.bounds.ClosestPoint(toPos);
            return p;
		}

        public void UpdateMaterial(Camera.StereoscopicEye eye = Camera.StereoscopicEye.Left, RenderTexture texture = null, MirrorRenderer myRenderer = null, int depth = 1, float distance = 0)
		{
            if (myMeshRenderer && _material)
            {
                //Debug.Log(gameObject.name + " set prop 3");

                _myRenderer = myRenderer;
                //Debug.Log(name + " : " + distance);
                Material m = _material;

				if (depth >= _myRenderer.recursions+1)
				{
                    // we need to be fully opaque.. no need to do anything else
                    if (m.HasProperty("_DistanceBlend"))
                    {
                        m.SetFloat("_DistanceBlend", 0);
                        //Debug.Log(gameObject.name + " blend " + blend + " distance: " + distance);
                    }
                    return;
                }


                if (m.HasProperty("_ForceEye"))
				{
                    m.SetInt("_ForceEye", eye == Camera.StereoscopicEye.Left ? 0 : 1);
				}

                if (eye == Camera.StereoscopicEye.Left && m.HasProperty("_TexLeft") && texture != null)
                {
                    m.SetTexture("_TexLeft", texture);
                }

                if (eye == Camera.StereoscopicEye.Right && XRSettings.enabled && m.HasProperty("_TexRight") && texture != null)
                {
                    m.SetTexture("_TexRight", texture);
                }


				if (depth != -1)
				{
                    float blend;
                    distance = distance - (maxRenderingDistance - (maxRenderingDistance * fadeDistance));
                    blend = Mathf.Clamp(1 - (distance / (maxRenderingDistance * fadeDistance)), 0, 1) * maxBlend;

                    if (useRecursiveDarkening && depth > 1)
					{
                        float recusiveDarkening = 1 - (((float)depth-1) / ((float)myRenderer.recursions));
                        recusiveDarkening = recursiveDarkeningCurve.Evaluate(recusiveDarkening);
                        blend = recusiveDarkening; // Mathf.Min(recusiveDarkening, blend);
                    }

                    if (m.HasProperty("_DistanceBlend"))
                    {
                        m.SetFloat("_DistanceBlend", blend);
                        //Debug.Log(gameObject.name + " blend " + blend + " distance: " + distance);
                    }
				}
            }
        }



#if UNITY_EDITOR
		void OnSelectionChange()
        {
            if (gameObject == Selection.activeGameObject)
            {
                _isSelectedInEditor = true;

                // make sure we're editing the source materials, not the instance
				if (Material != null)
				{
                    myMeshRenderer.sharedMaterial = Material;
                    _material = Material;
				}
            }
            else if (_isSelectedInEditor)
            {
                // i'm no longer selected
                _isSelectedInEditor = false;

                OnDisable();
                OnEnable();

				if (_myRenderer != null)
				{
                    _myRenderer.SurfaceGotDeselectedInEditor();
				}
			}
        }

		public void RefreshMaterialInEditor()
        {
            OnDisable();
            OnEnable();
        }

		private void Update()
		{
            //if (_oldFadeColor != (FadeColor.r + FadeColor.g + FadeColor.b))
            if (!FadeColor.Equals(_oldFadeColor))
            {
                if (_material)
                { 
                    //Debug.Log(gameObject.name + " set prop 1");
                    _material.SetColor("_FadeColor", FadeColor);
                }
                //_oldFadeColor = (FadeColor.r + FadeColor.g + FadeColor.b);
                _oldFadeColor = FadeColor;
            }

			if (_oldMaterial != Material)
			{
                _material = Material;
                RefreshMaterialInEditor();
            }
        }
#endif

		public void TurnOffForceEye()
		{
            if (_material && _material.HasProperty("_ForceEye"))
            {
                //Debug.Log(gameObject.name + " set prop 2");
                _material.SetInt("_ForceEye", -1);
            }
        }
	}
}