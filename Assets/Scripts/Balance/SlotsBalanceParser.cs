using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SlotsGame
{
    // converts SlotsBalanceSO to binray data
    public class SlotsBalanceParser : MonoBehaviour
    {
#if UNITY_EDITOR
        public static int CURVE_STEPS = 32;

        // Start is called before the first frame update
        void Start()
        {

        }

        [MenuItem("Slots/Balance/Parse Local")]
        public static void ParseLocal()
        {
            Debug.Log("Parse balance started!");
            AssignIDs();


            ParseSlotsBalance();

            Debug.Log("Parse balance finished!");
        }

        public static void AssignIDs()
        {
            List<Object> objects = new List<Object>();
            AddObjectsFromDirectory("Assets/Balance", objects, typeof(SlotsBalanceSO));
            int numObjects = objects.Count;
            for (int i = 0; i < numObjects; i++)
            {
                SlotsBalanceSO slotsBalanceSO = (SlotsBalanceSO)objects[i];
                string name = slotsBalanceSO.name.Split('_')[0];
                int nameID;
                if (!int.TryParse(name, out nameID))
                    Debug.LogErrorFormat("Slots Balance {0} has bad or missing ID in name", slotsBalanceSO.name);
                slotsBalanceSO.ID = nameID;
                EditorUtility.SetDirty(slotsBalanceSO);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void ParseSlotsBalance()
        {
            if (!Directory.Exists("Assets/Resources"))
                Directory.CreateDirectory("Assets/Resources");

            List<Object> objects = new List<Object>();
            AddObjectsFromDirectory("Assets/Balance", objects, typeof(SlotsBalanceSO));
            int numSlots = objects.Count;
            for (int i = 0; i < numSlots; i++)
            {
                SlotsBalanceSO slotsBalanceSO = (SlotsBalanceSO)objects[i];
                byte[] array = Parse(slotsBalanceSO);

                // save array
                string path = "Assets/Resources/balance" + slotsBalanceSO.ID + ".bytes";
                using (FileStream fs = File.Create(path))
                using (BinaryWriter bw = new BinaryWriter(fs))
                    bw.Write(array);

            }

            AssetDatabase.Refresh();
        }

        public static byte[] Parse(SlotsBalanceSO slotsBalanceSO)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    int version = 1;
                    bw.Write(version);

                    bw.Write(slotsBalanceSO.ID);
                    bw.Write(Mathf.RoundToInt(slotsBalanceSO.Reel1Speed * SlotsLogic.PRECISION));
                    bw.Write(Mathf.RoundToInt(slotsBalanceSO.Reel2Speed * SlotsLogic.PRECISION));
                    bw.Write(Mathf.RoundToInt(slotsBalanceSO.Reel3Speed * SlotsLogic.PRECISION));

                    bw.Write((short)CURVE_STEPS);
                    for (int i = 0; i < CURVE_STEPS; i++)
                        bw.Write((short)(slotsBalanceSO.StartCurve1.Evaluate((float)i / (float)CURVE_STEPS) * SlotsLogic.PRECISION));
                    for (int i = 0; i < CURVE_STEPS; i++)
                        bw.Write((short)(slotsBalanceSO.StartCurve2.Evaluate((float)i / (float)CURVE_STEPS) * SlotsLogic.PRECISION));
                    for (int i = 0; i < CURVE_STEPS; i++)
                        bw.Write((short)(slotsBalanceSO.StartCurve3.Evaluate((float)i / (float)CURVE_STEPS) * SlotsLogic.PRECISION));
                    bw.Write((int)(slotsBalanceSO.StartCurve1Time * SlotsLogic.PRECISION));
                    bw.Write((int)(slotsBalanceSO.StartCurve2Time * SlotsLogic.PRECISION));
                    bw.Write((int)(slotsBalanceSO.StartCurve3Time * SlotsLogic.PRECISION));

                    int numSymbols = slotsBalanceSO.Reel1Symbols.Length;
                    bw.Write((byte)numSymbols);
                    for (int i = 0; i < numSymbols; i++)
                        bw.Write((byte)slotsBalanceSO.Reel1Symbols[i]);
                    for (int i = 0; i < numSymbols; i++)
                        bw.Write((byte)slotsBalanceSO.Reel2Symbols[i]);
                    for (int i = 0; i < numSymbols; i++)
                        bw.Write((byte)slotsBalanceSO.Reel3Symbols[i]);
                }
                return stream.ToArray();
            }

        }

        public static void AddObjectsFromDirectory(string path, List<Object> items, System.Type type)
        {
            if (Directory.Exists(path))
            {
                string[] assets = Directory.GetFiles(path);
                foreach (string assetPath in assets)
                    if (assetPath.Contains(".asset") && !assetPath.Contains(".meta"))
                        items.Add(AssetDatabase.LoadAssetAtPath(assetPath, type));

                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories)
                    if (Directory.Exists(directory))
                        AddObjectsFromDirectory(directory, items, type);
            }
        }
#endif
    }
}