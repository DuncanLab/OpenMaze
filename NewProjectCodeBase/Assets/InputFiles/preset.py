import tkinter as tk
import tkinter.ttk as ttk
from tkColorPicker import askcolor

root = tk.Tk()
style = ttk.Style(root)
style.theme_use('clam')

val = askcolor((255, 255, 0), root)

print (val[1][1:])
