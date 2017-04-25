using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WebAPI.API.Commands.Address;
using WebAPI.API.Commands.Geocode;
using WebAPI.Common.Executors;
using WebAPI.Domain;
using WebAPI.Domain.Addresses;
using WebAPI.Domain.InputOptions;
using WebAPI.Domain.Linkers;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class GetLocatorsForAddressCommandTests
    {
        [TestFixtureSetUp]
        public void SetupGlobal()
        {
            CacheConfig.BuildCache();
        }

        [TestFixture]
        public class GridInAddressPointList
        {
            [SetUp]
            public void Setup()
            {
                CacheConfig.BuildCache();
            }
            /// <summary>
            ///     definite reversal with no address points only needs to go against two locators
            ///     some addresses match the reversal logic but are not reversals. Sending against
            ///     locator like a likely reversal.
            /// </summary>
            [Test]
            public void GetsLocatorsForAddressReversal()
            {
                var command = new ParseAddressCommand("400 S 532 E");
                command.Run();

                var parsedAddress = command.Result;

                var command2 = new ParseZoneCommand("84111", new GeocodeAddress(parsedAddress));
                command2.Run();

                var address = command2.Result;

                address.AddressGrids = new List<GridLinkable>
                    {
                        new ZipGridLink(84111, "grid_1", 0)
                    };

                App.GridsWithAddressPoints = new[]
                    {
                        "grid_1"
                    };

                var result = CommandExecutor.ExecuteCommand(new GetLocatorsForAddressCommand(address, new GeocodeOptions
                    {
                        AcceptScore = 70,
                        JsonFormat = JsonFormat.None,
                        Locators = LocatorType.All,
                        SuggestCount = 0,
                        WkId = 26912
                    }));

                Assert.That(result.Count, Is.EqualTo(6));
                Assert.That(result.Select(x => x.Url), Is.EquivalentTo(new[]
                    {
                        "http://localhost/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=532+East+400+South&City=grid_1&outSR=26912",
                         "http://localhost/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=400+South+532+East&City=grid_1&outSR=26912",
                        "http://localhost/arcgis/rest/services/Geolocators/AddressPoints_AddressSystem/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=400+South+532+East&City=grid_1&outSR=26912",
                        "http://localhost/arcgis/rest/services/Geolocators/AddressPoints_AddressSystem/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=532+East+400+South&City=grid_1&outSR=26912",
                        "http://localhost/arcgis/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=532+East+400+South&City=grid_1&outSR=26912",
                        "http://localhost/arcgis/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=400+South+532+East&City=grid_1&outSR=26912"
                    }));
            }

            /// <summary>
            ///     The supplied address has no address points, and is not a reversal
            ///     so it should get the centerline locators * grids
            /// </summary>
            [Test]
            public void GetsLocatorsForAllGrids()
            {
                var command = new ParseAddressCommand("326 east south temple");
                command.Run();

                var parsedAddress = command.Result;

                var command2 = new ParseZoneCommand("84111", new GeocodeAddress(parsedAddress));
                command2.Run();

                var address = command2.Result;

                address.AddressGrids = new List<GridLinkable>
                    {
                        new ZipGridLink(84111, "grid_1", 0),
                        new ZipGridLink(84111, "grid_2", 1)
                    };


                App.GridsWithAddressPoints = new[]
                    {
                        "grid_1", "grid_2"
                    };

                var result = CommandExecutor.ExecuteCommand(new GetLocatorsForAddressCommand(address, new GeocodeOptions
                    {
                        AcceptScore = 70,
                        JsonFormat = JsonFormat.None,
                        Locators = LocatorType.All,
                        SuggestCount = 0,
                        WkId = 26912
                    }));

                Assert.That(result.Count, Is.EqualTo(10));
                Assert.That(result.Select(x => x.Url), Is.EquivalentTo(new[]
                    {
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_1&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_1&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ALIAS1/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_1&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ALIAS2/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_1&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_2&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_2&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ALIAS1/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_2&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ALIAS2/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_2&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/AddressPoints_AddressSystem/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_1&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/AddressPoints_AddressSystem/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_2&outSR=26912"
                    }).IgnoreCase);
            }

            /// <summary>
            ///     possible reversal goes against all
            /// </summary>
            [Test]
            public void GetsLocatorsForPossibleAddressReversal()
            {
                var command = new ParseAddressCommand("5625 S 995 E");
                command.Run();

                var parsedAddress = command.Result;

                var command2 = new ParseZoneCommand("84111", new GeocodeAddress(parsedAddress));
                command2.Run();

                var address = command2.Result;

                address.AddressGrids = new List<GridLinkable>
                    {
                        new ZipGridLink(84111, "SALT LAKE CITY", 0)
                    };

                var result = CommandExecutor.ExecuteCommand(new GetLocatorsForAddressCommand(address, new GeocodeOptions
                    {
                        AcceptScore = 70,
                        JsonFormat = JsonFormat.None,
                        Locators = LocatorType.All,
                        SuggestCount = 0,
                        WkId = 26912
                    }));

                Assert.That(result.Count, Is.EqualTo(8));
                Assert.That(result.Select(x => x.Url), Is.EquivalentTo(new[]
                    {
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=995+East+5625+South&City=SALT LAKE CITY&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=995+East+5625+South&City=SALT LAKE CITY&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=5625+South+995+East&City=SALT LAKE CITY&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=5625+South+995+East&City=SALT LAKE CITY&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ALIAS1/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=5625+South+995+East&City=SALT LAKE CITY&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ALIAS2/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=5625+South+995+East&City=SALT LAKE CITY&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/AddressPoints_AddressSystem/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=995+East+5625+South&City=SALT LAKE CITY&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/AddressPoints_AddressSystem/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=5625+South+995+East&City=SALT LAKE CITY&outSR=26912"
                    }).IgnoreCase);
            }
        }

        [TestFixture]
        public class GridNotInAddressPointList
        {
            /// <summary>
            ///     definite reversal with no address points only needs to go against two locators.
            ///     update: send both ways for small case of exceptions
            /// </summary>
            [Test]
            public void GetsLocatorsForAddressReversal()
            {
                var command = new ParseAddressCommand("400 S 532 E");
                command.Run();

                var parsedAddress = command.Result;

                var command2 = new ParseZoneCommand("84111", new GeocodeAddress(parsedAddress));
                command2.Run();

                var address = command2.Result;

                address.AddressGrids = new List<GridLinkable>
                    {
                        new ZipGridLink(84111, "grid_1", 0)
                    };

                App.GridsWithAddressPoints = new string[] {};

                var result = CommandExecutor.ExecuteCommand(new GetLocatorsForAddressCommand(address, new GeocodeOptions
                    {
                        AcceptScore = 70,
                        JsonFormat = JsonFormat.None,
                        Locators = LocatorType.All,
                        SuggestCount = 0,
                        WkId = 26912
                    }));

                Assert.That(result.Count, Is.EqualTo(4));
                Assert.That(result.Select(x => x.Url), Is.EquivalentTo(new[]
                    {
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=532+East+400+South&City=grid_1&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=532+East+400+South&City=grid_1&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=400+South+532+East&City=grid_1&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=400+South+532+East&City=grid_1&outSR=26912"
                    }).IgnoreCase);
            }

            /// <summary>
            ///     The supplied address has no address points, and is not a reversal
            ///     so it should get the centerline locators * grids
            /// </summary>
            [Test]
            public void GetsLocatorsForAllGrids()
            {
                var command = new ParseAddressCommand("326 east south temple");
                command.Run();

                var parsedAddress = command.Result;

                var command2 = new ParseZoneCommand("84111", new GeocodeAddress(parsedAddress));
                command2.Run();

                var address = command2.Result;

                address.AddressGrids = new List<GridLinkable>
                    {
                        new ZipGridLink(84111, "grid_1", 0),
                        new ZipGridLink(84111, "grid_2", 1)
                    };

                App.GridsWithAddressPoints = new string[] {};

                var result = CommandExecutor.ExecuteCommand(new GetLocatorsForAddressCommand(address, new GeocodeOptions
                    {
                        AcceptScore = 70,
                        JsonFormat = JsonFormat.None,
                        Locators = LocatorType.All,
                        SuggestCount = 0,
                        WkId = 26912
                    }));

                Assert.That(result.Count, Is.EqualTo(8));
                Assert.That(result.Select(x => x.Url), Is.EquivalentTo(new[]
                    {
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_1&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_1&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ALIAS1/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_1&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ALIAS2/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_1&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_2&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_2&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ALIAS1/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_2&outSR=26912"
                        ,
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ALIAS2/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=326+east+south+temple&City=grid_2&outSR=26912"
                    }).IgnoreCase);
            }

            /// <summary>
            ///     possible reversal goes against all
            /// </summary>
            [Test]
            public void GetsLocatorsForPossibleAddressReversal()
            {
                var command = new ParseAddressCommand("5625 S 995 E");
                command.Run();

                var parsedAddress = command.Result;

                var command2 = new ParseZoneCommand("84111", new GeocodeAddress(parsedAddress));
                command2.Run();

                var address = command2.Result;

                address.AddressGrids = new List<GridLinkable>
                    {
                        new ZipGridLink(84111, "grid_1", 0)
                    };

                App.GridsWithAddressPoints = new string[] {};

                var result = CommandExecutor.ExecuteCommand(new GetLocatorsForAddressCommand(address, new GeocodeOptions
                    {
                        AcceptScore = 70,
                        JsonFormat = JsonFormat.None,
                        Locators = LocatorType.All,
                        SuggestCount = 0,
                        WkId = 26912
                    }));

                Assert.That(result.Count, Is.EqualTo(6));
                Assert.That(result.Select(x => x.Url), Is.EquivalentTo(new[]
                    {
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=995+East+5625+South&City=grid_1&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=995+East+5625+South&City=grid_1&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=5625+South+995+East&City=grid_1&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=5625+South+995+East&City=grid_1&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ALIAS1/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=5625+South+995+East&City=grid_1&outSR=26912",
                        "http://localhost/ArcGIS/rest/services/Geolocators/Roads_AddressSystem_ALIAS2/" +
                        "GeocodeServer/findAddressCandidates?f=json&Street=5625+South+995+East&City=grid_1&outSR=26912"
                    }).IgnoreCase);
            }
        }
    }
}