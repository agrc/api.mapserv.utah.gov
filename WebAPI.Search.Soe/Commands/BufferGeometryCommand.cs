using System;
using System.Linq;
using ESRI.ArcGIS.Geometry;
using Soe.Common.Infastructure.Commands;
using WebAPI.Search.Soe.Models;

namespace WebAPI.Search.Soe.Commands
{
    public class BufferGeometryCommand : Command<IGeometry>
    {
        public BufferGeometryCommand(GeometryContainer geometryContainer, double buffer)
        {
            GeometryContainer = geometryContainer;
            Buffer = buffer;
        }

        public GeometryContainer GeometryContainer { get; set; }
        public double Buffer { get; set; }

        protected override void Execute()
        {
            if (GeometryContainer.Geometry == null)
            {
                switch (GeometryContainer.Type)
                {
                    case "POINT":
                        {
                            var coordinates = GeometryContainer.Coordinates.First();
                            GeometryContainer.Geometry = new Point
                                {
                                    X = coordinates[0],
                                    Y = coordinates[1]
                                };

                            break;
                        }
                    default:
                        throw new NotImplementedException("Only points can be buffered currently.");
                }
            }

            var topologicalOperator = GeometryContainer.Geometry as ITopologicalOperator;

            if (topologicalOperator == null)
            {
                Result = null;
                ErrorMessage = "Something is not well with the topological operator. That is all I know.";

                return;
            }

            Result = topologicalOperator.Buffer(Buffer);
        }

        public override string ToString()
        {
            return string.Format("{0}, GeometryContainer: {1}, Buffer: {2}", "BufferGeometryCommand", GeometryContainer,
                                 Buffer);
        }
    }
}