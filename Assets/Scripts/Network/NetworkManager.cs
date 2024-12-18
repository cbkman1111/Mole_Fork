using Common.Global.Singleton;
using Common.Utils;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Network
{
    public class NetworkManager : MonoSingleton<NetworkManager>
    {
        private TcpClient tcp;
        private StreamReader reader;
        private StreamWriter writer;

        private Thread threadRead;

        public Action<IAsyncResult> OnConnectAction { get; set; }
        public Action OnDissConnectAction { get; set; }

        protected override bool Init()
        {
            tcp = null;
            reader = null;
            writer = null;
            threadRead = null;
            return true;
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        public void Connect()
        {
            string ip = "127.0.0.1";
            int port = 8000;

            if (tcp != null && tcp.Connected)
            {
                Debug.Log($"{Tag} - client is already connected");
                return;
            }

            tcp = new TcpClient();
            tcp.NoDelay = true;
            tcp.SendTimeout = 10000;
            tcp.ReceiveTimeout = 10000;
            tcp.BeginConnect(ip, port, OnConnect, tcp);
        }

        public void OnConnect(IAsyncResult result)
        {
            try
            {
                tcp.EndConnect(result);
                if (!tcp.Connected)
                {
                    Connect();
                    Debug.Log($"{Tag} - client is not connected");
                    return;
                }

                var stream = tcp.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
                threadRead = new Thread(Read);
                threadRead.Start();
            }
            catch (Exception e)
            {
                Debug.Log($"{Tag} - OnConnect error: {e.Message}");
            }
            finally
            {
                OnConnectAction?.Invoke(result);
            }
        }

        public void Disconnect()
        {
            if (tcp != null && tcp.Connected)
            {
                tcp.Close();
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

            OnDissConnectAction?.Invoke();
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
                    if (threadRead == null)
                        break;

                    var message = reader.ReadLine();
                    if (message == null)
                    {
                        GiantDebug.Log($"client is disconnected");
                        break;
                    }

                    Debug.Log(message);
                }
                catch (Exception e)
                {
                    GiantDebug.Log($"Error reading message: " + e.Message);
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
            if (tcp == null || !tcp.Connected)
            {
                Debug.Log($"{Tag} - client is not connected");
                return;
            }

            Write(message);
            //tcp.Client.Send(Encoding.ASCII.GetBytes(message));
        }
    }
}
