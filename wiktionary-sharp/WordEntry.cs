// See https://aka.ms/new-console-template for more information
struct WordEntry
{
    public WordEntry(long start, int length)
    {
        Start = start;
        Length = length;
    }

    public long Start { get; set; }
    public int Length { get; set; }
}
