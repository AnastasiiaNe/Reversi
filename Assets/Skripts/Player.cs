using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private string Name { get; set; }
    private int Score { get; set; }
    private char Fishka { get; set; }

    public Player()
    {

    }
    public Player(string Name, int Score, char Fishka)
    {
        this.Name = Name;
        this.Score = Score;
        this.Fishka = Fishka;
    }
    public void changeScore(int newScore)
    {
        Score = newScore;
    }
    public string GetName()
    {
        return Name;
    }
    public int GetScore()
    {
        return Score;
    }
    public char GetFishka()
    {
        return Fishka;
    }
}
