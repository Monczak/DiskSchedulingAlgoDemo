using System.Collections.Generic;

public class FDSCANAlgorithm : DiskSchedulingAlgorithm
{
    public override string AlgorithmName => "FD-SCAN";

    public override IEnumerator<MarkerVertex> GetMarkerVertices(List<Request> requests)
    {
        int headPosition = -1;
        int requestsLeft = requests.Count, deadlineRequestsLeft = 0;
        float currentTime = 0;

        bool goingRight = true;

        Dictionary<int, Queue<Request>> requestMap = new Dictionary<int, Queue<Request>>();

        bool areThereDeadlines = false;
        foreach (Request request in requests)
        {
            deadlineRequestsLeft += request.hasDeadline ? 1 : 0;
            areThereDeadlines |= request.hasDeadline;
        }

        foreach (Request request in requests)
        {
            if (deadlineRequestsLeft > 0 && request.hasDeadline)
            {
                if (!requestMap.ContainsKey(request.position))
                    requestMap.Add(request.position, new Queue<Request>());
                requestMap[request.position].Enqueue(request);
                if (headPosition == -1)
                    headPosition = request.position;
            }           
        }
        foreach (Request request in requests)
        {
            if (!request.hasDeadline)
            {
                if (!requestMap.ContainsKey(request.position))
                    requestMap.Add(request.position, new Queue<Request>());
                requestMap[request.position].Enqueue(request);
                if (headPosition == -1)
                    headPosition = request.position;
            }
        }
        if (!areThereDeadlines)
            headPosition = requests[0].position;

        while (requestsLeft > 0)
        {
            if (requestMap.ContainsKey(headPosition) && requestMap[headPosition].Count > 0)
            {
                if (deadlineRequestsLeft > 0)
                {
                    if (requestMap[headPosition].Peek().hasDeadline)
                    {
                        Request request = requestMap[headPosition].Dequeue();
                        yield return new MarkerVertex
                        {
                            request = request,
                            timePosition = currentTime,
                            hasMarker = true,
                            partOfLine = true
                        };
                        deadlineRequestsLeft--;
                        requestsLeft--;
                    }
                }
                else
                {
                    Request request = requestMap[headPosition].Dequeue();
                    yield return new MarkerVertex
                    {
                        request = request,
                        timePosition = currentTime,
                        hasMarker = true,
                        partOfLine = true
                    };
                    requestsLeft--;
                }
            }

            headPosition += goingRight ? 1 : -1;
            currentTime += 1 / SimulationManager.Instance.simulationSettings.diskHeadSpeed;

            if (headPosition > SimulationManager.Instance.simulationSettings.diskSectorCount)
            {
                goingRight = false;
                yield return new MarkerVertex
                {
                    request = new Request { position = SimulationManager.Instance.simulationSettings.diskSectorCount },
                    timePosition = currentTime,
                    hasMarker = false,
                    partOfLine = true
                };
            }

            if (headPosition < 0)
            {
                goingRight = true;
                yield return new MarkerVertex
                {
                    request = new Request { position = 0 },
                    timePosition = currentTime,
                    hasMarker = false,
                    partOfLine = true
                };
            }
        }
    }
}
