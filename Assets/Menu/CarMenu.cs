using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityExtensions;

public class CarMenu : MonoBehaviour
{
    public GameObject[] carModels;
    public int selectedModel = 0;

    public Transform cameraPoint;
    public PlayerUI player;
    public Text carName, carInfo;

    void Start() { OnEnable(); }

	void OnEnable() 
    {
        var camera = Camera.main.transform;
        camera.parent = transform.parent;
        camera.position = cameraPoint.position;
        camera.rotation = cameraPoint.rotation;

        SetupPlayerCar();
    }

    public void SelectNext(bool forward = true)
    {
        selectedModel += forward ? 1 : -1;
        SetupPlayerCar();
    }

	public void TestDrive()
    {
	    gameObject.SetActive(false);
        player.gameObject.SetActive(true);
        player.exit = delegate()
        {
            gameObject.SetActive(true);
            player.gameObject.SetActive(false); 
        };
	}

    public void SetupPlayerCar() 
    {
        if (player.car)
            DestroyImmediate(player.car.gameObject);

        if (selectedModel < 0)
            selectedModel = carModels.Length - 1;
        if (selectedModel >= carModels.Length)
            selectedModel = 0;

        var model = carModels[selectedModel];
        PlayerPrefs.SetInt("Selected.Model", selectedModel);

        var parent = transform.parent.gameObject;
        player.car = parent.MakeChild(model).GetComponent<CarControl>();

        carName.text = model.name;
        carInfo.text = player.car.info;
    }
}
