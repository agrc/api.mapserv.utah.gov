namespace api.migration.tests;

public class UnitTest1 {
    [Theory]
    [InlineData("geocode/326 east south temple/slc")]
    public async Task Test1(string path) {
        var currentClient = new HttpClient() {
            BaseAddress = new Uri("https://api.mapserv.utah.gov/api/v1/")
        };

        var cloudClient = new HttpClient() {
            BaseAddress = new Uri("https://ut-dts-agrc-web-api-dev.web.app/api/v1/"),
            DefaultRequestHeaders = {
                { "Authorization", "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IjVkZjFmOTQ1ZmY5MDZhZWFlZmE5M2MyNzY5OGRiNDA2ZDYwNmIwZTgiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiIzMjU1NTk0MDU1OS5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsImF1ZCI6IjMyNTU1OTQwNTU5LmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwic3ViIjoiMTEzMjA3MzEyOTAxNDM3NDQzNjMxIiwiaGQiOiJ1dGFoLmdvdiIsImVtYWlsIjoic2dvdXJsZXlAdXRhaC5nb3YiLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiYXRfaGFzaCI6IkptNjNhb3ZsbkxfSWNOemotalpmeGciLCJpYXQiOjE2Nzg4MzgyNzksImV4cCI6MTY3ODg0MTg3OX0.YPrP7CJN9_yfBpBnYj_PTfhu3taHBKszXaCXXG1BtIh_PorI7JcXbo0EgH_r0drKpRHYp9YXNcnitToDE2GmaNo1hkIkvq-B4xNWBf1-ap0wHH0WKVuOzBjwM7_zeZUP7LsVIZsomdFrSVioHNENyapTVb1nQT9U-z7RGYo89puBTWxSG9k0777fxDXSB-buo2Oy6XC5KDsYedJN6Xp7ozsM6f5RigXV-CV8AuFKGnaqC8IY7zR2FNfirZ__ZldpVDRTejINqDAC3GZD9w43Ajcsa85Q4k2FfCc-16Zdd3BqHarxi7qBeX6-v3yJvhbjILnXz7juvbSxnU4s1tQ3xA" }
            }
        };

        using var currentResponse = await currentClient.GetAsync(path + "?apikey=agrc-dev");
        using var cloudResponse = await cloudClient.GetAsync(path + "?apikey=agrc-dev");

        currentResponse.EnsureSuccessStatusCode();
        cloudResponse.EnsureSuccessStatusCode();

        var current = await currentResponse.Content.ReadAsStringAsync();
        var cloud = await cloudResponse.Content.ReadAsStringAsync();

        cloud.ShouldBe(current);
    }
}
