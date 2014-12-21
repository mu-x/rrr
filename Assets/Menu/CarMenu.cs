using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using UnityExtensions;

[System.Serializable]
public class Car 
{
    public GameObject model;
    public int owned;
}

public class CarMenu : MonoBehaviour
{
    public Car[] carModels;
    public int selectedModel = 0;

    public Transform cameraPoint;
    public PlayerUI player;
    public Text carName, carInfo, carPrice;

    void Start() { OnEnable(); }

	void OnEnable() 
    {
        var camera = Camera.main.transform;
        camera.parent = transform.parent;
        camera.position = cameraPoint.position;
        camera.rotation = cameraPoint.rotation;

        for (int i = 0; i < carModels.Length; ++i)
        {
            var id = "Selected.Car" + i.ToString();
            carModels[i].owned = PlayerPrefs.GetInt(id, carModels[i].owned);
        }

        selectedModel = PlayerPrefs.GetInt("Selected.Model", 0);
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

        var car = carModels[selectedModel];
        PlayerPrefs.SetInt("Selected.Model", selectedModel);

        var parent = transform.parent.gameObject;
        player.car = parent.MakeChild(car.model).GetComponent<CarControl>();

        carName.text = car.model.name;
        carInfo.text = player.car.info;
        carPrice.text = (car.owned == 0) ?
            string.Format("Buy\n{0} K", player.car.price) : "real\nrace";
    }
}
