using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Alva.Effect
{
    public class PreviewImage : MonoBehaviour, IPointerClickHandler
    {
        Image image;
        // Start is called before the first frame update
        void Awake()
        {
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / transform.parent.localScale.x + 20, Screen.height / transform.parent.localScale.x + 20);
            image = transform.Find("Image").GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void Preview(Sprite sprite)
        {
            if (sprite == null)
            {
                return;
            }
            int width = 0;
            int height = 0;
            int screenWidth = (int)(Screen.width / transform.parent.localScale.x);
            int screenHeight = (int)(Screen.height / transform.parent.localScale.x);
            int spriteWidth = sprite.texture.width;
            int spriteHeight = sprite.texture.height;
            float screenRatio = (float)screenWidth / screenHeight;
            float spriteRatio = (float)spriteWidth / spriteHeight;
            if (screenRatio > spriteRatio)
            {
                height = screenHeight;
                width = (int)(height * spriteRatio);
            }
            else
            {
                width = screenWidth;
                height = (int)(width / spriteRatio);
            }
            image.sprite = sprite;
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }
        public void Hide()
        {
            // Destroy(this.gameObject);
            Destroy(transform.parent.gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Hide();
        }
    }
}
