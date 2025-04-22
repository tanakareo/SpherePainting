namespace SpherePainting
{
    public struct RenderInformation
    {
        public readonly int LayerCount { get; }

        public RenderInformation(int layerCount)
        {
            LayerCount = layerCount;
        }
    }
}