using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance { get; private set; }

    [Header("Simulation Timing")]
    public float simulationDuration;
    public float currentTime;
    [HideInInspector] public float previousTime;
    public float speed;

    [Header("Time Marker")]
    public GameObject timeMarker;
    public float markerMovementSmoothing;
    private float markerVelocity;
    public float unitsPerSecond = 1;

    [Header("Request Marker Managers")]
    public GameObject requestMarkerManagerPrefab;

    [Header("Comfort Features")]
    public CameraMovement cameraMovement;

    private bool playing = false;
    private float playingSpeed;

    private Vector3 initialMarkerPos;

    public SimulationSettings simulationSettings;

    public Dictionary<AlgorithmType, bool> algorithms;
    public Dictionary<AlgorithmType, RequestMarkerManager> markerManagers;
    public Dictionary<AlgorithmType, int> totalSeekTimes;

    public List<Request> currentSequence;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        initialMarkerPos = timeMarker.transform.position;

        previousTime = currentTime;

        algorithms = new Dictionary<AlgorithmType, bool>
        {
            [AlgorithmType.FCFS] = false,
            [AlgorithmType.SSTF] = false,
            [AlgorithmType.SCAN] = false,
            [AlgorithmType.CSCAN] = false,
            [AlgorithmType.EDF] = false,
            [AlgorithmType.FDSCAN] = false
        };
        totalSeekTimes = new Dictionary<AlgorithmType, int>
        {
            [AlgorithmType.FCFS] = 0,
            [AlgorithmType.SSTF] = 0,
            [AlgorithmType.SCAN] = 0,
            [AlgorithmType.CSCAN] = 0,
            [AlgorithmType.EDF] = 0,
            [AlgorithmType.FDSCAN] = 0
        };
        markerManagers = new Dictionary<AlgorithmType, RequestMarkerManager>();

        cameraMovement = Camera.main.GetComponent<CameraMovement>();

        SetDefaultSettings();

        CreateMarkerManagers();
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateNewSequence();
        RunSimulation();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
        MoveTimeMarker();
        SetMarkerVisibility();
        SetSimulationDuration();
    }

    private void UpdateRequestManager()
    {
        RequestManager.Instance.diskSectorCount = simulationSettings.diskSectorCount;
        RequestManager.Instance.UpdateDisk();
    }

    public void GenerateNewSequence()
    {
        currentSequence = RequestSequenceGenerator.GenerateSequence();
    }

    private void SetMarkerVisibility()
    {
        foreach (KeyValuePair<AlgorithmType, bool> pair in algorithms)
        {
            markerManagers[pair.Key].visible = pair.Value;
        }
    }

    public void RunSimulation()
    {
        UpdateRequestManager();
        foreach (RequestMarkerManager manager in markerManagers.Values)
        {
            manager.ClearMarkers();

            Dispatcher dispatcher = manager.GetComponent<Dispatcher>();
            RunDispatcher(dispatcher);
        }
    }

    private void RunDispatcher(Dispatcher dispatcher)
    {
        Debug.Log($"Running {dispatcher.algorithmType}");
        int totalSeekTime = dispatcher.Dispatch(currentSequence);
        totalSeekTimes[dispatcher.algorithmType] = totalSeekTime;
        Debug.Log($"{dispatcher.algorithmType} finished");
    }

    private void SetSimulationDuration()
    {
        float maxDuration = 0;
        foreach (RequestMarkerManager manager in markerManagers.Values)
        {
            Dispatcher dispatcher = manager.GetComponent<Dispatcher>();
            if (manager.GetDuration() > maxDuration && algorithms[dispatcher.algorithmType])
                maxDuration = manager.GetDuration();
        }
        simulationDuration = maxDuration;
    }

    private void CreateMarkerManagers()
    {
        CreateMarkerManager(AlgorithmType.FCFS, UIManager.Instance.fcfsButton.GetComponent<TogglableButton>().activeColor);
        CreateMarkerManager(AlgorithmType.SSTF, UIManager.Instance.sstfButton.GetComponent<TogglableButton>().activeColor);
        CreateMarkerManager(AlgorithmType.SCAN, UIManager.Instance.scanButton.GetComponent<TogglableButton>().activeColor);
        CreateMarkerManager(AlgorithmType.CSCAN, UIManager.Instance.cscanButton.GetComponent<TogglableButton>().activeColor);
        CreateMarkerManager(AlgorithmType.EDF, UIManager.Instance.edfButton.GetComponent<TogglableButton>().activeColor);
        CreateMarkerManager(AlgorithmType.FDSCAN, UIManager.Instance.fdscanButton.GetComponent<TogglableButton>().activeColor);
    }

    private void CreateMarkerManager(AlgorithmType type, Color color)
    {
        RequestMarkerManager manager = Instantiate(requestMarkerManagerPrefab, transform).GetComponent<RequestMarkerManager>();
        Dispatcher dispatcher = manager.GetComponent<Dispatcher>();

        dispatcher.SetAlgorithm(type);
        manager.markerColor = color;
        manager.lineColor = color;

        markerManagers.Add(type, manager);
    }

    public void SetSimulationSettings(SimulationSettings settings)
    {
        simulationSettings = settings;
    }

    public void SetDefaultSettings()
    {
        simulationSettings = new SimulationSettings
        {
            requestCount = 10,
            minDeadline = 1,
            maxDeadline = 10,
            diskHeadSpeed = 100,
            simulationSpeed = 1,
            diskSectorCount = 200,
            deadlineChance = 0.1f
        };
    }

    public bool IsPlaying()
    {
        return playing;
    }

    public float GetPlayingSpeed()
    {
        return playingSpeed;
    }

    public void PlayForward()
    {
        if (!playing || playingSpeed < 0)
        {
            playing = true;
            playingSpeed = speed;
        }
        else
        {
            Pause();
        }
    }

    public void PlayBackward()
    {
        if (!playing || playingSpeed > 0)
        {
            playing = true;
            playingSpeed = -speed;
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        playing = false;
        playingSpeed = 0;
    }

    public void Rewind()
    {
        playingSpeed = 0;
        currentTime = 0;
        playing = false;
    }

    public void Forward()
    {
        playingSpeed = 0;
        currentTime = simulationDuration;
        playing = false;
    }

    private void UpdateTime()
    {
        currentTime += playingSpeed * simulationSettings.simulationSpeed * Time.deltaTime;
        ClampTime();
    }

    private void ClampTime()
    {
        if (currentTime < 0)
        {
            Pause();
            currentTime = 0;
        }
        if (currentTime > simulationDuration)
        {
            Pause();
            currentTime = simulationDuration;
        }
    }

    private void MoveTimeMarker()
    {
        timeMarker.transform.position = new Vector3(initialMarkerPos.x, initialMarkerPos.y, Mathf.SmoothDamp(timeMarker.transform.position.z, -currentTime * unitsPerSecond, ref markerVelocity, markerMovementSmoothing));
    }
}
