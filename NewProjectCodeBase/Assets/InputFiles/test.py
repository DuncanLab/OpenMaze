import sys;


if __name__ == "__main__":
	val = int(sys.argv[1])
	for i in range(5, val + 5):
		print(str(i) + ", " + str(i))