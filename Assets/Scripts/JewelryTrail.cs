using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JewelryTrail : MonoBehaviour
{
    public Button btnStartExperiment;
    public GameObject gobjStartExperiment;
    public GameObject gobjEndExperiment;
    public GameObject gobjSendTriggerPanel;
    public TextMeshProUGUI txtExperimentInstructions;

    void Start()
    {
        btnStartExperiment.onClick.AddListener(StartExperiment);
    }

    void StartExperiment()
    {
        gobjStartExperiment.SetActive(false);
        gobjEndExperiment.SetActive(true);
        gobjSendTriggerPanel.SetActive(true);

        txtExperimentInstructions.text = "Send triggers to your EEG or any biosignal acquisition system using the control panel below.";

        return;
    }
}
