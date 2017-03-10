﻿using System.Collections;
using Utility.TerrainAlgorithm;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace TerrainView
{
    public class TerrainControl : MonoBehaviour
    {
        /*
         * Classe geral de controle e acesso ao terreno
         */

        // Singleton
        public static TerrainControl Instance { get; private set; }

        public Terrain myTerrain;

        public float[,] heights;

        public TransformSet transformSet { get; private set; }

        // Parâmetros UI
        public Text textMass = null;

        void Start()
        {
            Instance = this;

            int x = myTerrain.terrainData.heightmapWidth;
            int z = myTerrain.terrainData.heightmapHeight;
            heights = myTerrain.terrainData.GetHeights(0, 0, x, z);

            transformSet = new TransformSet();
        }

        public void LoadHeightmap(string filename)
        {
            // http://damienclassen.blogspot.com.br/2014/02/loading-terrain-heightmap-data-via-c.html

            byte[] image = File.ReadAllBytes(filename);

            // Load heightmap.
            Texture2D heightmap = new Texture2D(2,2);
            heightmap.LoadImage(image);

            // Acquire an array of colour values.
            Color[] values = heightmap.GetPixels();
            heights = new float[heightmap.height, heightmap.width];

            // Run through array and read height values.
            int index = 0;
            for (int z = 0; z < heightmap.height; z++)
            {
                for (int x = 0; x < heightmap.width; x++)
                {
                    heights[z, x] = values[index].r;
                    index++;
                }
            }

            // Now set terrain heights.
            myTerrain.terrainData.heightmapResolution = heightmap.height;
            myTerrain.terrainData.SetHeights(0, 0, heights);
        }

        public void LoadHeights(float[,] newHeights)
        {
            heights = newHeights;
            myTerrain.terrainData.heightmapResolution = newHeights.GetLength(0);
            myTerrain.terrainData.SetHeights(0, 0, newHeights);
        }

        void Update()
        {
            RunAllTransforms();
        }

        void FixedUpdate()
        {
            myTerrain.ApplyDelayedHeightmapModification();
        }

        private void RunAllTransforms()
        {
            if (GameControl.Instance.BackgroundMode)
                return;

            foreach (TerrainTransform transform in transformSet.transformSet)
            {
                if (transform.IsActive())
                {
                    transform.ApplyTransform(ref heights);
                }
            }

            myTerrain.terrainData.SetHeightsDelayLOD(0, 0, heights);

            UpdateMass();
        }

        private void UpdateMass()
        {
            if (textMass != null)
            {
                float sumHeights = 0.0f;
                foreach (float height in heights)
                {
                    sumHeights += height;
                }
                textMass.text = sumHeights.ToString();
            }
        }

        private void MouseEditTerrain()
        {
            // Método para referência https://forum.unity3d.com/threads/edit-terrain-in-real-time.98410/

            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    RaiseTerrain(hit.point);
                }
            }
            if (Input.GetMouseButton(1))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    LowerTerrain(hit.point);
                }
            }
        }

        private void RaiseTerrain(Vector3 point)
        {
            // Método para referência https://forum.unity3d.com/threads/edit-terrain-in-real-time.98410/

            int terX = (int)((point.x / myTerrain.terrainData.size.x) * heights.GetLength(0));
            int terZ = (int)((point.z / myTerrain.terrainData.size.z) * heights.GetLength(1));
            float y = heights[terX, terZ];
            y += 0.001f;
            float[,] height = new float[1, 1];
            height[0, 0] = y;
            heights[terX, terZ] = y;
            myTerrain.terrainData.SetHeights(terX, terZ, height);
        }

        private void LowerTerrain(Vector3 point)
        {
            // Método para referência https://forum.unity3d.com/threads/edit-terrain-in-real-time.98410/

            int terX = (int)((point.x / myTerrain.terrainData.size.x) * heights.GetLength(0));
            int terZ = (int)((point.z / myTerrain.terrainData.size.z) * heights.GetLength(1));
            float y = heights[terX, terZ];
            y -= 0.001f;
            float[,] height = new float[1, 1];
            height[0, 0] = y;
            heights[terX, terZ] = y;
            myTerrain.terrainData.SetHeights(terX, terZ, height);
        }
    }
}