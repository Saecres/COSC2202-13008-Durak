using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    // Variables to hold texts (these are linked to the text fields in ui)
    public Text rankText;
    public Text playerText;
    public Text scoreText;

    // File path to the leaderboard file
    private static string filePath = "Assets/Statistics/leaderboard.txt";
    private static LeaderBoard instance;

    // Updates the leaderboard as the leaderboard scene starts
    private void Start()
    {
        UpdateLeaderboard();
    }

    // Updates the leaderboard using the leaderboard textfile
    void UpdateLeaderboard()
    {
        // Gets the current leaderboard entries from the txt file
        LeaderboardEntry[] leaderboardEntries = ReadLeaderboardFromFile(filePath);

        // Sort leaderboard entries
        SortLeaderboard(ref leaderboardEntries);

        // Updates the ui
        UpdateUI(leaderboardEntries);
    }

    void SortLeaderboard(ref LeaderboardEntry[] entries)
    {
        Array.Sort(entries, (x, y) => y.score.CompareTo(x.score));
    }

    // takes a leaderboard entries and updates the ui
    void UpdateUI(LeaderboardEntry[] entries)
    {
        // Clear previous text
        rankText.text = "";
        playerText.text = "";
        scoreText.text = "";

        // Populate leaderboard with new entries ( set to display only 10 players)
        for (int i = 0; i < 11; i++)
        {
            rankText.text += (i + 1).ToString() + "\n";
            playerText.text += entries[i].playerName + "\n";
            scoreText.text += entries[i].score.ToString() + "\n";
        }
    }

    // this method will be used to update the textfile based on the player name to either add
    // or update a player
    public static void UpdatePlayerLoses(string playerName)
    {
        // Variables
        LeaderboardEntry[] leaderboardEntries = ReadLeaderboardFromFile(filePath);
        bool playerFound = false;

        // Checking if the player exists
        for (int i = 0; i < leaderboardEntries.Length; i++)
        {
            // if the player is found then update the score and set playerfound to true
            if (leaderboardEntries[i].playerName == playerName)
            {
                leaderboardEntries[i].score++;
                playerFound = true;
                break;
            }
        }

        // If the player does not exist add a new player
        if (!playerFound)
        {
            // Add new player with 1 lose
            List<LeaderboardEntry> tempList = new List<LeaderboardEntry>(leaderboardEntries);
            tempList.Add(new LeaderboardEntry { playerName = playerName, score = 1 });
            leaderboardEntries = tempList.ToArray();
        }

        // Sort entries by score
        System.Array.Sort(leaderboardEntries, (x, y) => y.score.CompareTo(x.score));

        // Write updated leaderboard to file
        WriteLeaderboardToFile(filePath, leaderboardEntries);

        // Update UI
        instance.UpdateUI(leaderboardEntries);
    }

    // Method to read from a file
    static LeaderboardEntry[] ReadLeaderboardFromFile(string path)
    {
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 2)
                {
                    entries.Add(new LeaderboardEntry
                    {
                        playerName = parts[0],
                        score = int.Parse(parts[1])
                    });
                }
            }

            return entries.ToArray();
        }
        else
        {
            Debug.LogWarning("Leaderboard file not found: " + path);
            return new LeaderboardEntry[0];
        }
    }

    // Method to write to a file
    static void WriteLeaderboardToFile(string path, LeaderboardEntry[] entries)
    {
        using (StreamWriter writer = new StreamWriter(path))
        {
            foreach (var entry in entries)
            {
                writer.WriteLine($"{entry.playerName},{entry.score}");
            }
        }
    }
}


// Leaderboard entry object
public class LeaderboardEntry
{
    public string playerName;
    public int score;
}
