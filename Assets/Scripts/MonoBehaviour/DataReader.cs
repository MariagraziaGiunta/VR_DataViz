using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class DataReader : MonoBehaviour
{

    public static DataReader Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    //Percorso della cartella CSV (generato via script)
    private string csvFolderPath;

    //Lista dei files csv presenti nella cartella
    public List<string> csvFiles = new List<string>();

    //File che vogliamo selezionare
    public string fileName;

    //Percorso del file selezionato (generato via script)
    private string csvFilePath;

    private List<Line> lines = new List<Line>();

    void Start()
    {
        csvFolderPath = Application.dataPath + "/CSV/";

        GetAllCSVFiles();
        SelectFileByName(fileName);
        LoadFileLines();
    }
    public string GetFileName()
    {
        return fileName;
    }

    //Leggo la cartella e popola la lista dei files
    public void GetAllCSVFiles()
    {
        csvFiles.Clear();

        try
        {
            if (!Directory.Exists(csvFolderPath))
            {
                Directory.CreateDirectory(csvFolderPath);
            }

            string[] files = Directory.GetFiles(csvFolderPath, "*.csv");

            foreach (string file in files)
            {
                csvFiles.Add(Path.GetFileName(file));
            }
        }

        catch (UnauthorizedAccessException)
        {
            Debug.LogError("Access denied to some directories.");
        }

        catch (Exception e)
        {
            Debug.LogError($"An error occurred while accessing files: {e.Message}");
        }
    }

    //Cerco nella lista il file indicato
    public bool SelectFileByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;

        if (csvFiles.Contains(name))
        {
            fileName = name;
            csvFilePath = Path.Combine(csvFolderPath, fileName);
            return true;
        }

        Debug.LogWarning($"SelectFileByName: '{name}' not found in discovered CSV list.");
        return false;
    }

    //Leggo le righe del file
    public bool LoadFileLines()
    {
        if (string.IsNullOrEmpty(csvFilePath))
        {
            Debug.LogWarning("No file selected to load lines from.");
            return false;
        }

        if (!File.Exists(csvFilePath))
        {
            Debug.LogWarning($"LoadLinesFromFile: file does not exist: {csvFilePath}");
            return false;
        }

        try
        {
            string[] allLines = File.ReadAllLines(csvFilePath);
            Debug.Log("All lines read from: " + csvFilePath);

            lines = new List<Line>();

            foreach (string s in allLines)
            {
                if (string.IsNullOrWhiteSpace(s)) continue;

                string[] splitLine = s.Split(',');
                var l = new Line
                {
                    category1 = splitLine.Length > 0 ? splitLine[0] : string.Empty,
                    category2 = splitLine.Length > 1 ? splitLine[1] : string.Empty
                };
                lines.Add(l);
                Debug.Log($"Parsed line - Category1: {l.category1}, Category2: {l.category2}");
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Exception while reading/parsing file: " + e.Message);
            return false;
        }
    }

    //Ritorno il numero di righe del file
    public int GetLinesNumber()
    {
        try
        {
            if (string.IsNullOrEmpty(csvFilePath) || !File.Exists(csvFilePath))
            {
                Debug.LogWarning("GetLinesNumber: invalid or missing selectedFilePath.");
                return -1;
            }

            return File.ReadAllLines(csvFilePath).Length;
        }
        catch (Exception e)
        {
            Debug.Log("Exception on file read: " + e.Message);
        }
        return -1;
    }
}

//TO DO: Costruire questa classe dinamicamente in base alle colonne del file CSV?
internal class Line
{
    public string category1;
    public string category2;
}