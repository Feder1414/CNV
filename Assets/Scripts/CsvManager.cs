using System;
using UnityEngine;
using System.IO;
using System.Text;

public class CsvManager : MonoBehaviour
{
    [SerializeField] private string filename;


    public void Start()
    {
        EventManager.instance.RegisterCsvManager(this);
    }

    public void WriteTimeErrors(Guid userId, int totalErrors, float totalTime, int eventId, int condition)
    {
        try
        {
            if (!File.Exists(filename))
            {
                StreamWriter swCreation = new StreamWriter(filename, false, Encoding.UTF8);
                swCreation.WriteLine("uuid;totalErrors;totalTime;eventId;condition");
                swCreation.Close();
            }

            StreamWriter sw = new StreamWriter(filename, true, Encoding.UTF8);
            sw.WriteLine($"{userId};{totalErrors};{totalTime};{eventId};{condition}");
            sw.Close();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}