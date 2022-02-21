using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    char[,] board = new char[8, 8];
    Player player1 { get; set; }
    Player player2 { get; set; }

    private List<(int, int)> d = new List<(int, int)> { (0, 1), (1, 0), (1, 1), (1, -1), (-1, -1), (-1, 0), (0, -1), (-1, 1) };
    public Party() //створює дошку та гравців
    {
        for (int i = 0; i < 8; ++i)
            for (int j = 0; j < 8; ++j)
                board[i, j] = '.';

        board[3, 3] = board[4, 4] = 'w';
        board[4, 3] = board[3, 4] = 'b';

        player1 = new Player("Player 1", 2, 'b');
        player2 = new Player("Player 2", 2, 'w');
    }
    public Player changePlayer(char s) //змінює гравця
    {
        if (player1.GetFishka() == s)
            return player2;
        return player1;
    }
    public bool isOnBoard(int x, int y) //перевіряє чи точка знаходится в межах поля
    {
        return x >= 0 && x < 8 && y >= 0 && y < 8;
    }
    public List<(int, int)> GetValidMoves(char colorf) //повертає список можливих ходів
    {
        List<(int, int)> validMoves = new List<(int, int)>();
        for (int i = 0; i < 8; ++i)
            for (int j = 0; j < 8; ++j)
                if (isValidMove(colorf, i, j) != null && 0 < isValidMove(colorf, i, j).Count)
                    validMoves.Add((i, j));
        return validMoves;
    }
    public List<(int, int)> isValidMove(char colorf, int xstart, int ystart) //перевіряє чи можливий  хід 
    {
        if (board[xstart, ystart] != '.' || !isOnBoard(xstart, ystart))// якщо на місці вже стоїть фішка або крок за межі поля 
            return null;                                               // повертає null
                                                                       // визначає колір фішки суперника
        char otherf;
        if (colorf == 'w')
            otherf = 'b';
        else
            otherf = 'w';
        // список для можливих ходів
        List<(int, int)> tilesToFlip = new List<(int, int)>();
        for (int i = 0; i < 8; ++i) //цикл по 8-ми напрямкам
        {
            int x = xstart, y = ystart;
            x += d[i].Item1; //зміщення в одному з напрямків
            y += d[i].Item2; //зміщення в одному з напрямків
            while (isOnBoard(x, y) && board[x, y] == otherf)
            { //зміщюємся доки рухаємося по фішках суперника
                x += d[i].Item1;
                y += d[i].Item2;
                if (isOnBoard(x, y) && board[x, y] == colorf) //якщо дійшли до своєї фішшки
                {
                    while (true)// перевертаємо усі фішки по яких пройшлись
                    {
                        x -= d[i].Item1;
                        y -= d[i].Item2;
                        if (x == xstart && y == ystart)//якщо повернулись до фішки з якой починали, зупиняємось
                            break;
                        tilesToFlip.Add((x, y)); // додаєм координати фішки яку можемо перевернути
                    }
                }
            }
        }
        return tilesToFlip; //список перевернутих фішок
    }

    public bool makeMove(char fishka, int xstart, int ystart)// робить хід, якщо він задовілняє правилам
    {
        List<(int, int)> tilesToFlip = isValidMove(fishka, xstart, ystart);
        if (null == tilesToFlip)//якщо хід неможливий
            return false; // крок не виконуєтся

        if (0 == tilesToFlip.Count) //якщо хід не можливий через те що не перевертаются фішки суперника
            return false;

        //якщо хід за правилами
        board[xstart, ystart] = fishka; //поставити фішку
        foreach ((int, int) f in tilesToFlip) //перевертає фішки суперника
            board[f.Item1, f.Item2] = fishka;

        //перераховує фішки гравців
        player1.changeScore(GetScore(player1.GetFishka()));
        player2.changeScore(GetScore(player2.GetFishka()));
        return true;
    }
    public int GetScore(char s) //рахує кількість фішок на полі
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
