using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gaia
{
    /// <summary>
    /// Utility class for managing unity heightmaps. Height maps have allowable value range of 0..1f.
    /// </summary>
    public class HeightMap
    {
        protected int m_widthX;
        protected int m_depthZ;
        protected float[,] m_heights;
        protected bool m_isPowerOf2;
        protected float m_widthInvX;
        protected float m_depthInvZ;
        protected float m_statMinVal;
        protected float m_statMaxVal;
        protected double m_statSumVals;
        protected bool m_isDirty;
        protected byte[] m_metaData = new byte[0];

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public HeightMap()
        {
            Reset();
        }

        /// <summary>
        /// Construct a heightmap with given width and height
        /// </summary>
        /// <param name="width">Width of constructed heightmap</param>
        /// <param name="height">Height of constructed heightmap</param>
		public HeightMap(int width, int depth) 
		{
            m_widthX = width;
            m_depthZ = depth;
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_heights = new float[m_widthX, m_depthZ];
            m_isPowerOf2 = Gaia.Utils.Math_IsPowerOf2(m_widthX) && Gaia.Utils.Math_IsPowerOf2(m_depthZ);
            m_statMinVal = m_statMaxVal = 0f;
            m_statSumVals = 0;
            m_metaData = new byte[0];
            m_isDirty = false;
		}

        /// <summary>
        /// Create a heightmap from a float array
        /// </summary>
        /// <param name="source">Source array</param>
        public HeightMap(float[,] source)
        {
            m_widthX = source.GetLength(0);
            m_depthZ = source.GetLength(1);
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_heights = new float[m_widthX, m_depthZ];
            m_isPowerOf2 = Gaia.Utils.Math_IsPowerOf2(m_widthX) && Gaia.Utils.Math_IsPowerOf2(m_depthZ);
            m_statMinVal = m_statMaxVal = 0f;
            m_statSumVals = 0;
            m_metaData = new byte[0];

            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    m_heights[x, z] = source[x, z];
                }
            }
            m_isDirty = false;
        }

        /// <summary>
        /// Create a height mape from the particular slice passed in
        /// </summary>
        /// <param name="source">Height map arrays</param>
        /// <param name="slice">The slice to use</param>
        public HeightMap(float[,,] source, int slice)
        {
            m_widthX = source.GetLength(0);
            m_depthZ = source.GetLength(1);
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_heights = new float[m_widthX, m_depthZ];
            m_isPowerOf2 = Gaia.Utils.Math_IsPowerOf2(m_widthX) && Gaia.Utils.Math_IsPowerOf2(m_depthZ);
            m_statMinVal = m_statMaxVal = 0f;
            m_statSumVals = 0;
            m_metaData = new byte[0];

            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    m_heights[x, z] = source[x, z, slice];
                }
            }

            m_isDirty = false;
        }


        /// <summary>
        /// Create a heightmap from an int array - beware of precision issues
        /// </summary>
        /// <param name="source">Source array</param>
        public HeightMap(int[,] source)
        {
            m_widthX = source.GetLength(0);
            m_depthZ = source.GetLength(1);
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_heights = new float[m_widthX, m_depthZ];
            m_isPowerOf2 = Gaia.Utils.Math_IsPowerOf2(m_widthX) && Gaia.Utils.Math_IsPowerOf2(m_depthZ);
            m_statMinVal = m_statMaxVal = 0f;
            m_statSumVals = 0;
            m_metaData = new byte[0];

            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    m_heights[x, z] = (float)source[x, z];
                }
            }

            m_isDirty = false;
        }

        /// <summary>
        /// Create a height map that is a copy of another heightmap
        /// </summary>
        /// <param name="source">Source heightmap</param>
        public HeightMap(HeightMap source)
        {
            Reset();
            m_widthX = source.m_widthX;
            m_depthZ = source.m_depthZ;
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_heights = new float[m_widthX, m_depthZ];
            m_isPowerOf2 = source.m_isPowerOf2;

            m_metaData = new byte[source.m_metaData.Length];
            for (int idx = 0; idx < source.m_metaData.Length; idx++)
            {
                m_metaData[idx] = source.m_metaData[idx];
            }

            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    m_heights[x, z] = source.m_heights[x, z];
                }
            }

            m_isDirty = false;
        }

        /// <summary>
        /// Create a heightmap by reading in and processing a binary file
        /// </summary>
        /// <param name="sourceFile">File to read in</param>
        public HeightMap(string sourceFile)
        {
            Reset();
            LoadFromBinaryFile(sourceFile);
            m_isDirty = false;
        }

        /// <summary>
        /// Create a heightmap by reading in and processing the byte array
        /// </summary>
        /// <param name="sourceBytes">Source as a byte array</param>
        public HeightMap(byte[] sourceBytes)
        {
            Reset();
            LoadFromByteArray(sourceBytes);
            m_isDirty = false;
        }


        #endregion 

        #region Data access

        /// <summary>
        /// Get width of the height map (x component)
        /// </summary>
        /// <returns>Height map width</returns>
        public int Width()
        {
            return m_widthX;
        }

        /// <summary>
        /// Get depth or height of the height map (z component)
        /// </summary>
        /// <returns>Height map depth</returns>
        public int Depth()
        {
            return m_depthZ;
        }

        /// <summary>
        /// Get min value - need to call update stats before calling this
        /// </summary>
        /// <returns>Min value</returns>
        public float MinVal()
        {
            return m_statMinVal;
        }

        /// <summary>
        /// Get max value - need to call update stats before calling this
        /// </summary>
        /// <returns>Max value</returns>
        public float MaxVal()
        {
            return m_statMaxVal;
        }

        /// <summary>
        /// Get sum of values - need to call update stats before calling this
        /// </summary>
        /// <returns>Sum of values</returns>
        public double SumVal()
        {
            return m_statSumVals;
        }

        /// <summary>
        /// Get metadata
        /// </summary>
        /// <returns></returns>
        public byte[] GetMetaData()
        {
            return m_metaData;
        }

        /// <summary>
        /// Get dirty flag ie we have been modified
        /// </summary>
        /// <returns></returns>
        public bool IsDirty()
        {
            return m_isDirty;
        }

        /// <summary>
        /// Set dirty flag
        /// </summary>
        /// <param name="dirty"></param>
        /// <returns></returns>
        public void SetDirty(bool dirty = true)
        {
            m_isDirty = dirty;
        }

        /// <summary>
        /// Clear the dirty flag
        /// </summary>
        public void ClearDirty()
        {
            m_isDirty = false;
        }

        /// <summary>
        /// Set metadata
        /// </summary>
        /// <param name="metadata">The metadata to set</param>
        public void SetMetaData(byte[] metadata)
        {
            m_metaData = new byte[metadata.Length];
            Buffer.BlockCopy(metadata, 0, m_metaData, 0, metadata.Length);
            m_isDirty = true;
        }

        /// <summary>
        /// Get height map heights
        /// </summary>
        /// <returns>Height map heights</returns>
        public float[,] Heights()
        {
            return  m_heights;
        }

        /// <summary>
        /// Get height at the given location. If out of bounds will return nearest border.
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns></returns>
        public float GetSafeHeight(int x, int z)
        {
            if (x < 0) x = 0;
            if (z < 0) z = 0;
            if (x >= m_widthX) x = m_widthX - 1;
            if (z >= m_depthZ) z = m_depthZ - 1;
            return m_heights[x, z];
        }

        /// <summary>
        /// Set height at the given location. If out of bounds will set at nearest border.
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns></returns>
        public void SetSafeHeight(int x, int z, float height)
        {
            if (x < 0) x = 0;
            if (z < 0) z = 0;
            if (x >= m_widthX) x = m_widthX - 1;
            if (z >= m_depthZ) z = m_depthZ - 1;
            m_heights[x, z] = height;
            m_isDirty = true;
        }


        /// <summary>
        /// Get the interpolated height at the given location
        /// </summary>
        /// <param name="x">x location, range 0..1</param>
        /// <param name="z">z location, range 0..1</param>
        /// <returns>Interpolated height</returns>
        protected float GetInterpolatedHeight(float x, float z)
        {
            //Convert to scale of the heightmap
            x *= ((float)m_widthX - 1f);
            z *= ((float)m_depthZ - 1f);
            int x0 = (int)x;
            int z0 = (int)z;
            int x1 = x0 + 1;
            int z1 = z0 + 1;
            if (x1 >= m_widthX)
            {
                x1 = x0;
            }
            if (z1 >= m_depthZ)
            {
                z1 = z0;
            }
            float dx = x - x0;
            float dz = z - z0;
            float omdx = 1f - dx;
            float omdz = 1f - dz;
            return omdx * omdz * m_heights[x0, z0] +
                      omdx * dz * m_heights[x0, z1] +
                      dx * omdz * m_heights[x1, z0] +
                      dx * dz * m_heights[x1, z1];
        }

        /// <summary>
        /// Get and set the height at the given location
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns>Height at that location</returns>
        public float this[int x, int z]
        {
            get
            {
                return m_heights[x, z];
            }
            set
            {
                m_heights[x, z] = value;
                m_isDirty = true;
            }
        }

        /// <summary>
        /// Get and set the height at the location
        /// </summary>
        /// <param name="x">x location in 0..1f</param>
        /// <param name="z">z location in 0..1f</param>
        /// <returns>Height at that location</returns>
        public float this[float x, float z]
        {
            get
            {
                return GetInterpolatedHeight(x, z);
            }
            set
            {
                //x *= ((float)m_widthX - 1f);
                //z *= ((float)m_depthZ - 1f);
                x *= (float)m_widthX;
                z *= (float)m_depthZ;

                m_heights[(int)x, (int)z] = value;
                m_isDirty = true;
            }
        }

        /// <summary>
        /// Set the level of the entire map to the supplied value
        /// </summary>
        public void SetHeight(float height)
        {
            float newLevel = Gaia.Utils.Math_Clamp(0f, 1f, height);
            for (int hmX = 0; hmX < m_widthX; hmX++)
            {
                for (int hmZ = 0; hmZ < m_depthZ; hmZ++)
                {
                    m_heights[hmX, hmZ] = newLevel;
                }
            }
            m_isDirty = true;
        }

        /// <summary>
        /// Get the height rnage for this map
        /// </summary>
        /// <param name="minHeight">Minimum height</param>
        /// <param name="maxHeight">Maximum height</param>
        public void GetHeightRange(ref float minHeight, ref float maxHeight)
        {
            float currHeight;
            maxHeight = float.MinValue;
            minHeight = float.MaxValue;
            for (int hmX = 0; hmX < m_widthX; hmX++)
            {
                for (int hmZ = 0; hmZ < m_depthZ; hmZ++)
                {
                    currHeight = m_heights[hmX, hmZ];
                    if (currHeight > maxHeight)
                    {
                        maxHeight = currHeight;
                    }
                    if (currHeight < minHeight)
                    {
                        minHeight = currHeight;
                    }
                }
            }
        }

        /// <summary>
        /// Get the slope at the designated location
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns>Steepness at tha location</returns>
        public float GetSlope(int x, int z)
        {
            float height = m_heights[x, z];

            // Compute the differentials by stepping 1 in both directions.
            float dx = m_heights[x + 1, z] - height;
            float dy = m_heights[x, z + 1] - height;

            // The "steepness" is the magnitude of the gradient vector, 
            // For a faster but not as accurate computation, you can just use abs(dx) + abs(dy)
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Get the slope at the designated location
        /// </summary>
        /// <param name="x">x location in range 0..1</param>
        /// <param name="z">z location in range 0..1</param>
        /// <returns>Steepness</returns>
        public float GetSlope(float x, float z)
        {
            float dX = (GetInterpolatedHeight(x + (m_widthInvX * 0.9f), z) - GetInterpolatedHeight(x - (m_widthInvX * 0.9f), z));
            float dZ = (GetInterpolatedHeight(x, z + (m_depthInvZ * 0.9f)) - GetInterpolatedHeight(x, (z - m_depthInvZ * 0.9f)));
            //float direction = (float)Math.Atan2(deltaZ, deltaX);
            return Gaia.Utils.Math_Clamp(0f, 90f, (float)(Math.Sqrt((dX * dX) + (dZ * dZ)) * 10000));
        }

        /// <summary>
        /// Get the slope at the designated location
        /// </summary>
        /// <param name="x">x location in range 0..1</param>
        /// <param name="z">z location in range 0..1</param>
        /// <returns>Steepness</returns>
        public float GetSlope_a(float x, float z)
        {
            float center = GetInterpolatedHeight(x, z);
            float dTop = Math.Abs(GetInterpolatedHeight(x - m_widthInvX, z) - center);
            float dBot = Math.Abs(GetInterpolatedHeight(x + m_widthInvX, z) - center);
            float dLeft = Math.Abs(GetInterpolatedHeight(x, z - m_depthInvZ) - center);
            float dRight = Math.Abs(GetInterpolatedHeight(x, z + m_depthInvZ) - center);
            return ((dTop + dBot + dLeft + dRight) / 4f) * 400f;
        }

        /// <summary>
        /// Get the highest point around the edges of the heightmap - this is used as base level by scanner
        /// </summary>
        /// <returns>Base level</returns>
        public float GetBaseLevel()
        {
            float baseLevel = 0f;

            for (int x = 0; x < m_widthX; x++)
            {
                if (m_heights[x, 0] > baseLevel)
                {
                    baseLevel = m_heights[x, 0];
                }
                if (m_heights[x, m_depthZ-1] > baseLevel)
                {
                    baseLevel = m_heights[x, m_depthZ - 1];
                }
            }

            for (int z = 0; z < m_depthZ; z++)
            {
                if (m_heights[0, z] > baseLevel)
                {
                    baseLevel = m_heights[0, z];
                }
                if (m_heights[m_widthX-1, z] > baseLevel)
                {
                    baseLevel = m_heights[m_widthX - 1, z];
                }
            }

            return baseLevel;
        }

        /// <summary>
        /// Return true if we have data, false otherwise
        /// </summary>
        /// <returns>True if we have data, false otehrwise</returns>
        public bool HasData()
        {
            if (m_widthX <= 0 || m_depthZ <= 0)
            {
                return false;
            }
            if (m_heights == null)
            {
                return false;
            }
            if (m_heights.GetLength(0) != m_widthX || m_heights.GetLength(1) != m_depthZ)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Operations

        /// <summary>
        /// Reset the heightmap including all stats
        /// </summary>
        public void Reset()
        {
            m_widthX = m_depthZ = 0;
            m_widthInvX = m_depthInvZ = 0f;
            m_heights = null;
            m_statMinVal = m_statMaxVal = 0f;
            m_statSumVals = 0;
            m_metaData = new byte[0];
            m_isDirty = false;
        }

        /// <summary>
        /// Update heightmap stats
        /// </summary>
        public void UpdateStats()
        {
            m_statMinVal = 1f;
            m_statMaxVal = 0f;
            m_statSumVals = 0;
            float height = 0f;
            for (int hmX = 0; hmX < m_widthX; hmX++)
            {
                for (int hmZ = 0; hmZ < m_depthZ; hmZ++)
                {
                    height = m_heights[hmX, hmZ];
                    if (height < m_statMinVal)
                    {
                        m_statMinVal = height;
                    }
                    if (height > m_statMaxVal)
                    {
                        m_statMaxVal = height;
                    }
                    m_statSumVals += height;
                }
            }
        }

        /// <summary>
        /// Smooth the height map
        /// </summary>
        /// <param name="iterations">Number of iterations of smoothing to run</param>
        public void Smooth(int iterations)
        {
            for (int i = 0; i < iterations; i++ )
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        m_heights[x, z] = Gaia.Utils.Math_Clamp(0f, 1f, (GetSafeHeight(x - 1, z) + GetSafeHeight(x + 1, z) + GetSafeHeight(x, z - 1) + GetSafeHeight(x, z + 1)) / 4f);
                    }
                }
            }
            m_isDirty = true;
        }

        /// <summary>
        /// Return a new heightmap where each point at contains the slopes of this heightmap at that point
        /// </summary>
        /// <returns></returns>
        public HeightMap GetSlopeMap()
        {
            HeightMap slopeMap = new HeightMap(this);

            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    slopeMap[x, z] = GetSlope(x, z);
                }
            }

            return slopeMap;
        }
    
        /// <summary>
        /// Invert the heightmap
        /// </summary>
		public void Invert()
		{
			for (int x = 0; x < m_widthX; x++)
			{
				for (int z = 0; z < m_depthZ; z++)
				{
					m_heights[x, z] = 1f - m_heights[x, z];
				}
			}
            m_isDirty = true;
		}

        /// <summary>
        /// Flip the heightmap
        /// </summary>
        public void Flip()
        {
            float[,] heights = new float[m_depthZ, m_widthX];
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    heights[z, x] = m_heights[x, z];
                }
            }
            m_heights = heights;
            m_widthX = heights.GetLength(0);
            m_depthZ = heights.GetLength(1);
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_isPowerOf2 = Gaia.Utils.Math_IsPowerOf2(m_widthX) && Gaia.Utils.Math_IsPowerOf2(m_depthZ);
            m_statMinVal = m_statMaxVal = 0f;
            m_statSumVals = 0;
            m_isDirty = true;
        }

        /// <summary>
        /// Normalise the heightmap
        /// </summary>
        public void Normalise()
        {
            float height;
            float maxHeight = float.MinValue;
            float minHeight = float.MaxValue;
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    height = m_heights[x, z];
                    if (height > maxHeight)
                    {
                        maxHeight = height;
                    }
                    if (height < minHeight)
                    {
                        minHeight = height;
                    }
                }
            }
            float heightRange = maxHeight - minHeight;
            if (heightRange > 0f)
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        m_heights[x, z] = (m_heights[x, z] - minHeight) / heightRange;
                    }
                }
                m_isDirty = true;
            }
        }

        #endregion

        #region File Operations

        /// <summary>
        /// Save ourselves into the file provided
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="input"></param>
        public void SaveToBinaryFile(string fileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, m_widthX);
            formatter.Serialize(stream, m_depthZ);
            formatter.Serialize(stream, m_metaData);
            formatter.Serialize(stream, m_heights);
            stream.Close();
            m_isDirty = false;
        }

        /// <summary>
        /// Load ourselves from the file provided
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadFromBinaryFile(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                Debug.LogError("Could not locate file : " + fileName);
                return;
            }

            Reset();
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            m_widthX = (int)formatter.Deserialize(stream);
            m_depthZ = (int)formatter.Deserialize(stream);
            m_metaData = (byte[])formatter.Deserialize(stream);
            m_heights = (float[,])formatter.Deserialize(stream);
            stream.Close();
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_isPowerOf2 = Gaia.Utils.Math_IsPowerOf2(m_widthX) && Gaia.Utils.Math_IsPowerOf2(m_depthZ);
            m_isDirty = false;
        }

        /// <summary>
        /// Load ourselves from the byte array provided
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadFromByteArray(byte[] source)
        {
            if (source == null)
            {
                Debug.LogError("No data provided");
                return;
            }

            Reset();
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream(source);
            m_widthX = (int)formatter.Deserialize(stream);
            m_depthZ = (int)formatter.Deserialize(stream);
            m_metaData = (byte[])formatter.Deserialize(stream);
            m_heights = (float[,])formatter.Deserialize(stream);
            stream.Close();
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_isPowerOf2 = Gaia.Utils.Math_IsPowerOf2(m_widthX) && Gaia.Utils.Math_IsPowerOf2(m_depthZ);
            m_isDirty = false;
        }



        /// <summary>
        /// Load ourselves from the raw file provided
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadFromRawFile(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                Debug.LogError("Could not locate raw file : " + fileName);
                return;
            }

            Reset();
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                using (BinaryReader br = new BinaryReader(fileStream))
                {
                    m_widthX = m_depthZ = Mathf.CeilToInt(Mathf.Sqrt(fileStream.Length / 2));
                    m_heights = new float[m_widthX, m_depthZ];
                    for (int x = 0; x < m_widthX; x++)
                    {
                        for (int z = 0; z < m_depthZ; z++)
                        {
                            m_heights[x, z] = (float)br.ReadUInt16() / 65535.0f; //Should consider doing the unity HM switch here
                        }
                    }
                }
                fileStream.Close();
            }
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_isPowerOf2 = Gaia.Utils.Math_IsPowerOf2(m_widthX) && Gaia.Utils.Math_IsPowerOf2(m_depthZ);
            m_isDirty = false;
        }

        #endregion
    }
}