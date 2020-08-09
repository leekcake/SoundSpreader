#include <jni.h>
#include <string>
#include <Spreader.h>

extern "C" JNIEXPORT jlong JNICALL
Java_moe_leekcake_soundspreader_MainActivity_createEngine(
        JNIEnv* env,
        jobject /* this */) {

    Spreader* spreader = new Spreader();
    spreader->Init();
    return (jlong) spreader;

}

extern "C" JNIEXPORT void JNICALL
Java_moe_leekcake_soundspreader_MainActivity_runFetcher(JNIEnv* env,
        jobject self, jlong spreader) {
    ((Spreader*) spreader)->RunFetcher();
}