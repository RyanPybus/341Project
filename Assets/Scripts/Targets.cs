using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class Targets : MonoBehaviour
{
    List<GameObject> targetList = new List<GameObject>();
    List<GameObject> hashList = new List<GameObject>();
    List<GameObject> outlineList = new List<GameObject>();
    List<GameObject> wedgeList = new List<GameObject>();

    public GameObject Target;
    public GameObject Hash;
    public GameObject HashPin;
    public GameObject Wedge;
    public float spread;

    public GameObject nearest;
    float nearRad;
    public GameObject furthest;
    float farRad;



    // Start is called before the first frame update
    void Start()
    {
        spread = 7;
    }

    public void GenerateTargets(int num, float innerRadius, float outerRadius)
    {
        List<float> radiuses = new List<float>();
        List<float> angles = new List<float>();
        float range = outerRadius * 2 - innerRadius * 4;
        for (int i = 0; i < 2*num; i++)
        {
            radiuses.Add(i * range / (2 * num) + innerRadius*4);
            angles.Add(i * 2*Mathf.PI / (2 * num));
        }

        nearRad = 100;
        farRad = 0;
        for (int i = 0; i < num; i++)
        {
            int lim = angles.Count;
            int indRadius = Random.Range(0, lim);
            int indAngle = Random.Range(0, lim);

            float angle = angles[indAngle];
            float radius = radiuses[indRadius];

            angles.RemoveAt(indAngle);
            radiuses.RemoveAt(indRadius);

            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            Vector2 pos = new Vector2(x, y);
            targetList.Add((GameObject)Instantiate(Target, pos, Quaternion.identity));
            targetList[i].transform.localScale = new Vector2(0.5f, 0.5f);
            Color color = new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
            );
            targetList[i].GetComponent<SpriteRenderer>().color = color;

            if (radius > farRad)
            {
                farRad = radius;
                furthest = targetList[i];
            }
            if (radius < nearRad) 
            { 
                nearRad = radius;
                nearest = targetList[i];
            }

        }
    }

    public void DestroyTargets()
    {
        foreach (GameObject obj in targetList)
        {
            Destroy(obj);
        }

        foreach (GameObject obj in outlineList)
        {
            Destroy(obj);
        }

        foreach (GameObject obj in hashList)
        {
            Destroy(obj);
        }

        foreach (GameObject obj in wedgeList)
        {
            Destroy(obj);
        }

        targetList.Clear();
        outlineList.Clear();
        hashList.Clear();
        wedgeList.Clear();
    }

    public void GenerateRays()
    {
        foreach (GameObject obj in targetList)
        {
            Vector2 pos = obj.transform.position;

            Color color = obj.GetComponent<SpriteRenderer>().color;

            float y = 0;
            float x = 0;
            if (pos.y > 2*pos.x && pos.y > -2*pos.x)
            {
                y = 4;
                x = y * pos.x / pos.y;
                //  y = mx+b = ypos/xpos*x + 0.    x = y*xpos/ypos

            }
            else if (pos.y < 2 * pos.x && pos.y > -2 * pos.x)
            {
                x = 2;
                y = x * pos.y / pos.x;
            }
            else if (pos.y < 2 * pos.x && pos.y < -2 * pos.x)
            {
                y = -4;
                x = y * pos.x / pos.y;
            }
            else if (pos.y > 2 * pos.x && pos.y < -2 * pos.x)
            {
                x = -2;
                y = x * pos.y / pos.x;
            }

            GameObject n = (GameObject)Instantiate(HashPin, new Vector2(x, y), Quaternion.identity);
            n.transform.Find("Circle").GetComponent<SpriteRenderer>().color = color;


            if (nearest == obj)
            {
                nearest = n;
            }
            if (furthest == obj)
            {
                furthest = n;
            }
            //
            // may add to list of targets here
            //


            // tan theta = y/x, theta  = tan-1 y/x 
            float zrot = Mathf.Atan(y / x) * 180 / Mathf.PI;
            Vector3 rot = new Vector3(0, 0, zrot);
            n.transform.Rotate(rot);

            if (n.transform.position.x > 0)
            {
                Vector3 flip = new Vector3(0, 0, 180);
                n.transform.Rotate(flip);
            }

            Vector2 origin = new Vector2(-x,-y);
            Vector2 pos1 = Vector2.zero;
            Vector2 pos2 = Vector2.zero;

            Vector2 direction1 = Quaternion.Euler(0, 0, spread / 2) * origin;
            Vector2 direction2 = Quaternion.Euler(0, 0, -spread / 2) * origin;

            LayerMask mask = LayerMask.GetMask("MAP");

            RaycastHit2D hit1 = Physics2D.Raycast(pos, direction1.normalized, 100f, mask, 0.5f, 2);
            pos1 = hit1.point;

            RaycastHit2D hit2 = Physics2D.Raycast(pos, direction2.normalized, 100f, mask, 0.5f, 2);
            pos2 = hit2.point;

            GameObject duplicate1 = (GameObject)Instantiate(Hash, pos1, Quaternion.identity);
            Vector3 rot1 = rot;
            rot1.z = rot.z + spread / 2;
            duplicate1.transform.Rotate(rot1);

            GameObject duplicate2 = (GameObject)Instantiate(Hash, pos2, Quaternion.identity);
            Vector3 rot2 = rot;
            rot2.z = rot.z - spread / 2;
            duplicate2.transform.Rotate(rot2);

            n.GetComponent<SpriteRenderer>().color = color;
            duplicate1.GetComponent<SpriteRenderer>().color = color;
            duplicate2.GetComponent<SpriteRenderer>().color = color;
            hashList.Add(n);
            hashList.Add(duplicate1);
            hashList.Add(duplicate2);
        }

        generateOutlines(hashList);
    }
    void generateOutlines(List<GameObject> list)
    {
        foreach (GameObject obj in list)
        {
            GameObject dupe;
            Vector3 pos = obj.transform.position;
            pos.z += 0.01f;
            Quaternion rotation = obj.transform.rotation;
            Vector3 scale = obj.transform.localScale;

            scale.x += 0.04f;
            scale.y += 0.04f;
            if (obj.name == "HashPin(Clone)")
            {
                Vector3 circlepos = obj.transform.Find("Circle").transform.position;
                dupe = (GameObject)Instantiate(HashPin, pos, rotation);
                dupe.transform.localScale = scale;
                GameObject circle = dupe.transform.Find("Circle").gameObject;
                circlepos.z = pos.z;
                circle.transform.position = circlepos;

                Vector3 circlescale = circle.transform.localScale;
                circlescale.x *= 1.045f;
                circlescale.y = 3.4f;
                circle.transform.localScale = circlescale;
                dupe.GetComponent<SpriteRenderer>().color = Color.black;
            }
            else if (obj.name == "Hash(Clone)")
            {
                dupe = (GameObject)Instantiate(Hash, pos, rotation);
                dupe.transform.localScale = scale;
                dupe.GetComponent<SpriteRenderer>().color = Color.black;
            }
            else
            {
                Vector3 trianglepos = obj.transform.Find("Triangle").transform.position;
                dupe = (GameObject)Instantiate(Wedge, pos, rotation);
                dupe.transform.localScale = scale;
                GameObject triangle = dupe.transform.Find("Triangle").gameObject;

                trianglepos.z = pos.z;
                triangle.transform.position = trianglepos;

                Vector3 triianglescale = triangle.transform.localScale;
                triianglescale.x *= 1.1f;
                triianglescale.y = 2.015f;
                triangle.transform.localScale = triianglescale;

                triangle.GetComponent<SpriteRenderer>().color = Color.black;

            }


            outlineList.Add(dupe);
        }
    }

    public void GenerateWedges()
    {
        float maxdist = furthest.transform.position.magnitude;
        float mindist = nearest.transform.position.magnitude;
        foreach (GameObject obj in targetList)
        {
            Vector3 pos = obj.transform.position;

            float y = 0;
            float x = 0;
            if (pos.y > 2 * pos.x && pos.y > -2 * pos.x)
            {
                //top
                y = 3.8f;
                x = y * pos.x / pos.y;
                //  y = mx+b = ypos/xpos*x + 0.    x = y*xpos/ypos
            }
            else if (pos.y < 2 * pos.x && pos.y > -2 * pos.x)
            {
                //right
                x = 1.8f;
                y = x * pos.y / pos.x;
            }
            else if (pos.y < 2 * pos.x && pos.y < -2 * pos.x)
            {
                //bottom
                y = -3.8f;
                x = y * pos.x / pos.y;
            }
            else if (pos.y > 2 * pos.x && pos.y < -2 * pos.x)
            {
                //left
                x = -1.8f;
                y = x * pos.y / pos.x;
            }

            float distance = Mathf.Sqrt(Mathf.Pow(pos.y - y, 2) + Mathf.Pow(pos.x - x, 2));

            float radius = Mathf.Sqrt(pos.x * pos.x + pos.y * pos.y);

            // base length = 3.5
            float scaleY = 1.06f * distance / 3.5f;


            GameObject n = (GameObject)Instantiate(Wedge, pos, Quaternion.identity);
            if (nearest == obj)
            {
                nearest = n;
            }
            if (furthest == obj)
            {
                furthest = n;
            }

            float zrot = Mathf.Atan(pos.y /pos.x) * 180 / Mathf.PI + 90;
            Vector3 rot = new Vector3(0, 0, zrot);
            n.transform.Rotate(rot);

            if (n.transform.position.x > 0)
            {
                Vector3 flip = new Vector3(0, 0, 180);
                n.transform.Rotate(flip);
            }

            // set based on furthest target
            float scaleX = 1;

            float actualRange = maxdist - mindist;

            float nrange = n.transform.position.magnitude - mindist + 0.01f;

            scaleX = 1.2f*nrange/actualRange + 0.9f;



            Vector2 scale = n.transform.localScale;
            scale.x *= scaleX;
            scale.y *= scaleY;
            n.transform.localScale = scale;

            wedgeList.Add(n);

        }
        generateOutlines(wedgeList);
    }
}
