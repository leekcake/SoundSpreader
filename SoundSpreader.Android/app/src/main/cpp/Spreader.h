#include <oboe/Oboe.h>
#include <LockFreeQueue.h>

class Spreader : public oboe::AudioStreamCallback {
private:
    oboe::ManagedStream outStream;
    LockFreeQueue<int16_t, 1024 * 128, uint32_t> queue;
public:
    void Init();
    void RunFetcher();
    oboe::DataCallbackResult onAudioReady(oboe::AudioStream *oboeStream, void *audioData, int32_t numFrames) override;
};

