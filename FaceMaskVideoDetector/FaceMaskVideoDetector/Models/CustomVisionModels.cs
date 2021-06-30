using System;
using System.Collections.Generic;

namespace FaceMaskVideoDetector.Models
{
    public class Prediction
    {
        public string TagId { get; set; }
        public string TagName { get; set; }
        public double Probability { get; set; }
        public BoundingBox BoundingBox { get; set; }
    }

    public class CustomVisionResult
    {
        public string Id { get; set; }
        public string Project { get; set; }
        public string Iteration { get; set; }
        public DateTime Created { get; set; }
        public List<Prediction> Predictions { get; set; }
    }

    public class BoundingBox
    {
        private int v1;
        private int v2;
        private int v3;
        private int v4;

        public BoundingBox(int v1, int v2, int v3, int v4)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.v4 = v4;
        }

        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
