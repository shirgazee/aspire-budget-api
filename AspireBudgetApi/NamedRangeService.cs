namespace AspireBudgetApi
{
    public interface INamedRangeService
    {
        public string GetA1Range(string namedRange);

        public void SetA1Range(string namedRange);
    }
}