namespace CustomRenderer.Interfaces
{
    using System.Threading.Tasks;
    using Models;

    public interface IGpsDataProvider
    {
        Task<Position> GetNativeGpsData();
    }
}