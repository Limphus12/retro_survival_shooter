using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class WorldManager : MonoBehaviour
    {
        [SerializeField] private TerrainGenerator terrainGenerator;

        Queue<TerrainThreadInfo<TerrainData>> terrainThreadInfoQueue = new Queue<TerrainThreadInfo<TerrainData>>();

        private void Update()
        {
            CheckTerrainQueue();
        }

        private void CheckTerrainQueue()
        {
            if (terrainThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < terrainThreadInfoQueue.Count; i++)
                {
                    TerrainThreadInfo<TerrainData> threadInfo = terrainThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.paramater);
                }
            }
        }

        public void RequestTerrainData(Action<TerrainData> callback)
        {
            ThreadStart threadStart = delegate
            {
                TerrainDataThread(callback);
            };

            new Thread(threadStart).Start();
        }

        void TerrainDataThread(Action<TerrainData> callback)
        {
            TerrainData terrainData = terrainGenerator.GenerateTerrainData();

            //lock so we cannot access it when the thread reaches this stage
            //other threads will have to wait their turn?
            lock (terrainThreadInfoQueue)
            {
                terrainThreadInfoQueue.Enqueue(new TerrainThreadInfo<TerrainData>(callback, terrainData));
            }
        }

        struct TerrainThreadInfo<T>
        {
            public readonly Action<T> callback;
            public readonly T paramater;

            public TerrainThreadInfo(Action<T> callback, T paramater)
            {
                this.callback = callback;
                this.paramater = paramater;
            }
        }
    }
}