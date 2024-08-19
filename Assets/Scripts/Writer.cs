using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;

public class Writer : MonoBehaviour
{
    public string filename = "";

    public Master Master;

    public float time = 0.0f;
    public float finalTime = 0.0f;
    public int wrong = 0;

    public bool counting;

    // Start is called before the first frame update
    void Start()
    {
        counting = false;
        filename = Application.dataPath + "/" + Master.vis + Master.ID + ".csv";
        TextWriter w = new StreamWriter(filename, false);
        w.WriteLine("ID,Vis,Range,Density,Errors,Time");
        w.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (counting) time += Time.deltaTime;
    }
    
    public void WriteCSV()
    {
        TextWriter w = new StreamWriter(filename, true);
        finalTime = time;

        w.WriteLine(Master.ID + ", " + Master.vis + ", " + Master.range + ", " + Master.density + ", " + wrong + ", " + finalTime);
        w.Close();

        time = 0;
        wrong = 0;
        finalTime = 0;
        counting = false;
    }
}
