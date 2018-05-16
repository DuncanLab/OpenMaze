import math
from numpy.random import normal

#create anker points


#This is the command line argument for the file
import json, sys

j_input = raw_input();

d = json.loads(j_input[3:])




#d = json.loads(j)

if d["EnvironmentType"] == 2:
    d["EnvironmentType"] = 'a'
if d["EnvironmentType"] == 3:
    d["EnvironmentType"] = 'b'
if d["Sides"] >= 20:
    d["Sides"] = 'Circle'
if d["Sides"] == 4:
    d["Sides"] = 'Square'

#string = str(str(d["EnvironmentType"])+ "_" + str(d["PickupType"])+ "_" + str(d["Sides"]))
r = d['Radius']
h = r/2
anker1 = (h,0)
anker2 = (h/2,(h/2)*math.sqrt(3))
anker3 = (-h/2,(h/2)*math.sqrt(3))
anker4 = (-h,0)
anker5 = (-h/2,-(h/2)*math.sqrt(3))
anker6 = (h/2,-(h/2)*math.sqrt(3))

dank = {
'a_1_Circle': anker1,
'a_1_Square': anker2, 
'a_2_Circle': anker3, 
'a_2_Square': anker4,
'a_3_Circle': anker5,
'a_3_Square': anker6,
'b_1_Circle': anker4,
'b_1_Square': anker5,
'b_2_Circle': anker6,
'b_2_Square': anker1,
'b_3_Circle': anker2,
'b_3_Square': anker3,
}

anker = dank[string]

x=16
y=16

while abs(x)>15 or abs(y)>15:
    x = normal(5,2)
    y = normal(5,2)
    z = 0.5

print x,",",y,",",z