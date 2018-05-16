import math
from numpy.random import normal

#create anker points


#This is the command line argument for the file
import json, sys

j_input = raw_input();

d = json.loads(j_input[3:])


r = d['Radius']
h = r/2
anker1 = (h,0)
anker2 = (h/2,(h/2)*math.sqrt(3))
anker3 = (-h/2,(h/2)*math.sqrt(3))
anker4 = (-h,0)
anker5 = (-h/2,-(h/2)*math.sqrt(3))
anker6 = (h/2,-(h/2)*math.sqrt(3))

# dank = {
# 'a_1_Circle': anker1,
# 'a_1_Square': anker2, 
# 'a_2_Circle': anker3, 
# 'a_2_Square': anker4,
# 'a_3_Circle': anker5,
# 'a_3_Square': anker6,
# 'b_1_Circle': anker4,
# 'b_1_Square': anker5,
# 'b_2_Circle': anker6,
# 'b_2_Square': anker1,
# 'b_3_Circle': anker2,
# 'b_3_Square': anker3,
# }


x=16
y=16
z=0.75

while abs(x)>15 or abs(y)>15:
	if d["EnvironmentType"] == 2:
		if d["Sides"] == 4:
			x = normal(anker2[0],2)
			y = normal(anker2[1],2) 
		else:
			x = normal(anker1[0],2)
			y = normal(anker1[1],2)
	else:
		if d["Sides"] == 4:
			x = normal(anker5[0],2)
			y = normal(anker5[1],2) 
		else:
			x = normal(anker4[0],2)
			y = normal(anker4[1],2)


print x,",",y,",",z