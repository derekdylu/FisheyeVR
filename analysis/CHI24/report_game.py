#!/usr/bin/env python
# coding: utf-8

# # Test Report (for Integrated Game)

# ## Preprocess

# ### Packages and Constants

# In[52]:


import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import math
import datetime
from pyquaternion import Quaternion
import plotly.express as px
from ipywidgets import interact, interactive, fixed, interact_manual
import ipywidgets as widgets
import argparse


# In[53]:


config_list = [
  'startedTime',
  'user',
  'stage',
  'beginScore',
  'mode',
  'fov',
  'method',
  'interaxial',
  'sensitivity',
  'frameLength',
]

result={
  'method': 'na',
  'stage': 'na',
  'user': 'na',
  'score': -1,
  'widePortion': -1,
  'transitionPortion': -1,
  'transitionCount': -1,
  'head_qu_distance': -1,
  'head_qu_mean_velocity': -1,
  'head_qu_mean_acc': -1,
  'rightHand_qu_distance': -1,
  'rightHand_qu_mean_velocity': -1,
  'rightHand_qu_mean_acc': -1,
  'leftHand_qu_distance': -1,
  'leftHand_qu_mean_velocity': -1,
  'leftHand_qu_mean_acc': -1,
  'player_mean_velocity': -1,
  'player_mean_acc': -1,
}


# ### Only For Packaging

# In[54]:


parser = argparse.ArgumentParser(
  description="Generate metrics from one private, pseudonymized FisheyeVR game log"
)
parser.add_argument(
  '--file',
  type=str,
  required=True,
  help='Path to a locally stored, pseudonymized experiment log',
)
args = parser.parse_args()

# Access the arguments using args.file


# In[55]:


# !jupyter nbconvert --to script report_game.ipynb


# ### Utilities

# In[56]:


############### General ###############
def remove_outlier (arr):
  mean = np.mean(arr, axis=0)
  std = np.std(arr, axis=0)
  final_list = [x for x in arr if (x > mean - 2 * std)]
  final_list = [x for x in final_list if (x < mean + 2 * std)]
  return final_list

############### Euler ###############
def euler_diff (arr, time):
  diff = []
  for i in range(len(arr)-1):
    diff.append((np.array(arr[i+1])-np.array(arr[i]))/time)
  return diff

def euler_diff_cir (arr, time):
  diff = []
  for i in range(len(arr)-1):
    diff.append(euler_diff_cir_array(arr[i], arr[i+1])/time)
  return diff

def euler_diff_cir_array (arr1, arr2):
  res = np.array([0,0,0])
  if (len(arr1) != len(arr2)):
    print("Error: length of two arrays are not equal")
    return
  for i in range(len(arr1)):
    res[i] = euler_diff_cir_element(arr1[i], arr2[i])
  return res

def euler_diff_cir_element (a, b):
  if (b - a > 180):
    return (b-360)-a
  elif (b - a < -180):
    return b-(a-360)
  else:
    return b-a

def euler_distance (arr, threshold = 0):
  sum = 0
  for i in range(len(arr)-1):
    if abs(arr[i+1]-arr[i]) > threshold:
      sum += abs(arr[i+1]-arr[i])
  return sum

def euler_circle (arr):
  circle = []
  for i in range(len(arr)):
    if (arr[i] > 180):
      circle.append(arr[i]-360)
    else:
      circle.append(arr[i])
  return circle

def euler_negative (arr):
  neg = []
  for i in range(len(arr)):
    neg.append(-arr[i])
  return neg

############### Quaternion ###############
def quaternion_distance (arr, threshold = 0):
  sum = 0
  for i in range(len(arr)-1):
    if abs(Quaternion.absolute_distance(Quaternion(arr[i]), Quaternion(arr[i+1]))) > threshold:
      sum += Quaternion.absolute_distance(Quaternion(arr[i]), Quaternion(arr[i+1]))
  return sum

def quaternion_diff_abs (arr, time):
  diff = []
  for i in range(len(arr)-1):
    diff.append(Quaternion.absolute_distance(Quaternion(arr[i]), Quaternion(arr[i+1]))/time)
  return diff

def quaternion_diff (arr, time):
  diff = []
  for i in range(len(arr)-1):
    diff.append(Quaternion.distance(Quaternion(arr[i]), Quaternion(arr[i+1]))/time)
  return diff


# ### Data Input

# In[60]:


config = {}
headAngle_qu = np.array([[0,0,0,0]])
headAngle_eu = np.array([[0,0,0]])
rightHandPosition = np.array([[0,0,0]])
leftHandPosition = np.array([[0,0,0]])
rightHandAngle_qu = np.array([[0,0,0,0]])
leftHandAngle_qu = np.array([[0,0,0,0]])
rightHandAngle_eu = np.array([[0,0,0]])
leftHandAngle_eu = np.array([[0,0,0]])
position = np.array([[0,0,0]])
unstruct_tags = []
unstruct_zoom_tags = []
duration = 0.0

completeFileName = args.file

f = open(completeFileName, 'r')
count = 0
frameCount = 0
for i, line in enumerate(f):
  if i < len(config_list) and i == count:
    if (line.find("startedTime") == 0):
      startedTime = datetime.datetime.strptime(line[line.find(":")+1:].strip('\n'), '%m/%d/%Y %I:%M:%S %p')
  if i >= len(config_list) and i == count:
    if (line.find("endedTime") == 0):
      endedTime = datetime.datetime.strptime(line[line.find(":")+1:].strip('\n'), '%m/%d/%Y %I:%M:%S %p')
    if (line.find("*") != 0 and line.find("#") != 0):
      frameCount += 1
  count += 1

print("startedTime: ", startedTime)
print("endedTime: ", endedTime)
actualDuration = (endedTime - startedTime).total_seconds()
print("frameCount: ", frameCount)
print("actual duration: ", actualDuration, "seconds")
actualFrameRate = frameCount / actualDuration # in FPS
print("actual frameRate: ", actualFrameRate, "FPS")
turncatePoint = round(240 * actualFrameRate)
print("trucate point frame: ", turncatePoint)

f = open(completeFileName, 'r')
count = 0
frameCount = 0
for i, line in enumerate(f):
  if (frameCount > turncatePoint):
    break
  if (line == "===\n"):
    break
  if i < len(config_list) and i == count:
    config[config_list[count]] = line[line.find(":")+1:].strip('\n')
  if i >= len(config_list) and i == count:
    if (line.find("*") == 0):
      unstruct_tags.append(str(frameCount) + line.split("\n")[0])
    elif (line.find("#") == 0):
      unstruct_zoom_tags.append(str(frameCount) + line.split("\n")[0])
    else:
      frameCount += 1
      headAngle_qu = np.append(headAngle_qu, [[ float(line.split("%")[0][1:].strip(')').split(",")[0]),
                                                float(line.split("%")[0][1:].strip(')').split(",")[1]),
                                                float(line.split("%")[0][1:].strip(')').split(",")[2]),
                                                float(line.split("%")[0][1:].strip(')').split(",")[3]) ]],
                                                axis=0)

      headAngle_eu = np.append(headAngle_eu, [[ float(line.split("%")[1][1:].strip(')').split(",")[0]),
                                                float(line.split("%")[1][1:].strip(')').split(",")[1]),
                                                float(line.split("%")[1][1:].strip(')').split(",")[2]) ]],
                                                axis=0)

      rightHandPosition = np.append(rightHandPosition, [[ float(line.split("%")[2][1:].strip(')').split(",")[0]),
                                                          float(line.split("%")[2][1:].strip(')').split(",")[1]),
                                                          float(line.split("%")[2][1:].strip(')').split(",")[2]) ]],
                                                          axis=0)

      leftHandPosition = np.append(leftHandPosition, [[ float(line.split("%")[3][1:].strip(')').split(",")[0]),
                                                        float(line.split("%")[3][1:].strip(')').split(",")[1]),
                                                        float(line.split("%")[3][1:].strip(')').split(",")[2]) ]],
                                                        axis=0)

      rightHandAngle_qu = np.append(rightHandAngle_qu, [[ float(line.split("%")[4][1:].strip(')').split(",")[0]),
                                                          float(line.split("%")[4][1:].strip(')').split(",")[1]),
                                                          float(line.split("%")[4][1:].strip(')').split(",")[2]),
                                                          float(line.split("%")[4][1:].strip(')').split(",")[3]) ]],
                                                          axis=0)

      leftHandAngle_qu = np.append(leftHandAngle_qu, [[ float(line.split("%")[5][1:].strip(')').split(",")[0]),
                                                        float(line.split("%")[5][1:].strip(')').split(",")[1]),
                                                        float(line.split("%")[5][1:].strip(')').split(",")[2]),
                                                        float(line.split("%")[5][1:].strip(')').split(",")[3]) ]],
                                                        axis=0)

      rightHandAngle_eu = np.append(rightHandAngle_eu, [[ float(line.split("%")[6][1:].strip(')').split(",")[0]),
                                                          float(line.split("%")[6][1:].strip(')').split(",")[1]),
                                                          float(line.split("%")[6][1:].strip(')').split(",")[2]) ]],
                                                          axis=0)

      leftHandAngle_eu = np.append(leftHandAngle_eu, [[ float(line.split("%")[7][1:].strip(')').split(",")[0]),
                                                        float(line.split("%")[7][1:].strip(')').split(",")[1]),
                                                        float(line.split("%")[7][1:].strip(')').split(",")[2]) ]],
                                                        axis=0)

      position = np.append(position, [[ float(line.split("%")[8][1:].strip(')\n').split(",")[0]),
                                        float(line.split("%")[8][1:].strip(')\n').split(",")[1]),
                                        float(line.split("%")[8][1:].strip(')\n').split(",")[2]) ]],
                                        axis=0)
  count += 1

# clear initialized zeros
headAngle_qu = headAngle_qu[1:]
headAngle_eu = headAngle_eu[1:]
rightHandPosition = rightHandPosition[1:]
leftHandPosition = leftHandPosition[1:]
rightHandAngle_qu = rightHandAngle_qu[1:]
leftHandAngle_qu = leftHandAngle_qu[1:]
rightHandAngle_eu = rightHandAngle_eu[1:]
leftHandAngle_eu = leftHandAngle_eu[1:]
position = position[1:]
duration = (1/actualFrameRate) * frameCount # trucated duration

# check fov
if (config['mode'] == 'Static'):
  config.update({'method': 'N/A'})
  config.update({'sensitivity': 'N/A'})
  if (config['fov'] != 'Normal'):
    print("\n⚠️ STATIC FOV IS NOT NORMAL")
elif (config['mode'] == 'Dynamic'):
  config.update({'fov': 'N/A'})

# check duration
config.update({'frameLength': (1/actualFrameRate)})
config.update({'duration': duration}) # trucated duration

# check score
if (config['stage'] == '3 maze'):
  config.update({'score': duration})
else:
  config.update({'score': len(unstruct_tags)})
if (config['beginScore'] != '0 Pt'):
  print("\n⚠️ BEGIN SCORE IS NOT 0")

print("\nconfig", config)
print("\ndocument length", headAngle_qu.shape[0])
print("\nhead angle quaternion", headAngle_qu)
print("\nhead angle euler", headAngle_eu)
print("\nright hand pos", rightHandPosition)
print("\nleft hand pos", leftHandPosition)
print("\nright hand quaternion", rightHandAngle_qu)
print("\nleft hand quaternion", leftHandAngle_qu)
print("\nright hand ang", rightHandAngle_eu)
print("\nleft hand ang", leftHandAngle_eu)
print("\nposition", position)

### totlly 9 columns


# ## Report

# ### Data Check

# #### Configs

# In[61]:


for i, k in enumerate(config):
  print(k, ":", config[k])


# #### Tags

# In[62]:


tags = []

for i, k in enumerate(unstruct_tags):
  tags.append(int(k.split('*')[0]))

print(tags)

if (len(tags) != config['score'] and config['stage'] != '3 maze'):
  print("\n⚠️ TAGS LENGTH AND CONFIG SCORE IS NOT EQUAL")
else:
  print("\n✅ tags length ok")


# In[63]:


zoom_tags = []
zoom_tags_frame = set()

for i, k in enumerate(unstruct_zoom_tags):
  zoom_tags.append({"frame": int(k.split('#')[0]), "zoom_tag": k.split('#')[1]})
  zoom_tags_frame.add(int(k.split('#')[0]))

print(zoom_tags)
print(zoom_tags_frame)


# ##### Zoom Tags Group by Frame to Check Trigger Method Conflict

# In[64]:


# create zoom tag dataframe and combine those with same frame
if (len(zoom_tags) > 0):
  zoom_tags_df = pd.DataFrame(zoom_tags)
  zoom_tags_df = zoom_tags_df.groupby(['frame'], as_index=False).agg(lambda x: '/'.join(x.tolist()))

  # display(zoom_tags_df)


# In[65]:


targetCnt = 0
idleCnt = 0
handsCnt = 0
manualCnt = 0

for i, k in enumerate(zoom_tags):
  if (k['zoom_tag'].find('Ta') != -1):
    targetCnt += 1
  elif (k['zoom_tag'].find('Id') != -1):
    idleCnt += 1
  elif (k['zoom_tag'].find('Ha') != -1):
    handsCnt += 1
  elif (k['zoom_tag'].find('Ma') != -1):
    manualCnt += 1

print("Trigger Count")
print("Manual: ", manualCnt)
print("Target: ", targetCnt)
print("Idle: ", idleCnt)
print("Hands: ", handsCnt)


# ### FOV

# In[66]:


fov_series = []
curFov = 0
zoomTagFlag = 0
for i in range(turncatePoint + 1):
  if (i in zoom_tags_frame):
    while (zoom_tags[zoomTagFlag]['frame'] == i):
      if (zoom_tags[zoomTagFlag]['zoom_tag'].find('In') != -1):
        curFov -= 1
      elif (zoom_tags[zoomTagFlag]['zoom_tag'].find('Out') != -1):
        curFov += 1
      zoomTagFlag += 1
      if (zoomTagFlag == len(zoom_tags)):
        break
  fov_series.append(curFov)

fov_series_df = pd.DataFrame([list(range(turncatePoint + 1)), fov_series], index=['frame', 'fov']).T


# In[67]:


tags_arr = []
for i in range(fov_series_df.shape[0]):
    if i in tags:
        tags_arr.append("*")
    else:
        tags_arr.append("")

fov_series_df['tag'] = tags_arr


# In[68]:


if (len(zoom_tags) > 0):
  # combine fov_series_df with zoom_tags_df based on frame
  fov_series_df = fov_series_df.merge(zoom_tags_df, on='frame', how='left')
  # fill na with empty list
  fov_series_df = fov_series_df.fillna(value={'zoom_tag': ""})


# In[69]:


fig = px.line(fov_series_df, y='fov', x="frame", title="FOV Series by Score", text='tag')
fig.update_yaxes()
# fig.show()


# In[70]:


if (len(zoom_tags) > 0):
  fig = px.line(fov_series_df, y='fov', x="frame", title="FOV Series by Trigger", text='zoom_tag')
  fig.update_yaxes()
  # fig.show()


# In[71]:


if (config['mode'] == 'Dynamic'):
  wideCnt = 0
  for i, v in enumerate(fov_series):
    if (min(fov_series) == max(fov_series)):
      break
    if (v == max(fov_series)):
      wideCnt += 1

  widePortion = wideCnt / len(fov_series)
  print("Wide FOV portion: ", round(widePortion, 4)*100, "%")
  transPortion = len(zoom_tags_frame) / len(fov_series)
  print("Transition portion: ", round(transPortion, 4)*100, "%")

  result.update({'widePortion': widePortion})
  result.update({'transitionPortion': transPortion})


# In[72]:


if (len(zoom_tags) > 0) :
  curTag = zoom_tags[0]['zoom_tag']
  curFrame = zoom_tags[0]['frame']
  transCnt = 0
  for i, k in enumerate(zoom_tags):
    if (k['frame'] == curFrame):
      continue
    else:
      curFrame = k['frame']
      if (k['zoom_tag'] != curTag):
        print("Transition: ", curTag, " -> ", k['zoom_tag'])
        transCnt += 1
        curTag = k['zoom_tag']

  transCnt += 1
  print("\nTransition Count: ", transCnt)

  result.update({'transitionCount': transCnt})


# ### Head Direction

# #### Euler

# ##### Euler Head Angular Distance

# In[73]:


headAngle_distance_eu = [euler_distance(headAngle_eu[:, 0]), euler_distance(headAngle_eu[:, 1]), euler_distance(headAngle_eu[:, 2])]

th = 1
headAngle_distance_th_eu = [euler_distance(headAngle_eu[:, 0], th), euler_distance(headAngle_eu[:, 1], th), euler_distance(headAngle_eu[:, 2], th)]

print("Euler Head Angular Distance")
print("pitch distance:", round(headAngle_distance_eu[0], 2), "degrees", " |  thresholded:", round(headAngle_distance_th_eu[0], 2), "degrees")
print("yaw distance:", round(headAngle_distance_eu[1], 2), "degrees", " |  thresholded:", round(headAngle_distance_th_eu[1], 2), "degrees")
print("roll distance:", round(headAngle_distance_eu[2], 2), "degrees", " |  thresholded:", round(headAngle_distance_th_eu[2], 2), "degrees")


# ##### Euler Head Direction Distribution

# In[74]:


### for raw data checking

size = (2,2)

fig, ax = plt.subplots(figsize=size)
fig.subplots_adjust(bottom=0.2, left=0.2)
ax.hist(remove_outlier(headAngle_eu[:,1]), bins=100, label='x', range=[0, 360], color='slateblue')
ax.set_title('Euler Head Yaw Direction Distribution (raw)')
ax.set_xlabel('degree')
ax.set_ylabel('frequency')
plt.show()


# In[75]:


N = 180

direction = ['Pitch', 'Yaw', 'Roll']

for i in range(len(direction)):
  theta = np.linspace(0.0, 2 * np.pi, N, endpoint=False)
  radii = np.histogram(headAngle_eu[:,i], bins=N, range=[0, 360])[0]
  width = (2*np.pi) / N

  ax = plt.subplot(polar=True)
  bars = ax.bar(theta, radii, width=width, bottom=max(np.histogram(headAngle_eu[:,i], bins=N, range=[0, 360])[0])/3, color='slateblue')

  ax.set_title('Euler Head ' + direction[i] + ' Direction Distribution')
  ax.set_xlabel('degree [every ' + str(360/N) + r'$\degree$' + ']')
  ax.set_ylabel('frequency', rotation=0, labelpad=-50)

  plt.show()

  # 0 to 90 degrees means heads down


# ##### Euler Head Direction Series

# In[76]:


headAngle_eu_cir_df = pd.DataFrame([list(range(0, headAngle_eu[:,1].shape[0])), euler_circle(headAngle_eu[:,0]), euler_circle(headAngle_eu[:,1]), euler_circle(headAngle_eu[:,2])], index=['frame', 'pitch', 'yaw', 'roll']).T


# In[77]:


tags_arr = []
for i in range(headAngle_eu_cir_df.shape[0]):
    if i in tags:
        tags_arr.append("*")
    else:
        tags_arr.append("")

headAngle_eu_cir_df['tag'] = tags_arr


# In[78]:


fig = px.line(headAngle_eu_cir_df, y='yaw', x="frame", title="Euler Head Yaw Direction Series", text="tag")
fig.update_yaxes(range=[-180, 180])
# fig.show()


# ##### Eular Head Direction Velocity and Acceleration

# In[79]:


headAngle_velocity_eu = np.array(euler_diff_cir(headAngle_eu, float(config['frameLength'])))
print(headAngle_velocity_eu)


# In[80]:


headAngle_acc_eu = np.array(euler_diff(headAngle_velocity_eu, float(config['frameLength'])))
print(headAngle_acc_eu)


# ##### Euler Head Direction Velocity and Acceleration Series

# In[81]:


# axis = ['x (pitch)', 'y (yaw)', 'z (roll)']
# size = (5,3)

# for i in range(len(axis)):
#   fig, ax = plt.subplots(figsize=size)
#   fig.subplots_adjust(bottom=0.2, left=0.2)
#   ax.hist(remove_outlier(headAngle_velocity_eu[:,i]), bins=100, label='x', )
#   ax.set_title(axis[i] + ' axis head angle velocity distribution')
#   ax.set_xlabel('velocity [degree/s]')
#   ax.set_ylabel('frequency')
#   plt.show()

# for i in range(len(axis)):
#   fig, ax = plt.subplots(figsize=size)
#   fig.subplots_adjust(bottom=0.2, left=0.2)
#   ax.hist(remove_outlier(headAngle_acc_eu[:,i]), bins=100, label='x', color = "lightblue", )
#   ax.set_title(axis[i] +' axis head angle acceleration distribution')
#   ax.set_xlabel('acceleration [degree/s' + r'$^2$' + ']')
#   ax.set_ylabel('frequency')
#   plt.show()


# In[82]:


headAngle_velocity_eu_df = pd.DataFrame([list(range(0, headAngle_velocity_eu[:,1].shape[0])), euler_circle(headAngle_velocity_eu[:,0]), euler_circle(headAngle_velocity_eu[:,1]), euler_circle(headAngle_velocity_eu[:,2])], index=['frame', 'pitch', 'yaw', 'roll']).T
fig = px.line(headAngle_velocity_eu_df, y='yaw', x="frame", title="Euler Head Yaw Direction Velocity Series")
# fig.show()


# In[83]:


headAngle_acc_eu_df = pd.DataFrame([list(range(0, headAngle_acc_eu[:,1].shape[0])), euler_circle(headAngle_acc_eu[:,0]), euler_circle(headAngle_acc_eu[:,1]), euler_circle(headAngle_acc_eu[:,2])], index=['frame', 'pitch', 'yaw', 'roll']).T
fig = px.line(headAngle_acc_eu_df, y='yaw', x="frame", title="Euler Head Yaw Direction Acceleration Series")
# fig.show()


# #### Quaternion

# ##### Quaternion Head Angular Distance

# In[84]:


distance_quaternion = quaternion_distance(headAngle_qu)

distance_quaternion_threshold = quaternion_distance(headAngle_qu, 0)

print("Quaternion Head Angular Distance:", round(distance_quaternion, 2), "rad", " |  thresholded:", round(distance_quaternion_threshold, 2), "rad")

result.update({'head_qu_distance': distance_quaternion})


# ##### Quaternion Head Direction Velocity and Acceleration Distribution

# In[85]:


headAngle_velocity_qu = quaternion_diff(headAngle_qu, float(config['frameLength']))
print(headAngle_velocity_qu) # scalar value


# In[86]:


headAngle_acc_qu = euler_diff(headAngle_velocity_qu, float(config['frameLength']))
print(headAngle_acc_qu)


# In[87]:


size = (5,3)

fig, ax = plt.subplots(figsize=size)
fig.subplots_adjust(bottom=0.2, left=0.2)
ax.hist(remove_outlier(headAngle_velocity_qu), bins=100, label='x', color='slateblue')
ax.set_title('Quaternion Head Direction Velocity Distribution')
ax.set_xlabel('velocity (rad/s)')
ax.set_ylabel('frequency')
plt.show()

print("Average Quaternion Head Direction Velocity", np.mean(headAngle_velocity_qu), "rad/s")
result.update({'head_qu_mean_velocity': np.mean(headAngle_velocity_qu)})


# In[88]:


size = (5,3)

fig, ax = plt.subplots(figsize=size)
fig.subplots_adjust(bottom=0.2, left=0.2)
ax.hist(remove_outlier(headAngle_acc_qu), bins=100, label='x', color='slateblue')
ax.set_title('Quaternion Head Direction Acceleration Distribution')
ax.set_xlabel('acceleration (rad/s' + r'$^2$' + ')')
ax.set_ylabel('frequency')
plt.show()

print("Average Quaternion Head Direction Acceleration", np.mean(headAngle_acc_qu), "rad/s^2")
result.update({'head_qu_mean_acc': np.mean(headAngle_acc_qu)})


# ### Hands

# #### Position

# In[89]:


def plotHands(start, end):
  size = (7,7)

  print("viewing frame", start, "to", end)
  fig = plt.figure(figsize=size)
  ax = fig.add_subplot(projection='3d')

  ax.set_xlim3d(-0.5, 0.5)
  ax.set_ylim3d(-0.5, 0.5)
  ax.set_zlim3d(-0.5, 0)

  ax.set_xlabel('X Label')
  ax.set_ylabel('Y Label')
  ax.set_zlabel('Z Label', labelpad=-5)
  ax.set_title('Hands Position Distribution (red: left hand | blue: right hand)')

  ax.scatter(rightHandPosition[start:end,0] - position[start:end,0], rightHandPosition[start:end,2] - position[start:end,2], rightHandPosition[start:end,1] - position[start:end,1], c=np.arange(0,1,1/(end-start)), cmap='Blues', marker='.')
  # [0] and [2] are parralel to the ground, [1] represents the height

  ax.scatter(leftHandPosition[start:end,0] - position[start:end,0], leftHandPosition[start:end,2] - position[start:end,2], leftHandPosition[start:end,1] - position[start:end,1], c=np.arange(0,1,1/(end-start)), cmap='Reds', marker='.')
  # [0] and [2] are parralel to the ground, [1] represents the height

  plt.show()


# In[90]:


start = 0
end = position.shape[0]


# In[91]:


def handleEnd(_end):
  end = _end
  start = end - tail
  if (start < 0): start = 0
  plotHands(start, end)
  return _end

def handleSelect(_select):
  end = _select
  if tags.index(end) == 0:
    start = 0
  else:
    start = tags[tags.index(end) - 1]
  plotHands(start, end)
  return _select


# ##### Hands Position Viewing by Tags

# In[92]:


print("select each scoring time frame, and view the trail from the last scoring time frame")
if(len(tags) > 0):
  interact(handleSelect, _select=tags)
else:
  print("THERE IS NO TAGS")


# ##### Hands Position Viewing Arbitrarily

# In[93]:


seconds = 3
tail = int(seconds / float(config['frameLength']))
print("Viewing length of trail in", seconds, "seconds", ", ", tail, "frames")


# In[94]:


interact(handleEnd, _end=(1,position.shape[0],1))


# #### Euler

# ##### Euler Hands Yaw Diretion Distribution

# In[95]:


N = 180


theta = np.linspace(0.0, 2 * np.pi, N, endpoint=False)
radii = np.histogram(rightHandAngle_eu[:,1] - headAngle_eu[:,1], bins=N, range=[0, 360])[0]
width = (2*np.pi) / N

ax = plt.subplot(polar=True)
bars = ax.bar(theta, radii, width=width, bottom=max(np.histogram(rightHandAngle_eu[:,1] - headAngle_eu[:,1], bins=N, range=[0, 360])[0])/3, color='b')

ax.set_title('Euler Right Hand ' + direction[1] + ' Relative Direction (relativet to head direction )')
ax.set_xlabel('degree [every ' + str(360/N) + r'$\degree$' + ']')
ax.set_ylabel('frequency', rotation=0, labelpad=-50)

plt.show()


# In[96]:


N = 180


theta = np.linspace(0.0, 2 * np.pi, N, endpoint=False)
radii = np.histogram(leftHandAngle_eu[:,1] - headAngle_eu[:,1], bins=N, range=[0, 360])[0]
width = (2*np.pi) / N

ax = plt.subplot(polar=True)
bars = ax.bar(theta, radii, width=width, bottom=max(np.histogram(leftHandAngle_eu[:,1] - headAngle_eu[:,1], bins=N, range=[0, 360])[0])/3, color='r')

ax.set_title('Euler Left Hand ' + direction[1] + ' Relative Direction (relativet to head direction )')
ax.set_xlabel('degree [every ' + str(360/N) + r'$\degree$' + ']')
ax.set_ylabel('frequency', rotation=0, labelpad=-50)

plt.show()


# In[97]:


N = 180

direction = ['Pitch', 'Yaw', 'Roll']

for i in range(len(direction)):
  theta = np.linspace(0.0, 2 * np.pi, N, endpoint=False)
  radii = np.histogram(rightHandAngle_eu[:,i], bins=N, range=[0, 360])[0]
  width = (2*np.pi) / N

  ax = plt.subplot(polar=True)
  bars = ax.bar(theta, radii, width=width, bottom=max(np.histogram(rightHandAngle_eu[:,i], bins=N, range=[0, 360])[0])/3, color="b")

  ax.set_title('Euler Right Hand ' + direction[1] + ' Absolute Direction')
  ax.set_xlabel('degree [every ' + str(360/N) + r'$\degree$' + ']')
  ax.set_ylabel('frequency', rotation=0, labelpad=-50)

  plt.show()


# In[98]:


N = 180

direction = ['Pitch', 'Yaw', 'Roll']

for i in range(len(direction)):
  theta = np.linspace(0.0, 2 * np.pi, N, endpoint=False)
  radii = np.histogram(leftHandAngle_eu[:,i], bins=N, range=[0, 360])[0]
  width = (2*np.pi) / N

  ax = plt.subplot(polar=True)
  bars = ax.bar(theta, radii, width=width, bottom=max(np.histogram(leftHandAngle_eu[:,i], bins=N, range=[0, 360])[0])/3, color='r')

  ax.set_title('Euler Left Hand ' + direction[1] + ' Absolute Direction')
  ax.set_xlabel('degree [every ' + str(360/N) + r'$\degree$' + ']')
  ax.set_ylabel('frequency', rotation=0, labelpad=-50)

  plt.show()


# #### Quaternion

# ##### Quaternion Hands Angular Distance

# In[99]:


rightHand_distance_quaternion = quaternion_distance(rightHandAngle_qu)
rightHand_distance_distance_quaternion_threshold = quaternion_distance(rightHandAngle_qu, 0)

leftHand_distance_quaternion = quaternion_distance(leftHandAngle_qu)
leftHand_distance_distance_quaternion_threshold = quaternion_distance(leftHandAngle_qu, 0)

print("Quaternion Right Hand Angular Distance:", round(rightHand_distance_quaternion, 2), "rad", " |  thresholded:", round(rightHand_distance_distance_quaternion_threshold, 2), "rad")
print("Quaternion Left Hand Angular Distance:", round(leftHand_distance_quaternion, 2), "rad", " |  thresholded:", round(leftHand_distance_distance_quaternion_threshold, 2), "rad")

result.update({'rightHand_qu_distance': rightHand_distance_quaternion})
result.update({'leftHand_qu_distance': leftHand_distance_quaternion})


# ##### Quaternion Right Hand Direction Velocity and Acceleration Distribution

# In[100]:


rightHandAngle_velocity_qu = quaternion_diff(rightHandAngle_qu, float(config['frameLength']))
print(rightHandAngle_velocity_qu)


# In[101]:


rightHandAngle_acc_qu = euler_diff(rightHandAngle_velocity_qu, float(config['frameLength']))
print(rightHandAngle_acc_qu)


# In[102]:


size = (5,3)

fig, ax = plt.subplots(figsize=size)
fig.subplots_adjust(bottom=0.2, left=0.2)
ax.hist(remove_outlier(rightHandAngle_velocity_qu), bins=100, label='x', )
ax.set_title('Quaternion Right Hand Direction Velocity Distribution (absolute)')
ax.set_xlabel('velocity (rad/s)')
ax.set_ylabel('frequency')
plt.show()

print("Average Quternion Right Hand Direction Velocity", np.mean(rightHandAngle_velocity_qu), "rad/s")
result.update({'rightHand_qu_mean_velocity': np.mean(rightHandAngle_velocity_qu)})


# In[103]:


size = (5,3)

fig, ax = plt.subplots(figsize=size)
fig.subplots_adjust(bottom=0.2, left=0.2)
ax.hist(remove_outlier(rightHandAngle_acc_qu), bins=100, label='x', )
ax.set_title('Quaternion Right Hand Direction Acceleration Distribution (absolute)')
ax.set_xlabel('acceleration (rad/s' + r'$^2$' + ')')
ax.set_ylabel('frequency')
plt.show()

print("Average Quternion Right Hand Direction Velocity", np.mean(rightHandAngle_acc_qu), "rad/s^2")
result.update({'rightHand_qu_mean_acc': np.mean(rightHandAngle_acc_qu)})


# ##### Quaternion Left Hand Direction Velocity and Acceleration Distribution

# In[104]:


leftHandAngle_velocity_qu = quaternion_diff(leftHandAngle_qu, float(config['frameLength']))
print(leftHandAngle_velocity_qu)


# In[105]:


leftHandAngle_acc_qu = euler_diff(leftHandAngle_velocity_qu, float(config['frameLength']))
print(leftHandAngle_acc_qu)


# In[106]:


size = (5,3)

fig, ax = plt.subplots(figsize=size)
fig.subplots_adjust(bottom=0.2, left=0.2)
ax.hist(remove_outlier(leftHandAngle_velocity_qu), bins=100, label='x', color='r')
ax.set_title('Quaternion Left Hand Direction Velocity Distribution (absolute)')
ax.set_xlabel('velocity (rad/s)')
ax.set_ylabel('frequency')
plt.show()

print("Average Quternion Left Hand Direction Velocity", np.mean(leftHandAngle_velocity_qu), "rad/s")
result.update({'leftHand_qu_mean_velocity': np.mean(leftHandAngle_velocity_qu)})


# In[107]:


size = (5,3)

fig, ax = plt.subplots(figsize=size)
fig.subplots_adjust(bottom=0.2, left=0.2)
ax.hist(remove_outlier(leftHandAngle_acc_qu), bins=100, label='x', color='r')
ax.set_title('Quaternion Left Hand Direction Acceleration Distribution (absolute)')
ax.set_xlabel('acceleration (rad/s' + r'$^2$' + ')')
ax.set_ylabel('frequency')
plt.show()

print("Average Quternion Left Hand Direction Acceleration", np.mean(leftHandAngle_acc_qu), "rad/s^2")
result.update({'leftHand_qu_mean_acc': np.mean(leftHandAngle_acc_qu)})


# ### Body

# #### Distance and Velocity

# ##### Distance

# In[108]:


distance = 0
for i in range(position.shape[0]-1):
  distance += np.linalg.norm(position[i+1] - position[i])
print("Total Player Distance:", round(distance, 2), "units")


# ##### Velocity Distribution

# In[109]:


# calculate player velocity
player_velocity = []
for i in range(position.shape[0]-1):
  player_velocity.append(np.linalg.norm(position[i+1] - position[i]) / float(config['frameLength']))
print("Average Player Velocity:", round(np.mean(player_velocity), 2), "units/s")
result.update({'player_mean_velocity': np.mean(player_velocity)})


# ##### Velocity Series

# In[110]:


player_velocity_df = pd.DataFrame([list(range(0, len(player_velocity))), player_velocity], index=['frame', 'velocity']).T


# In[111]:


zoom_tags_arr = []
for i in range(player_velocity_df.shape[0]):
    if i in zoom_tags:
        zoom_tags_arr.append("⬆️")
    else:
        zoom_tags_arr.append("")

player_velocity_df['tag'] = zoom_tags_arr


# In[112]:


fig = px.line(player_velocity_df, y="velocity", x="frame", title="Player Velocity Series", text="tag")
# fig.show()


# ##### Acceleration Distribution

# In[113]:


# calculate player velocity
player_acc = []
for i in range(len(player_velocity)-1):
  player_acc.append(np.linalg.norm(player_velocity[i+1] - player_velocity[i]) / float(config['frameLength']))
print("Average Player Acceleration:", round(np.mean(player_acc), 2), "units/s^2")
result.update({'player_mean_acc': np.mean(player_acc)})


# ##### Acceleration Series

# In[114]:


player_acc_df = pd.DataFrame([list(range(0, len(player_acc))), player_acc], index=['frame', 'acceleration']).T


# In[115]:


zoom_tags_arr = []
for i in range(player_acc_df.shape[0]):
    if i in zoom_tags:
        zoom_tags_arr.append("⬆️")
    else:
        zoom_tags_arr.append("")

player_acc_df['tag'] = zoom_tags_arr


# In[116]:


fig = px.line(player_acc_df, y="acceleration", x="frame", title="Player Acceleration Series", text="tag")
# fig.show()


# #### Position

# ##### Body Position Static

# In[117]:


size = (12,12)

fig, ax = plt.subplots(figsize=size)
fig.subplots_adjust(bottom=0.2, left=0.2)

plt.scatter(position[:,0], position[:,2], c='red', marker='.')
# 0:x 2:y 0,2 are in the top view surface

if (config['stage'].split(' ')[0] == '3'):
  ext = [-37, 4, 33, 74.5]
  img = plt.imread("Assets/maze.png")

if (config['stage'].split(' ')[0] == '4'):
  ext = [-11, 48, -6.5, 15.5]
  img = plt.imread("Assets/game.png")

if (config['stage'].split(' ')[0] == '4' or config['stage'].split(' ')[0] == '3'):
  plt.xlim(ext[0], ext[1])
  plt.ylim(ext[2], ext[3])
  plt.imshow(img, zorder=0, extent=ext)
  aspect=img.shape[0]/float(img.shape[1])*((ext[1]-ext[0])/(ext[3]-ext[2]))
  plt.gca().set_aspect(aspect)

ax.set_xlabel('X')
ax.set_ylabel('Y')
ax.set_title(config['stage'].split(' ')[1] + ' stage: Player Position Distribution')

plt.show()


# In[118]:


def plotPos(start, end):
  print("viewing frame", start, "to", end)
  size = (10,10)

  fig, ax = plt.subplots(figsize=size)
  fig.subplots_adjust(bottom=0.2, left=0.2)

  plt.scatter(position[start:end,0], position[start:end,2], c=np.arange(0,1,1/(end-start)), cmap='Reds', marker='.')
  # 0:x 2:y 0,2 are in the top view surface

  if (config['stage'].split(' ')[0] == '3'):
    ext = [-37, 4, 33, 74.5]
    img = plt.imread("Assets/maze.png")

  if (config['stage'].split(' ')[0] == '4'):
    ext = [-11, 48, -6.5, 15.5]
    img = plt.imread("Assets/game.png")

  if (config['stage'].split(' ')[0] == '4' or config['stage'].split(' ')[0] == '3'):
    plt.xlim(ext[0], ext[1])
    plt.ylim(ext[2], ext[3])
    plt.imshow(img, zorder=0, extent=ext)
    aspect=img.shape[0]/float(img.shape[1])*((ext[1]-ext[0])/(ext[3]-ext[2]))
    plt.gca().set_aspect(aspect)

  ax.set_xlabel('X')
  ax.set_ylabel('Y')
  ax.set_title(config['stage'].split(' ')[1] + ' stage: Player Position Distribution')

  plt.show()


# In[119]:


start = 0
end = position.shape[0]


# In[120]:


def handleEndPos(_end):
  end = _end
  start = end - tail
  if (start < 0): start = 0
  plotPos(start, end)
  return _end

def handleSelectPos(_select):
  end = _select
  if tags.index(end) == 0:
    start = 0
  else:
    start = tags[tags.index(end) - 1]
  plotPos(start, end)
  return _select


# ##### Body Position Viewing by Tags

# In[121]:


print("select each scoring time frame, and view the trail from the last scoring time frame")
if(len(tags) > 0):
  interact(handleSelectPos, _select=tags)
else:
  print("THERE IS NO TAGS")


# ##### Body Position Viewing Arbitrarily

# In[122]:


seconds = 10
tail = int(seconds / float(config['frameLength']))
print("Viewing length of trail in", seconds, "seconds", ", ", tail, "frames")


# In[123]:


interact(handleEndPos, _end=(1,position.shape[0],1))


# ## Export Report

# In[124]:


print(config)


# In[125]:


if config['mode'] == 'Static':
  result.update({'method': 'None'})
elif config['mode'] == 'Dynamic':
  if config['method'] == 'Manual':
    result.update({'method': 'Manual'})
  elif config['method'] == 'Hand Position':
    result.update({'method': 'Hands'})
  elif config['method'] == 'Idle Detection':
    result.update({'method': 'Idle'})
  elif config['method'] == 'Target Detection':
    result.update({'method': 'Target'})
  elif config['method'] == 'Holistic':
    result.update({'method': 'Holistic'})

result.update({'user': config['user']})
result.update({'stage': config['stage']})
result.update({'score': config['score']})


# In[126]:


print(result)


# In[127]:


result_df = pd.DataFrame(result, index=[0])
# display(result_df)
