using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public bool ObjectPlaced = false;
    public GameObject itemToPlace;
    public PlacingObjectAutomatic objectAutomatic;
    // Start is called before the first frame update
    void Start()
    {
        if(ObjectPlaced == false)
        {
            itemToPlace.SetActive(false);
        }
    }

    public void ButtonClicked()
    {
        if(ObjectPlaced == false)
        {
            if(objectAutomatic.buttonController != this)
            {
                objectAutomatic.NewObjectToPlace(this);
            }
            else
            {
                PutItemAway();
            }
        }   
        else
        {
            PutItemAway();
        }
        
    }
    public GameObject GetGameObjectToPlace()
    {
        return itemToPlace;
    }
    public GameObject DuplicateGameObject()
    {
        GameObject Duplicate = Instantiate(itemToPlace, itemToPlace.transform.position, itemToPlace.transform.rotation, itemToPlace.transform.parent);
        return Duplicate;
    }
    public void PutItemAway()
    {
        objectAutomatic.NewObjectToPlace(this);
        ObjectPlaced = false;
        objectAutomatic.HideItem();
        objectAutomatic.RemoveItem();
    }
}
