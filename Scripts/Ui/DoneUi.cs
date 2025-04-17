using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoneUi : MonoBehaviour
{
    [SerializeField] public GameObject timer;
    [SerializeField] public GameObject commonButton;

    void Start()
    {
        var gm = GameInstance.GetInstance().GameManager;

        if (commonButton.GetComponent<CommonButton>() != null || timer.GetComponent<Timer>() != null)
        {
            timer.GetComponent<Timer>().Init();
            timer.GetComponent<Timer>().OnTimerEnd = () =>
            {
                gm.TrunDone();
                Destroy(this.gameObject);
            };
            commonButton.GetComponent<CommonButton>().Init("Done", () =>
            {
                gm.TrunDone();
                Destroy(this.gameObject);
            });
        }
    }
}
