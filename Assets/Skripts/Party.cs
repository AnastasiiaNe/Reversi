using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    char[,] board = new char[8, 8];
    Player player1 { get; set; }
    Player player2 { get; set; }

    private List<(int, int)> d = new List<(int, int)> { (0, 1), (1, 0), (1, 1), (1, -1), (-1, -1), (-1, 0), (0, -1), (-1, 1) };
    public Party()
    {
        for (int i = 0; i < 8; ++i)
            for (int j = 0; j < 8; ++j)
                board[i, j] = '.';

        board[3, 3] = board[4, 4] = 'w';
        board[4, 3] = board[3, 4] = 'b';

        player1 = new Player("Player 1", 2, 'b');
        player2 = new Player("Player 2", 2, 'w');
    }
    public Player changePlayer(char s) 
    {
        if (player1.GetFishka() == s)
            return player2;
        return player1;
    }
    public bool isOnBoard(int x, int y) 
    {
        return x >= 0 && x < 8 && y >= 0 && y < 8;
    }
    public List<(int, int)> GetValidMoves(char colorf)
    {
        List<(int, int)> validMoves = new List<(int, int)>();
        for (int i = 0; i < 8; ++i)
            for (int j = 0; j < 8; ++j)
                if (isValidMove(colorf, i, j) != null && 0 < isValidMove(colorf, i, j).Count)
                    validMoves.Add((i, j));
        return validMoves;
    }
    public List<(int, int)> isValidMove(char colorf, int xstart, int ystart) 
    {
        if (board[xstart, ystart] != '.' || !isOnBoard(xstart, ystart))
            return null;                                               
                                                                       
        char otherf;
        if (colorf == 'w')
            otherf = 'b';
        else
            otherf = 'w';
        
        List<(int, int)> tilesToFlip = new List<(int, int)>();
        for (int i = 0; i < 8; ++i) 
        {
            int x = xstart, y = ystart;
            x += d[i].Item1; 
            y += d[i].Item2; 
            while (isOnBoard(x, y) && board[x, y] == otherf)
            { 
                x += d[i].Item1;
                y += d[i].Item2;
                if (isOnBoard(x, y) && board[x, y] == colorf) 
                {
                    while (true)
                    {
                        x -= d[i].Item1;
                        y -= d[i].Item2;
                        if (x == xstart && y == ystart)
                            break;
                        tilesToFlip.Add((x, y)); 
                    }
                }
            }
        }
        return tilesToFlip; 
    }

    public bool makeMove(char fishka, int xstart, int ystart)
    {
        List<(int, int)> tilesToFlip = isValidMove(fishka, xstart, ystart);
        if (null == tilesToFlip)
            return false;

        if (0 == tilesToFlip.Count) 
            return false;

        board[xstart, ystart] = fishka;
        foreach ((int, int) f in tilesToFlip) 
            board[f.Item1, f.Item2] = fishka;

        player1.changeScore(GetScore(player1.GetFishka()));
        player2.changeScore(GetScore(player2.GetFishka()));
        return true;
    }
    public int GetScore(char s) 
    {
        int score = 0;
        for (int i = 0; i < 8; ++i)
            for (int j = 0; j < 8; ++j)
                if (board[i, j] == s)
                    ++score;
        return score;
    }
    public char[,] GetBoard()
    {
        return board;
    }
    public Player GetPlayer1()
    {
        return player1;
    }
    public Player GetPlayer2()
    {
        return player2;
    }
}
