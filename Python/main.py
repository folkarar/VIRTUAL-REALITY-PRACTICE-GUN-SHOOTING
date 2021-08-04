import cv2
import numpy as np
import os

import imutils



import socket
import serial 
import time
import json

import HSV_filter as hsv
import shape_recognition as shape
import triangulation as tri

serialPort = serial.Serial(port='COM7', baudrate=115200, timeout=0, parity=serial.PARITY_EVEN, stopbits=1)
size = 1024

host , port = "127.0.0.1",25001
data = "s"
""" data = "s0,0,359.81,359.09,-10,0" """
sock = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
sock.connect((host,port))


    
# posi = 'x,y,z'-------------------------------------------------------------------------------------------------------------------------------------

d_log = json.load(open("cam_log.txt"))

cap_i = d_log["cam"]





cap_right = cv2.VideoCapture(int(cap_i[1]), cv2.CAP_DSHOW)                    
cap_left =  cv2.VideoCapture(int(cap_i[0]), cv2.CAP_DSHOW)


depth = 0
frame_rate = 30    

B = 5               #Distance between the cameras [cm]
f = 3               #Camera lense's focal length [mm]
alpha = 42        #Camera field of view in the horisontal plane [degrees]


#Initial values
count = -1
xy_p = [0,0]
posi=""
while(True):
    count += 1

    ret_right, frame_right = cap_right.read()
    ret_left, frame_left = cap_left.read()



    # If cannot catch any frame, break
    if ret_right==False or ret_left==False:      
        # print("bye")              
        break

    else:
        # APPLYING HSV-FILTER:
        mask_right = hsv.add_HSV_filter(frame_right, 1)
        mask_left = hsv.add_HSV_filter(frame_left, 0)

        # Result-frames after applying HSV-filter mask
        res_right = cv2.bitwise_and(frame_right, frame_right, mask=mask_right)
        res_left = cv2.bitwise_and(frame_left, frame_left, mask=mask_left) 

        # APPLYING SHAPE RECOGNITION:
        circles_right = shape.find_circles(frame_right, mask_right)
        circles_left  = shape.find_circles(frame_left, mask_left)
        xy=circles_left
        # print(xy)
        if xy != None:
            xy_p = xy
        else:
            xy= xy_p
        # If no ball can be caught in one camera show text "TRACKING LOST"
        if np.all(circles_right) == None or np.all(circles_left) == None:
            pass

        else:
            
            depth = tri.find_depth(circles_right, circles_left, frame_right, frame_left, B, f, alpha)
            
            print("Depth: ", depth)                                            
              
        time.sleep(0.01)
        posi =str(int(xy[1]-320))+","+str(int(240-xy[0]))+","+str(int(depth))
        posi = "0,0,0"
        print(posi)
        data="s"
        serialPort.flushInput()
        time.sleep(.01)
        ble = serialPort.readline(size)
        
        print(ble)
        if ble :
            print("data :",ble,"len",len(ble))
            if ble !=b'\r\n':
                data += posi+","+ble.decode("utf-8")
                print(data)
                sock.sendall(data.encode("utf-8"))
                data = sock.recv(1024).decode("utf-8") 
        # ble = ",0,"+"0,"+"0,"+"0"
        # data += posi+ble
        # print(data)
        # sock.sendall(data.encode("utf-8"))
        # data = sock.recv(1024).decode("utf-8") 
       


cap_right.release()
cap_left.release()

cv2.destroyAllWindows()


  
    
            
    
