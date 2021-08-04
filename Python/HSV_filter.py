import sys
import cv2
import numpy as np
import time
import json

def add_HSV_filter(frame, camera):
    d_log = json.load(open("cam_log.txt"))
    for key ,value in d_log.items():
        # print(key,value)
        if key == str(camera):
            
            hsv_c = value
            # print(hsv_c)
    l_b = np.array(hsv_c[0])
    u_b = np.array(hsv_c[1])

	# Blurring the frame
    blur = cv2.GaussianBlur(frame,(5,5),0) 

    # Converting RGB to HSV
    hsv = cv2.cvtColor(blur, cv2.COLOR_BGR2HSV)

    # l_b_r = np.array([0, 0, 70])        # Lower limit for red ball
    # u_b_r = np.array([65, 255, 255])       # Upper limit for red ball
    # l_b_l = np.array([0, 0, 70])        # Lower limit for red ball
    # u_b_l = np.array([65, 255, 255])       # Upper limit for red ball

	#l_b = np.array([140, 106, 0])        # LOWER LIMIT FOR BLUE COLOR!!!
	#u_b = np.array([255, 255, 255])

	# HSV-filter mask
	#mask = cv2.inRange(hsv, l_b_l, u_b_l)

    # if(camera == 1):
    #     mask = cv2.inRange(hsv, l_b_r, u_b_r)
    # else:
    #     mask = cv2.inRange(hsv, l_b_l, u_b_l)

    mask = cv2.inRange(hsv, l_b, u_b)
    # Morphological Operation - Opening - Erode followed by Dilate - Remove noise
    mask = cv2.erode(mask, None, iterations=2)
    mask = cv2.dilate(mask, None, iterations=2)

    return mask