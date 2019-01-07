using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour {

    public Button button;

    public Button saveBtn;

    public Text content;

    public Button disposeBtn;
    void Awake()
    {
        button.onClick.AddListener(OnClick);
        saveBtn.onClick.AddListener(OnSaveBtn);
    }
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnClick()
    {
        SDKTCI.Instance.Open();
    }

    void OnSaveBtn()
    {
        
        SDKTCI.Instance.SaveTexture();
    }


}
