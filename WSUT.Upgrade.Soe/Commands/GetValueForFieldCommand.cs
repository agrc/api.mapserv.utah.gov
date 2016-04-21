using ESRI.ArcGIS.Geodatabase;
using Soe.Common.Infastructure.Commands;

namespace Wsut.Upgrade.Soe.Commands
{
    public class GetValueForFieldCommand : Command<object>
    {
        public GetValueForFieldCommand(string x, IFields fields, IObject row)
        {
            X = x;
            Fields = fields;
            Row = row;
        }

        public string X { get; set; }
        public IFields Fields { get; set; }
        public IObject Row { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, X: {1}", "GetValueForFieldCommand", X);
        }

        protected override void Execute()
        {
            var findField = Fields.FindField(X.Trim());

            Result = findField < 0 ? string.Format("Attribute {0} not found.", X) : Row.Value[findField];
        }
    }
}