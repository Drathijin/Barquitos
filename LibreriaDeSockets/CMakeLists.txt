cmake_minimum_required( VERSION 2.8 )
project( BarquitosSocket )
set(CMAKE_INSTALL_RPATH_USE_LINK_PATH TRUE)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/../BarquitosServidor/server/bin/Debug/net5.0)
message( ${CMAKE_RUNTIME_OUTPUT_DIRECTORY} )


set(SRC "")
list(APPEND SRC "src/test.cc")
list(APPEND SRC "src/test.h")
add_library( socket SHARED ${SRC} )

install(TARGETS socket
    DESTINATION ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}
    )