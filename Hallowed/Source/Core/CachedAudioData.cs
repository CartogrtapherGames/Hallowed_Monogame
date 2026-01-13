namespace Hallowed.Core;

/// <summary>
/// Stores decoded audio data in memory (like Web Audio's AudioBuffer)
/// This is what gets cached so we don't have to decode the same file multiple times
/// </summary>
public class CachedAudioData
{
  
  /// <summary>
  /// Raw audio samples (decoded from OGG)
  /// </summary>
  public float[] pcmData;  
  
  /// <summary>
  /// The audio sample rate
  /// </summary>
  public int sampleRate;  
  /// <summary>
  /// the audio channels.
  /// </summary>
  public int channels;     // 1 = mono, 2 = stereo
  
  /// <summary>
  /// the loop start point in the samples.
  /// </summary>
  public int loopStart;   
  
  /// <summary>
  /// the loop end point in the samples.
  /// </summary>
  public int loopEnd;
  
  /// <summary>
  /// the total samples in pcmData
  /// </summary>
  public int totalSamples; // Total number of samples in pcmData
}
