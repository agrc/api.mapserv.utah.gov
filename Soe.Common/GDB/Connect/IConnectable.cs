using ESRI.ArcGIS.Geodatabase;

namespace Soe.Common.GDB.Connect
{
    internal interface IConnectable
    {
        IWorkspace Connect();
    }
}