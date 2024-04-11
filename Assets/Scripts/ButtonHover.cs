using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterCustomize : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject descText;

    // Start is called before the first frame update
    void Start()
    {
        descText.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        descText.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        descText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
