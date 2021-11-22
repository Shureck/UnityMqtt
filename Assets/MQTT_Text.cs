using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
public class MQTT_Text : MonoBehaviour
{

    // Start is called before the first frame update
    public TextMeshPro textOBJ;
    string clientId;
    public MqttClient client;
    public string receive_message = "";
    public string subscribe_chanel = "CaseMQTT/groups/Case/json";
    bool toogle = false;
    public Button yourButton;
    [Serializable]
    class CaseObj
    {
        public Feeds feeds;
    }
    [Serializable]
    class Feeds
    {
        public int temp;
        public int hum;
    }

    public void onConnect()
    {
        string txtIP = "io.adafruit.com";
        string txtPort = "1883";
        string clientId = "56a4s6d49q5wd6sad";
        //The server default password is this
        string username = "CaseMQTT";
        string password = "aio_DJqU63eBUfHi9tVvlHVqt63UEFKZ";
        client = new MqttClient("io.adafruit.com", int.Parse(txtPort), false, null);

        client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        client.MqttMsgSubscribed += Client_MqttMsgSubscribed;
        client.Connect(clientId, username, password);

        if (client != null && subscribe_chanel != "")
        {
            client.Subscribe(new string[] { subscribe_chanel }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }
    }

    private void Client_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
    {
        string message = System.Text.Encoding.UTF8.GetString(e.Message);
        string topic = e.Topic;
        receive_message = message;
        Debug.Log("Received message is" + message+" "+topic);
    }

    private void Client_MqttMsgSubscribed(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgSubscribedEventArgs e)
    {
        Debug.Log("Subscribe" + e.MessageId);
    }

    void TaskOnClick()
    {
        toogle = !toogle;
        if (toogle)
        {
            client.Publish("CaseMQTT/groups/Case/json", Encoding.ASCII.GetBytes("{\"feeds\":{\"toggle\":\"ON\"}}"), 0, false);
        }
        else
        {
            client.Publish("CaseMQTT/groups/Case/json", Encoding.ASCII.GetBytes("{\"feeds\":{\"toggle\":\"OFF\"}}"), 0, false);
        }
        Debug.Log("You have clicked the button! "+toogle);
    }

    void Start()
    {
        textOBJ.text = "Ожидаем данные";
        onConnect();
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        if (receive_message != "")
        {
            CaseObj caseObj = new CaseObj();
            caseObj = JsonUtility.FromJson<CaseObj>(receive_message);
            //JSONNode jsonNode = SimpleJSON.JSON.Parse(jsonData);
            if (caseObj.feeds.temp != 0 && caseObj.feeds.hum != 0){
                textOBJ.text = "Температура: " + caseObj.feeds.temp + "\n" + "Влажность: " + caseObj.feeds.hum;
            }
            receive_message = "";
        }
    }
}
