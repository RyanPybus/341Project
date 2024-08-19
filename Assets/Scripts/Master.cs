using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Master : MonoBehaviour
{

    public int countset;
    public int counttarget;
    public enum Vis {Wedge, Hash }
    public Vis vis;

    public Targets Targets;
    public Writer Writer;
    public bool near;
    GameObject Target;

    public GameObject Button;

    public int density;
    public float range;
    public string ID;

    // Start is called before the first frame update
    void Start()
    {
        countset = 0;
        counttarget = 0;
        near = true;        
    }

    public void nextSet()
    {
        if (counttarget > 9)
        {
            countset++;
            counttarget = 0;
        }

        switch (countset)
        {
            case 0:
                density = 5;
                range = 10f;
                break;
            case 1:
                density = 10;
                range = 10f;
                break;
            case 2:
                density = 5;
                range = 5f;
                break;
            case 3:
                density = 10;
                range = 5f;
                break;
            case 4:
                if (near)
                {
                    // NEED TO INFORM USER THAT THEY NOW SEEK THE FURTHEST
                    near = false;
                    countset = 0;
                    Button.GetComponent<SpriteRenderer>().color = Color.black;
                    Button.SetActive(true);
                    return;
                }
                else
                {
                    Debug.Log("finished testing");
                    EditorApplication.ExitPlaymode();
                }
                break;

        }

        Targets.GenerateTargets(density, 1.15f, range);

        if (vis == Vis.Wedge)
        {
            Targets.GenerateWedges();
        } else
        {
            Targets.GenerateRays();
        }

        Button.SetActive(false);
        counttarget++;
        Writer.counting = true;
    }

    public void nextTarget(GameObject obj)
    {

        if (near)
        {
            Target = Targets.nearest;
        } else
        {
            Target = Targets.furthest;
        }

        Debug.Log(Target);

        if (Target == obj)
        {
            Writer.WriteCSV();
            Button.SetActive(true);
            Targets.DestroyTargets();
        }

        else { Writer.wrong++; }

    }

}
