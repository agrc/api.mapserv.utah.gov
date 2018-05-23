using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using api.mapserv.utah.gov.Models.Constants;

namespace api.mapserv.utah.gov.Models
{
  public abstract class AddressBase
  {
      protected AddressBase()
      {
      }

      protected AddressBase(string inputAddress, int? houseNumber, double milepost, int poBox,
                            Direction prefixDirection, string streetName, StreetType streetType,
                            Direction suffixDirection, int zip4, int? zip5, bool isHighway, bool isPoBox)
      {
          InputAddress = inputAddress;
          HouseNumber = houseNumber;
          Milepost = milepost;
          PoBox = poBox;
          PrefixDirection = prefixDirection;
          StreetName = streetName;
          StreetType = streetType;
          SuffixDirection = suffixDirection;
          Zip4 = zip4;
          Zip5 = zip5;
          IsHighway = isHighway;
          IsPoBox = isPoBox;
      }

      public string InputAddress { get; set; }

      public int? HouseNumber { get; set; }

      public double Milepost { get; set; }

      public int PoBox { get; set; }

      public Direction PrefixDirection { get; set; }

      public string StreetName { get; set; }

      public StreetType StreetType { get; set; }

      public Direction SuffixDirection { get; set; }

      public int Zip4 { get; set; }

      public int? Zip5 { get; set; }

      public bool IsHighway { get; set; }

      public bool IsPoBox { get; set; }

      public virtual string ReversalAddress
      {
          get
          {
              var address = string.Format("{2} {3} {4} {0} {1}", HouseNumber, PrefixDirection, StreetName,
                                          SuffixDirection, StreetType);

              address = address.Replace("None", "");

              var regex = new Regex(@"[ ]{2,}", RegexOptions.None);
              address = regex.Replace(address, @" ");

              return address.Trim();
          }
      }

      public virtual bool IsNumericStreetName()
      {
          if (string.IsNullOrEmpty(StreetName))
          {
              return false;
          }

          return int.TryParse(StreetName, out int _);
        }

      public virtual bool HasPrefix()
      {
          return PrefixDirection != Direction.None;
      }

      public virtual bool IsReversal()
      {
          if (string.IsNullOrEmpty(StreetName))
          {
              return false;
          }

          if (!IsNumericStreetName())
          {
              return false;
          }

          var r = new Regex(@"(1|2|3|4|6|7|8|9)$");

          return r.IsMatch(StreetName);
      }

      public virtual bool PossibleReversal()
      {
          if (string.IsNullOrEmpty(StreetName))
          {
              return false;
          }

          int num;
          var stepOne = false;

          if (int.TryParse(StreetName, out num))
          {
              stepOne = num % 5 == 0;
          }

          var stepTwo = HouseNumber % 5 == 0;

          return (stepOne && stepTwo);
      }

      public virtual bool IsIntersection()
      {
          var regex = new Regex(@"\band\b", RegexOptions.IgnoreCase);

          return regex.IsMatch(InputAddress);
      }

      public virtual bool IsMachable()
      {
          if (IsIntersection())
          {
              return true;
          }

          if (!HouseNumber.HasValue)
          {
              return SuffixDirection != Direction.None;
          }

          return true;
      }

      public abstract override string ToString();

      public virtual AddressBase DeepClone()
      {
          using (var stream = new MemoryStream())
          {
              var formatter = new BinaryFormatter();
              formatter.Serialize(stream, this);
              stream.Position = 0;

              return (AddressBase)formatter.Deserialize(stream);
          }
      }
  }
}
