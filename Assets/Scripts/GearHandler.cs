using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject currentGear;

    private Transform handNode;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform childTransform in transform.GetComponentsInChildren<Transform>())
        {
            if (childTransform.name == "Hand Node")
            {
                handNode = childTransform;
            }
        }

        SetGear();
    }

    public void SetGear()
    {
        if (currentGear != null)
        {
            currentGear.transform.parent = handNode;
            currentGear.transform.localPosition = Vector3.zero;
            currentGear.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            currentGear.GetComponent<Gear>().SetUser(gameObject);
        }
    }

    public Gear GetCurrentGear()
    {
        return currentGear.GetComponent<Gear>();
    }
}
