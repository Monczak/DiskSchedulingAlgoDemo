using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public TopMask topMask;

    [Header("Algorithm Selection Buttons")]
    public TogglableButton fcfsButton;
    public TogglableButton sstfButton, scanButton, cscanButton, edfButton, fdscanButton;

    [Header("Activity Markers")]
    public ActivityMarker reverseMarker;
    public ActivityMarker playMarker, cameraFollowMarker;

    [Header("Seek Time Texts")]
    public TMP_Text fcfsSeekTimeText;
    public TMP_Text sstfSeekTimeText, scanSeekTimeText, cscanSeekTimeText, edfSeekTimeText, fdscanSeekTimeText;

    [Header("Other UI Elements")]
    public GameObject sideMenuDeco;
    public GameObject bottomMenuDeco;

    public OptionsMenu optionsMenu;
    public bool showOptionsMenu;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        topMask.Initialize();

        SetupSeekTimeTexts();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateActivityMarkers();
        UpdateMenus();
        UpdateSeekTimeTexts();

        SimulationManager.Instance.cameraMovement.panLeft = showOptionsMenu;
    }

    private void UpdateMenus()
    {
        sideMenuDeco.SetActive(!showOptionsMenu);
        bottomMenuDeco.SetActive(!showOptionsMenu);
    }

    private void UpdateActivityMarkers()
    {
        reverseMarker.active = SimulationManager.Instance.IsPlaying() && SimulationManager.Instance.GetPlayingSpeed() < 0;
        playMarker.active = SimulationManager.Instance.IsPlaying() && SimulationManager.Instance.GetPlayingSpeed() > 0;
        cameraFollowMarker.active = SimulationManager.Instance.cameraMovement.following;
    }

    private void SetupSeekTimeTexts()
    {
        fcfsSeekTimeText.color = fcfsButton.GetComponentInChildren<TogglableButton>().activeColor;
        sstfSeekTimeText.color = sstfButton.GetComponentInChildren<TogglableButton>().activeColor;
        scanSeekTimeText.color = scanButton.GetComponentInChildren<TogglableButton>().activeColor;
        cscanSeekTimeText.color = cscanButton.GetComponentInChildren<TogglableButton>().activeColor;
        edfSeekTimeText.color = edfButton.GetComponentInChildren<TogglableButton>().activeColor;
        fdscanSeekTimeText.color = fdscanButton.GetComponentInChildren<TogglableButton>().activeColor;
    }

    private void UpdateSeekTimeTexts()
    {
        fcfsSeekTimeText.text = SimulationManager.Instance.totalSeekTimes[AlgorithmType.FCFS].ToString();
        sstfSeekTimeText.text = SimulationManager.Instance.totalSeekTimes[AlgorithmType.SSTF].ToString();
        scanSeekTimeText.text = SimulationManager.Instance.totalSeekTimes[AlgorithmType.SCAN].ToString();
        cscanSeekTimeText.text = SimulationManager.Instance.totalSeekTimes[AlgorithmType.CSCAN].ToString();
        edfSeekTimeText.text = SimulationManager.Instance.totalSeekTimes[AlgorithmType.EDF].ToString();
        fdscanSeekTimeText.text = SimulationManager.Instance.totalSeekTimes[AlgorithmType.FDSCAN].ToString();

        fcfsSeekTimeText.gameObject.SetActive(SimulationManager.Instance.algorithms[AlgorithmType.FCFS]);
        sstfSeekTimeText.gameObject.SetActive(SimulationManager.Instance.algorithms[AlgorithmType.SSTF]);
        scanSeekTimeText.gameObject.SetActive(SimulationManager.Instance.algorithms[AlgorithmType.SCAN]);
        cscanSeekTimeText.gameObject.SetActive(SimulationManager.Instance.algorithms[AlgorithmType.CSCAN]);
        edfSeekTimeText.gameObject.SetActive(SimulationManager.Instance.algorithms[AlgorithmType.EDF]);
        fdscanSeekTimeText.gameObject.SetActive(SimulationManager.Instance.algorithms[AlgorithmType.FDSCAN]);
    }

    public void OnMenuButtonPressed()
    {
        showOptionsMenu = true;
        optionsMenu.Show();
    }

    public void OnNewSequenceButtonPressed()
    {
        SimulationManager.Instance.GenerateNewSequence();
        SimulationManager.Instance.RunSimulation();
    }

    public void OnReverseButtonPressed()
    {
        SimulationManager.Instance.PlayBackward();
    }

    public void OnPlayButtonPressed()
    {
        SimulationManager.Instance.PlayForward();
    }

    public void OnRewindButtonPressed()
    {
        SimulationManager.Instance.Rewind();
    }

    public void OnForwardButtonPressed()
    {
        SimulationManager.Instance.Forward();
    }

    public void OnCameraFollowButtonPressed()
    {
        SimulationManager.Instance.cameraMovement.following ^= true;
    }

    public void OnFCFSButtonPressed()
    {
        ToggleAlgorithmActive(AlgorithmType.FCFS, fcfsButton);
    }

    public void OnSSTFButtonPressed()
    {
        ToggleAlgorithmActive(AlgorithmType.SSTF, sstfButton);
    }

    public void OnSCANButtonPressed()
    {
        ToggleAlgorithmActive(AlgorithmType.SCAN, scanButton);
    }

    public void OnCSCANButtonPressed()
    {
        ToggleAlgorithmActive(AlgorithmType.CSCAN, cscanButton);
    }

    public void OnEDFButtonPressed()
    {
        ToggleAlgorithmActive(AlgorithmType.EDF, edfButton);
    }

    public void OnFDSCANButtonPressed()
    {
        ToggleAlgorithmActive(AlgorithmType.FDSCAN, fdscanButton);
    }

    private void ToggleAlgorithmActive(AlgorithmType type, TogglableButton button)
    {
        SimulationManager.Instance.algorithms[type] ^= true;
        button.SetActive(SimulationManager.Instance.algorithms[type]);
    }

    private void SetAlgorithmActive(AlgorithmType type, bool active, TogglableButton button)
    {
        SimulationManager.Instance.algorithms[type] = active;
        button.SetActive(active);
    }
}
