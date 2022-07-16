using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeGame
{

    public class SpriteScaler : MonoBehaviour
    {
        public float DefaultScale = 1.0f;
        int m_oldWidth;

        private void Awake()
        {
            m_oldWidth = Screen.width;
            ScaleScreen();
        }

        void Update()
        {
            if (Screen.width != m_oldWidth)
                ScaleScreen();
        }

        void ScaleScreen()
        {
            float width = Screen.width;
            float height = Screen.height;
            float ratio = width / height;
            if (ratio < (9.0f / 16.0f))
            {
                float newWidth = ratio * 1920.0f;
                transform.localScale = Vector3.one * DefaultScale * (newWidth / 1080.0f);
            }
            else
                transform.localScale = Vector3.one * DefaultScale;

            m_oldWidth = Screen.width;
        }
    }
}