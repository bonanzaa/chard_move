using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ChardMove
{
    [Serializable]
    public class SaveSystem
    {
        public delegate void ProgressCleared();
        public static event ProgressCleared onProgressCleared;
        public bool[] CompletedLevels;
        public int LastSceneIndex;
        private void InitializeArray()
        {
            CompletedLevels = new bool[LevelLoader.Instance.GetLevelCount];
            for (int i = 0; i < CompletedLevels.Length; i++)
            {
                if(i < LastSceneIndex)
                {
                    CompletedLevels[i] = true;
                }
                else
                {
                    CompletedLevels[i] = false;
                }
            }
        }
        private void AssignLastLevelIndex()
        {
                LastSceneIndex = LevelLoader.SceneIndex;
        }
        public void Serialize()
        {
            //CompletedLevels[index] = true;
            Debug.Log(RefreshLvlIndex());
            LastSceneIndex = RefreshLvlIndex();
            Debug.Log(RefreshLvlIndex());

            FileStream fs = new FileStream(Application.persistentDataPath + "/SaveFile.info", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, LastSceneIndex);
                //formatter.Serialize(fs, CompletedLevels);
            }
            catch(SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }
        public void Deserialize()
        {
            if(!File.Exists(Application.persistentDataPath + "/SaveFile.info"))
            {
                AssignLastLevelIndex();
                //InitializeArray();
                return;
            }
            FileStream fs = new FileStream(Application.persistentDataPath + "/SaveFile.info", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                LastSceneIndex = (int)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

        }
        public void ClearSaveData()
        {
            File.Delete(Application.persistentDataPath + "/SaveFile.info");
            
            onProgressCleared();
            Serialize();
            Deserialize();
        }
        public int RefreshLvlIndex()
        {
            return LastSceneIndex;
        }
    }
}
