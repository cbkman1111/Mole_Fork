using Common.Global.Singleton;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Network
{
    public class NetworkManager : MonoSingleton<NetworkManager>
    {
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;

        private Thread threadRead;


        protected override bool Init()
        {
            client = null;
            reader = null;
            writer = null;
            threadRead = null;

            return true;
        }

        public void Connect()
        {
            string ip = "127.0.0.1";
            int port = 8000;

            if (client != null && client.Connected)
            {
                Debug.Log($"{Tag} - client is already connected");
                return;
            }

            client = new TcpClient();
            client.BeginConnect(ip, port, OnConnect, client);
        }

        public void OnConnect(IAsyncResult result)
        {
            try
            {
                client.EndConnect(result);
                if (!client.Connected)
                {
                    Connect();
                    Debug.Log($"{Tag} - client is not connected");
                    return;
                }

                var stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
                threadRead = new Thread(Read);
                threadRead.Start();
            }
            catch (Exception e)
            {
                Debug.Log($"{Tag} - OnConnect error: {e.Message}");
            }
        }

        public void Disconnect()
        {
            if (client != null && client.Connected)
            {
                client.Close();
            }

            if (reader != null)
            {
                reader.Close();
                reader = null;
            }

            if (writer != null)
            {
                writer.Close();
                writer = null;
            }

            if (threadRead != null)
            {
                threadRead.Abort();
                threadRead = null;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void Read()
        {
            while (true)
            {
                try
                {
                    var message = reader.ReadLine();
                    Debug.Log(message);
                }
                catch (Exception e)
                {
                    Debug.Log($"{Tag} - Error reading message: " + e.Message);
                    break;
                }
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="message"></param>
        private void Write(string message)
        {
            writer.WriteLine(message);
            writer.Flush();
        }

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {
            if (client == null || !client.Connected)
            {
                Debug.Log($"{Tag} - client is not connected");
                return;
            }


        }
    }
}
