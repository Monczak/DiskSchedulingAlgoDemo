using System.Collections.Generic;
using UnityEngine;

public class EDFAlgorithm : DiskSchedulingAlgorithm
{
    public override string AlgorithmName => "EDF";

    public override IEnumerator<MarkerVertex> GetMarkerVertices(List<Request> requests)
    {
        Dictionary<Request, bool> handled = new Dictionary<Request, bool>();

        Request? FindNearestActiveRequest(int headPosition, Request? currentRequest, List<Request> requestList)
        {
            int minDistance = int.MaxValue;
            Request? minRequest = null;
            foreach (Request request in requestList)
            {
                if (request.Equals(currentRequest)) continue;
                if (handled[request]) continue;

                int distance = Mathf.Abs(request.position - headPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minRequest = request;
                }
            }
            return minRequest;
        }

        List<Request> noDeadlineRequests = new List<Request>(), deadlineRequests = new List<Request>();

        foreach (Request request in requests)
        {
            if (request.hasDeadline)
                deadlineRequests.Add(request);
            else
                noDeadlineRequests.Add(request);
            handled.Add(request, false);
        }
        deadlineRequests.Sort((r1, r2) => r1.deadlineDuration.CompareTo(r2.deadlineDuration));

        Request? currentRequest;
        if (deadlineRequests.Count > 0)
            currentRequest = deadlineRequests[0];
        else
            currentRequest = noDeadlineRequests[0];

        int headPosition = currentRequest.Value.position;
        currentRequest = null;

        float currentTime = 0;
        int requestsLeft = requests.Count;

        while (requestsLeft > 0)
        {
            currentRequest = FindNearestActiveRequest(headPosition, currentRequest, deadlineRequests);
            if (currentRequest == null)
                currentRequest = FindNearestActiveRequest(headPosition, currentRequest, noDeadlineRequests);

            float timeToReach = Mathf.Abs(headPosition - currentRequest.Value.position) / SimulationManager.Instance.simulationSettings.diskHeadSpeed;
            currentTime += timeToReach;
            headPosition = currentRequest.Value.position;

            yield return new MarkerVertex
            {
                request = currentRequest,
                timePosition = currentTime,
                hasMarker = true,
                partOfLine = true
            };

            requestsLeft--;
            handled[currentRequest.Value] = true;
        }
    }
}