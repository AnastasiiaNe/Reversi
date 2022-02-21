using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUtils : MonoBehaviour
{
    public static string serialize(char[,] data)
    {
        string result = "";
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                result += data[i, j].ToString();
                if (j != 7) result += "_";

            }
            if (i != 7) result += "*";
        }
        return result;
    }
    public static char[,] deserialize(string data)
    {
        char[,] result = new char[8, 8];
        string[] s1 = data.Split('*');
        for (int i = 0; i < 8; i++)
        {
            string[] s2 = s1[i].Split('_');
            for (int j = 0; j < 8; j++)
            {
                result[i, j] = s2[j].ToCharArray()[0];
            }
        }
        return result;
    }
}
