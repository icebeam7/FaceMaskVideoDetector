using System.Collections.Generic;

namespace FaceMaskVideoDetector.Models
{
    public class VideoResultClean
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public string ThumbnailId { get; set; }
    }

    public class VideoResultInsights
    {
        public string Id { get; set; }
        public string VideoUri { get; set; }
        public string Name { get; set; }

        public List<KeyFrameClean> KeyFrameList { get; set; }
    }

    public class KeyFrameClean
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string Thumbnail { get; set; }
        public string ThumbnailId { get; set; }
        public string Labels { get; set; }
        public string CustomLabel { get; set; }
    }

    public class LabelClean
    {
        public int id { get; set; }
        public string name { get; set; }
        public IList<AppearanceClean> appearances { get; set; }
    }

    public class AppearanceClean
    {
        public double StartTime { get; set; }
        public double EndTime { get; set; }
    }

    public class Duration
    {
        public string time { get; set; }
        public double seconds { get; set; }
    }

    public class Label
    {
        public int id { get; set; }
        public string name { get; set; }
        public string referenceId { get; set; }
        public string language { get; set; }
        public IList<Instance> instances { get; set; }
    }

    public class Instance
    {
        public string start { get; set; }
        public string end { get; set; }
        public string duration { get; set; }
        public string thumbnailId { get; set; }
    }

    public class Scene
    {
        public int id { get; set; }
        public IList<Instance> instances { get; set; }
    }

    public class KeyFrame
    {
        public int id { get; set; }
        public IList<Instance> instances { get; set; }
    }

    public class Shot
    {
        public int id { get; set; }
        public IList<KeyFrame> keyFrames { get; set; }
        public IList<Instance> instances { get; set; }
        public IList<string> tags { get; set; }
    }

    public class Block
    {
        public int id { get; set; }
        public IList<Instance> instances { get; set; }
    }

    public class Insights
    {
        public IList<Label> labels { get; set; }
        public IList<Scene> scenes { get; set; }
        public IList<Shot> shots { get; set; }
        public IList<Block> blocks { get; set; }
    }

    public class Video
    {
        public Insights insights { get; set; }
        public string thumbnailId { get; set; }
    }

    public class Range
    {
        public string start { get; set; }
        public string end { get; set; }
    }

    public class VideoIndex
    {
        public IList<Video> videos { get; set; }
    }
}