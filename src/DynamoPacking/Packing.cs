using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;
using Nuclex.Game.Packing;
using Point = Autodesk.DesignScript.Geometry.Point;

namespace DynamoPack
{
    public static class Packing
    {
        public static List<Point> ByCoordinateSystems(IEnumerable<BoundingBox> boundingBoxes, double width, double height, double gap)
        {
            var locs = new List<Point>();

            var packer = new CygonRectanglePacker(width, height);
            
            foreach (var bbox in boundingBoxes)
            {
                var packLocation = UV.ByCoordinates();
                var w = bbox.MaxPoint.X - bbox.MinPoint.X + gap;
                var h = bbox.MaxPoint.Y - bbox.MinPoint.Y + gap;
                if (packer.TryPack(w, h, out packLocation))
                {
                    locs.Add(Point.ByCoordinates(packLocation.U, packLocation.V, 0));
                }
            }

            return locs;
        }

        public static Point PolygonCentroidByPoints(IList<Point> vertices)
        {
            var cx = 0.0;
            var cy = 0.0;

            double signedArea = 0.0;
            double x0 = 0.0; // Current vertex X
            double y0 = 0.0; // Current vertex Y
            double x1 = 0.0; // Next vertex X
            double y1 = 0.0; // Next vertex Y
            double a = 0.0;  // Partial signed area

            // For all vertices except last
            int i=0;
            for (i=0; i<vertices.Count-1; ++i)
            {
                x0 = vertices[i].X;
                y0 = vertices[i].Y;
                x1 = vertices[i+1].X;
                y1 = vertices[i+1].Y;
                a = x0*y1 - x1*y0;
                signedArea += a;
                cx += (x0 + x1)*a;
                cy += (y0 + y1)*a;
            }

            // Do last vertex
            x0 = vertices[i].X;
            y0 = vertices[i].Y;
            x1 = vertices[0].X;
            y1 = vertices[0].Y;
            a = x0*y1 - x1*y0;
            signedArea += a;
            cx += (x0 + x1)*a;
            cy += (y0 + y1)*a;

            signedArea *= 0.5;
            cx /= (6.0*signedArea);
            cy /= (6.0*signedArea);

            return Point.ByCoordinates(cx,cy, vertices[0].Z);
        }
    }
}
