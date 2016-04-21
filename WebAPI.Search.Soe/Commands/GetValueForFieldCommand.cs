using System;
using ESRI.ArcGIS.Geodatabase;
using Soe.Common.Infastructure.Commands;

namespace WebAPI.Search.Soe.Commands
{
    public class GetValueForFieldCommand : Command<object>
    {
        public GetValueForFieldCommand(string attributeName, IFields fields, IObject row)
        {
            AttributeName = attributeName;
            Fields = fields;
            Row = row;
        }

        public string AttributeName { get; set; }
        public IFields Fields { get; set; }
        public IObject Row { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, X: {1}", "GetValueForFieldCommand", AttributeName);
        }

        protected override void Execute()
        {
            var findField = Fields.FindField(AttributeName.Trim());

            if (findField < 0)
            {
                throw new ArgumentException(AttributeName + " "); 
            }
            
            Result = Row.Value[findField];
        }
    }
}