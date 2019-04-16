using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.EventSystems;

public class PlacingObjectAutomatic : MonoBehaviour
{

    public LayerMask layerMask;
    public GameObject[] TestingGround;
    public bool isPlacing=false;
    public ButtonController buttonController;
    public Material planeMaterial;
    Vector3 lastPlacementPos;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARSessionOrigin m_SessionOrigin;
    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>

    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();

        if (Application.isEditor)
        {
            for (int i = 0; i < TestingGround.Length; i++)
            {
                TestingGround[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < TestingGround.Length; i++)
            {
                TestingGround[i].SetActive(false);
            }
        }
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        SetPlaneOn(true);
    }

    void Update()
    {

        if (buttonController != null)
        {
            // if the application is editor then create a ray that will start from the middle of the screen
            if (Application.isEditor)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth * 0.5f, Camera.main.pixelHeight * 0.5f, 0f));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 500f, layerMask))
                {
                    GameObjectPlacing(hit.point);
                }
            }
            else
            {
                
                if (m_SessionOrigin.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    // Raycast hits are sorted by distance, so the first one
                    // will be the closest hit.
                    Pose hitPose = s_Hits[0].pose;
                    GameObjectPlacing(hitPose.position);

                }
            }
            if (isPlacing == false && buttonController.ObjectPlaced == false)
            {
                HideItem();
            }
            else
            {
                CheckTouchTap();
            }
            isPlacing = false;
        }
    }
    void GameObjectPlacing(Vector3 newPosition)
    {
        if (buttonController != null)
        {
            if (buttonController.ObjectPlaced == false)
            {
                isPlacing = true;
                lastPlacementPos = newPosition;
                buttonController.GetGameObjectToPlace().SetActive(true);
                buttonController.GetGameObjectToPlace().transform.position = Vector3.Lerp(buttonController.GetGameObjectToPlace().transform.position, newPosition, 0.1f);
                buttonController.GetGameObjectToPlace().transform.parent = null;

                Vector3 CameraFlatPos = new Vector3(Camera.main.transform.position.x, newPosition.y, Camera.main.transform.position.z);

                buttonController.GetGameObjectToPlace().transform.LookAt(CameraFlatPos);

                if (!buttonController.GetGameObjectToPlace().activeSelf)
                {
                    buttonController.GetGameObjectToPlace().SetActive(true);
                }
            }
        }
    }
    public void CheckTouchTap()
    {
        //Stops going through UI to place object
        if(EventSystem.current.IsPointerOverGameObject() ||
           EventSystem.current.currentSelectedGameObject!=null)
        {
            return;
        }

        if (Application.isEditor)
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray,out hit, 500f, layerMask))
                {
                    TapHasOccured();
                }
            }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (m_SessionOrigin.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    // Raycast hits are sorted by distance, so the first one
                    // will be the closest hit.
                    if(touch.phase == TouchPhase.Ended) // uncomment to use duplicate
                    {
                    Pose hitPose = s_Hits[0].pose;
                    TapHasOccured();
                    }

                }
            }
                
        }
    }
    public void SetPlaneOn(bool ison)
    {
        Color color = planeMaterial.color;

        if (ison==true)
        {
            color.a = 0.3f;
        }
        else
        {
            color.a = 0;
            LineRenderer[] allLines = GetComponentsInChildren<LineRenderer>();
            for(int i=0; i < allLines.Length; i++)
            {
                Destroy(allLines[i]);
            }
        }
        planeMaterial.color = color;
    }
    public void TapHasOccured()
    {
        if(buttonController.ObjectPlaced == false)
        {
            //buttonController.ObjectPlaced = true;  //Comment this to dupplicate 
            buttonController.DuplicateGameObject().transform.position = lastPlacementPos;  //Replace for duplicate method 
            //SetPlaneOn(false);   //Comment this to dupplicate 
        }
            
    }

    
    public void NewObjectToPlace(ButtonController buttonController1)
    {
        ShouldWeHideItem();
        this.buttonController = buttonController1;
        SetPlaneOn(true);
    }
    public void ShouldWeHideItem()
    {
        if (buttonController != null)
        {
            if (buttonController.ObjectPlaced == false)
            {
                HideItem();
            }
        }
    }
    public void HideItem()
    {
        if (buttonController != null)
        {
            buttonController.GetGameObjectToPlace().SetActive(false);
            buttonController.GetGameObjectToPlace().transform.parent = Camera.main.transform;
            buttonController.GetGameObjectToPlace().transform.localPosition = Vector3.zero;
        }
    }
    public void RemoveItem()
    {
        buttonController = null;
        SetPlaneOn(false);
    }

    
}
