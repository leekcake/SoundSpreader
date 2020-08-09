#include <Spreader.h>

#include <android/log.h>
#define LOGV(...) __android_log_print(ANDROID_LOG_VERBOSE, "libSpreader", __VA_ARGS__)
#define LOGD(...) __android_log_print(ANDROID_LOG_DEBUG  , "libSpreader", __VA_ARGS__)
#define LOGI(...) __android_log_print(ANDROID_LOG_INFO   , "libSpreader", __VA_ARGS__)
#define LOGW(...) __android_log_print(ANDROID_LOG_WARN   , "libSpreader", __VA_ARGS__)
#define LOGE(...) __android_log_print(ANDROID_LOG_ERROR  , "libSpreader", __VA_ARGS__)

#include "sys/types.h"
#include "sys/socket.h"
#include "netinet/in.h"
#include <arpa/inet.h>
#include <netdb.h>
#include <stdlib.h>

void Spreader::Init() {    
    oboe::AudioStreamBuilder builder;
        // The builder set methods can be chained for convenience.
        builder.setSharingMode(oboe::SharingMode::Exclusive)
          ->setPerformanceMode(oboe::PerformanceMode::LowLatency)
          ->setChannelCount(2)
          ->setSampleRate(44100)
          ->setFormat(oboe::AudioFormat::I16)
          ->setCallback(this)
          ->openManagedStream(outStream);
        // Typically, start the stream after querying some stream information, as well as some input from the user
        outStream->requestStart();
}

float ReverseFloat(float inFloat){
  float retVal;
  char *floatToConvert = ( char* ) &inFloat;
  char *returnFloat = ( char* ) &retVal;

  // swap the bytes into a temporary buffer
  returnFloat[0] = floatToConvert[3];
  returnFloat[1] = floatToConvert[2];
  returnFloat[2] = floatToConvert[1];
  returnFloat[3] = floatToConvert[0];
  return retVal;
}

void Spreader::RunFetcher() {
    int server_fd, client_fd;
    sockaddr_in server_addr, client_addr;
    memset(&server_addr, 0x00, sizeof(server_addr));
    server_addr.sin_family = AF_INET;
    server_addr.sin_addr.s_addr = htonl(INADDR_ANY);
    server_addr.sin_port = htons(10039);
    
    if((server_fd = socket(AF_INET, SOCK_STREAM, 0)) == -1)
    {// 소켓 생성
        LOGE("Server : Can't open stream socket %s\n", strerror(errno));
        return;
    }

    if(bind(server_fd, (struct sockaddr *) &server_addr, sizeof(server_addr)) <0)
    {//bind() 호출
        LOGE("Server : Can't bind local address.\n");
        return;
    }

    if(listen(server_fd, 5) < 0)
    {
        LOGE("Server : Can't listening connect.\n");
        return;
    }

    int clientAddrSize =  sizeof(client_addr);

    int readed = 0;
    char buffer[1024 * 128];
    memset(buffer, 0x00, sizeof(buffer));
    while(true)
    {
        LOGI("Server : Wait for client.\n");
        // client에 대해 허용
        client_fd = accept(server_fd, (struct sockaddr *) &client_addr, (socklen_t *) &clientAddrSize);
        if(client_fd < 0)
        {
            LOGE("Server: accept failed.\n");
            exit(0);
        }
        
        LOGI("Server : Connected, Receive start\n");
        while(true) {
            readed = read(client_fd, buffer, sizeof(buffer));
            
            for(int i = 0 ; i < readed; i += sizeof(int16_t)) {
                int16_t provide;
                memcpy(&provide, &buffer[i], sizeof(int16_t));
                queue.push( provide );
            }
        }
    }
}

oboe::DataCallbackResult Spreader::onAudioReady(oboe::AudioStream *oboeStream, void *audioData, int32_t numFrames) {
    int16_t* cast = (int16_t*) audioData;
    for(int i = 0; i < numFrames; i++) {
        cast[i] = 0;
    }

    if(numFrames > queue.size()) {
        return oboe::DataCallbackResult::Continue;
    }
    int16_t buf;
    for(int i = 0; i < numFrames; i++) {
        queue.pop(buf);
        cast[i] = buf;
    }

    return oboe::DataCallbackResult::Continue;
}