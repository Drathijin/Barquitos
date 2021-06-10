cmake_minimum_required( VERSION 2.8 )
project( BarquitosSocket )
set(CMAKE_INSTALL_RPATH_USE_LINK_PATH TRUE)

set(CMAKE_INSTALL_RPATH "/usr/local/lib")
set(CMAKE_INSTALL_RPATH_USE_LINK_PATH TRUE)

set(SRC "src/test.cc")
add_library( socket SHARED ${SRC} )

install(TARGETS socket
    DESTINATION ${CMAKE_INSTALL_RPATH}
    )