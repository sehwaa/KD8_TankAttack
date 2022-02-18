using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public TMP_Text roomInfo;
    public TMP_Text chatMsgList;
    public TMP_InputField msg_IF;
    private PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();

        List<Transform> points = new List<Transform>();
        GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>(points);

        int idx = Random.Range(1, points.Count);

        Vector3 pos = points[idx].position;

        PhotonNetwork.Instantiate("Tank",
                                  pos,
                                  Quaternion.identity,
                                  0);

        DisplayRoomInfo();
    }

    void DisplayRoomInfo()
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;

        string msg = $"{currentRoom.Name} (<color=#ff0000>{currentRoom.PlayerCount}</color>/<color=#00ff00>{currentRoom.MaxPlayers}</color>)";
        roomInfo.text = msg;

        // string msg1 = $"[0] (<color=#ff0000>[1]</color>/<color=#00ff00>[2]</color>)";
        // roomInfo.text = string.Format(msg,
        //                               currentRoom.Name,
        //                               currentRoom.PlayerCount,
        //                               currentRoom.MaxPlayers);
    }

    public void OnExitButtonClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnSendButtonClick()
    {
        string msg = $"<color=#00ff00>[{PhotonNetwork.NickName}]</color> {msg_IF.text}";
        pv.RPC("ChatMessage", RpcTarget.AllBufferedViaServer, msg);

        msg_IF.text = "";
    }

    [PunRPC]
    public void ChatMessage(string msg)
    {
        chatMsgList.text += msg + "\n";
    }

    // Client's Object Cleanup 호출되는 콜백
    public override void OnLeftRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        DisplayRoomInfo();
        string msg = $"<color=#00ff00>[{newPlayer.NickName}]</color>님이 입장하셨습니다.";
        ChatMessage(msg);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        DisplayRoomInfo();
        string msg = $"<color=#ff0000>[{otherPlayer.NickName}]</color>님이 퇴장하셨습니다.";
        ChatMessage(msg);
    }


}
