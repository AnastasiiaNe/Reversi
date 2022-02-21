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
    
    public char[,] board; 
    public Player playerMove; 
    public Party partyMain = new Party();

    void Start()
    {
        PanelDialog.SetActive(true);
    }
    public void startParty() 
    {
        board = partyMain.GetBoard(); 
        playerMove = partyMain.GetPlayer1(); 
        upDateBoard(board); 

        moveText = playerMove.GetName() + " move " +
                        (playerMove.GetFishka() == 'w' ? "white" : "black");
        
        statusText = partyMain.GetPlayer1().GetScore().ToString() +
                 " vs " + partyMain.GetPlayer2().GetScore().ToString();
        UpdateMessage(moveText, statusText);

        photonView = PhotonView.Get(this);
        Dictionary<string, string> dataToShare = new Dictionary<string, string>(); 
        dataToShare.Add("board", BoardUtils.serialize(board));
        photonView.RPC(nameof(InitializeStartRPC), RpcTarget.Others, dataToShare); 
    }
    private void OnMouseDown()
    {
        int x = (int)Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        int y = -1 * (int)Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        if (PhotonNetwork.LocalPlayer.NickName == "Player 2")
        {
            photonView = PhotonView.Get(this);
            Dictionary<string, string> dataToShare = new Dictionary<string, string>
            {
                { "x", x.ToString() },
                { "y", y.ToString() }
            };
            photonView.RPC(nameof(InitializePlayer2RPC), RpcTarget.Others, dataToShare);
        }
        else if (PhotonNetwork.LocalPlayer.NickName == playerMove.GetName() && partyMain.makeMove(playerMove.GetFishka(), x, y))
        {   
            board = partyMain.GetBoard();
            upDateBoard(board);
            int k = partyMain.GetValidMoves(playerMove.GetFishka()).Count;  
            playerMove = partyMain.changePlayer(playerMove.GetFishka());
            isEndParty(k); 
        }
    }
    public void isEndParty(int k1)
    {
        int k2 = partyMain.GetValidMoves(playerMove.GetFishka()).Count;
        if (0 == k2 && 0 == k1) 
        {
            Winner();
            return;
        }
        else if (0 == k2) 
            playerMove = partyMain.changePlayer(playerMove.GetFishka());
        
        moveText = playerMove.GetName() + " move " +
                        (playerMove.GetFishka() == 'w' ? "white" : "black");
        
        statusText = partyMain.GetPlayer1().GetScore().ToString() +
                 " vs " + partyMain.GetPlayer2().GetScore().ToString();
        UpdateMessage(moveText, statusText); 

        photonView = PhotonView.Get(this);
        Dictionary<string, string> dataToShare = new Dictionary<string, string>
            {
                { "moveText", moveText },
                { "statusText", statusText }
            };
        photonView.RPC(nameof(InitializeRPC), RpcTarget.Others, dataToShare);
    }
    [PunRPC]
    void InitializeStartRPC(object dataToShare) 
    {
        Dictionary<string, string> data = (Dictionary<string, string>)dataToShare;
        board = BoardUtils.deserialize(data["board"]);
        upDateBoard(board);
        UpdateMessage("Player 1 move black", "2 vs 2");
        partyMain = new Party();
    }
    [PunRPC]
    void InitializeRPC(object dataToShare) 
    {
        Dictionary<string, string> data = (Dictionary<string, string>)dataToShare;
        UpdateMessage(data["moveText"], data["statusText"]);
    }
    [PunRPC]
    void InitializePlayer2RPC(object dataToShare) 
    {
        Dictionary<string, string> data = (Dictionary<string, string>)dataToShare;
        int x = Convert.ToInt32(data["x"]);
        int y = Convert.ToInt32(data["y"]);
        int k = 1;
        if (playerMove.GetName() == "Player 2" && partyMain.makeMove(playerMove.GetFishka(), x, y))
        {
            board = partyMain.GetBoard();
            upDateBoard(board); 
            k = partyMain.GetValidMoves(playerMove.GetFishka()).Count;  
            playerMove = partyMain.changePlayer(playerMove.GetFishka());
        }
        isEndParty(k);
    }
    public void buttonNewGame_Click()
    {
        partyMain = new Party();
        startParty();
    }
    public void Winner() 
    {
        int pl1Score = partyMain.GetPlayer1().GetScore(),
            pl2Score = partyMain.GetPlayer2().GetScore();

        photonView = PhotonView.Get(this);
        if (pl1Score > pl2Score)
        {
            string moveText = "Winner " + partyMain.GetPlayer1().GetName();
            string statusText = pl1Score.ToString() + " vs " + pl2Score.ToString();

            UpdateMessage(moveText, statusText); 

            Dictionary<string, string> dataToShare = new Dictionary<string, string>
            {
                { "moveText", moveText },
                { "statusText", statusText }
            };
            photonView.RPC(nameof(InitializeRPC), RpcTarget.Others, dataToShare); 
        }
        else if (pl1Score < pl2Score)
        {
            string moveText = "Winner " + partyMain.GetPlayer2().GetName();
            string statusText = pl1Score.ToString() + " vs " + pl2Score.ToString();
            UpdateMessage(moveText, statusText);

            Dictionary<string, string> dataToShare = new Dictionary<string, string>
            {
                { "moveText", moveText },
                { "statusText", statusText }
            };
            photonView.RPC(nameof(InitializeRPC), RpcTarget.Others, dataToShare); 
        }
        else
        {
            string moveText = "Not Winner";
            string statusText = pl1Score.ToString() + " vs " + pl2Score.ToString();
            UpdateMessage(moveText, statusText); 

            Dictionary<string, string> dataToShare = new Dictionary<string, string>
            {
                { "moveText", moveText },
                { "statusText", statusText }
            };
            photonView.RPC(nameof(InitializeRPC), RpcTarget.Others, dataToShare); 
        }
    }
    void destroyTokens()
    {
        for (int i = 0; i < 8; ++i)
            for (int j = 0; j < 8; ++j)
                if (tokens[i, j] != null)
                    PhotonNetwork.Destroy(tokens[i, j]);
    }
    void upDateBoard(char[,] board)
    {
        destroyTokens();
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                if (board[i, j] == 'w')
                    tokens[i, j] = PhotonNetwork.Instantiate(tokenObjWhite.name,
                        new Vector3(i + 0.5f, -j - 0.5f, 0),
                        Quaternion.identity);
                else if (board[i, j] == 'b') 
                    tokens[i, j] = PhotonNetwork.Instantiate(tokenObjBlack.name,
                        new Vector3(i + 0.5f, -j - 0.5f, 0),
                        Quaternion.identity);
            }
        }
        Debug.Log("Board updated");
    }
    private void UpdateMessage(string move, string status)
    {
        messageMove.text = move;
        messageStatus.text = status;
    }
}
