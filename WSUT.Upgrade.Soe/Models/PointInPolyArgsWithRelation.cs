namespace WSUT.Upgrade.Soe.Models
{
    internal class PointInPolyArgsWithRelation : PointInPolyArgs
    {
        public PointInPolyArgsWithRelation(string layerName, double utmx, double utmy, string[] attributeList,
                                           string[] relatedAttributes, string relatedLayer)
            : base(layerName, utmx, utmy, attributeList)
        {
            RelatedAttributeList = relatedAttributes;
            RelationLayerName = relatedLayer;
        }

        public string RelationLayerName { get; set; }
        public string[] RelatedAttributeList { get; set; }
    }
}