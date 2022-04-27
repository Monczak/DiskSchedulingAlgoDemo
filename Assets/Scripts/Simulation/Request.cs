public struct Request
{
    public int position;
    public bool hasDeadline;
    public float deadlineDuration;

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return position == ((Request)obj).position && deadlineDuration == ((Request)obj).deadlineDuration && hasDeadline == ((Request)obj).hasDeadline;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
