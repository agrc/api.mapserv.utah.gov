using api.mapserv.utah.gov.Models.Constants;

namespace api.mapserv.utah.gov.Models.RequestOptions
{
  public class OptionBase
  {
      /// <summary>
      /// Initializes a new instance of the <see cref="Options"/> class.
      /// </summary>
      public OptionBase()
      {
          JsonFormat = JsonFormat.None;
      }

      /// <summary>
      ///     Gets or sets the json format for the geometry response.
      /// </summary>
      /// <value>
      ///     The json format.
      /// </value>
      public JsonFormat JsonFormat { get; set; }
  }
}
