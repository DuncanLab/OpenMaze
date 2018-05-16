#NOTE THAT THIS IS A PYTHON 2 IMPLEMENTATION!!!!

import Tkinter, tkFileDialog

root = Tkinter.Tk()
root.withdraw()

file_path = tkFileDialog.askopenfilename	()
print file_path