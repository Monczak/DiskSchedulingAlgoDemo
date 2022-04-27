public struct MarkerVertex
{
    public Request? request;
    public float timePosition;
    public bool partOfLine;
    public bool hasMarker;

    public MarkerVertex(Request? request, float timePosition, bool partOfLine, bool hasMarker)
    {
        this.request = request;
        this.timePosition = timePosition;
        this.partOfLine = partOfLine;
        this.hasMarker = hasMarker;
    }
}
