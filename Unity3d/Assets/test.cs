using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;


public class test : MonoBehaviour
{
    // Start is called before the first frame update

    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    Vector3 pos = Vector3.zero;
    Vector4 ors = Vector3.zero;
    Vector3 startPos;
    
    int[] data_1;
    bool running;
    private Quaternion inverseQt;
    private Quaternion rawQt;
    private Quaternion myFixedQuaternion;
private void Update()
      {
        
        
        transform.position = startPos+pos;
        rawQt = new Quaternion(ors[1], ors[2], -ors[0], -ors[3]);
        
        transform.rotation = rawQt;
        //Quaternion target = Quaternion.Euler(ors[2],ors[0],ors[2]);
        //transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 2.0f);
        
      }
        
      private void Start()
      {
          
          ThreadStart ts = new ThreadStart(GetInfo);
          mThread = new Thread(ts);
          mThread.Start();
          inverseQt = Quaternion.identity;
          startPos = transform.position;
      }

      public static string GetLocalIPAddress()
      {
          var host = Dns.GetHostEntry(Dns.GetHostName());
          foreach (var ip in host.AddressList)
          {
              if (ip.AddressFamily == AddressFamily.InterNetwork)
              {
                  return ip.ToString();
              }
          }
          throw new System.Exception("No network adapters with an IPv4 address in the system!");
   }
      void GetInfo()
      {
          localAdd = IPAddress.Parse(connectionIP);
          listener = new TcpListener(IPAddress.Any, connectionPort);
          listener.Start();

          client = listener.AcceptTcpClient();
    

          running = true;
          while (running)
          {
              Connection();
          }
          listener.Stop();
      }

      void Connection()
      {
          NetworkStream nwStream = client.GetStream();
          byte[] buffer = new byte[client.ReceiveBufferSize];

          int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
          string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
          
          if (dataReceived != null)
          {

              
              
              if (dataReceived.StartsWith("s"))
              {
                    print("moved");
                    dataReceived = dataReceived.Remove(0,1);
                    Debug.Log(dataReceived);

                    string[] sArray = dataReceived.Split(',');
                
                    // store as a Vector3
                    Vector3 result = new Vector3(
                        float.Parse(sArray[0]),
                        float.Parse(sArray[1]),
                        float.Parse(sArray[2])
                        );
                    Vector4 result2 = new Vector4(
                        float.Parse(sArray[3]),
                        float.Parse(sArray[4]),
                        float.Parse(sArray[5]),
                        float.Parse(sArray[6])
                        );
                    pos = new Vector3(result[0]*0.008f , result[1]*0.008f , -result[2]*0.05f);
                    ors = result2;
                   
                    Debug.Log(pos);
                    Debug.Log(ors);
                    nwStream.Write(buffer, 0, bytesRead);
              }
              
              
              
          }
      }

      public static Vector3 StringToVector3(string sVector)
      {
          Debug.Log(sVector);
          string[] sArray = sVector.Split(',');
        
          // store as a Vector3
          Vector3 result = new Vector3(
              float.Parse(sArray[0]),
              float.Parse(sArray[1]),
              float.Parse(sArray[2])
              );
          Vector3 result2 = new Vector3(
              float.Parse(sArray[3]),
              float.Parse(sArray[4]),
              float.Parse(sArray[5])
              );
          
          return result;
      }
      public static Vector3 StringToOrein3(string sVector)
      {
          // Remove the parentheses

          if (sVector.StartsWith("(") && sVector.EndsWith(")"))
          {
              sVector = sVector.Substring(2, sVector.Length -1);
          }
    
          // split the items
          
          string[] sArray = sVector.Split(',');

          // store as a Vector3
          Vector3 result = new Vector3(
              float.Parse(sArray[0]),
              float.Parse(sArray[1]),
              float.Parse(sArray[2]));
            
          return result;
      }
  }