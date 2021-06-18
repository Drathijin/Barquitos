import os, re, os.path, shutil


mypath = "build"
for root, dirs, files in os.walk(mypath):
    for file in files:
        os.remove(os.path.join(root, file))
    
os.system('cmd /c "cd ./build & cmake ../ & cmake --build ."')

# os.system('cmd /c "cp ./build/Debug/socket.dll ../BarquitosServidor/server/bin/Debug/net5.0"')

shutil.copy("./build/Debug/socket.dll","../BarquitosServidor/server/bin/Debug/net5.0")