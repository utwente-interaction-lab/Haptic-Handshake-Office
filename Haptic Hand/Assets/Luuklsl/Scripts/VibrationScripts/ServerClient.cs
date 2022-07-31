/*
** Thanks to  https://gist.github.com/danielbierwirth/0636650b005834204cb19ef5ae6ccedb
 * And Daniel Davison for pointing me at this.
 *
 * Furthermore, thanks to Rolf van Kleef for helping me make this work
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace Luuklsl.Scripts.VibrationScripts
{
    public class ServerClient : MonoBehaviour
    {
        #region private members

        /// <summary> 	
        /// TCPListener to listen for incomming TCP connection 	
        /// requests. 	
        /// </summary> 	
        private TcpListener tcpListener;

        /// <summary> 
        /// Background thread for TcpServer workload. 	
        /// </summary> 	
        private Thread tcpListenerThread;

        /// <summary> 	
        /// Create handle to connected tcp client. 	
        /// </summary> 	
        private TcpClient connectedTcpClient;

        // private NetworkStream stream;

        #endregion


        [Header("IP")] [Tooltip("IP to be connected to, is given in the ESP32 module when connected via USB")]
        public string ipAddress;

        [Header("Ports")]
        // [Tooltip("Opposite of the settings in the ESP32 module")]
        // // public int sendPort = 7501;
        [Tooltip("Opposite of the settings in the ESP32 module")]
        public int connectionPort = 7600;


        // Start is called before the first frame update
        void Start()
        {
            // Start TcpServer background thread 		
            tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
            tcpListenerThread.IsBackground = true;
            tcpListenerThread.Start();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                SendMessage("{'m1Intensity': 120,'m2Intensity': 120,'m3Intensity':0,'m4Intensity':0} \n");
            }
        }

        private void ListenForIncomingRequests()
        {
            try
            {
                // Create listener on localhost port 8052. 			
                tcpListener = new TcpListener(IPAddress.Any, connectionPort);
                tcpListener.Start();
                Debug.Log("Vibration Message Server is listening");
                Byte[] bytes = new Byte[1024];
                while (true)
                {
                    connectedTcpClient = tcpListener.AcceptTcpClient();
                    //check if tcp-client has set ip address
                    Thread listener = new Thread(ReceiveMessages);
                    listener.IsBackground = true;
                    listener.Start();
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("SocketException " + socketException.ToString());
            }
        }


        private void ReceiveMessages()
        {
            // Get a stream object for reading 		
            Byte[] bytes = new Byte[1024];
            NetworkStream stream = connectedTcpClient.GetStream();
            int length;
            // Read incoming stream into byte array. 						
            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                var incomingData = new byte[length];
                Array.Copy(bytes, 0, incomingData, 0, length);
                // Convert byte array to string message. 							
                string clientMessage = Encoding.ASCII.GetString(incomingData);
                Debug.Log("client message received as: " + clientMessage);
            }
        }

        public void SendServerMessage(Dictionary<String, int> serverMessage)
        {
            Debug.Log(JsonConvert.SerializeObject(serverMessage));

            if (connectedTcpClient == null)
            {
                return;
            }

            try
            {
                // Get a stream object for writing. 			
                NetworkStream stream = connectedTcpClient.GetStream();
                if (stream.CanWrite)
                {
                    // string sendMessage = "{'m1Intensity':255,'m2Intensity':240,'m3Intensity':230,'m4Intensity':200} \n";
                    StringBuilder messageBuilder = new StringBuilder();
                    messageBuilder.Append("{");
                    foreach (KeyValuePair<string, int> messagePart in serverMessage)
                    {
                        messageBuilder.Append("'" + messagePart.Key + "'");
                        messageBuilder.Append(":");
                        messageBuilder.Append(messagePart.Value);
                        messageBuilder.Append(",");
                    }

                    messageBuilder.Remove(messageBuilder.Length - 1, 1);
                    messageBuilder.Append("} " + "\n");

                    string sendMessage = messageBuilder.ToString();


                    // Convert string message to byte array.                 
                    byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(sendMessage);
                    // Write byte array to socketConnection stream.               
                    stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                    // Debug.Log("Server sent his message - should be received by client");           
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
        }


        private void OnApplicationQuit()
        {
            Dictionary<String, int> message = new Dictionary<string, int>();
            message.Add("m1Intensity", 0);
            message.Add("m2Intensity", 0);
            message.Add("m3Intensity", 0);
            message.Add("m4Intensity", 0);
            // string message = "{'m1Intensity':0,'m2Intensity':0," +
            // "'m3Intensity':0,'m4Intensity':0} \n";
            message["m1Intensity"] = 0;
            message["m2Intensity"] = 0;
            message["m3Intensity"] = 0;
            message["m4Intensity"] = 0;


            this.SendServerMessage(message);
        }
    }
}
