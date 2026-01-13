using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NVorbis;

namespace Hallowed.Core;

/// <summary>
/// the class that handle audio playback.
/// </summary>
public class Audio : IDisposable
{
   
   static Dictionary<string, CachedAudioData> cachedAudioData = new Dictionary<string, CachedAudioData>();

   CachedAudioData audioData;
   DynamicSoundEffectInstance dynamicSound;
   byte[] byteBuffer;
   bool isPlaying;
   bool isLooping;
   int currentPosition;
   float volume;
   float pitch;
   float pan;
   
   public string Name { get; set; }
   public int frameCount;
   
   public Audio(string path)
   {
      Name = Path.GetFileNameWithoutExtension(path);
      
      if (!cachedAudioData.ContainsKey(path))
      {
         cachedAudioData[path] = DecodeOggFile(path);
      }
      audioData = cachedAudioData[path];

      dynamicSound = new DynamicSoundEffectInstance(
         audioData.sampleRate,
         audioData.channels == 1 ? AudioChannels.Mono : AudioChannels.Stereo
         );
      byteBuffer = new byte[audioData.sampleRate * audioData.channels];
      dynamicSound.BufferNeeded += OnBufferNeeded;
   }

   /// <summary>
   /// the audio volume.
   /// </summary>
   public float Volume
   {
      get => volume;
      set
      {
         volume = MathHelper.Clamp(value, 0f, 1f);
         if(dynamicSound != null) dynamicSound.Volume = volume;
      }
   }

   /// <summary>
   /// the audio pitch.
   /// </summary>
   public float Pitch
   {
      get => pitch;
      set
      {
         pitch = MathHelper.Clamp(value, -1f, 1f);
         if(dynamicSound != null) dynamicSound.Pitch = pitch;
      }
   }
   
   /// <summary>
   /// the audio pan.
   /// </summary>
   public float Pan
   {
      get => pan;
      set
      {
         pan = MathHelper.Clamp(value, -1f, 1f);
         if(dynamicSound != null) dynamicSound.Pan = pan;
      }
   }

   /// <summary>
   /// play the audio.
   /// </summary>
   /// <param name="loop">whether the sound should loop</param>
   /// <param name="pos"> the audio position</param>
   public void Play(bool loop = false, float pos = 0f)
   {
      isLooping = loop;
      currentPosition = (int)(pos * audioData.sampleRate * audioData.channels);
      currentPosition = Math.Clamp(currentPosition,0,audioData.totalSamples-1);
      isPlaying = true;
      dynamicSound.Play();
   }

   /// <summary>
   /// Stop the audio.
   /// </summary>
   public void Stop()
   {
      isPlaying = false;
      dynamicSound.Stop();
      currentPosition = 0;
   }
  
   /// <summary>
   /// return whether the audio is currently playing.
   /// </summary>
   /// <returns></returns>
   public bool IsPlaying() => isPlaying && dynamicSound.State == SoundState.Playing;
   
   /// <summary>
   /// return the current audio position.
   /// </summary>
   /// <returns></returns>
   public float Seek() => (float)currentPosition / (audioData.sampleRate * audioData.channels);

   public void Dispose()
   {
      Stop();
      dynamicSound?.Dispose();
      dynamicSound = null;
   }

   CachedAudioData DecodeOggFile(string path)
   {
      var stream = TitleContainer.OpenStream(path);
      var vorbis = new VorbisReader(stream, closeOnDispose: false);

      int loopStart = 0;
      int loopEnd = (int)vorbis.TotalSamples;

      if (vorbis.Tags is not null)
      {
         // GetTagMulti returns IReadOnlyList<string>
         var loopStartTags = vorbis.Tags.GetTagMulti("LOOPSTART");
         if (loopStartTags != null && loopStartTags.Count > 0)
         {
            loopStart = int.Parse(loopStartTags[0]);
         }
        
         var loopEndTags = vorbis.Tags.GetTagMulti("LOOPEND");
         if (loopEndTags != null && loopEndTags.Count > 0)
         {
            loopEnd = int.Parse(loopEndTags[0]);
         }
        
         // Alternative: LOOPLENGTH
         var loopLengthTags = vorbis.Tags.GetTagMulti("LOOPLENGTH");
         if (loopLengthTags != null && loopLengthTags.Count > 0)
         {
            loopEnd = loopStart + int.Parse(loopLengthTags[0]);
         }
      }

      var totalSamples = (int)vorbis.TotalSamples;
      var pcmData = new float[totalSamples];

      var offset = 0;
      while (offset < totalSamples)
      {
         var samplesRead = vorbis.ReadSamples(pcmData,offset,pcmData.Length-offset);
         if(samplesRead == 0) break;
         offset += samplesRead;
      }
      vorbis.Dispose();
      stream.Dispose();
    
      return new CachedAudioData
      {
         pcmData = pcmData,
         sampleRate = vorbis.SampleRate,
         channels = vorbis.Channels,
         loopStart = loopStart,
         loopEnd = loopEnd,
         totalSamples = offset
      };
   }

   void OnBufferNeeded(object sender, EventArgs e)
   {
      if (!isPlaying) return;

      var samplesToRead = byteBuffer.Length / 2;
      var samplesAvailable = audioData.totalSamples - currentPosition;

      if (samplesAvailable == 0)
      {
         if (isLooping)
         {
            currentPosition = audioData.loopStart;
            samplesAvailable = audioData.totalSamples - currentPosition;
         }
         else
         {
            Stop();
            return;
         }
      }

      if (isLooping && currentPosition < audioData.loopEnd && currentPosition + samplesToRead >= audioData.loopEnd)
      {
        samplesToRead = audioData.loopEnd - currentPosition;
      }
      samplesToRead = Math.Min(samplesToRead, samplesAvailable);

      if (samplesToRead <= 0) return;
      
      for (var i = 0; i < samplesToRead; i++)
      {
         var sample = (short)(audioData.pcmData[currentPosition + i] * short.MaxValue);
         byteBuffer[i * 2] = (byte)(sample & 0xFF);
         byteBuffer[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
      }
         
      dynamicSound.SubmitBuffer(byteBuffer, 0, samplesToRead * 2);
      currentPosition += samplesToRead;
         
      if(isLooping && currentPosition >= audioData.loopEnd) currentPosition = audioData.loopStart;
   }

}
