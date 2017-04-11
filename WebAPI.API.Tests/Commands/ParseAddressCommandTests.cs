using NUnit.Framework;
using WebAPI.API.Commands.Address;
using WebAPI.Common.Executors;
using WebAPI.Domain;
using WebAPI.Domain.Addresses;

namespace WebAPI.API.Tests.Commands
{
    public class ParseAddressCommandTests
    {
        [TestFixture]
        public class GeocodingServiceTests
        {
            [TestFixtureSetUp]
            public void SetupGlobal()
            {
                CacheConfig.BuildCache();
            }

            [SetUp]
            public void Setup()
            {
                _parseAddressCommand = new ParseAddressCommand();
            }

            private ParseAddressCommand _parseAddressCommand;

            private CleansedAddress Execute()
            {
                var address = CommandExecutor.ExecuteCommand(_parseAddressCommand);
                return address;
            }

            [TestFixture]
            public class PoBoxes
            {
                [SetUp]
                public void Setup()
                {
                    _parseAddressCommand = new ParseAddressCommand();
                }

                private ParseAddressCommand _parseAddressCommand;

                private CleansedAddress Execute()
                {
                    var address = CommandExecutor.ExecuteCommand(_parseAddressCommand);
                    return address;
                }

                [Test]
                public void PoBoxWithPeriods()
                {
                    _parseAddressCommand.SetStreet("P O Box 123");
                    var address = Execute();
                    Assert.That(address.IsPoBox, Is.True);
                    Assert.That(address.PoBox, Is.EqualTo(123));
                    Assert.That(address.HouseNumber, Is.Null);
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StreetName, Is.EqualTo("P.O. Box"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("P.O. Box 123").IgnoreCase);
                    Assert.That(address.IsReversal(), Is.False);
                }

                [Test]
                public void PoBoxWithoutPeriods()
                {
                    _parseAddressCommand.SetStreet("PO Box 123");
                    var address = Execute();
                    Assert.That(address.IsPoBox, Is.True);
                    Assert.That(address.PoBox, Is.EqualTo(123));
                    Assert.That(address.HouseNumber, Is.Null);
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StreetName, Is.EqualTo("P.O. Box"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("P.O. Box 123").IgnoreCase);
                    Assert.That(address.IsReversal(), Is.False);
                }

                [Test]
                public void PoBoxWithoutSpace()
                {
                    _parseAddressCommand.SetStreet("POBox 123");
                    var address = Execute();
                    Assert.That(address.IsPoBox, Is.True);
                    Assert.That(address.PoBox, Is.EqualTo(123));
                    Assert.That(address.HouseNumber, Is.Null);
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StreetName, Is.EqualTo("P.O. Box"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("P.O. Box 123").IgnoreCase);
                    Assert.That(address.IsReversal(), Is.False);
                }

                [Test]
                public void PoBoxWithPeriodsWithoutSpace()
                {
                    _parseAddressCommand.SetStreet("P.O.Box 123");
                    var address = Execute();
                    Assert.That(address.IsPoBox, Is.True);
                    Assert.That(address.PoBox, Is.EqualTo(123));
                    Assert.That(address.HouseNumber, Is.Null);
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StreetName, Is.EqualTo("P.O. Box"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("P.O. Box 123").IgnoreCase);
                    Assert.That(address.IsReversal(), Is.False);
                }
                
                [Test]
                public void NotAPoBox()
                {
                    _parseAddressCommand.SetStreet("123 west house st");
                    var address = Execute();
                    Assert.That(address.IsPoBox, Is.False);
                    Assert.That(address.PoBox, Is.EqualTo(0));
                    Assert.That(address.HouseNumber, Is.EqualTo(123));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.West));
                    Assert.That(address.StreetName, Is.EqualTo("house"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.Street));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("123 west house street").IgnoreCase);
                    Assert.That(address.IsReversal(), Is.False);
                }
            }

            [TestFixture]
            public class UnitsWithNoTypeDesignation
            {
                [SetUp]
                public void Setup()
                {
                    _parseAddressCommand = new ParseAddressCommand();
                }

                private ParseAddressCommand _parseAddressCommand;

                private CleansedAddress Execute()
                {
                    var address = CommandExecutor.ExecuteCommand(_parseAddressCommand);
                    return address;
                }

                [Test]
                public void SingleNumber()
                {
                    _parseAddressCommand.SetStreet("625 NORTH REDWOOD ROAD 6");
                    var address = Execute();
                    Assert.That(address.HouseNumber, Is.EqualTo(625));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.North));
                    Assert.That(address.StreetName, Is.EqualTo("REDWOOD"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.Road));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("625 north redwood road").IgnoreCase);


                    _parseAddressCommand.SetStreet("295 North 120 West 1");
                    address = Execute();
                    Assert.That(address.HouseNumber, Is.EqualTo(295));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.North));
                    Assert.That(address.StreetName, Is.EqualTo("120"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.West));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("295 north 120 west").IgnoreCase);

                }

                [Test]
                public void SingleNumberWithDigitAfter()
                {
                    _parseAddressCommand.SetStreet("625 NORTH REDWOOD ROAD 6D");
                    var address = Execute();
                    Assert.That(address.HouseNumber, Is.EqualTo(625));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.North));
                    Assert.That(address.StreetName, Is.EqualTo("REDWOOD"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.Road));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("625 north redwood road").IgnoreCase);

                    _parseAddressCommand.SetStreet("295 North 120 West 1F");
                    address = Execute();
                    Assert.That(address.HouseNumber, Is.EqualTo(295));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.North));
                    Assert.That(address.StreetName, Is.EqualTo("120"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.West));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("295 north 120 west").IgnoreCase);
                }

                [Test]
                public void PoundSigns()
                {
                    // and 1991 e westminister unit# 800
                    _parseAddressCommand.SetStreet("1991 e westminister # 800");
                    var address = Execute();
                    Assert.That(address.HouseNumber, Is.EqualTo(1991));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.East));
                    Assert.That(address.StreetName, Is.EqualTo("WESTMINISTER").IgnoreCase);
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("1991 east westminister").IgnoreCase);
                    address = null;

                    _parseAddressCommand.SetStreet("1992 e westminister unit# 800");
                    address = Execute();
                    Assert.That(address.HouseNumber, Is.EqualTo(1992));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.East));
                    Assert.That(address.StreetName, Is.EqualTo("WESTMINISTER").IgnoreCase);
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("1992 east westminister").IgnoreCase);
                    address = null;

                    _parseAddressCommand.SetStreet("1990 e westminister #800");
                    address = Execute();
                    Assert.That(address.HouseNumber, Is.EqualTo(1990));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.East));
                    Assert.That(address.StreetName, Is.EqualTo("WESTMINISTER").IgnoreCase);
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("1990 east westminister").IgnoreCase);
                    address = null;

                    _parseAddressCommand.SetStreet("1991 S 800 E #34");
                    address = Execute();
                    Assert.That(address.HouseNumber, Is.EqualTo(1991));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.South));
                    Assert.That(address.StreetName, Is.EqualTo("800").IgnoreCase);
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.East));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("1991 South 800 East").IgnoreCase);
                    address = null;
                }

                [Test]
                public void FunkyApartmentThing()
                {
                    _parseAddressCommand.SetStreet("680 NORTH MAIN STREET #A-8");
                    var address = Execute();
                    Assert.That(address.HouseNumber, Is.EqualTo(680));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.North));
                    Assert.That(address.StreetName, Is.EqualTo("MAIN"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.Street));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("680 NORTH MAIN STREET").IgnoreCase);
                }
            }

            [Test]
            public void Abbreviated_Highways()
            {
                //arrange
                _parseAddressCommand.SetStreet("401 N HWY 68");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(401, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.North, address.PrefixDirection, "Prefix");
                Assert.AreEqual("Highway 68", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.None, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("401 North Highway 68"));
                address = null;

                //arrange
                _parseAddressCommand.SetStreet("401 N SR 68");

                //act
                address = Execute();

                //assert
                Assert.AreEqual(401, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.North, address.PrefixDirection, "Prefix");
                Assert.AreEqual("Highway 68", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.None, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("401 North Highway 68"));
                address = null;

                //arrange
                _parseAddressCommand.SetStreet("401 N US 89");

                //act
                address = Execute();

                //assert
                Assert.AreEqual(401, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.North, address.PrefixDirection, "Prefix");
                Assert.AreEqual("Highway 89", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.None, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("401 North Highway 89"));
                address = null;

                //arrange
                _parseAddressCommand.SetStreet("401 N U.S. 89");

                //act
                address = Execute();

                //assert
                Assert.AreEqual(401, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.North, address.PrefixDirection, "Prefix");
                Assert.AreEqual("Highway 89", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.None, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("401 North Highway 89"));
                address = null;
            }

            [TestFixture]
            public class SecondaryAddressUnits
            {
                [SetUp]
                public void Setup()
                {
                    _parseAddressCommand = new ParseAddressCommand();
                }

                private ParseAddressCommand _parseAddressCommand;

                private CleansedAddress Execute()
                {
                    var address = CommandExecutor.ExecuteCommand(_parseAddressCommand);
                    return address;
                }

                [Test]
                public void ApartmentsLotsUnitsAndOtherWeirdStuff()
                {
                    _parseAddressCommand.SetStreet("435 N MAIN ST bsmt");
                    var address = Execute();

                    Assert.That(address.HouseNumber, Is.EqualTo(435));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.North));
                    Assert.That(address.StreetName, Is.EqualTo("MAIN"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.Street));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("435 North MAIN STreet").IgnoreCase);
                    address = null;

                    _parseAddressCommand.SetStreet("435 N MAIN ST PENTHOUSE");
                    address = Execute();

                    Assert.That(address.HouseNumber, Is.EqualTo(435));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.North));
                    Assert.That(address.StreetName, Is.EqualTo("MAIN"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.Street));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("435 North MAIN STreet").IgnoreCase);
                    address = null;

                    _parseAddressCommand.SetStreet("436 N MAIN ST APT 7");
                    address = Execute();

                    Assert.That(address.HouseNumber, Is.EqualTo(436));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.North));
                    Assert.That(address.StreetName, Is.EqualTo("MAIN"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.Street));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("436 North MAIN STreet").IgnoreCase);
                    address = null;

                    _parseAddressCommand.SetStreet("3700 W 150 N LOT 122");
                    address = Execute();

                    Assert.That(address.HouseNumber, Is.EqualTo(3700));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.West));
                    Assert.That(address.StreetName, Is.EqualTo("150"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.North));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("3700 West 150 North").IgnoreCase);
                    address = null;

                    _parseAddressCommand.SetStreet("222 N 1200 W TRLR 44");
                    address = Execute();

                    Assert.That(address.HouseNumber, Is.EqualTo(222));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.North));
                    Assert.That(address.StreetName, Is.EqualTo("1200"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.West));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("222 North 1200 West").IgnoreCase);
                    address = null;

                    _parseAddressCommand.SetStreet("1015 S RIVER RD UNIT 42");
                    address = Execute();

                    Assert.That(address.HouseNumber, Is.EqualTo(1015));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.South));
                    Assert.That(address.StreetName, Is.EqualTo("RIVER"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.Road));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("1015 South RIVER RoaD").IgnoreCase);
                    address = null;

                    _parseAddressCommand.SetStreet("859 PINEWAY DR APT 11E");
                    address = Execute();

                    Assert.That(address.HouseNumber, Is.EqualTo(859));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StreetName, Is.EqualTo("PINEWAY"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.Drive));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("859 PINEWAY DRive").IgnoreCase);
                    address = null;

                    _parseAddressCommand.SetStreet("902 E 500 S APT F");
                    address = Execute();

                    Assert.That(address.HouseNumber, Is.EqualTo(902));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.East));
                    Assert.That(address.StreetName, Is.EqualTo("500"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.South));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("902 East 500 South").IgnoreCase);
                    address = null;

                    _parseAddressCommand.SetStreet("1388 W 699 S Bsmt");
                    address = Execute();

                    Assert.That(address.HouseNumber, Is.EqualTo(1388));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.West));
                    Assert.That(address.StreetName, Is.EqualTo("699"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.South));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("1388 West 699 South").IgnoreCase);
                    address = null;

                    _parseAddressCommand.SetStreet("1389 W 699 S Suite 900");
                    address = Execute();

                    Assert.That(address.HouseNumber, Is.EqualTo(1389));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.West));
                    Assert.That(address.StreetName, Is.EqualTo("699"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.South));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("1389 West 699 South").IgnoreCase);
                    address = null;

                    _parseAddressCommand.SetStreet("1387 W 699 S Ste 900");
                    address = Execute();

                    Assert.That(address.HouseNumber, Is.EqualTo(1387));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.West));
                    Assert.That(address.StreetName, Is.EqualTo("699"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.South));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("1387 West 699 South").IgnoreCase);
                    address = null;

                    _parseAddressCommand.SetStreet("1387 W 699 S Ste B150");
                    address = Execute();

                    Assert.That(address.HouseNumber, Is.EqualTo(1387));
                    Assert.That(address.PrefixDirection, Is.EqualTo(Direction.West));
                    Assert.That(address.StreetName, Is.EqualTo("699"));
                    Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                    Assert.That(address.SuffixDirection, Is.EqualTo(Direction.South));
                    Assert.That(address.StandardizedAddress, Is.EqualTo("1387 West 699 South").IgnoreCase);
                    address = null;
                }
            }

            [Test]
            public void Suffix_in_StreetName()
            {
                _parseAddressCommand.SetStreet("4463 SUMMER PLACE CIR");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(4463, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.None, address.PrefixDirection, "Prefix");
                Assert.AreEqual("SUMMER PLACE", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Circle, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("4463 SUMMER PLACE Circle"));
                address = null;
            }

            [Test]
            public void Suffix_abbreviation_in_StreetName()
            {
                _parseAddressCommand.SetStreet("4463 SUMMER PL CIR");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(4463, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.None, address.PrefixDirection, "Prefix");
                Assert.AreEqual("SUMMER PL", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Circle, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("4463 SUMMER PL Circle"));
                address = null;
            }

            [Test]
            public void Direction_in_StreetName()
            {
                //arrange
                _parseAddressCommand.SetStreet("478 S WEST FRONTAGE RD");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(478, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.South, address.PrefixDirection, "Prefix");
                Assert.AreEqual("WEST FRONTAGE", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Road, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("478 South WEST FRONTAGE Road"));
                address = null;
            }

            [Test]
            public void GetWordIndex_findsPatternInWord_AndReturnsTheWordIndex()
            {
                //arrange
                const string words = "My house number is 23";

                //act
                var firstWord = ParseAddressCommand.GetWordIndex(1, words);
                var secondWord = ParseAddressCommand.GetWordIndex(5, words);
                var thirdWord = ParseAddressCommand.GetWordIndex(15, words);
                var fourthWord = ParseAddressCommand.GetWordIndex(17, words);
                var fifthWord = ParseAddressCommand.GetWordIndex(21, words);
                var notFound = ParseAddressCommand.GetWordIndex(45, words);

                //assert
                Assert.AreEqual(firstWord, 0, "first word");
                Assert.AreEqual(secondWord, 1, "2nd");
                Assert.AreEqual(thirdWord, 2, "3rd");
                Assert.AreEqual(fourthWord, 3, "4th");
                Assert.AreEqual(fifthWord, 4, "5th");
                Assert.AreEqual(notFound, -1, "not found");
            }

            [Test]
            public void HouseNumberOrNumericStreetNameAndDirectionalConcatentation()
            {
                _parseAddressCommand.SetStreet("1991S 800E");

                var address = Execute();

                Assert.That(address.HouseNumber, Is.EqualTo(1991));
                Assert.That(address.PrefixDirection, Is.EqualTo(Direction.South));
                Assert.That(address.StreetName, Is.EqualTo("800"));
                Assert.That(address.SuffixDirection, Is.EqualTo(Direction.East));
                Assert.That(address.StandardizedAddress, Is.EqualTo("1991 South 800 East"));
                address = null;

                _parseAddressCommand.SetStreet("1991S 900 E");

                address = Execute();

                Assert.That(address.HouseNumber, Is.EqualTo(1991));
                Assert.That(address.PrefixDirection, Is.EqualTo(Direction.South));
                Assert.That(address.StreetName, Is.EqualTo("900"));
                Assert.That(address.SuffixDirection, Is.EqualTo(Direction.East));
                Assert.That(address.StandardizedAddress, Is.EqualTo("1991 South 900 East"));
                address = null;
            }

            [Test, Explicit]
            public void Intersection_addresses()
            {
                //arrange
                _parseAddressCommand.SetStreet("1100 East & 1300 South");
                _parseAddressCommand.SetStreet("Roosevelt & McClelland");
                _parseAddressCommand.SetStreet("1400 E & Roosevelt");
                _parseAddressCommand.SetStreet("Roosevelt & 1400 East");


                //act


                //assert
            }

            [Test]
            public void MultipleStreetTypesInStretName()
            {
                //arrange
                _parseAddressCommand.SetStreet("5301 w jacob hill cir");

                var address = Execute();

                //assert
                Assert.AreEqual(5301, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.West, address.PrefixDirection, "Prefix");
                Assert.AreEqual("jacob hill", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Circle, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("5301 West jacob hill circle").IgnoreCase);
                address = null;
            }

            [Test]
            public void NonStandard_NumericStreet_Names()
            {
                //arrange
                _parseAddressCommand.SetStreet("1048 W 1205 N");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(1048, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.West, address.PrefixDirection, "Prefix");
                Assert.AreEqual("1205", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.None, address.StreetType, "street type");
                Assert.AreEqual(Direction.North, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("1048 West 1205 North"));
                address = null;

                //arrange
                _parseAddressCommand.SetStreet("2139 N 50 W");

                //act
                address = Execute();

                //assert
                Assert.AreEqual(2139, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.North, address.PrefixDirection, "Prefix");
                Assert.AreEqual("50", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.None, address.StreetType, "street type");
                Assert.AreEqual(Direction.West, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("2139 North 50 West"));
                address = null;

            }

            [Test]
            public void One_Character_StreetNames()
            {
                //if you dont' have a street name but you have a prefix direction then the
                //prefix diretion is probably the street name.
                //arrange
                _parseAddressCommand.SetStreet("168 N ST");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(168, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.None, address.PrefixDirection, "Prefix");
                Assert.AreEqual("N", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Street, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("168 N Street"));
                address = null;

                //arrange
                _parseAddressCommand.SetStreet("168 N N ST");

                //act
                address = Execute();

                //assert
                Assert.AreEqual(168, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.North, address.PrefixDirection, "Prefix");
                Assert.AreEqual("N", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Street, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("168 North N Street"));
                address = null;

            }

            [Test]
            public void One_Digit_HouseNumbers()
            {
                //arrange
                _parseAddressCommand.SetStreet("5 Cedar Ave");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(5, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.None, address.PrefixDirection, "Prefix");
                Assert.AreEqual("Cedar", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Avenue, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("5 Cedar avenue").IgnoreCase);

                //Assert.AreEqual(true, address.IsNumericStreetName(), "Is numeric");
                //Assert.AreEqual(false, address.HasPrefix(), "Has Prefix");
                //Assert.AreEqual(false, address.IsReversal(), "Likely reversal");
                //Assert.AreEqual(false, address.PossibleReversal(), "try reversal");
                //Assert.AreEqual(true, address.NeedsPrefix, "Needs Prefix");
            }

            [Test]
            public void Ordinal_street_addresses()
            {
                //arrange
                _parseAddressCommand.SetStreet("1238 E 1ST Avenue");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(1238, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.East, address.PrefixDirection, "Prefix");
                Assert.AreEqual("1ST", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Avenue, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("1238 East 1ST Avenue").IgnoreCase);

                //arrange
                _parseAddressCommand.SetStreet("1238 E FIRST Avenue");

                //act
                address = Execute();

                //assert
                Assert.AreEqual(1238, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.East, address.PrefixDirection, "Prefix");
                Assert.AreEqual("FIRST", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Avenue, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("1238 East First Avenue").IgnoreCase);

                //arrange
                _parseAddressCommand.SetStreet("1238 E 2ND Avenue");

                //act
                address = Execute();

                //assert
                Assert.AreEqual(1238, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.East, address.PrefixDirection, "Prefix");
                Assert.AreEqual("2ND", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Avenue, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("1238 East 2nd avenue").IgnoreCase);

                //arrange
                _parseAddressCommand.SetStreet("1238 E 3RD Avenue");

                //act
                address = Execute();

                //assert
                Assert.AreEqual(1238, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.East, address.PrefixDirection, "Prefix");
                Assert.AreEqual("3RD", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Avenue, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("1238 East 3rd avenue").IgnoreCase);


                //arrange
                _parseAddressCommand.SetStreet("1573 24TH Street");

                //act
                address = Execute();

                //assert
                Assert.AreEqual(1573, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.None, address.PrefixDirection, "Prefix");
                Assert.AreEqual("24TH", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Street, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("1573 24th street").IgnoreCase);

            }

            [Test]
            public void ParserDeterminesCorrectElements_for_B()
            {
                _parseAddressCommand.SetStreet("400 S 532 E");

                var address = Execute();

                Assert.AreEqual(400, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.South, address.PrefixDirection, "Prefix");
                Assert.AreEqual("532", address.StreetName, "Streen Name");
                Assert.AreEqual(Direction.East, address.SuffixDirection, "Suffix");
                Assert.AreEqual(true, address.IsReversal(), "likely reversal");
                Assert.AreEqual(false, address.PossibleReversal(), "Try reversal");
                Assert.AreEqual(true, address.HasPrefix(), "Has Prefix");
                Assert.AreEqual(true, address.IsNumericStreetName(), "Is numeric");
                //Assert.AreEqual(false, address.NeedsPrefix, "Needs Prefix");
            }

            [Test]
            public void ParserDeterminesCorrectElements_for_C()
            {
                //arrange
                _parseAddressCommand.SetStreet("5625 S 995 E");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(5625, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.South, address.PrefixDirection, "Prefix");
                Assert.AreEqual("995", address.StreetName, "Streen Name");
                Assert.AreEqual(Direction.East, address.SuffixDirection, "Suffix");
                Assert.AreEqual(false, address.IsReversal(), "Likely reversal");
                Assert.AreEqual(true, address.PossibleReversal(), "try reversal");
                Assert.AreEqual(true, address.HasPrefix(), "Has Prefix");
                Assert.AreEqual(true, address.IsNumericStreetName(), "Is numeric");
                //Assert.AreEqual(true, address.NeedsPrefix, "Needs Prefix");
            }

            [Test]
            public void ParserDeterminesCorrectElements_for_D()
            {
                //arrange
                _parseAddressCommand.SetStreet("372 North 600 East");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(372, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.North, address.PrefixDirection, "Prefix");
                Assert.AreEqual("600", address.StreetName, "Streen Name");
                Assert.AreEqual(Direction.East, address.SuffixDirection, "Suffix");
                Assert.AreEqual(true, address.IsNumericStreetName(), "Is numeric");
                Assert.AreEqual(true, address.HasPrefix(), "Has Prefix");
                Assert.AreEqual(false, address.IsReversal(), "Likely reversal");
                Assert.AreEqual(false, address.PossibleReversal(), "try reversal");
                //Assert.AreEqual(true, address.NeedsPrefix, "Needs Prefix");
            }

            [Test]
            public void ParserDeterminesCorrectElements_for_E()
            {
                //arrange
                _parseAddressCommand.SetStreet("30 WEST 300 NORTH");

                //act
                var address = Execute();


                //assert
                Assert.AreEqual(30, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.West, address.PrefixDirection, "Prefix");
                Assert.AreEqual("300", address.StreetName, "Streen Name");
                Assert.AreEqual(Direction.North, address.SuffixDirection, "Suffix");
                Assert.AreEqual(true, address.IsNumericStreetName(), "Is numeric");
                Assert.AreEqual(true, address.HasPrefix(), "Has Prefix");
                Assert.AreEqual(false, address.IsReversal(), "Likely reversal");
                Assert.AreEqual(true, address.PossibleReversal(), "try reversal");
                //Assert.AreEqual(true, address.NeedsPrefix, "Needs Prefix");
            }

            [Test]
            public void ParserDeterminesCorrectElements_for_F()
            {
                //arrange
                _parseAddressCommand.SetStreet("126 E 400 N");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(126, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.East, address.PrefixDirection, "Prefix");
                Assert.AreEqual("400", address.StreetName, "Streen Name");
                Assert.AreEqual(Direction.North, address.SuffixDirection, "Suffix");
                Assert.AreEqual(true, address.IsNumericStreetName(), "Is numeric");
                Assert.AreEqual(true, address.HasPrefix(), "Has Prefix");
                Assert.AreEqual(false, address.IsReversal(), "Likely reversal");
                Assert.AreEqual(false, address.PossibleReversal(), "try reversal");
                //Assert.AreEqual(true, address.NeedsPrefix, "Needs Prefix");
            }

            [Test]
            public void ParserDeterminesCorrectElements_for_G()
            {
                //arrange
                _parseAddressCommand.SetStreet("270 South 1300 East");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(270, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.South, address.PrefixDirection, "Prefix");
                Assert.AreEqual("1300", address.StreetName, "Streen Name");
                Assert.AreEqual(Direction.East, address.SuffixDirection, "Suffix");
                Assert.AreEqual(true, address.IsNumericStreetName(), "Is numeric");
                Assert.AreEqual(true, address.HasPrefix(), "Has Prefix");
                Assert.AreEqual(false, address.IsReversal(), "Likely reversal");
                Assert.AreEqual(true, address.PossibleReversal(), "try reversal");
                //Assert.AreEqual(true, address.NeedsPrefix, "Needs Prefix");
            }

            [Test]
            public void ParserDeterminesCorrectElements_for_H()
            {
                //arrange
                _parseAddressCommand.SetStreet("126 W SEGO LILY DR");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(126, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.West, address.PrefixDirection, "Prefix");
                Assert.AreEqual("SEGO LILY", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Drive, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.AreEqual(false, address.IsNumericStreetName(), "Is numeric");
                Assert.AreEqual(true, address.HasPrefix(), "Has Prefix");
                Assert.AreEqual(false, address.IsReversal(), "Likely reversal");
                Assert.AreEqual(false, address.PossibleReversal(), "try reversal");
                //Assert.AreEqual(true, address.NeedsPrefix, "Needs Prefix");
            }

            [Test]
            public void ParserDeterminesCorrectElements_for_I()
            {
                //arrange
                _parseAddressCommand.SetStreet("261 E MUELLER PARK RD");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(261, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.East, address.PrefixDirection, "Prefix");
                Assert.AreEqual("MUELLER PARK", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Road, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.AreEqual(false, address.IsNumericStreetName(), "Is numeric");
                Assert.AreEqual(true, address.HasPrefix(), "Has Prefix");
                Assert.AreEqual(false, address.IsReversal(), "Likely reversal");
                Assert.AreEqual(false, address.PossibleReversal(), "try reversal");
                //Assert.AreEqual(true, address.NeedsPrefix, "Needs Prefix");
            }

            [Test]
            public void ParserDeterminesCorrectElements_for_J()
            {
                //arrange
                _parseAddressCommand.SetStreet("17 S VENICE MAIN ST");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(17, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.South, address.PrefixDirection, "Prefix");
                Assert.AreEqual("VENICE MAIN", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Street, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.AreEqual(false, address.IsNumericStreetName(), "Is numeric");
                Assert.AreEqual(true, address.HasPrefix(), "Has Prefix");
                Assert.AreEqual(false, address.IsReversal(), "Likely reversal");
                Assert.AreEqual(false, address.PossibleReversal(), "try reversal");
                //Assert.AreEqual(true, address.NeedsPrefix, "Needs Prefix");
            }

            [Test]
            public void ParserDeterminesCorrectElements_for_K()
            {
                //arrange
                _parseAddressCommand.SetStreet("20 W Center St");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(20, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.West, address.PrefixDirection, "Prefix");
                Assert.AreEqual("Center", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Street, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.AreEqual(false, address.IsNumericStreetName(), "Is numeric");
                Assert.AreEqual(true, address.HasPrefix(), "Has Prefix");
                Assert.AreEqual(false, address.IsReversal(), "Likely reversal");
                Assert.AreEqual(false, address.PossibleReversal(), "try reversal");
                //Assert.AreEqual(true, address.NeedsPrefix, "Needs Prefix");
            }

            [Test]
            public void ParserDeterminesCorrectElements_for_L()
            {
                //arrange
                _parseAddressCommand.SetStreet("9314 ALVEY LN");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(9314, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.None, address.PrefixDirection, "Prefix");
                Assert.AreEqual("ALVEY", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Lane, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.AreEqual(false, address.IsNumericStreetName(), "Is numeric");
                Assert.AreEqual(false, address.HasPrefix(), "Has Prefix");
                Assert.AreEqual(false, address.IsReversal(), "Likely reversal");
                Assert.AreEqual(false, address.PossibleReversal(), "try reversal");
                //Assert.AreEqual(true, address.NeedsPrefix, "Needs Prefix");
            }

            [Test]
            public void ParserDeterminesCorrectElements_for_M()
            {
                //arrange
                _parseAddressCommand.SetStreet("167 DALY AVE");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(167, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.None, address.PrefixDirection, "Prefix");
                Assert.AreEqual("DALY", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Avenue, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.AreEqual(false, address.IsNumericStreetName(), "Is numeric");
                Assert.AreEqual(false, address.HasPrefix(), "Has Prefix");
                Assert.AreEqual(false, address.IsReversal(), "Likely reversal");
                Assert.AreEqual(false, address.PossibleReversal(), "try reversal");
                //Assert.AreEqual(true, address.NeedsPrefix, "Needs Prefix");
            }

            [Test]
            public void ParserDeterminesCorrectElements_for_N()
            {
                //arrange
                _parseAddressCommand.SetStreet("1147 MCDANIEL CIR");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(1147, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.None, address.PrefixDirection, "Prefix");
                Assert.AreEqual("MCDANIEL", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Circle, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.AreEqual(false, address.IsNumericStreetName(), "Is numeric");
                Assert.AreEqual(false, address.HasPrefix(), "Has Prefix");
                Assert.AreEqual(false, address.IsReversal(), "Likely reversal");
                Assert.AreEqual(false, address.PossibleReversal(), "try reversal");
                //Assert.AreEqual(true, address.NeedsPrefix, "Needs Prefix");
            }

            [Test]
            public void ParserDeterminesCorrectElements_for_O()
            {
                //arrange
                _parseAddressCommand.SetStreet("300 Walk St");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(300, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.None, address.PrefixDirection, "Prefix");
                Assert.AreEqual("Walk", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Street, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.AreEqual(false, address.IsNumericStreetName(), "Is numeric");
                Assert.AreEqual(false, address.HasPrefix(), "Has Prefix");
                Assert.AreEqual(false, address.IsReversal(), "Likely reversal");
                Assert.AreEqual(false, address.PossibleReversal(), "try reversal");
                //Assert.AreEqual(true, address.NeedsPrefix, "Needs Prefix");
            }

            [Test, Explicit]
            public void ParserDeterminesCorrectElements_for_P()
            {
                //arrange
                //_parseAddressCommand.SetStreet("500 S");

                ////act
                //var address = Execute();

                //assert
                //Assert.AreEqual(false, address.Machable, "Unmachable");
            }

            [Test]
            public void RealReversedAddress()
            {
                //arrange
                _parseAddressCommand.SetStreet("1625 SOUTH 672 EAST");

                //act
                var address = Execute();

                //assert
                Assert.That(address.ReversalAddress.ToUpperInvariant(),
                            Is.EqualTo("672 EAST 1625 SOUTH"));

                _parseAddressCommand.SetStreet("1400 NORTH 500 EAST");

                //act
                address = Execute();

                //assert
                Assert.That(address.ReversalAddress.ToUpperInvariant(),
                            Is.EqualTo("500 EAST 1400 NORTH"));
            }

            [Test]
            public void StreetTypeInStreetName()
            {
                //arrange
                _parseAddressCommand.SetStreet("180 N STATE ST");

                //act
                var address = Execute();

                //assert
                Assert.AreEqual(180, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.North, address.PrefixDirection, "Prefix");
                Assert.AreEqual("STATE", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Street, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("180 north state street").IgnoreCase);
                address = null;
            }

            [Test]
            public void TwoStreetTypesInName()
            {
                _parseAddressCommand.SetStreet("5301 W Jacob Hill Cir");
                var address = Execute();

                //assert
                Assert.AreEqual(5301, address.HouseNumber, "House Number");
                Assert.AreEqual(Direction.West, address.PrefixDirection, "Prefix");
                Assert.AreEqual("Jacob Hill", address.StreetName, "Streen Name");
                Assert.AreEqual(StreetType.Circle, address.StreetType, "street type");
                Assert.AreEqual(Direction.None, address.SuffixDirection, "Suffix");
                Assert.That(address.StandardizedAddress, Is.EqualTo("5301 West Jacob Hill Circle").IgnoreCase);
                address = null;
            }

            [Test, Explicit]
            public void TwoSuffixDirectionLikeApartment()
            {
                _parseAddressCommand.SetStreet("455 E APPLE BLOSSOM LN E");
                var address = Execute();

                Assert.That(address.HouseNumber, Is.EqualTo(455));
                Assert.That(address.PrefixDirection, Is.EqualTo(Direction.East));
                Assert.That(address.StreetName, Is.EqualTo("apple blossom").IgnoreCase);
                Assert.That(address.SuffixDirection, Is.EqualTo(Direction.East));
                Assert.That(address.StreetType, Is.EqualTo(StreetType.Lane));
                Assert.That(address.StandardizedAddress, Is.EqualTo("455 East apple blossom lane east").IgnoreCase);
            }

            [Test]
            public void OneSuffixDirectionLikeApartment()
            {
                _parseAddressCommand.SetStreet("582 DAMMERON VALLEY DR W");
                var address = Execute();

                Assert.That(address.HouseNumber, Is.EqualTo(582));
                Assert.That(address.PrefixDirection, Is.EqualTo(Direction.None));
                Assert.That(address.StreetName, Is.EqualTo("dammeron valley").IgnoreCase);
                Assert.That(address.SuffixDirection, Is.EqualTo(Direction.West));
                Assert.That(address.StreetType, Is.EqualTo(StreetType.Drive));
                Assert.That(address.StandardizedAddress, Is.EqualTo("582 Dammeron valley drive west").IgnoreCase);
            }

            [Test]
            public void PeriodInAddress()
            {
                _parseAddressCommand.SetStreet("326 east south temple st.");
                var address = Execute();

                Assert.That(address.HouseNumber, Is.EqualTo(326));
                Assert.That(address.PrefixDirection, Is.EqualTo(Direction.East));
                Assert.That(address.StreetName, Is.EqualTo("south temple").IgnoreCase);
                Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                Assert.That(address.StreetType, Is.EqualTo(StreetType.Street));
                Assert.That(address.StandardizedAddress, Is.EqualTo("326 east south temple street").IgnoreCase);
            }

            [Test]
            public void SillyAbbreviations()
            {
                _parseAddressCommand.SetStreet("9258 So 3090 W");
                var address = Execute();

                Assert.That(address.HouseNumber, Is.EqualTo(9258));
                Assert.That(address.PrefixDirection, Is.EqualTo(Direction.South));
                Assert.That(address.StreetName, Is.EqualTo("3090").IgnoreCase);
                Assert.That(address.SuffixDirection, Is.EqualTo(Direction.West));
                Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                Assert.That(address.StandardizedAddress, Is.EqualTo("9258 south 3090 west").IgnoreCase);

                _parseAddressCommand.SetStreet("4536 W 6090 So");
                address = Execute();

                Assert.That(address.HouseNumber, Is.EqualTo(4536));
                Assert.That(address.PrefixDirection, Is.EqualTo(Direction.West));
                Assert.That(address.StreetName, Is.EqualTo("6090").IgnoreCase);
                Assert.That(address.SuffixDirection, Is.EqualTo(Direction.South));
                Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                Assert.That(address.StandardizedAddress, Is.EqualTo("4536 west 6090 south").IgnoreCase);

                _parseAddressCommand.SetStreet("4536 W. 6090 So.");
                address = Execute();

                Assert.That(address.HouseNumber, Is.EqualTo(4536));
                Assert.That(address.PrefixDirection, Is.EqualTo(Direction.West));
                Assert.That(address.StreetName, Is.EqualTo("6090").IgnoreCase);
                Assert.That(address.SuffixDirection, Is.EqualTo(Direction.South));
                Assert.That(address.StreetType, Is.EqualTo(StreetType.None));
                Assert.That(address.StandardizedAddress, Is.EqualTo("4536 west 6090 south").IgnoreCase);
            }

            [Test]
            public void PointeAndPoint()
            {
                _parseAddressCommand.SetStreet("5974 FASHION POINT DR");
                var address = Execute();

                Assert.That(address.HouseNumber, Is.EqualTo(5974));
                Assert.That(address.PrefixDirection, Is.EqualTo(Direction.None));
                Assert.That(address.StreetName, Is.EqualTo("fashion point").IgnoreCase);
                Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                Assert.That(address.StreetType, Is.EqualTo(StreetType.Drive));
                Assert.That(address.StandardizedAddress, Is.EqualTo("5974 fashion point drive").IgnoreCase);
            }

            [Test]
            public void TownAndTowne()
            {
                _parseAddressCommand.SetStreet("1551 S RENAISSANCE TWN DR SUITE 420");
                var address = Execute();

                Assert.That(address.HouseNumber, Is.EqualTo(1551));
                Assert.That(address.PrefixDirection, Is.EqualTo(Direction.South));
                Assert.That(address.StreetName, Is.EqualTo("renaissance twn").IgnoreCase);
                Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                Assert.That(address.StreetType, Is.EqualTo(StreetType.Drive));
                Assert.That(address.StandardizedAddress, Is.EqualTo("1551 south renaissance twn drive").IgnoreCase);
            }

            [Test]
            public void Suite()
            {
                _parseAddressCommand.SetStreet("1490 E FOREMASTER DR STE 150");
                var address = Execute();

                Assert.That(address.HouseNumber, Is.EqualTo(1490));
                Assert.That(address.PrefixDirection, Is.EqualTo(Direction.East));
                Assert.That(address.StreetName, Is.EqualTo("foremaster").IgnoreCase);
                Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                Assert.That(address.StreetType, Is.EqualTo(StreetType.Drive));
                Assert.That(address.StandardizedAddress, Is.EqualTo("1490 east foremaster drive").IgnoreCase);
            }

            [Test]
            public void Issue44_Missing_Loop()
            {
                _parseAddressCommand.SetStreet("9211 N Pebble Creek Loop");
                var address = Execute();

                Assert.That(address.HouseNumber, Is.EqualTo(9211));
                Assert.That(address.PrefixDirection, Is.EqualTo(Direction.North));
                Assert.That(address.StreetName, Is.EqualTo("Pebble Creek").IgnoreCase);
                Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                Assert.That(address.StreetType, Is.EqualTo(StreetType.Loop));
                Assert.That(address.StandardizedAddress, Is.EqualTo("9211 north pebble creek loop").IgnoreCase);
            }

            [Test]
            public void Issue44_So_Much_West()
            {
                _parseAddressCommand.SetStreet("5811 W Park West Rd");
                var address = Execute();

                Assert.That(address.HouseNumber, Is.EqualTo(5811));
                Assert.That(address.PrefixDirection, Is.EqualTo(Direction.West));
                Assert.That(address.StreetName, Is.EqualTo("Park West").IgnoreCase);
                Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                Assert.That(address.StreetType, Is.EqualTo(StreetType.Road));
                Assert.That(address.StandardizedAddress, Is.EqualTo("5811 west park west road").IgnoreCase);
            }

            [Test]
            public void WhistleStop_unittype_in_streetname()
            {
                _parseAddressCommand.SetStreet("6112 W Whistle Stop Road");
                var address = Execute();

                Assert.That(address.HouseNumber, Is.EqualTo(6112));
                Assert.That(address.PrefixDirection, Is.EqualTo(Direction.West));
                Assert.That(address.StreetName, Is.EqualTo("Whistle Stop").IgnoreCase);
                Assert.That(address.SuffixDirection, Is.EqualTo(Direction.None));
                Assert.That(address.StreetType, Is.EqualTo(StreetType.Road));
                Assert.That(address.StandardizedAddress, Is.EqualTo("6112 West Whistle Stop Road").IgnoreCase);
            }
        }
    }
}