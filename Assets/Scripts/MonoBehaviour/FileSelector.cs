using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class FileSelector : MonoBehaviour
{

    public static FileSelector Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public List<string> csvFiles = new List<string>();
    public string fileName;
    private string selectedFilePath;
    private string applicationDataPath;
    private string csvFilesPath;
    private List<Line> lines = new List<Line>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set the data paths for the application and the custom folder
        applicationDataPath = Application.dataPath;
        csvFilesPath = applicationDataPath + "/CSV/";
        // Load all of the CSV files and set the dropdown options
        GetAllCSVFiles();
        SelectFileByName(fileName);
        LoadSelectedFileLines();
    }
    public string GetFileName()
    {
        return fileName;
    }
    // Get the CSV file names and store them in a List<string>
    public void GetAllCSVFiles()
    {
        // Clear the current list as this code is re-used
        csvFiles.Clear();

        // Attempt to find all of the CSV files
        try
        {
            // Ensure the directory exists, if not, create it. 
            if (!Directory.Exists(csvFilesPath))
            {
                Directory.CreateDirectory(csvFilesPath);
            }


            // Create a list of all of the files ending with .csv in this directory
            string[] files = Directory.GetFiles(csvFilesPath, "*.csv");

            // Go through each file and add their file's name to the csvFiles (NOT THEIR FILE PATH), these will be the dropdown values
            foreach (string file in files)
            {
                csvFiles.Add(Path.GetFileName(file));
            }
        }

        // Catch access denied
        catch (UnauthorizedAccessException)
        {
            Debug.LogError("Access denied to some directories.");
        }
        // Catch other errors and debug their message
        catch (Exception e)
        {
            Debug.LogError($"An error occurred while accessing files: {e.Message}");
        }
    }

    // Select a discovered file by its exact filename
    public bool SelectFileByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;

        if (csvFiles.Contains(name))
        {
            fileName = name;
            selectedFilePath = Path.Combine(csvFilesPath, fileName);
            return true;
        }

        Debug.LogWarning($"SelectFileByName: '{name}' not found in discovered CSV list.");
        return false;
    }

    // Load lines from the currently selected file into memory
    public bool LoadSelectedFileLines()
    {
        if (string.IsNullOrEmpty(selectedFilePath))
        {
            Debug.LogWarning("No file selected to load lines from.");
            return false;
        }

        return LoadLinesFromFile(selectedFilePath);
    }

    // Parse the CSV file (simple comma split, resilient to short/empty lines)
    private bool LoadLinesFromFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            Debug.LogWarning($"LoadLinesFromFile: file does not exist: {filePath}");
            return false;
        }

        try
        {
            string[] allLines = File.ReadAllLines(filePath);
            Debug.Log("All lines read from: " + filePath);

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

    // Return number of lines in the selected file (or -1 on error)
    public int GetNumberOfLinesInSelectedFile()
    {
        try
        {
            if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
            {
                Debug.LogWarning("GetNumberOfLinesInSelectedFile: invalid or missing selectedFilePath.");
                return -1;
            }

            return File.ReadAllLines(selectedFilePath).Length;
        }
        catch (Exception e)
        {
            Debug.Log("Exception on file read: " + e.Message);
        }
        return -1;
    }

    // Utility: open the custom folder in OS file explorer
    public void OpenCsvFilesPath()
    {
        Application.OpenURL(csvFilesPath);
    }

    public void SetSelectedFileName()
    {
        PlayerPrefs.SetString("Custom File Path", selectedFilePath);
    }
}

internal class Line
{
    public string category1;
    public string category2;
}