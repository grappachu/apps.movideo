namespace Grappachu.Movideo.Core.Interfaces
{
    public interface IConfigReader
    {
        ApiSettings GetApiSettings();
        JobSettings GetJobSettings();
    }
}