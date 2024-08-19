using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextButton : MonoBehaviour
{
    public Master Master;

    private void OnMouseDown()
    {
        Debug.Log("next clicked");

        Master.nextSet();

    }
}
