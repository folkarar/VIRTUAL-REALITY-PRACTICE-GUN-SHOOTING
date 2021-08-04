using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
public class Guncontroller : MonoBehaviour
{
     Thread mThread;
    public Text scoreText;
    public GameObject BulletHole ;
    public float distance = 10f;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    int testst = 0;
    int score1 = 0;
    int a= 5;
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
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    private Transform _Gun_model;
    [SerializeField]
    private GameObject _bulletHolePrefab;
    [SerializeField]
    Transform bulletSpawnPoint;
    [SerializeField]
    int magazineCapacity = 13;
    int bulletsInMagazine;
    [SerializeField]
    float fireRateDelay = 0.8f;
    [SerializeField]
    float reloadTime = 2f;
    bool canShoot = true;
    bool hasAmmo = true ;
    [SerializeField]
    bool automaticReload = false;
    [SerializeField]
    bool automaticWeapon = false;

    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    // private ParticleSystem muzzleFlash;
    // [SerializeField]
    private AudioSource shootSound;
    // Start is called before the first frame update



    void Start()
    {
        bulletsInMagazine = magazineCapacity;
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
        inverseQt = Quaternion.identity;
        startPos = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPos+pos;
        rawQt = new Quaternion(ors[1], ors[2], -ors[0], -ors[3]);
        
        transform.rotation = rawQt;
        if(Input.GetMouseButton(0) && automaticWeapon)
        {
            if(hasAmmo){

                if (canShoot){
                Shoot();
                }
            }
        }
        else if (Input.GetMouseButton(0)&& !automaticWeapon)
        {
            if(hasAmmo)
            {
                if(canShoot)
                {
                    Shoot();
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
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
                    pos = new Vector3(result[1]*0.005f , -result[0]*0.005f , 1-result[2]*0.0166f);
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
    
  
    void Shoot()
    {
        StartCoroutine(FireRateDelay());
        
        // Ray rayOrigin = mainCamera.ScreenPointToRay(bulletSpawnPoint.position);
        RaycastHit hitInfo;
        // muzzleFlash.Play();
        shootSound.Play();
        GameObject bullet =Instantiate(bulletPrefab,bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        if (Physics.Raycast(mainCamera.transform.position,mainCamera.transform.forward,out hitInfo , distance))
        {
            GameObject bH = Instantiate(BulletHole,hitInfo.point + new Vector3(0f,0f,-.02f),Quaternion.LookRotation(-hitInfo.normal));
            Target target = hitInfo.transform.GetComponent<Target>();
            //Debug.Log(hitInfo.transform.name);
            if (hitInfo.transform.name == "7cube")
            {
                score1+=7;
                
                
            }
            else if (hitInfo.transform.name == "8cube")
            {
                score1+=8;
            }
            else if (hitInfo.transform.name == "9cube")
            {
                score1+=9;
            }
            else if (hitInfo.transform.name == "10cube")
            {
                score1+=10;
            }
            else if (hitInfo.transform.name == "Cylinder7")
            {
                score1+=7;
            }
            else if (hitInfo.transform.name == "Cylinder8")
            {
                score1+=8;
            }
            else if (hitInfo.transform.name == "Cylinder9")
            {
                score1+=9;
            }
            else if (hitInfo.transform.name == "Cylinder10")
            {
                score1+=10;
            }
            scoreText.text = "Score: "+score1.ToString();
            Debug.Log(score1);
        }
        
        bulletsInMagazine--;
        
        if(bulletsInMagazine <=0)
        {
            hasAmmo = false;
            if(automaticReload)
                Reload();
        }
    }
    void Reload()
    {
        StartCoroutine(ReloadDelay());
    }

   


    IEnumerator FireRateDelay()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireRateDelay);
        canShoot = true;
       
    }
     IEnumerator ReloadDelay()
    {
        canShoot = false;
        yield return new WaitForSeconds(reloadTime);
        bulletsInMagazine = magazineCapacity;
        hasAmmo = true;
        canShoot = true;
       
    }
}

