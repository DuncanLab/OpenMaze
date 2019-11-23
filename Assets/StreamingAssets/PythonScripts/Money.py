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
# 'Jungle_Circle_food: anker1,
# 'Jungle_Square_food': anker2, 
# 'Jungle_Circle_Water': anker3, 
# 'Jungle_Square_Water': anker4,
# 'Jungle_Circle_Money': anker5,
# 'Jungle_Square_Money': anker6,
# 'City_Circle_food': anker4,
# 'City_Square_food': anker5,
# 'City_Circle_Water': anker6,
# 'City_Square_Water': anker1,
# 'City_Circle_Money': anker2,
# 'City_Square_Money': anker3,
# }


x=16
y=16
z=0.75

while abs(x)>15 or abs(y)>15:
	if d["EnvironmentType"] == 2:
		if d["Sides"] == 4:
			x = normal(anker6[0],2)
			y = normal(anker6[1],2) 
		else:
			x = normal(anker5[0],2)
			y = normal(anker5[1],2)
	else:
		if d["Sides"] == 4:
			x = normal(anker3[0],2)
			y = normal(anker3[1],2) 
		else:
			x = normal(anker2[0],2)
			y = normal(anker2[1],2)


print x,",",y,",",z