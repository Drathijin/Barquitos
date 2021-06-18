import os, re, os.path, shutil
import platform

mypath = "build"
for root, dirs, files in os.walk(mypath):
    for file in files:
        os.remove(os.path.join(root, file))

if (platform.system() == "Linux" or platform.system() == "Linux2"):
	os.system("cd ./build && cmake ../ && cmake --build .")
	os.system("cp ./build/libsocket.so ../BarquitosServidor/server/bin/Debug/net5.0")
else:
	os.system('cmd /c "cd ./build & cmake ../ & cmake --build ."')
	shutil.copy("./build/Debug/socket.dll","../BarquitosServidor/server/bin/Debug/net5.0")


# os.system('cmd /c "cp ./build/Debug/socket.dll ../BarquitosServidor/server/bin/Debug/net5.0"')

# shutil.copy("./build/Debug/socket.dll","../BarquitosServidor/server/bin/Debug/net5.0")