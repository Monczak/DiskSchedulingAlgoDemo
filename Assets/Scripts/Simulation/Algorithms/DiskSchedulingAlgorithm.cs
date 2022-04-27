using System.Collections.Generic;

public abstract class DiskSchedulingAlgorithm
{
    public abstract string AlgorithmName { get; }

    public abstract IEnumerator<MarkerVertex> GetMarkerVertices(List<Request> requests);
}
