using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotsGame
{
    [Serializable]
    public struct GUIRefGameObject
    {
        public string Name;
        public GameObject Value;
    }

    [Serializable]
    public struct GUIRefText
    {
        public string Name;
        public Text Value;
    }

    [Serializable]
    public struct GUIRefImage
    {
        public string Name;
        public Image Value;
    }

    [Serializable]
    public struct GUIRefRawImage
    {
        public string Name;
        public RawImage Value;
    }

    [Serializable]
    public struct GUIRefButton
    {
        public string Name;
        public Button Value;
    }

    [Serializable]
    public struct GUIRefSlider
    {
        public string Name;
        public Slider Value;
    }

    public class GUIRef : MonoBehaviour
    {
        public GUIRefGameObject[] GUIRefGameObjects;
        public GUIRefText[] GUIRefTexts;
        public GUIRefImage[] GUIRefImages;
        public GUIRefRawImage[] GUIRefRawImages;
        public GUIRefButton[] GUIRefButtons;
        public GUIRefSlider[] GUIRefSliders;

        public GameObject GetGameObject(string name)
        {
            int numObjects = GUIRefGameObjects.Length;
            for (int i = 0; i < numObjects; i++)
                if (GUIRefGameObjects[i].Name == name)
                    return GUIRefGameObjects[i].Value;

            Debug.LogErrorFormat("GUIRef GetGameObject({0}) does not exist!", name);
            return null;
        }

        public Text GetText(string name)
        {
            int numObjects = GUIRefTexts.Length;
            for (int i = 0; i < numObjects; i++)
                if (GUIRefTexts[i].Name == name)
                    return GUIRefTexts[i].Value;

            Debug.LogErrorFormat("GUIRef GetText({0}) does not exist!", name);
            return null;
        }

        public Image GetImage(string name)
        {
            int numObjects = GUIRefImages.Length;
            for (int i = 0; i < numObjects; i++)
                if (GUIRefImages[i].Name == name)
                    return GUIRefImages[i].Value;

            Debug.LogErrorFormat("GUIRef GetImage({0}) does not exist!", name);
            return null;
        }

        public RawImage GetRawImage(string name)
        {
            int numObjects = GUIRefRawImages.Length;
            for (int i = 0; i < numObjects; i++)
                if (GUIRefRawImages[i].Name == name)
                    return GUIRefRawImages[i].Value;

            Debug.LogErrorFormat("GUIRef GetRawImage({0}) does not exist!", name);
            return null;
        }

        public Button GetButton(string name)
        {
            int numObjects = GUIRefButtons.Length;
            for (int i = 0; i < numObjects; i++)
                if (GUIRefButtons[i].Name == name)
                    return GUIRefButtons[i].Value;

            Debug.LogErrorFormat("GUIRef GetButton({0}) does not exist!", name);
            return null;
        }

        public Slider GetSlider(string name)
        {
            int numObjects = GUIRefSliders.Length;
            for (int i = 0; i < numObjects; i++)
                if (GUIRefSliders[i].Name == name)
                    return GUIRefSliders[i].Value;

            Debug.LogErrorFormat("GUIRef GetSlider({0}) does not exist!", name);
            return null;
        }
    }
}