using System;

namespace WebAPI.Common.Models.Esri.ImageService
{
    public class Identify
    {
        public class RequestContract
        {
            public string Geometry { get; set; }
            public GeometryType GeometryType { get; set; }

            public override string ToString()
            {
                return $"?geometry={Geometry}&geometryType={GeometryType}&returnGeometry=false&returnCatalogItems=false&f=json";
            }
        }

        public class ResponseContract
        {
            public string Value { get; set; }

            public string Feet
            {
                get
                {
                    double meters;
                    try
                    {
                        meters = Convert.ToDouble(Value);
                    }
                    catch (Exception)
                    {
                        return "NoData";
                    }

                    return (meters * 3.28084).ToString();
                }
            }
        }
    }
}
