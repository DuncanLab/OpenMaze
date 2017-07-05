#This is the command line argument for the file
import sys 

#This line reads in the first command line argument for the trial type
trial_type = int(sys.argv[1]) 

#Here we output a location for the file.
#This is a very simple example for testing purposes

if trial_type == 1:
	print("5, 5") #Just print a string of type <number>, <number>. Whitespace doesn't matter
elif trial_type == 2:
	print("-5, 5")
else:
	print("-5, -5")