using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DebugLogExporter : MonoBehaviour
{
    private FileStream fs;
    private StreamWriter sw;
    public static DebugLogExporter _logExporter;
    // Start is called before the first frame update

    private void Awake()
    {
        _logExporter = this;
        fs = new FileStream("C:\\Users\\user\\Downloads\\BadmintonScoreLog.txt", FileMode.OpenOrCreate);
    }

    void Start()
    {
        sw = new StreamWriter(fs);
    }

    public void SetLog(string log)
    {
        sw.WriteLine(DateTime.Now + log);
    }
    
}
