#!/bin/bash

cd build;
cmake ../;
cmake --build .;

cp libsocket.so ../../BarquitosServidor/server/bin/Debug/net5.0;