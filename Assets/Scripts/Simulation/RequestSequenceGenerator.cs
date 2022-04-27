using System.Collections.Generic;

using Random = UnityEngine.Random;

public class RequestSequenceGenerator
{
    public static List<Request> GenerateSequence()
    {
        List<Request> requests = new List<Request>();
        for (int i = 0; i < SimulationManager.Instance.simulationSettings.requestCount; i++)
        {
            requests.Add(new Request
            {
                position = Random.Range(0, SimulationManager.Instance.simulationSettings.diskSectorCount),
                hasDeadline = Random.value < SimulationManager.Instance.simulationSettings.deadlineChance,
                deadlineDuration = Random.Range(SimulationManager.Instance.simulationSettings.minDeadline, SimulationManager.Instance.simulationSettings.maxDeadline)
            });
        }
        return requests;
    }
}
