namespace Soe.Common.GDB.Connect
{
    public class SdeConnectorFactory
    {
        public static Connector Create(string layerName)
        {
            layerName = layerName.ToUpperInvariant();

            if(layerName.StartsWith("SGID10")){
                return new SdeConnector(new Sgid10ConnectionInformation());
            }

            return null;
        }
    }
}