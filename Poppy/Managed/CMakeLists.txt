# Builds a SpatialOS worker using the C++ API

# Replace project name below with your own worker name
# !!! This needs to match the artifact_name in spatialos.<name>.worker.json
# !!! because ${PROJECT_NAME} is used below when creating the zip target
project(Managed)
cmake_minimum_required(VERSION 3.0)

set(APPLICATION_ROOT "${PROJECT_SOURCE_DIR}/../..")
set(SCHEMA_SOURCE_DIR "${APPLICATION_ROOT}/schema")
set(WORKER_SDK_DIR "${APPLICATION_ROOT}/dependencies")

set(PLAYER_AI "Player_AI")
set(PLOT_AI "Plot_AI")
set(DIRECTOR_AI "Director_AI")
set(NPC_AI "NPC_AI")
set(DEFAULT_WORKER "Default_Worker")

if(MSVC)
  add_definitions(/W3 /bigobj)
else()
  # Strict warnings.
  set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")
  add_definitions(-Wall -Wextra -Werror -pedantic)
endif()

add_subdirectory(${WORKER_SDK_DIR} "${CMAKE_CURRENT_BINARY_DIR}/WorkerSdk") # WorkerSdk target created
add_subdirectory(${SCHEMA_SOURCE_DIR} "${CMAKE_CURRENT_BINARY_DIR}/Schema")    # Schema target created

set(GENERATED_CODE_DIR "${APPLICATION_ROOT}/generated_code/cpp")

# The worker binary.
# Make sure you add all worker and generated sources to the executable
add_executable(${PLAYER_AI} src/startup.cc)
add_executable(${PLOT_AI} src/startup.cc)
add_executable(${DIRECTOR_AI} src/startup.cc)
add_executable(${NPC_AI} src/startup.cc)
add_executable(${DEFAULT_WORKER} src/startup.cc)

target_link_libraries(${PLAYER_AI} Schema WorkerSdk smile_dbg)
target_include_directories(${PLAYER_AI} SYSTEM PRIVATE
                           ${GENERATED_CODE_DIR}
                           "${WORKER_SDK_DIR}/worker_sdk/cpp/include"
                           "${WORKER_SDK_DIR}/bayes/include")					   

target_link_libraries(${PLOT_AI} Schema WorkerSdk smile_dbg)
target_include_directories(${PLOT_AI} SYSTEM PRIVATE
                           ${GENERATED_CODE_DIR}
                           "${WORKER_SDK_DIR}/worker_sdk/cpp/include"
                           "${WORKER_SDK_DIR}/bayes/include")

target_link_libraries(${DIRECTOR_AI} Schema WorkerSdk smile_dbg)
target_include_directories(${DIRECTOR_AI} SYSTEM PRIVATE
                           ${GENERATED_CODE_DIR}
                           "${WORKER_SDK_DIR}/worker_sdk/cpp/include"
                           "${WORKER_SDK_DIR}/bayes/include")
						   
target_link_libraries(${NPC_AI} Schema WorkerSdk smile_dbg)
target_include_directories(${NPC_AI} SYSTEM PRIVATE
                           ${GENERATED_CODE_DIR}
                           "${WORKER_SDK_DIR}/worker_sdk/cpp/include"
                           "${WORKER_SDK_DIR}/bayes/include")
						   
target_link_libraries(${DEFAULT_WORKER} Schema WorkerSdk smile_dbg)
target_include_directories(${DEFAULT_WORKER} SYSTEM PRIVATE
                           ${GENERATED_CODE_DIR}
                           "${WORKER_SDK_DIR}/worker_sdk/cpp/include"
                           "${WORKER_SDK_DIR}/bayes/include")

# Set artifact subdirectories.
# WORKER_BUILD_DIR should not be changed so that spatial local launch
# and spatial upload can find the worker assemblies
set(WORKER_BUILD_DIR "${APPLICATION_ROOT}/build/assembly/worker")
set(WORKER_PACKAGE_COMMAND "spatial")

# The worker in a zip.
add_custom_target(
  ${PLAYER_AI}Zip ALL
  WORKING_DIRECTORY ${CMAKE_BINARY_DIR}
  COMMAND ${CMAKE_COMMAND} -E make_directory "${WORKER_BUILD_DIR}"
  COMMAND ${WORKER_PACKAGE_COMMAND} file zip -b "$<TARGET_FILE_DIR:${PLAYER_AI}>"
          -o "${WORKER_BUILD_DIR}/${PLAYER_AI}"
          --worker_platform=current
          "$<TARGET_FILE_NAME:${PLAYER_AI}>"
  DEPENDS ${PLAYER_AI})

add_custom_target(
  ${PLOT_AI}Zip ALL
  WORKING_DIRECTORY ${CMAKE_BINARY_DIR}
  COMMAND ${CMAKE_COMMAND} -E make_directory "${WORKER_BUILD_DIR}"
  COMMAND ${WORKER_PACKAGE_COMMAND} file zip -b "$<TARGET_FILE_DIR:${PLOT_AI}>"
          -o "${WORKER_BUILD_DIR}/${PLOT_AI}"
          --worker_platform=current
          "$<TARGET_FILE_NAME:${PLOT_AI}>"
  DEPENDS ${PLOT_AI})

add_custom_target(
  ${DIRECTOR_AI}Zip ALL
  WORKING_DIRECTORY ${CMAKE_BINARY_DIR}
  COMMAND ${CMAKE_COMMAND} -E make_directory "${WORKER_BUILD_DIR}"
  COMMAND ${WORKER_PACKAGE_COMMAND} file zip -b "$<TARGET_FILE_DIR:${DIRECTOR_AI}>"
          -o "${WORKER_BUILD_DIR}/${DIRECTOR_AI}"
          --worker_platform=current
          "$<TARGET_FILE_NAME:${DIRECTOR_AI}>"
  DEPENDS ${DIRECTOR_AI})

add_custom_target(
  ${NPC_AI}Zip ALL
  WORKING_DIRECTORY ${CMAKE_BINARY_DIR}
  COMMAND ${CMAKE_COMMAND} -E make_directory "${WORKER_BUILD_DIR}"
  COMMAND ${WORKER_PACKAGE_COMMAND} file zip -b "$<TARGET_FILE_DIR:${NPC_AI}>"
          -o "${WORKER_BUILD_DIR}/${NPC_AI}"
          --worker_platform=current
          "$<TARGET_FILE_NAME:${NPC_AI}>"
  DEPENDS ${NPC_AI})

add_custom_target(
  ${DEFAULT_WORKER}Zip ALL
  WORKING_DIRECTORY ${CMAKE_BINARY_DIR}
  COMMAND ${CMAKE_COMMAND} -E make_directory "${WORKER_BUILD_DIR}"
  COMMAND ${WORKER_PACKAGE_COMMAND} file zip -b "$<TARGET_FILE_DIR:${DEFAULT_WORKER}>"
          -o "${WORKER_BUILD_DIR}/${DEFAULT_WORKER}"
          --worker_platform=current
          "$<TARGET_FILE_NAME:${DEFAULT_WORKER}>"
  DEPENDS ${DEFAULT_WORKER})