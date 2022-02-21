using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class Game : MonoBehaviour
{
    public GameObject tokenObjWhite;
    public GameObject tokenObjBlack;
    private GameObject[,] tokens = new GameObject[8, 8];
    public PhotonView photonView;

    public GameObject PanelDialog;
    public Text messageMove, messageStatus, messageDebug;
    string moveText, statusText;
    
    public char[,] board; //поле
    public Player playerMove; //ігрок, який робить хід
    public Party partyMain = new Party(); //партія

    // host - користувач що створив кімнату та тримає дані
    // guest - другий користувач
    void Start()
    {
        PanelDialog.SetActive(true); // активує панель повідомлень
    }
    public void startParty() //заповнює форму 
    {
        board = partyMain.GetBoard(); //ініціалізує поле
        playerMove = partyMain.GetPlayer1(); //дає право ходу 1-му ігроку
        upDateBoard(board); //візуалузує поле
                            //записує чий хід 
        moveText = playerMove.GetName() + " move " +
                        (playerMove.GetFishka() == 'w' ? "white" : "black");
        // кількість фішок на полі суперників
        statusText = partyMain.GetPlayer1().GetScore().ToString() +
                 " vs " + partyMain.GetPlayer2().GetScore().ToString();
        UpdateMessage(moveText, statusText);

        photonView = PhotonView.Get(this); // обєкт візуалізації поля 
        Dictionary<string, string> dataToShare = new Dictionary<string, string>(); // конвертація даних для передачі guest 
        dataToShare.Add("board", BoardUtils.serialize(board));
        photonView.RPC(nameof(InitializeStartRPC), RpcTarget.Others, dataToShare); // викликає функцію в решти гравців  у кімнаті та передає у неї дані
    }
    private void OnMouseDown()
    {
        // отримання координат ходу гравця
        int x = (int)Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        int y = -1 * (int)Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        if (PhotonNetwork.LocalPlayer.NickName == "Player 2") // якщо це guest
        {
            photonView = PhotonView.Get(this);
            Dictionary<string, string> dataToShare = new Dictionary<string, string>
            {
                { "x", x.ToString() },
                { "y", y.ToString() }
            };
            photonView.RPC(nameof(InitializePlayer2RPC), RpcTarget.Others, dataToShare); // викликати функцію для перевірки ходу
        }
        else if (PhotonNetwork.LocalPlayer.NickName == playerMove.GetName() && partyMain.makeMove(playerMove.GetFishka(), x, y)) //перевірка можливості ходу hostа
        {   //якщо можлива
            board = partyMain.GetBoard(); //повертає змінене поле
            upDateBoard(board); //візуалізує поле
            int k = partyMain.GetValidMoves(playerMove.GetFishka()).Count;  //кількість можливих ходів поточного гравця, змінилась після ходу
            playerMove = partyMain.changePlayer(playerMove.GetFishka());//змінює гравця
            isEndParty(k); //перевіряє чи настав кінець гри
        }
    }
    public void isEndParty(int k1) //перевіряє чи настав кінець гри
    {
        int k2 = partyMain.GetValidMoves(playerMove.GetFishka()).Count;
        if (0 == k2 && 0 == k1) //якщо в обох гравців не моє можливості для ходу
        {
            Winner();
            return;
        }
        else if (0 == k2) //якщо поточний гравець не має можливості для ходу, змінює гравця
            playerMove = partyMain.changePlayer(playerMove.GetFishka());
        //записує чий хід 
        moveText = playerMove.GetName() + " move " +
                        (playerMove.GetFishka() == 'w' ? "white" : "black");
        // кількість фішок на полі суперників
        statusText = partyMain.GetPlayer1().GetScore().ToString() +
                 " vs " + partyMain.GetPlayer2().GetScore().ToString();
        UpdateMessage(moveText, statusText); // оновлює повідомлення про статус гри у host

        photonView = PhotonView.Get(this);
        Dictionary<string, string> dataToShare = new Dictionary<string, string>
            {
                { "moveText", moveText },
                { "statusText", statusText }
            };
        photonView.RPC(nameof(InitializeRPC), RpcTarget.Others, dataToShare);
    }
    [PunRPC]
    void InitializeStartRPC(object dataToShare) // викликаєтся для оновлення статусу гри та у вітдаленого клієнта
    {
        Dictionary<string, string> data = (Dictionary<string, string>)dataToShare;
        board = BoardUtils.deserialize(data["board"]);
        upDateBoard(board); //візуалізує поле
        UpdateMessage("Player 1 move black", "2 vs 2");
        partyMain = new Party(); // створюється для обробки кліку guest
    }
    [PunRPC]
    void InitializeRPC(object dataToShare) // викликаєтся для оновлення статусу гри у вітдаленого клієнта
    {
        Dictionary<string, string> data = (Dictionary<string, string>)dataToShare;
        UpdateMessage(data["moveText"], data["statusText"]);
    }
    [PunRPC]
    void InitializePlayer2RPC(object dataToShare) // викликає у користувача перевірку чи можливий хід гравця-клієнта
    {
        Dictionary<string, string> data = (Dictionary<string, string>)dataToShare;
        int x = Convert.ToInt32(data["x"]);
        int y = Convert.ToInt32(data["y"]);
        int k = 1;
        if (playerMove.GetName() == "Player 2" && partyMain.makeMove(playerMove.GetFishka(), x, y))
        { //перевірка можливості ходу
            //якщо можлива
            board = partyMain.GetBoard(); //повертає змінене поле
            upDateBoard(board); //візуалізує поле
            k = partyMain.GetValidMoves(playerMove.GetFishka()).Count;  //кількість можливих ходів поточного гравця, змінилась після ходу
            playerMove = partyMain.changePlayer(playerMove.GetFishka());//змінює гравця
        }
        isEndParty(k);
    }
    public void buttonNewGame_Click()
    {
        partyMain = new Party(); //нова партія
        startParty();
    }
    public void Winner() //об'являє переможця
    {
        int pl1Score = partyMain.GetPlayer1().GetScore(), // кількість фішок на дошці кожного з гравців
            pl2Score = partyMain.GetPlayer2().GetScore();

        photonView = PhotonView.Get(this);
        if (pl1Score > pl2Score)
        {
            string moveText = "Winner " + partyMain.GetPlayer1().GetName();
            string statusText = pl1Score.ToString() + " vs " + pl2Score.ToString();

            UpdateMessage(moveText, statusText); // оновлює повідомлення про статус гри у host

            Dictionary<string, string> dataToShare = new Dictionary<string, string>
            {
                { "moveText", moveText },
                { "statusText", statusText }
            };
            photonView.RPC(nameof(InitializeRPC), RpcTarget.Others, dataToShare); // оновлення повідомлення про статус гри у guest
        }
        else if (pl1Score < pl2Score)
        {
            string moveText = "Winner " + partyMain.GetPlayer2().GetName();
            string statusText = pl1Score.ToString() + " vs " + pl2Score.ToString();
            UpdateMessage(moveText, statusText);  // оновлює повідомлення про статус гри у host

            Dictionary<string, string> dataToShare = new Dictionary<string, string>
            {
                { "moveText", moveText },
                { "statusText", statusText }
            };
            photonView.RPC(nameof(InitializeRPC), RpcTarget.Others, dataToShare); // оновлення повідомлення про статус гри у guest
        }
        else
        {
            string moveText = "Not Winner";
            string statusText = pl1Score.ToString() + " vs " + pl2Score.ToString();
            UpdateMessage(moveText, statusText); // оновлює повідомлення про статус гри у host

            Dictionary<string, string> dataToShare = new Dictionary<string, string>
            {
                { "moveText", moveText },
                { "statusText", statusText }
            };
            photonView.RPC(nameof(InitializeRPC), RpcTarget.Others, dataToShare); // оновлення повідомлення про статус гри у guest
        }
    }
    void destroyTokens() // видаляє усі фішки з поля
    {
        for (int i = 0; i < 8; ++i)
            for (int j = 0; j < 8; ++j)
                if (tokens[i, j] != null)
                    PhotonNetwork.Destroy(tokens[i, j]);
    }
    void upDateBoard(char[,] board) //візуалізує поле
    {
        destroyTokens();
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                if (board[i, j] == 'w') //якщо на місці має стояти біла фішка
                    tokens[i, j] = PhotonNetwork.Instantiate(tokenObjWhite.name,
                        new Vector3(i + 0.5f, -j - 0.5f, 0),
                        Quaternion.identity);
                else if (board[i, j] == 'b') //якщо на місці має стояти чорна фішка
                    tokens[i, j] = PhotonNetwork.Instantiate(tokenObjBlack.name,
                        new Vector3(i + 0.5f, -j - 0.5f, 0),
                        Quaternion.identity);
            }
        }
        Debug.Log("Board updated");
    }
    private void UpdateMessage(string move, string status) //функція оновлення статусу гри
    {
        messageMove.text = move;
        messageStatus.text = status;
    }
}
