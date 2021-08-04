import numpy as np
import cv2
from tkinter import *
from PIL import Image,ImageTk
import os
import json



# print("Current Working Directory " , os.getcwd())

def nothing(x):
    pass

def HSV_setting():
    index = int(CamEntry.get())
    CamnumLabel2.config(text =index)
    # index= int(index)
    
    cv2.namedWindow("Tracking")
    
    cv2.createTrackbar("LH", "Tracking", 0, 255, nothing)
    cv2.createTrackbar("LS", "Tracking", 0, 255, nothing)
    cv2.createTrackbar("LV", "Tracking", 0, 255, nothing)
    cv2.createTrackbar("UH", "Tracking", 255, 255, nothing)
    cv2.createTrackbar("US", "Tracking", 255, 255, nothing)
    cv2.createTrackbar("UV", "Tracking", 255, 255, nothing)

    video_capture_0 = cv2.VideoCapture(index)

    while True:
        # Capture frame-by-frame
        ret0, frame0 = video_capture_0.read()
        

        hsv = cv2.cvtColor(frame0, cv2.COLOR_BGR2HSV)

        l_h = cv2.getTrackbarPos("LH", "Tracking")
        l_s = cv2.getTrackbarPos("LS", "Tracking")
        l_v = cv2.getTrackbarPos("LV", "Tracking")

        u_h = cv2.getTrackbarPos("UH", "Tracking")
        u_s = cv2.getTrackbarPos("US", "Tracking")
        u_v = cv2.getTrackbarPos("UV", "Tracking")

        l_b = np.array([l_h, l_s, l_v])
        u_b = np.array([u_h, u_s, u_v])

        mask = cv2.inRange(hsv, l_b, u_b)

        res = cv2.bitwise_and(frame0, frame0, mask=mask)

        cv2.imshow('Cam', frame0)
        cv2.imshow("mask", mask)
        cv2.imshow("res", res)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break
    
    cv2.destroyAllWindows()
    Label_lower2.config(text =l_b)
    Label_upper2.config(text =u_b)



def ListCamera():
    index = 0
    arr = []
    while True:
        cap = cv2.VideoCapture(index)
        if not cap.read()[0]:
            break
        else:
            arr.append(index)
        cap.release()
        index += 1
    print(arr)
    return arr

def list_dev():
    inp =   str(ListCamera())
    lbl.config(text = "device found :"+inp)

def write_log():
    index = 1+int(CamnumLabel2["text"])
    value = Label_lower2["text"]+Label_upper2["text"]
    with open("cam_log.txt",'r') as f:
        get_all=f.readlines()

    with open("cam_log.txt",'w') as f:
        for i,line in enumerate(get_all,1):         ## STARTS THE NUMBERING FROM 1 (by default it begins with 0)    
            if i == index:                              ## OVERWRITES line:2
                f.writelines(str(value)+"\n")
            else:
                f.writelines(line)
def text_to_array(a):
    a = np.fromstring(a[1:-1], dtype=np.int, sep=' ')
    return a

def log_json_hsv():
    d_log = json.load(open("cam_log.txt"))
    data_lower = Label_lower2["text"]
    data_lower = text_to_array(data_lower)
    data_lower = data_lower.tolist()
    # print(data_lower)
    data_upper = Label_upper2["text"]
    data_upper = text_to_array(data_upper)
    data_upper = data_upper.tolist()
    # print(d_log)

    d_log[CamnumLabel2["text"]] = [data_lower,data_upper]
    # print(d_log)
    json.dump(d_log, open("cam_log.txt",'w'))



def log_index():
    d_log = json.load(open("cam_log.txt"))
    index = CamEntry_index.get()
    index = index.split(',')
    arr=[]
    for i in index:
        arr.append(int(i))
    d_log["cam"] = arr

    json.dump(d_log, open("cam_log.txt",'w'))




############### tk gui ########################
root = Tk()
root.geometry("320x380")
root.title("Settings") 





CheckButton = Button(root,text = "List device", command = list_dev)
CheckButton.place(x=20, y=10)
lbl = Label(root, text = "")
lbl.place(x=125, y=10)
CamLabel = Label(root,text="VideoCapture:")
CamLabel.place(x=20, y=50)

CamEntry = Entry(root)
CamEntry.place(x=125, y=50)

CamnumLabel = Label(root,text="Camera :")
CamnumLabel.place(x=20, y=80)
CamnumLabel2 = Label(root)
CamnumLabel2.place(x=125, y=80)

detectButton = Button(root,text="Detect",command = HSV_setting)
detectButton.place(x=120, y=110)
T1=Label(root,text="press Q for quit")
T1.place(x=105, y=140)




Label_lower = Label(root, text = "Lower")
Label_lower.place(x=20, y=160)
Label_lower2 = Label(root)
Label_lower2.place(x=120, y=160)
Label_upper = Label(root, text = "Upper")
Label_upper.place(x=20, y=190)
Label_upper2 = Label(root)
Label_upper2.place(x=120, y=190)



SaveButton = Button(root,text="SaveHSV",command = log_json_hsv)
SaveButton.place(x=115, y=220)

Label_camera_index  = Label(root,text="Index Camera :")
Label_camera_index.place(x=20, y=260)
CamEntry_index = Entry(root)
CamEntry_index.place(x=120, y=260)
Label_camera_exm = Label(root,text="Example : 0,1 -->[left , right]")
Label_camera_exm.place(x=120, y=280)

SaveButton = Button(root,text="SaveIndex",command = log_index)
SaveButton.place(x=115, y=320)


root.mainloop()

# while(1):
#     ret0, frame0 = video_capture_0.read()
#     cv2.imshow("mask", frame0)
    
#     if cv2.waitKey(1) & 0xFF == ord('q'):
#         break
    
# video_capture_0.release()
# cv2.destroyAllWindows()