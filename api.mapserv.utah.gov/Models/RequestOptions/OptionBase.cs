using api.mapserv.utah.gov.Models.Constants;

namespace api.mapserv.utah.gov.Models.RequestOptions
{
  public class OptionBase
  {
      public OptionBase()
      {
          Format = JsonFormat.None;
      }

      /// <summary>
      ///     Gets or sets the json format for the geometry response.
      /// </summary>
      /// <value>
      ///     The json format.
      /// </value>
      public JsonFormat Format { get; set; }
  }
}
