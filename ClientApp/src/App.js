import React from 'react';
import { EventStreamMarshaller } from '@aws-sdk/eventstream-marshaller';
import { toUtf8, fromUtf8 } from '@aws-sdk/util-utf8-node';
import mic from 'microphone-stream';
import Axios from 'axios';

const sampleRate = 44100;
const eventStreamMarshaller = new EventStreamMarshaller(toUtf8, fromUtf8);

// eslint-disable-next-line import/no-anonymous-default-export
export default () => {
  const [webSocket, setWebSocket] = React.useState();
  const [inputSampleRate, setInputSampleRate] = React.useState();

  const streamAudioToWebSocket = async (userMediaStream) => {
    const micStream = new mic();

    micStream.on('format', (data) => {
      setInputSampleRate(data.sampleRate);
    });

    micStream.setStream(userMediaStream);

    const url = await Axios.post('http://localhost:3016/url');

    const socket = new WebSocket(url.data);
    socket.binaryType = 'arraybuffer';

    socket.onopen = () => {
      micStream.on('data', (rawAudioChunk) => {
        const binary = convertAudioToBinaryMessage(rawAudioChunk);
        if (socket.readyState === socket.OPEN) {
          socket.send(binary);
        }
      });
    };

    socket.onmessage = (message) => {
      const messageWrapper = eventStreamMarshaller.unmarshall(Buffer.from(message.data));
      const messageBody = JSON.parse(String.fromCharCode.apply(String, messageWrapper.body));
      if (messageWrapper.headers[':message-type'].value === 'event') {
        handleEventStreamMessage(messageBody);
      } else {
        console.error(messageBody.Message);
        stop(socket);
      }
    };

    socket.onerror = () => {
      stop(socket);
    };

    socket.onclose = () => {
      micStream.stop();
    };

    setWebSocket(socket);

    setTimeout(() => {
      stop(socket);
    }, 15000);

    console.log('Amazon started');
  };

  const convertAudioToBinaryMessage = (audioChunk) => {
    const raw = mic.toRaw(audioChunk);

    if (raw == null) return;

    const downsampledBuffer = downsampleBuffer(raw, inputSampleRate, sampleRate);
    const pcmEncodedBuffer = pcmEncode(downsampledBuffer);
    const audioEventMessage = getAudioEventMessage(Buffer.from(pcmEncodedBuffer));
    const binary = eventStreamMarshaller.marshall(audioEventMessage);

    return binary;
  };

  const getAudioEventMessage = (buffer) => {
    return {
      headers: {
        ':message-type': {
          type: 'string',
          value: 'event',
        },
        ':event-type': {
          type: 'string',
          value: 'AudioEvent',
        },
      },
      body: buffer,
    };
  };

  const pcmEncode = (input) => {
    var offset = 0;
    var buffer = new ArrayBuffer(input.length * 2);
    var view = new DataView(buffer);
    for (var i = 0; i < input.length; i++, offset += 2) {
      var s = Math.max(-1, Math.min(1, input[i]));
      view.setInt16(offset, s < 0 ? s * 0x8000 : s * 0x7fff, true);
    }
    return buffer;
  };

  const downsampleBuffer = (buffer, inputSampleRate = 44100, outputSampleRate = 16000) => {
    if (outputSampleRate === inputSampleRate) {
      return buffer;
    }

    var sampleRateRatio = inputSampleRate / outputSampleRate;
    var newLength = Math.round(buffer.length / sampleRateRatio);
    var result = new Float32Array(newLength);
    var offsetResult = 0;
    var offsetBuffer = 0;

    while (offsetResult < result.length) {
      var nextOffsetBuffer = Math.round((offsetResult + 1) * sampleRateRatio);

      var accum = 0,
        count = 0;

      for (var i = offsetBuffer; i < nextOffsetBuffer && i < buffer.length; i++) {
        accum += buffer[i];
        count++;
      }

      result[offsetResult] = accum / count;
      offsetResult++;
      offsetBuffer = nextOffsetBuffer;
    }

    return result;
  };

  const handleEventStreamMessage = (messageJson) => {
    const results = messageJson.Transcript.Results;
    if (results.length > 0) {
      if (results[0].Alternatives.length > 0) {
        const transcript = decodeURIComponent(escape(results[0].Alternatives[0].Transcript));
        if (!results[0].IsPartial) {
          const text = transcript.toLowerCase().replace('.', '').replace('?', '').replace('!', '');
          console.log(text);
        }
      }
    }
  };

  const start = () => {
    window.navigator.mediaDevices
      .getUserMedia({
        video: false,
        audio: true,
      })
      .then(streamAudioToWebSocket)
      .catch(() => {
        console.error('Please check thet you microphose is working and try again.');
      });
  };

  const stop = (socket) => {
    if (socket) {
      socket.close();
      setWebSocket(undefined);
      console.log('Amazon stoped');
    }
  };

  return (
    <div className="App">
      <button onClick={() => (webSocket ? stop(webSocket) : start())}>{webSocket ? 'Stop' : 'Start'}</button>
    </div>
  );
};