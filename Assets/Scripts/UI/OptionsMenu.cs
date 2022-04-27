using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public TMP_InputField requestCountInput;
    public TMP_InputField minRequestDeadlineInput, maxRequestDeadlineInput;
    public TMP_InputField diskHeadSpeedInput;
    public TMP_InputField simulationSpeedInput;

    public TogglableButtonText applyButton;

    public Color normalInputColor, invalidInputColor;
    public float colorChangeRate;

    private Dictionary<TMP_InputField, bool> inputValidity;

    private SimulationSettings currentSettings;

    public float minSimulationSpeed, maxSimulationSpeed;

    private bool valid;

    private void Awake()
    {
        inputValidity = new Dictionary<TMP_InputField, bool>
        {
            { requestCountInput, true },
            { minRequestDeadlineInput, true },
            { maxRequestDeadlineInput, true },
            { diskHeadSpeedInput, true },
            { simulationSpeedInput, true }
        };

        requestCountInput.onValueChanged.AddListener(s => OnRequestCountUpdate(s));
        minRequestDeadlineInput.onValueChanged.AddListener(s => OnMinRequestDeadlineUpdate(s));
        maxRequestDeadlineInput.onValueChanged.AddListener(s => OnMaxRequestDeadlineUpdate(s));
        diskHeadSpeedInput.onValueChanged.AddListener(s => OnDiskHeadSpeedUpdate(s));
        simulationSpeedInput.onValueChanged.AddListener(s => OnSimulationSpeedUpdate(s));
    }

    private void Start()
    {
        UpdateInputs();
    }

    private void Update()
    {
        CheckValidity();
        UpdateColors();
    }

    private void CheckValidity()
    {
        valid = AreInputsValid();
    }

    private void UpdateInputs()
    {
        currentSettings = SimulationManager.Instance.simulationSettings;

        requestCountInput.text = currentSettings.requestCount.ToString();
        minRequestDeadlineInput.text = currentSettings.minDeadline.ToString();
        maxRequestDeadlineInput.text = currentSettings.maxDeadline.ToString();
        diskHeadSpeedInput.text = currentSettings.diskHeadSpeed.ToString();
        simulationSpeedInput.text = currentSettings.simulationSpeed.ToString();
    }

    private void ApplySettings()
    {
        bool newSequence =
            currentSettings.requestCount != SimulationManager.Instance.simulationSettings.requestCount ||
            currentSettings.minDeadline != SimulationManager.Instance.simulationSettings.minDeadline ||
            currentSettings.maxDeadline != SimulationManager.Instance.simulationSettings.maxDeadline;


        SimulationManager.Instance.SetSimulationSettings(currentSettings);
        if (newSequence)
        {
            SimulationManager.Instance.GenerateNewSequence();
        }
        SimulationManager.Instance.RunSimulation();

        SimulationManager.Instance.Rewind();
        Hide();
    }

    public void Show()
    {
        UpdateInputs();
        UIManager.Instance.showOptionsMenu = true;
    }

    public void Hide()
    {
        UIManager.Instance.showOptionsMenu = false;
    }

    private void UpdateColors()
    {
        foreach (KeyValuePair<TMP_InputField, bool> pair in inputValidity)
        {
            pair.Key.textComponent.color = Color.Lerp(pair.Key.textComponent.color, pair.Value ? normalInputColor : invalidInputColor, colorChangeRate * Time.deltaTime);
        }

        applyButton.SetActive(valid);
    }

    private bool AreInputsValid()
    {
        bool valid = true;
        foreach (bool v in inputValidity.Values)
            valid &= v;
        return valid;
    }

    private void OnRequestCountUpdate(string input)
    {
        if (int.TryParse(input, out int value))
        {
            if (value < 0)
                MarkInvalid(requestCountInput);
            else
            {
                MarkNormal(requestCountInput);
                currentSettings.requestCount = value;
            }

        }
        else
        {
            MarkInvalid(requestCountInput);
        }
    }

    private void OnMinRequestDeadlineUpdate(string input, bool sub = false)
    {
        if (float.TryParse(input, out float minDeadline))
        {
            if (minDeadline < 0 || minDeadline > currentSettings.maxDeadline)
                MarkInvalid(minRequestDeadlineInput);
            else
            {
                MarkNormal(minRequestDeadlineInput);
                currentSettings.minDeadline = minDeadline;
            }
        }
        else
        {
            MarkInvalid(minRequestDeadlineInput);
        }

        if (!sub)
            OnMaxRequestDeadlineUpdate(maxRequestDeadlineInput.text, true);
    }

    private void OnMaxRequestDeadlineUpdate(string input, bool sub = false)
    {
        if (float.TryParse(input, out float maxDeadline))
        {
            if (maxDeadline < 0 || maxDeadline < currentSettings.minDeadline)
                MarkInvalid(maxRequestDeadlineInput);
            else
            {
                MarkNormal(maxRequestDeadlineInput);
                currentSettings.maxDeadline = maxDeadline;
            }
        }
        else
        {
            MarkInvalid(maxRequestDeadlineInput);
        }

        if (!sub)
            OnMinRequestDeadlineUpdate(minRequestDeadlineInput.text, true);
    }

    private void OnDiskHeadSpeedUpdate(string input)
    {
        if (float.TryParse(input, out float value))
        {
            if (value <= 0)
                MarkInvalid(diskHeadSpeedInput);
            else
            {
                MarkNormal(diskHeadSpeedInput);
                currentSettings.diskHeadSpeed = value;
            }
        }
        else
        {
            MarkInvalid(diskHeadSpeedInput);
        }
    }

    private void OnSimulationSpeedUpdate(string input)
    {
        if (float.TryParse(input, out float value))
        {
            if (value < minSimulationSpeed || value > maxSimulationSpeed)
                MarkInvalid(simulationSpeedInput);
            else
            {
                MarkNormal(simulationSpeedInput);
                currentSettings.simulationSpeed = value;
            }
        }
        else
        {
            MarkInvalid(simulationSpeedInput);
        }
    }

    private void MarkInvalid(TMP_InputField input)
    {
        inputValidity[input] = false;
    }

    private void MarkNormal(TMP_InputField input)
    {
        inputValidity[input] = true;
    }

    public void OnApply()
    {
        ApplySettings();
    }

    public void OnReturn()
    {
        Hide();
    }
}
