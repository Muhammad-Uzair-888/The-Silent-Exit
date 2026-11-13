using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SilentExit.EditorScripts
{
    public static class AudioSynthesizer
    {
        private const int SampleRate = 44100;

        [MenuItem("SilentExit/Generate Audio Assets")]
        public static void GenerateAll()
        {
            string baseDir = Application.dataPath + "/Project/Audio/";
            Directory.CreateDirectory(baseDir + "Player");
            Directory.CreateDirectory(baseDir + "Monster");
            Directory.CreateDirectory(baseDir + "Environment");

            // 1 & 2: Carpet footsteps
            for (int step = 1; step <= 2; step++)
            {
                float dur = 0.22f;
                int count = (int)(dur * SampleRate);
                float[] data = new float[count];
                float freq = step == 1 ? 75f : 85f;
                var rng = new System.Random(step * 100);
                for (int i = 0; i < count; i++)
                {
                    float t = (float)i / SampleRate;
                    float thump = Mathf.Sin(2f * Mathf.PI * freq * t) * Mathf.Exp(-t * 30f);
                    float noise = ((float)rng.NextDouble() * 2f - 1f) * Mathf.Exp(-t * 22f) * 0.4f;
                    data[i] = (thump * 0.7f + noise) * 0.7f;
                }
                SaveWav(baseDir + "Player/sfx_footstep_carpet_0" + step + ".wav", data);
            }

            // 3 & 4: Tile footsteps
            for (int step = 1; step <= 2; step++)
            {
                float dur = 0.22f;
                int count = (int)(dur * SampleRate);
                float[] data = new float[count];
                float clickFreq = step == 1 ? 2200f : 2400f;
                float resFreq = step == 1 ? 850f : 920f;
                var rng = new System.Random(step * 200);
                for (int i = 0; i < count; i++)
                {
                    float t = (float)i / SampleRate;
                    float click = Mathf.Sin(2f * Mathf.PI * clickFreq * t) * Mathf.Exp(-t * 120f);
                    float res = Mathf.Sin(2f * Mathf.PI * resFreq * t) * Mathf.Exp(-t * 35f);
                    float noise = ((float)rng.NextDouble() * 2f - 1f) * Mathf.Exp(-t * 80f) * 0.2f;
                    data[i] = (click * 0.5f + res * 0.4f + noise) * 0.75f;
                }
                SaveWav(baseDir + "Player/sfx_footstep_tile_0" + step + ".wav", data);
            }

            // 5 & 6: Water splashes
            for (int step = 1; step <= 2; step++)
            {
                float dur = 0.55f;
                int count = (int)(dur * SampleRate);
                float[] data = new float[count];
                var rng = new System.Random(step * 300);
                for (int i = 0; i < count; i++)
                {
                    float t = (float)i / SampleRate;
                    float burst = ((float)rng.NextDouble() * 2f - 1f) * Mathf.Exp(-t * 10f);
                    float bubble1 = Mathf.Sin(2f * Mathf.PI * (400f + 300f * t) * t) * Mathf.Exp(-Mathf.Abs(t - 0.08f) * 20f);
                    float bubble2 = Mathf.Sin(2f * Mathf.PI * (600f + 200f * t) * t) * Mathf.Exp(-Mathf.Abs(t - 0.18f) * 25f);
                    float droplet = Mathf.Sin(2f * Mathf.PI * 1100f * t) * Mathf.Exp(-Mathf.Abs(t - 0.25f) * 35f);
                    data[i] = (burst * 0.45f + bubble1 * 0.35f + bubble2 * 0.3f + droplet * 0.2f) * 0.8f;
                }
                SaveWav(baseDir + "Player/sfx_water_splash_0" + step + ".wav", data);
            }

            // 7 & 8: Golem footsteps
            for (int step = 1; step <= 2; step++)
            {
                float dur = 0.6f;
                int count = (int)(dur * SampleRate);
                float[] data = new float[count];
                float baseF = step == 1 ? 50f : 45f;
                var rng = new System.Random(step * 400);
                for (int i = 0; i < count; i++)
                {
                    float t = (float)i / SampleRate;
                    float sub = Mathf.Sin(2f * Mathf.PI * (baseF - 15f * t) * t) * Mathf.Exp(-t * 8f);
                    float crunch = ((float)rng.NextDouble() * 2f - 1f) * Mathf.Exp(-t * 30f) * 0.3f;
                    data[i] = (sub * 0.85f + crunch) * 0.9f;
                }
                SaveWav(baseDir + "Monster/sfx_golem_footstep_0" + step + ".wav", data);
            }

            // 9: Golem growl
            {
                float dur = 2.2f;
                int count = (int)(dur * SampleRate);
                float[] data = new float[count];
                var rng = new System.Random(500);
                for (int i = 0; i < count; i++)
                {
                    float t = (float)i / SampleRate;
                    float env = Mathf.Sin(Mathf.PI * (t / dur));
                    float lfo = 1f + 0.3f * Mathf.Sin(2f * Mathf.PI * 6f * t);
                    float saw = Mathf.Sin(2f * Mathf.PI * 68f * t) + 0.5f * Mathf.Sin(2f * Mathf.PI * 136f * t);
                    float rasp = ((float)rng.NextDouble() * 2f - 1f) * 0.35f;
                    data[i] = (saw * lfo + rasp) * env * 0.75f;
                }
                SaveWav(baseDir + "Monster/sfx_golem_growl.wav", data);
            }

            // 10: Golem attack
            {
                float dur = 1.1f;
                int count = (int)(dur * SampleRate);
                float[] data = new float[count];
                var rng = new System.Random(600);
                for (int i = 0; i < count; i++)
                {
                    float t = (float)i / SampleRate;
                    float roarEnv = Mathf.Clamp01(t / 0.2f) * Mathf.Exp(-t * 2.5f);
                    float roarFreq = 100f + 160f * (t / dur);
                    float roar = Mathf.Sin(2f * Mathf.PI * roarFreq * t) + 0.6f * Mathf.Sin(2f * Mathf.PI * roarFreq * 2f * t);
                    float noise = ((float)rng.NextDouble() * 2f - 1f) * 0.4f;
                    float raw = (roar + noise) * roarEnv;
                    float impact = 0f;
                    if (t >= 0.35f)
                    {
                        float it = t - 0.35f;
                        impact = Mathf.Sin(2f * Mathf.PI * 45f * it) * Mathf.Exp(-it * 10f);
                    }
                    data[i] = Mathf.Clamp((raw * 0.7f + impact * 0.8f), -1f, 1f) * 0.9f;
                }
                SaveWav(baseDir + "Monster/sfx_golem_attack.wav", data);
            }

            // 11: Atmospheric drone loop
            {
                float dur = 6.0f;
                int count = (int)(dur * SampleRate);
                float[] data = new float[count];
                var rng = new System.Random(700);
                for (int i = 0; i < count; i++)
                {
                    float t = (float)i / SampleRate;
                    float s1 = Mathf.Sin(2f * Mathf.PI * 55.0f * t);
                    float s2 = Mathf.Sin(2f * Mathf.PI * 55.3333f * t);
                    float overtone = 0.25f * Mathf.Sin(2f * Mathf.PI * 110.0f * t);
                    float fifth = 0.15f * Mathf.Sin(2f * Mathf.PI * 82.5f * t);
                    float hiss = ((float)rng.NextDouble() * 2f - 1f) * 0.04f;
                    data[i] = (s1 * 0.4f + s2 * 0.35f + overtone + fifth + hiss) * 0.45f;
                }
                SaveWav(baseDir + "Environment/amb_drone_loop.wav", data);
            }

            // 12: Fluorescent buzz
            {
                float dur = 4.0f;
                int count = (int)(dur * SampleRate);
                float[] data = new float[count];
                var rng = new System.Random(800);
                for (int i = 0; i < count; i++)
                {
                    float t = (float)i / SampleRate;
                    float f60 = Mathf.Sin(2f * Mathf.PI * 60f * t);
                    float f120 = 0.5f * Mathf.Sin(2f * Mathf.PI * 120f * t);
                    float f180 = 0.25f * Mathf.Sin(2f * Mathf.PI * 180f * t);
                    float f240 = 0.15f * Mathf.Sin(2f * Mathf.PI * 240f * t);
                    float crackle = (rng.NextDouble() < 0.001) ? ((float)rng.NextDouble() * 2f - 1f) * 0.5f : 0f;
                    data[i] = (f60 * 0.4f + f120 + f180 + f240 + crackle) * 0.35f;
                }
                SaveWav(baseDir + "Environment/env_fluorescent_buzz.wav", data);
            }

            // 13 & 14: Water drip
            for (int step = 1; step <= 2; step++)
            {
                float dur = 0.4f;
                int count = (int)(dur * SampleRate);
                float[] data = new float[count];
                float startF = step == 1 ? 1600f : 1850f;
                float endF = step == 1 ? 1200f : 1380f;
                for (int i = 0; i < count; i++)
                {
                    float t = (float)i / SampleRate;
                    float currentF = t < 0.03f ? Mathf.Lerp(startF, endF, t / 0.03f) : endF;
                    float ring = Mathf.Sin(2f * Mathf.PI * currentF * t) * Mathf.Exp(-t * 22f);
                    data[i] = ring * 0.65f;
                }
                SaveWav(baseDir + "Environment/env_water_drip_0" + step + ".wav", data);
            }

            AssetDatabase.Refresh();
            Debug.Log("<color=green>AudioSynthesizer: All 14 audio clips generated and refreshed!</color>");
        }

        private static void SaveWav(string path, float[] samples)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            using (var bw = new BinaryWriter(fs))
            {
                short channels = 1;
                short bitsPerSample = 16;
                int numSamples = samples.Length;
                int byteRate = SampleRate * channels * bitsPerSample / 8;
                short blockAlign = (short)(channels * bitsPerSample / 8);
                int subChunk2Size = numSamples * channels * bitsPerSample / 8;
                int chunkSize = 36 + subChunk2Size;

                bw.Write(Encoding.ASCII.GetBytes("RIFF"));
                bw.Write(chunkSize);
                bw.Write(Encoding.ASCII.GetBytes("WAVE"));
                bw.Write(Encoding.ASCII.GetBytes("fmt "));
                bw.Write(16);
                bw.Write((short)1);
                bw.Write(channels);
                bw.Write(SampleRate);
                bw.Write(byteRate);
                bw.Write(blockAlign);
                bw.Write(bitsPerSample);
                bw.Write(Encoding.ASCII.GetBytes("data"));
                bw.Write(subChunk2Size);

                for (int i = 0; i < numSamples; i++)
                {
                    float s = Mathf.Clamp(samples[i], -1f, 1f);
                    short val = (short)(s * 32767f);
                    bw.Write(val);
                }
            }
        }
    }
}
