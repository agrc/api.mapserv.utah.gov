(function() {

    var detective = {
        init: function() {
            this.OS = this.searchString(this.dataOS) || "an unknown OS";
            this.MAC = this.OS === "Mac";
            this.WINDOWS = this.OS === "Windows";
        },
        searchString: function(data) {
            for (var i = 0; i < data.length; i++) {
                var dataString = data[i].string;
                var dataProp = data[i].prop;
                this.versionSearchString = data[i].versionSearch || data[i].identity;
                if (dataString) {
                    if (dataString.indexOf(data[i].subString) != -1)
                        return data[i].identity;
                } else if (dataProp)
                    return data[i].identity;
            }
        },
        dataOS: [{
                string: navigator.platform,
                subString: "Win",
                identity: "Windows"
            },
            {
                string: navigator.platform,
                subString: "Mac",
                identity: "Mac"
            }]
    };

    detective.init();

    window.$.client = { os: detective.OS, mac: detective.MAC, windows: detective.WINDOWS };
})();