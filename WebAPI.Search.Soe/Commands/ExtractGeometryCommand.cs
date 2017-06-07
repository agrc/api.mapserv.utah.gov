using System;
using System.Collections.ObjectModel;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SOESupport;
using Soe.Common.Infastructure.Commands;
using WebAPI.Search.Soe.Models;

namespace WebAPI.Search.Soe.Commands
{
    public class ExtractGeometryCommand : Command<GeometryContainer>
    {
        private readonly string _geometryText;
        private readonly int _wkid;

        public ExtractGeometryCommand(string geometryText, int wkid)
        {
            _geometryText = geometryText;
            _wkid = wkid;
        }

        public override string ToString()
        {
            return string.Format("{0}, GeometryText: {1}", "ExtractCoordinatesForGeometryCommand", _geometryText);
        }

        /// <summary>
        ///     code to execute when command is run.
        /// </summary>
        protected override void Execute()
        {
            var container = new GeometryContainer();

            var geometryType = _geometryText.Substring(0, _geometryText.IndexOf(':')).ToUpperInvariant();
            esriGeometryType esriGeometryType;
            var convertToPolygon = false;

            switch (geometryType)
            {
                case "ENVELOPE":
                {
                    esriGeometryType = esriGeometryType.esriGeometryEnvelope;
                    convertToPolygon = true;
                    break;
                }
                case "POINT":
                {
                    esriGeometryType = esriGeometryType.esriGeometryPoint;
                    break;
                }
                case "MULTIPOINT":
                {
                    esriGeometryType = esriGeometryType.esriGeometryMultipoint;
                    break;
                }
                case "POLYLINE":
                {
                    esriGeometryType = esriGeometryType.esriGeometryPolyline;
                    break;
                }
                case "POLYGON":
                {
                    esriGeometryType = esriGeometryType.esriGeometryPolygon;
                    break;
                }
                default:
                {
                    Result = CheckForLegacyGeometryInput(container, _geometryText);
                    return;
                }
            }

            var geometryText = _geometryText.Remove(0, _geometryText.IndexOf(':') + 1);

            container.Type = geometryType;
            container.Geometry = Conversion.ToGeometry(geometryText, esriGeometryType);

            if (container.Geometry == null)
            {
                container = CheckForLegacyGeometryInput(container, _geometryText);
            }

            if (container.Geometry != null && container.Geometry.SpatialReference == null)
            {
                var srFactory = new SpatialReferenceEnvironmentClass();

                var isProjected = true;
                ISpatialReference newSpatialRefefence = null;

                try
                {
                    newSpatialRefefence = srFactory.CreateProjectedCoordinateSystem(_wkid);
                }
                catch (ArgumentException)
                {
                    isProjected = false;
                }

                if (!isProjected)
                {
                    newSpatialRefefence = srFactory.CreateGeographicCoordinateSystem(_wkid);
                }

                container.Geometry.SpatialReference = newSpatialRefefence;
            }

            if (convertToPolygon)
            {
                IPointCollection polygon = new PolygonClass();
                var envelope = (IEnvelope) container.Geometry;
                if (envelope != null)
                {
                    polygon.AddPoint(envelope.UpperLeft);
                    polygon.AddPoint(envelope.UpperRight);
                    polygon.AddPoint(envelope.LowerRight);
                    polygon.AddPoint(envelope.LowerLeft);

                    container.Geometry = polygon as IGeometry;
                    container.Geometry.SpatialReference = envelope.SpatialReference;
                }
            }

            Result = container;
        }

        private static GeometryContainer CheckForLegacyGeometryInput(GeometryContainer container, string geometryText)
        {
            var parts = geometryText.Split(':');
            if (parts.Length != 2)
            {
                return null;
            }

            var ok = true;
            container.Type = parts[0].ToUpperInvariant();
            geometryText = parts[1];

            switch (container.Type)
            {
                case "POINT":
                {
                    geometryText = geometryText.Replace("[", "")
                        .Replace("]", "")
                        .Replace(" ", "");

                    var splits = geometryText.Split(',');
                    var coordinates = new Collection<double[]>();
                    try
                    {
                        var point = new double[2]
                        {
                            Convert.ToDouble(splits[0]),
                            Convert.ToDouble(splits[1])
                        };

                        coordinates.Add(point);
                    }
                    catch (FormatException)
                    {
                        ok = false;
                    }
                    catch (OverflowException)
                    {
                        ok = false;
                    }

                    if (!ok)
                    {
                        return null;
                    }

                    container.Coordinates = coordinates;

                    try
                    {
                        container.Geometry = new Point
                        {
                            X = coordinates[0][0],
                            Y = coordinates[0][1]
                        };
                    }
                    catch
                    {
                        //breaks unit tests since we're not in SOE la la land.
                    }

                    break;
                }
                case "POLYLINE":
                {
                    throw new NotImplementedException("Maybe later");
                }
                case "POLYGON":
                {
                    throw new NotImplementedException("Maybe later");
                }
                default:
                {
                    return null;
                }
            }

            return container;
        }
    }
}