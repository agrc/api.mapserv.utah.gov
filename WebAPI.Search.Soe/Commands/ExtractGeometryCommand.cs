using System;
using System.Collections.ObjectModel;
using ESRI.ArcGIS.Geometry;
using Soe.Common.Infastructure.Commands;
using WebAPI.Search.Soe.Models;

namespace WebAPI.Search.Soe.Commands
{
    public class ExtractGeometryCommand : Command<GeometryContainer>
    {
        private string _geometryText;
        private bool _ok = true;

        public ExtractGeometryCommand(string geometryText)
        {
            _geometryText = geometryText;
        }

        public override string ToString()
        {
            return string.Format("{0}, GeometryText: {1}", "ExtractCoordinatesForGeometryCommand", _geometryText);
        }

        protected override void Execute()
        {
            var container = new GeometryContainer();

            var parts = _geometryText.Split(new[] {':'});
            if (parts.Length != 2)
            {
                Result = null;
                return;
            }

            container.Type = parts[0].ToUpperInvariant();
            _geometryText = parts[1];

            switch (container.Type)
            {
                case "POINT":
                    {
                        _geometryText = _geometryText.Replace("[", "")
                                                     .Replace("]", "")
                                                     .Replace(" ", "");

                        var splits = _geometryText.Split(',');
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
                            _ok = false;
                        }
                        catch (OverflowException)
                        {
                            _ok = false;
                        }

                        if (!_ok)
                        {
                            Result = null;
                            return;
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
                        Result = null;
                        return;
                    }
            }

            Result = container;
        }
    }
}